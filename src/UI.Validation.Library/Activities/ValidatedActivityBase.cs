using System.Activities;
using System.ComponentModel;
using System.Diagnostics;
using UiPath.Robot.Activities.Api;
using Vodafone.Robotics.UiValidation.Automation;
using Vodafone.Robotics.UiValidation.Engine;
using Vodafone.Robotics.UiValidation.Exceptions;
using Vodafone.Robotics.UiValidation.Logging;
using Vodafone.Robotics.UiValidation.Models;

namespace Vodafone.Robotics.UiValidation.Activities;

public abstract class ValidatedActivityBase : CodeActivity
{
    [Category("1. PreCondition")]
    [Description("Complete UiPath selector used only for the optional precondition.")]
    public InArgument<string?> PreConditionSelector { get; set; } = new();

    [Category("1. PreCondition")]
    [Description("Condition that must pass before Do Work is allowed to execute.")]
    public InArgument<ConditionKind> PreConditionKind { get; set; } = new(ConditionKind.None);

    [Category("1. PreCondition")]
    public InArgument<string?> PreConditionAttribute { get; set; } = new();

    [Category("1. PreCondition")]
    public InArgument<string?> PreConditionExpectedValue { get; set; } = new();

    [RequiredArgument]
    [Category("2. Do Work")]
    [Description("Required complete UiPath selector for the element on which this activity performs its main operation.")]
    public InArgument<string> TargetSelector { get; set; } = null!;

    [Category("2. Do Work")]
    public InArgument<bool> AllowMultipleMatches { get; set; } = new(false);

    [Category("2. Do Work - Target Identity")]
    public InArgument<bool> RequireVisible { get; set; } = new(true);

    [Category("2. Do Work - Target Identity")]
    public InArgument<bool> RequireEnabled { get; set; } = new(false);

    [Category("3. PostCondition")]
    [Description("Complete UiPath selector used only for optional outcome verification.")]
    public InArgument<string?> PostConditionSelector { get; set; } = new();

    [Category("3. PostCondition")]
    public InArgument<PostConditionKind> PostConditionKind { get; set; } = new(Models.PostConditionKind.None);

    [Category("3. PostCondition")]
    public InArgument<string?> PostConditionAttribute { get; set; } = new();

    [Category("3. PostCondition")]
    public InArgument<string?> PostConditionExpectedValue { get; set; } = new();

    [Category("4. Retry and Timeout")]
    public InArgument<int> TimeoutMilliseconds { get; set; } = new(10_000);

    [Category("4. Retry and Timeout")]
    [Description("Additional retries after the first attempt. MaxAttempts = 1 + RetryCount.")]
    public InArgument<int> RetryCount { get; set; } = new(2);

    [Category("4. Retry and Timeout")]
    public InArgument<int> RetryIntervalMilliseconds { get; set; } = new(500);

    [Category("5. Comparison")]
    public InArgument<bool> CaseSensitive { get; set; } = new(true);

    [Category("5. Comparison")]
    public InArgument<bool> TrimForValidation { get; set; } = new(false);

    [Category("5. Comparison")]
    public InArgument<bool> NormalizeWhitespace { get; set; } = new(false);

    [Category("6. Logging and Diagnostics")]
    public InArgument<string?> CorrelationId { get; set; } = new();

    [Category("6. Logging and Diagnostics")]
    public InArgument<bool> LogSensitiveValues { get; set; } = new(false);

    [Category("6. Logging and Diagnostics")]
    public InArgument<bool> LogFullSelector { get; set; } = new(false);

    [Category("6. Logging and Diagnostics")]
    public InArgument<bool> ScreenshotOnFinalFailure { get; set; } = new(true);

    [Category("7. Behavior")]
    public InArgument<bool> ThrowOnFailure { get; set; } = new(true);

    [Category("8. Output")]
    public OutArgument<UIValidationResult> ValidationResult { get; set; } = null!;

    protected sealed override void Execute(CodeActivityContext context)
    {
        var configuration = CreateConfiguration(context);
        var logger = new RobotValidationLogger(context.GetExtension<IExecutorRuntime>());
        var execution = ExecuteCore(new ValidationEngine(ActivityServices.AdapterFactory(), logger), configuration);
        ValidationResult.Set(context, execution.Result);
        SetActivityOutputs(context, execution);
        if (!execution.Result.Success && configuration.ThrowOnFailure)
            throw new UIValidationException(execution.Result.FailureReason, execution.Result.FailureCategory, execution.Result.ValidationStage, result: execution.Result);
    }

    protected abstract ActivityConfiguration CreateConfiguration(CodeActivityContext context);
    protected abstract ValidationExecutionResult ExecuteCore(ValidationEngine engine, ActivityConfiguration configuration);
    protected virtual void SetActivityOutputs(CodeActivityContext context, ValidationExecutionResult execution) { }

    protected T Common<T>(CodeActivityContext context, T configuration) where T : ActivityConfiguration => configuration with
    {
        ActivityName = DisplayName,
        WorkSelector = TargetSelector.Get(context),
        CorrelationId = CorrelationId.Get(context),
        TimeoutMilliseconds = TimeoutMilliseconds.Get(context),
        RetryCount = RetryCount.Get(context),
        RetryIntervalMilliseconds = RetryIntervalMilliseconds.Get(context),
        AllowMultipleMatches = AllowMultipleMatches.Get(context),
        ThrowOnFailure = ThrowOnFailure.Get(context),
        ScreenshotOnFinalFailure = ScreenshotOnFinalFailure.Get(context),
        LogSensitiveValues = LogSensitiveValues.Get(context),
        LogFullSelector = LogFullSelector.Get(context),
        Identity = new(RequireVisible: RequireVisible.Get(context), RequireEnabled: RequireEnabled.Get(context)),
        PreCondition = new(PreConditionKind.Get(context), PreConditionSelector.Get(context), PreConditionAttribute.Get(context), PreConditionExpectedValue.Get(context)),
        PostCondition = new(PostConditionKind.Get(context), PostConditionSelector.Get(context), PostConditionAttribute.Get(context), PostConditionExpectedValue.Get(context)),
        Comparison = new(CaseSensitive.Get(context), TrimForValidation.Get(context), NormalizeWhitespace.Get(context))
    };

    internal static class ActivityServices
    {
        internal static Func<IUiAutomationAdapter> AdapterFactory { get; set; } = static () => new UiPathAutomationAdapter();
    }

    private sealed class RobotValidationLogger : IValidationLogger
    {
        private readonly IExecutorRuntime? _runtime;
        public RobotValidationLogger(IExecutorRuntime? runtime) => _runtime = runtime;
        public void Log(ValidationLogEvent logEvent)
        {
            _runtime?.LogMessage(new LogMessage
            {
                EventType = logEvent.Level switch
                {
                    ValidationLogLevel.Error => TraceEventType.Error,
                    ValidationLogLevel.Warning => TraceEventType.Warning,
                    ValidationLogLevel.Trace => TraceEventType.Verbose,
                    _ => TraceEventType.Information
                },
                Message = $"[{logEvent.ActivityType}] {logEvent.EventName}: {logEvent.Message} | {SafeLog.SerializeContext(logEvent.Context)}"
            });
        }
    }
}
