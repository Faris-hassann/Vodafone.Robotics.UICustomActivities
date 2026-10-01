using Vodafone.Robotics.UiValidation.Automation;
using Vodafone.Robotics.UiValidation.Exceptions;
using Vodafone.Robotics.UiValidation.Logging;
using Vodafone.Robotics.UiValidation.Models;
using Vodafone.Robotics.UiValidation.Validation;

namespace Vodafone.Robotics.UiValidation.Engine;

public sealed class ValidationEngine
{
    private readonly IUiAutomationAdapter _adapter;
    private readonly IValidationLogger _logger;
    private readonly Action<TimeSpan, CancellationToken> _delay;

    public ValidationEngine(IUiAutomationAdapter adapter, IValidationLogger? logger = null, Action<TimeSpan, CancellationToken>? delay = null)
    {
        _adapter = adapter;
        _logger = logger ?? NullValidationLogger.Instance;
        _delay = delay ?? ((duration, token) => Task.Delay(duration, token).GetAwaiter().GetResult());
    }

    public ValidationExecutionResult Execute(GetTextConfiguration configuration, CancellationToken cancellationToken = default)
        => ExecuteCore(configuration, cancellationToken, state => ExecuteGetText(configuration, state, cancellationToken));

    public ValidationExecutionResult Execute(TypeIntoConfiguration configuration, CancellationToken cancellationToken = default)
        => ExecuteCore(configuration, cancellationToken, state => ExecuteTypeInto(configuration, state, cancellationToken));

    public ValidationExecutionResult Execute(ClickConfiguration configuration, CancellationToken cancellationToken = default)
        => ExecuteCore(configuration, cancellationToken, state => ExecuteClick(configuration, state, cancellationToken));

    private ValidationExecutionResult ExecuteCore(ActivityConfiguration configuration, CancellationToken token, Func<ExecutionState, string?> operation)
    {
        var state = new ExecutionState(configuration);
        try
        {
            state.Stage = ValidationStage.ConfigurationValidation;
            ConfigurationValidator.Validate(configuration);
            Log(configuration, state, ValidationLogLevel.Information, "ActivityStarted", "Validation activity started.");
            Log(configuration, state, ValidationLogLevel.Trace, "ConfigurationValidated", "Configuration validated before UI mutation.");
            var raw = operation(state);
            state.Stage = ValidationStage.Completed;
            state.PostConditionValidated = configuration.PostCondition.Kind == PostConditionKind.None || state.PostConditionValidated;
            Log(configuration, state, ValidationLogLevel.Information, "ActivitySucceeded", "Validation activity completed successfully.");
            return new(BuildResult(configuration, state, true, FailureCategory.None, string.Empty, null), raw);
        }
        catch (OperationCanceledException ex)
        {
            state.Stage = state.Stage == ValidationStage.NotStarted ? ValidationStage.ConfigurationValidation : state.Stage;
            Log(configuration, state, ValidationLogLevel.Warning, "ActivityCancelled", "Validation activity was cancelled.");
            return new(BuildResult(configuration, state, false, FailureCategory.Cancelled, "Operation cancelled.", ex));
        }
        catch (Exception ex)
        {
            var mapped = Map(ex, state.Stage);
            state.Stage = mapped.Stage;
            var screenshot = TryCaptureFailureScreenshot(configuration, state);
            state.ScreenshotPath = screenshot;
            Log(configuration, state, ValidationLogLevel.Error, "ActivityFailed", mapped.Message, new Dictionary<string, string> { ["failureCategory"] = mapped.Category.ToString() });
            return new(BuildResult(configuration, state, false, mapped.Category, mapped.Message, mapped));
        }
    }

    private string? ExecuteGetText(GetTextConfiguration configuration, ExecutionState state, CancellationToken token)
    {
        Exception? last = null;
        string? raw = null;
        for (var attempt = 1; attempt <= state.MaxAttempts; attempt++)
        {
            state.Attempt = attempt;
            token.ThrowIfCancellationRequested();
            try
            {
                using var target = ResolveWorkTarget(configuration, state, token);
                ValidatePreCondition(configuration, state, token);
                state.Stage = ValidationStage.DoWork;
                Log(configuration, state, ValidationLogLevel.Trace, "ActionStarted", "Reading target text.");
                raw = target.GetText();
                state.ActionExecuted = true;
                state.RawValueState = TextValidation.Classify(raw);
                Log(configuration, state, ValidationLogLevel.Trace, "ActionCompleted", "Target text read completed.", new Dictionary<string, string> { ["rawState"] = state.RawValueState.Value.ToString() });
                if (!TextValidation.ValidateRules(raw, configuration, out var reason)) throw new UITextValidationException(reason);
                ValidatePostCondition(configuration, state, token, null);
                state.VerificationAvailable = true;
                state.VerificationMode = "TextRules";
                state.SafeActual = SafeLog.Value(raw, configuration.LogSensitiveValues);
                return raw;
            }
            catch (Exception ex) when (IsRetryable(ex, configuration.WaitForRules))
            {
                last = ex;
                if (attempt < state.MaxAttempts) ScheduleRetry(configuration, state, token, ex.Message);
                else if (state.MaxAttempts == 1) throw;
            }
        }
        throw new UIValidationException($"Get Text retries exhausted. {last?.Message}", FailureCategory.RetryExhausted, state.Stage, last);
    }

    private string? ExecuteTypeInto(TypeIntoConfiguration configuration, ExecutionState state, CancellationToken token)
    {
        using var target = ResolveMutatingTarget(configuration, state, token, "Type Into");
        ValidatePreCondition(configuration, state, token);
        state.Stage = ValidationStage.PreCondition;
        var original = target.GetText();
        if (configuration.ExpectedExistingValue is not null && !TextValidation.Equals(original, configuration.ExpectedExistingValue, configuration.Comparison))
            throw new UIValidationException("Existing target value did not match the required precondition.", FailureCategory.TargetIdentityMismatch, ValidationStage.PreCondition);

        var expected = configuration.ExpectedFinalValue ?? configuration.Mode switch
        {
            TypeIntoMode.Replace => configuration.InputText,
            TypeIntoMode.Append => (original ?? string.Empty) + configuration.AppendSeparator + configuration.InputText,
            TypeIntoMode.ClearOnly => string.Empty,
            _ => throw new UIConfigurationException("Unsupported Type Into mode.")
        };

        state.Stage = ValidationStage.DoWork;
        Log(configuration, state, ValidationLogLevel.Information, "ActionStarted", $"Executing Type Into mode {configuration.Mode}.", new Dictionary<string, string> { ["input"] = SafeLog.Value(configuration.InputText, configuration.LogSensitiveValues, configuration.IsSecure) });
        var appendNoOp = configuration.Mode == TypeIntoMode.Append && configuration.InputText.Length == 0 && configuration.ExpectedFinalValue is null;
        if (!appendNoOp)
        {
            try { target.SetText(expected); }
            catch (Exception ex) { throw new UIActionException("Type Into action failed.", ex); }
            state.ActionExecuted = true;
        }
        Log(configuration, state, ValidationLogLevel.Information, "ActionCompleted", "Type Into action completed.");

        if (configuration.IsSecure || configuration.VerificationMode == VerificationMode.ActionOnly)
        {
            state.VerificationAvailable = false;
            state.VerificationMode = VerificationMode.ActionOnly.ToString();
            ValidatePostCondition(configuration, state, token, null);
            return null;
        }

        Exception? last = null;
        for (var attempt = 1; attempt <= state.MaxAttempts; attempt++)
        {
            state.Attempt = Math.Max(state.Attempt, attempt);
            token.ThrowIfCancellationRequested();
            string? actual;
            try { actual = ReadBack(target, configuration, out var verificationMode); state.VerificationMode = verificationMode; }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                state.RawValueState = RawValueState.ReadFailed;
                last = new UIValidationException("Type Into read-back failed.", FailureCategory.ReadFailed, ValidationStage.PostCondition, ex);
                if (attempt < state.MaxAttempts) { ScheduleRetry(configuration, state, token, last.Message); continue; }
                break;
            }
            if (TextValidation.Equals(actual, expected, configuration.Comparison))
            {
                state.VerificationAvailable = true;
                state.MismatchKind = MismatchKind.ExactMatch;
                state.SafeExpected = SafeLog.Value(expected, configuration.LogSensitiveValues, configuration.IsSecure);
                state.SafeActual = SafeLog.Value(actual, configuration.LogSensitiveValues, configuration.IsSecure);
                ValidatePostCondition(configuration, state, token, null);
                return null;
            }
            state.MismatchKind = TextValidation.ClassifyMismatch(original, expected, actual);
            last = new UITextValidationException($"Type Into read-back mismatch ({state.MismatchKind}).");
            if (attempt < state.MaxAttempts) ScheduleRetry(configuration, state, token, last.Message);
        }
        throw new UIValidationException($"Type Into verification retries exhausted. {last?.Message}", FailureCategory.RetryExhausted, ValidationStage.PostCondition, last);
    }

    private string? ExecuteClick(ClickConfiguration configuration, ExecutionState state, CancellationToken token)
    {
        var evaluator = new ConditionEvaluator(_adapter);
        using var initialTarget = ResolveMutatingTarget(configuration, state, token, "Click");
        ValidatePreCondition(configuration, state, token);
        var priorState = evaluator.CapturePriorState(configuration.PostCondition, configuration.TimeoutMilliseconds, token);
        if (configuration.PostCondition.Kind != PostConditionKind.None && configuration.SkipWhenPostConditionAlreadySatisfied && evaluator.Evaluate(configuration.PostCondition, configuration.TimeoutMilliseconds, configuration.Comparison, priorState, token))
        {
            state.PostConditionValidated = true;
            state.OutcomeIndependentlyVerified = true;
            state.VerificationAvailable = true;
            state.VerificationMode = configuration.PostCondition.Kind.ToString();
            return null;
        }

        state.Stage = ValidationStage.DoWork;
        Log(configuration, state, ValidationLogLevel.Information, "ActionStarted", "Click action started.");
        try { initialTarget.Click(); }
        catch (Exception ex) { throw new UIActionException("Click action failed.", ex); }
        state.ActionExecuted = true;
        Log(configuration, state, ValidationLogLevel.Information, "ActionCompleted", "Click action completed.");

        if (configuration.PostCondition.Kind == PostConditionKind.None)
        {
            state.VerificationAvailable = false;
            state.VerificationMode = "OutcomeNotIndependentlyVerified";
            Log(configuration, state, ValidationLogLevel.Warning, "OutcomeNotIndependentlyVerified", "Click executed without a postcondition.");
            return null;
        }

        Exception? last = null;
        for (var attempt = 1; attempt <= state.MaxAttempts; attempt++)
        {
            state.Attempt = Math.Max(state.Attempt, attempt);
            token.ThrowIfCancellationRequested();
            state.Stage = ValidationStage.PostCondition;
            if (evaluator.Evaluate(configuration.PostCondition, configuration.TimeoutMilliseconds, configuration.Comparison, priorState, token))
            {
                state.PostConditionValidated = true;
                state.OutcomeIndependentlyVerified = true;
                state.VerificationAvailable = true;
                state.VerificationMode = configuration.PostCondition.Kind.ToString();
                Log(configuration, state, ValidationLogLevel.Information, "PostConditionValidated", "Click postcondition validated.");
                return null;
            }
            last = new UIPostConditionException("Click postcondition is not yet satisfied.");
            if (attempt < state.MaxAttempts)
            {
                ScheduleRetry(configuration, state, token, last.Message);
                if (configuration.AllowActionRetry)
                {
                    if (evaluator.Evaluate(configuration.PostCondition, configuration.TimeoutMilliseconds, configuration.Comparison, priorState, token)) continue;
                    using var retryTarget = ResolveWorkTarget(configuration, state, token, requireEnabled: true);
                    state.Stage = ValidationStage.DoWork;
                    Log(configuration, state, ValidationLogLevel.Warning, "ActionStarted", "Explicitly allowed click retry started.");
                    try { retryTarget.Click(); }
                    catch (Exception ex) { throw new UIActionException("Click retry failed.", ex); }
                    Log(configuration, state, ValidationLogLevel.Information, "ActionCompleted", "Click retry completed.");
                }
            }
        }
        throw new UIValidationException($"Click postcondition retries exhausted. {last?.Message}", FailureCategory.RetryExhausted, ValidationStage.PostCondition, last);
    }

    private IUiTarget ResolveMutatingTarget(ActivityConfiguration configuration, ExecutionState state, CancellationToken token, string operation)
    {
        Exception? last = null;
        for (var attempt = 1; attempt <= state.MaxAttempts; attempt++)
        {
            state.Attempt = attempt;
            token.ThrowIfCancellationRequested();
            try { return ResolveWorkTarget(configuration, state, token, requireEnabled: true); }
            catch (Exception ex) when (ex is UITargetNotFoundException or UITargetIdentityException)
            {
                last = ex;
                if (attempt < state.MaxAttempts) ScheduleRetry(configuration, state, token, ex.Message);
                else if (state.MaxAttempts == 1) throw;
            }
        }
        throw new UIValidationException($"{operation} target retries exhausted. {last?.Message}", FailureCategory.RetryExhausted, state.Stage, last);
    }

    private IUiTarget ResolveWorkTarget(ActivityConfiguration configuration, ExecutionState state, CancellationToken token, bool requireEnabled = false)
    {
        state.Stage = ValidationStage.TargetResolution;
        Log(configuration, state, ValidationLogLevel.Trace, "TargetSearchStarted", "Resolving Target Selector.", new Dictionary<string, string> { ["selector"] = SafeLog.Selector(configuration.WorkSelector, configuration.LogFullSelector) });
        var targets = _adapter.Resolve(configuration.WorkSelector, configuration.TimeoutMilliseconds, token);
        if (targets.Count == 0) throw new UITargetNotFoundException("No target matched the Target Selector.");
        if (targets.Count > 1 && !configuration.AllowMultipleMatches)
        {
            foreach (var target in targets) target.Dispose();
            throw new UITargetAmbiguousException($"Target Selector matched {targets.Count} targets; exactly one is required.");
        }
        var selected = targets[0];
        for (var index = 1; index < targets.Count; index++) targets[index].Dispose();
        state.TargetFound = true;
        state.Stage = ValidationStage.TargetIdentityValidation;
        var identity = requireEnabled ? configuration.Identity with { RequireEnabled = true } : configuration.Identity;
        try { TargetValidation.Validate(selected, identity, configuration.Comparison); }
        catch { selected.Dispose(); throw; }
        state.TargetIdentityValidated = true;
        Log(configuration, state, ValidationLogLevel.Trace, "TargetIdentityValidated", "Target uniqueness and identity validated.");
        return selected;
    }

    private void ValidatePreCondition(ActivityConfiguration configuration, ExecutionState state, CancellationToken token)
    {
        if (configuration.PreCondition.Kind == ConditionKind.None) return;
        state.Stage = ValidationStage.PreCondition;
        Log(configuration, state, ValidationLogLevel.Trace, "PreConditionValidationStarted", "PreCondition validation started.");
        var evaluator = new ConditionEvaluator(_adapter);
        if (!evaluator.Evaluate(configuration.PreCondition, configuration.TimeoutMilliseconds, configuration.Comparison, token))
            throw new UIValidationException("PreCondition was not satisfied; Do Work was not executed.", FailureCategory.TargetIdentityMismatch, ValidationStage.PreCondition);
        Log(configuration, state, ValidationLogLevel.Information, "PreConditionValidated", "PreCondition validated.");
    }

    private void ValidatePostCondition(ActivityConfiguration configuration, ExecutionState state, CancellationToken token, string? priorAttribute)
    {
        if (configuration.PostCondition.Kind == PostConditionKind.None) return;
        state.Stage = ValidationStage.PostCondition;
        Log(configuration, state, ValidationLogLevel.Trace, "PostConditionValidationStarted", "PostCondition validation started.");
        var evaluator = new ConditionEvaluator(_adapter);
        if (!evaluator.Evaluate(configuration.PostCondition, configuration.TimeoutMilliseconds, configuration.Comparison, priorAttribute, token))
            throw new UIPostConditionException("PostCondition was not satisfied.");
        state.PostConditionValidated = true;
        state.OutcomeIndependentlyVerified = true;
        Log(configuration, state, ValidationLogLevel.Information, "PostConditionValidated", "PostCondition validated.");
    }

    private static string? ReadBack(IUiTarget target, TypeIntoConfiguration configuration, out string verificationMode)
    {
        if (configuration.VerificationMode == VerificationMode.ValueAttribute)
        {
            verificationMode = VerificationMode.ValueAttribute.ToString();
            return target.GetAttribute("value")?.ToString();
        }
        if (configuration.VerificationMode == VerificationMode.SpecificAttribute)
        {
            verificationMode = $"SpecificAttribute:{configuration.VerificationAttribute}";
            return target.GetAttribute(configuration.VerificationAttribute!)?.ToString();
        }
        if (configuration.VerificationMode == VerificationMode.Text)
        {
            verificationMode = VerificationMode.Text.ToString();
            return target.GetText();
        }
        var text = target.GetText();
        if (text is not null)
        {
            verificationMode = VerificationMode.Text.ToString();
            return text;
        }
        verificationMode = VerificationMode.ValueAttribute.ToString();
        return target.GetAttribute("value")?.ToString();
    }

    private void ScheduleRetry(ActivityConfiguration configuration, ExecutionState state, CancellationToken token, string reason)
    {
        Log(configuration, state, ValidationLogLevel.Warning, "RetryScheduled", reason);
        _delay(TimeSpan.FromMilliseconds(configuration.RetryIntervalMilliseconds), token);
    }

    private static bool IsRetryable(Exception exception, bool waitForRules) => exception is UITargetNotFoundException || (waitForRules && exception is UITextValidationException);

    private string? TryCaptureFailureScreenshot(ActivityConfiguration configuration, ExecutionState state)
    {
        if (!configuration.ScreenshotOnFinalFailure) return null;
        var primaryStage = state.Stage;
        try
        {
            state.Stage = ValidationStage.Diagnostics;
            var directory = Path.Combine(Path.GetTempPath(), "UI.Validation.Library", "Screenshots");
            var safeName = string.Concat(configuration.ActivityType.Where(char.IsLetterOrDigit));
            var path = Path.Combine(directory, $"{safeName}_{state.CorrelationId}_{DateTimeOffset.UtcNow:yyyyMMdd_HHmmssfff}.png");
            var captured = _adapter.CaptureScreenshot(configuration.WorkSelector, path, configuration.TimeoutMilliseconds, CancellationToken.None);
            if (captured is not null) Log(configuration, state, ValidationLogLevel.Information, "ScreenshotCaptured", "Final-failure screenshot captured.", new Dictionary<string, string> { ["path"] = captured });
            return captured;
        }
        catch (Exception ex)
        {
            Log(configuration, state, ValidationLogLevel.Warning, "DiagnosticFailure", $"Screenshot capture failed without replacing the primary failure: {ex.GetType().Name}.");
            return null;
        }
        finally { state.Stage = primaryStage; }
    }

    private void Log(ActivityConfiguration configuration, ExecutionState state, ValidationLogLevel level, string eventName, string message, IReadOnlyDictionary<string, string>? extra = null)
    {
        var context = new Dictionary<string, string>
        {
            ["activity"] = configuration.ActivityType,
            ["stage"] = state.Stage.ToString(),
            ["attempt"] = state.Attempt.ToString(System.Globalization.CultureInfo.InvariantCulture),
            ["maxAttempts"] = state.MaxAttempts.ToString(System.Globalization.CultureInfo.InvariantCulture),
            ["correlationId"] = state.CorrelationId
        };
        if (extra is not null) foreach (var pair in extra) context[pair.Key] = pair.Value;
        try { _logger.Log(new(level, eventName, message, configuration.ActivityType, state.Stage, state.Attempt, state.MaxAttempts, state.CorrelationId, context)); }
        catch { }
    }

    private static UIValidationException Map(Exception exception, ValidationStage stage)
    {
        if (exception is UIValidationException known) return known;
        return new UIValidationException(exception.Message, FailureCategory.Unknown, stage, exception);
    }

    private static UIValidationResult BuildResult(ActivityConfiguration configuration, ExecutionState state, bool success, FailureCategory category, string reason, Exception? exception) => new()
    {
        Success = success,
        ActivityType = configuration.ActivityType,
        ActivityName = configuration.ActivityName,
        ValidationStage = state.Stage,
        FailureCategory = category,
        FailureReason = reason,
        AttemptCount = state.Attempt,
        MaxAttempts = state.MaxAttempts,
        TargetFound = state.TargetFound,
        TargetIdentityValidated = state.TargetIdentityValidated,
        ActionExecuted = state.ActionExecuted,
        PostConditionValidated = state.PostConditionValidated,
        VerificationAvailable = state.VerificationAvailable,
        VerificationMode = state.VerificationMode,
        SafeExpectedSummary = state.SafeExpected,
        SafeActualSummary = state.SafeActual,
        StartedAt = state.StartedAt,
        CompletedAt = DateTimeOffset.UtcNow,
        CorrelationId = state.CorrelationId,
        ScreenshotPath = state.ScreenshotPath,
        ExceptionType = exception?.GetType().FullName,
        ExceptionMessage = exception?.Message,
        RawValueState = state.RawValueState,
        MismatchKind = state.MismatchKind,
        OutcomeIndependentlyVerified = state.OutcomeIndependentlyVerified
    };

    private sealed class ExecutionState
    {
        public ExecutionState(ActivityConfiguration configuration)
        {
            StartedAt = DateTimeOffset.UtcNow;
            CorrelationId = string.IsNullOrWhiteSpace(configuration.CorrelationId) ? Guid.NewGuid().ToString("N") : configuration.CorrelationId!;
            MaxAttempts = 1 + configuration.RetryCount;
        }
        public DateTimeOffset StartedAt { get; }
        public string CorrelationId { get; }
        public int MaxAttempts { get; }
        public int Attempt { get; set; } = 1;
        public ValidationStage Stage { get; set; }
        public bool TargetFound { get; set; }
        public bool TargetIdentityValidated { get; set; }
        public bool ActionExecuted { get; set; }
        public bool PostConditionValidated { get; set; }
        public bool VerificationAvailable { get; set; }
        public bool OutcomeIndependentlyVerified { get; set; }
        public string VerificationMode { get; set; } = string.Empty;
        public string SafeExpected { get; set; } = string.Empty;
        public string SafeActual { get; set; } = string.Empty;
        public string? ScreenshotPath { get; set; }
        public RawValueState? RawValueState { get; set; }
        public MismatchKind MismatchKind { get; set; }
    }
}
