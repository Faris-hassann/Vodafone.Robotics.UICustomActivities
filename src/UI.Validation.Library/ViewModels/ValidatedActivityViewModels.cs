using System.Activities.DesignViewModels;
using Vodafone.Robotics.UiValidation.Models;

namespace Vodafone.Robotics.UiValidation.ViewModels;

public abstract class ValidatedActivityViewModelBase : DesignPropertiesViewModel
{
    public DesignInArgument<string?> PreConditionSelector { get; set; } = null!;
    public DesignInArgument<ConditionKind> PreConditionKind { get; set; } = null!;
    public DesignInArgument<string?> PreConditionAttribute { get; set; } = null!;
    public DesignInArgument<string?> PreConditionExpectedValue { get; set; } = null!;
    public DesignInArgument<string> TargetSelector { get; set; } = null!;
    public DesignInArgument<bool> AllowMultipleMatches { get; set; } = null!;
    public DesignInArgument<bool> RequireVisible { get; set; } = null!;
    public DesignInArgument<bool> RequireEnabled { get; set; } = null!;
    public DesignInArgument<string?> PostConditionSelector { get; set; } = null!;
    public DesignInArgument<PostConditionKind> PostConditionKind { get; set; } = null!;
    public DesignInArgument<string?> PostConditionAttribute { get; set; } = null!;
    public DesignInArgument<string?> PostConditionExpectedValue { get; set; } = null!;
    public DesignInArgument<int> TimeoutMilliseconds { get; set; } = null!;
    public DesignInArgument<int> RetryCount { get; set; } = null!;
    public DesignInArgument<int> RetryIntervalMilliseconds { get; set; } = null!;
    public DesignInArgument<bool> CaseSensitive { get; set; } = null!;
    public DesignInArgument<bool> TrimForValidation { get; set; } = null!;
    public DesignInArgument<bool> NormalizeWhitespace { get; set; } = null!;
    public DesignInArgument<string?> CorrelationId { get; set; } = null!;
    public DesignInArgument<bool> LogSensitiveValues { get; set; } = null!;
    public DesignInArgument<bool> LogFullSelector { get; set; } = null!;
    public DesignInArgument<bool> ScreenshotOnFinalFailure { get; set; } = null!;
    public DesignInArgument<bool> ThrowOnFailure { get; set; } = null!;
    public DesignOutArgument<UIValidationResult> ValidationResult { get; set; } = null!;

    protected ValidatedActivityViewModelBase(IDesignServices services) : base(services) { }

    protected override void InitializeModel()
    {
        base.InitializeModel();
        var order = 0;
        Configure(PreConditionSelector, "PreCondition Selector", Help(nameof(PreConditionSelector), "Complete selector evaluated before Do Work."), false, false, ref order);
        Configure(PreConditionKind, "PreCondition Kind", Help(nameof(PreConditionKind), "Rule that gates Do Work."), false, false, ref order);
        Configure(PreConditionAttribute, "PreCondition Attribute", Help(nameof(PreConditionAttribute), "Attribute used by the precondition rule."), false, false, ref order);
        Configure(PreConditionExpectedValue, "PreCondition Expected Value", Help(nameof(PreConditionExpectedValue), "Expected state or value for the precondition."), false, false, ref order);
        Configure(TargetSelector, "Target Selector", Help(nameof(TargetSelector), "Required complete UiPath selector for the main operation."), true, true, ref order);
        Configure(AllowMultipleMatches, "Allow Multiple Matches", Help(nameof(AllowMultipleMatches), "Explicitly permits selection of the first match."), false, false, ref order);
        Configure(RequireVisible, "Require Visible", Help(nameof(RequireVisible), "Rejects a target that is not visible."), false, false, ref order);
        Configure(RequireEnabled, "Require Enabled", Help(nameof(RequireEnabled), "Rejects a target that is disabled."), false, false, ref order);
        ConfigureSpecific(ref order);
        Configure(PostConditionSelector, "PostCondition Selector", Help(nameof(PostConditionSelector), "Complete selector used to prove the outcome."), false, false, ref order);
        Configure(PostConditionKind, "PostCondition Kind", Help(nameof(PostConditionKind), "Outcome rule evaluated after Do Work."), false, false, ref order);
        Configure(PostConditionAttribute, "PostCondition Attribute", Help(nameof(PostConditionAttribute), "Attribute used by the outcome rule."), false, false, ref order);
        Configure(PostConditionExpectedValue, "PostCondition Expected Value", Help(nameof(PostConditionExpectedValue), "Expected outcome value."), false, false, ref order);
        Configure(TimeoutMilliseconds, "Timeout (ms)", Help(nameof(TimeoutMilliseconds), "Per-resolution timeout in milliseconds."), false, false, ref order);
        Configure(RetryCount, "Retry Count", Help(nameof(RetryCount), "Additional attempts after the first attempt."), false, false, ref order);
        Configure(RetryIntervalMilliseconds, "Retry Interval (ms)", Help(nameof(RetryIntervalMilliseconds), "Fixed delay between retries or polls."), false, false, ref order);
        Configure(CaseSensitive, "Case Sensitive", Help(nameof(CaseSensitive), "Uses ordinal case-sensitive comparison when enabled."), false, false, ref order);
        Configure(TrimForValidation, "Trim For Validation", Help(nameof(TrimForValidation), "Trims only the comparison view, never returned raw text."), false, false, ref order);
        Configure(NormalizeWhitespace, "Normalize Whitespace", Help(nameof(NormalizeWhitespace), "Normalizes only the comparison view."), false, false, ref order);
        Configure(CorrelationId, "Correlation ID", Help(nameof(CorrelationId), "Optional workflow-wide correlation identifier."), false, false, ref order);
        Configure(LogSensitiveValues, "Log Sensitive Values", Help(nameof(LogSensitiveValues), "Explicit opt-in; secure text remains redacted."), false, false, ref order);
        Configure(LogFullSelector, "Log Full Selector", Help(nameof(LogFullSelector), "Explicit opt-in to full selector logging."), false, false, ref order);
        Configure(ScreenshotOnFinalFailure, "Screenshot On Final Failure", Help(nameof(ScreenshotOnFinalFailure), "Captures a diagnostic image after the primary failure."), false, false, ref order);
        Configure(ThrowOnFailure, "Throw On Failure", Help(nameof(ThrowOnFailure), "Throws a classified exception instead of returning only a failed result."), false, false, ref order);
        Configure(ValidationResult, "Validation Result", Help(nameof(ValidationResult), "Structured lifecycle, verification, retry, and failure result."), false, ref order);
        PersistValuesChangedDuringInit();
    }

    protected abstract void ConfigureSpecific(ref int order);
    protected virtual string Help(string propertyName, string fallback) => CommonArgumentHelp.Get(propertyName, fallback);

    protected static void Configure<T>(DesignInArgument<T> property, string name, string tooltip, bool required, bool principal, ref int order)
    {
        property.DisplayName = name;
        property.Tooltip = tooltip;
        property.IsRequired = required;
        property.IsPrincipal = principal;
        property.OrderIndex = order++;
    }

    protected static void Configure<T>(DesignOutArgument<T> property, string name, string tooltip, bool principal, ref int order)
    {
        property.DisplayName = name;
        property.Tooltip = tooltip;
        property.IsPrincipal = principal;
        property.OrderIndex = order++;
    }
}

public sealed class ValidatedGetTextViewModel : ValidatedActivityViewModelBase
{
    public new DesignInArgument<TextRule> Rule { get; set; } = null!;
    public DesignInArgument<TextRule> AdditionalRule { get; set; } = null!;
    public DesignInArgument<string?> ExpectedText { get; set; } = null!;
    public DesignInArgument<string?> RegexPattern { get; set; } = null!;
    public DesignInArgument<bool> WaitForRules { get; set; } = null!;
    public DesignOutArgument<string?> Text { get; set; } = null!;
    public ValidatedGetTextViewModel(IDesignServices services) : base(services) { }
    protected override void ConfigureSpecific(ref int order)
    {
        Configure(Rule, "Text Rule", "Primary validation applied to the retrieved text. Example: NotEmpty.", false, true, ref order);
        Configure(AdditionalRule, "Additional Text Rule", "Optional second rule combined with the primary rule using AND semantics. Example: Contains.", false, false, ref order);
        Configure(ExpectedText, "Expected Text", "Value used by exact, contains, prefix, or suffix rules. Example: Ready.", false, true, ref order);
        Configure(RegexPattern, "Regex Pattern", "Regular expression used when Text Rule is Regex. Example: ^ORD-[0-9]+$.", false, false, ref order);
        Configure(WaitForRules, "Wait For Rules", "Retries while text rules are not yet satisfied. Example: true for text that loads asynchronously.", false, false, ref order);
        Configure(Text, "Raw Text", "Returns the unchanged text read from the target. Example: bind this output to a String variable.", false, ref order);
    }
}

public sealed class ValidatedTypeIntoViewModel : ValidatedActivityViewModelBase
{
    public DesignInArgument<TypeIntoMode> Mode { get; set; } = null!;
    public DesignInArgument<string?> InputText { get; set; } = null!;
    public DesignInArgument<string?> AppendSeparator { get; set; } = null!;
    public DesignInArgument<string?> ExpectedExistingValue { get; set; } = null!;
    public DesignInArgument<string?> ExpectedFinalValue { get; set; } = null!;
    public DesignInArgument<VerificationMode> VerificationMode { get; set; } = null!;
    public DesignInArgument<string?> VerificationAttribute { get; set; } = null!;
    public DesignInArgument<bool> IsSensitive { get; set; } = null!;
    public DesignInArgument<bool> IsSecure { get; set; } = null!;
    public DesignInArgument<bool> DisallowEmptyInput { get; set; } = null!;
    public ValidatedTypeIntoViewModel(IDesignServices services) : base(services) { }
    protected override string Help(string propertyName, string fallback) => TypeIntoArgumentHelp.Get(propertyName);
    protected override void ConfigureSpecific(ref int order)
    {
        Configure(Mode, "Mode", Help(nameof(Mode), string.Empty), false, true, ref order);
        Configure(InputText, "Input Text", Help(nameof(InputText), string.Empty), false, true, ref order);
        Configure(AppendSeparator, "Append Separator", Help(nameof(AppendSeparator), string.Empty), false, false, ref order);
        Configure(ExpectedExistingValue, "Expected Existing Value", Help(nameof(ExpectedExistingValue), string.Empty), false, false, ref order);
        Configure(ExpectedFinalValue, "Expected Final Value", Help(nameof(ExpectedFinalValue), string.Empty), false, false, ref order);
        Configure(VerificationMode, "Verification Mode", Help(nameof(VerificationMode), string.Empty), false, false, ref order);
        Configure(VerificationAttribute, "Verification Attribute", Help(nameof(VerificationAttribute), string.Empty), false, false, ref order);
        Configure(IsSensitive, "Is Sensitive", Help(nameof(IsSensitive), string.Empty), false, false, ref order);
        Configure(IsSecure, "Is Secure", Help(nameof(IsSecure), string.Empty), false, false, ref order);
        Configure(DisallowEmptyInput, "Disallow Empty Input", Help(nameof(DisallowEmptyInput), string.Empty), false, false, ref order);
    }
}

public sealed class ValidatedClickViewModel : ValidatedActivityViewModelBase
{
    public DesignInArgument<bool> AllowActionRetry { get; set; } = null!;
    public DesignInArgument<bool> SkipWhenPostConditionAlreadySatisfied { get; set; } = null!;
    public ValidatedClickViewModel(IDesignServices services) : base(services) { }
    protected override void ConfigureSpecific(ref int order)
    {
        Configure(AllowActionRetry, "Allow Action Retry", "Allows another click only after the postcondition remains false. Example: false for a Submit button.", false, false, ref order);
        Configure(SkipWhenPostConditionAlreadySatisfied, "Skip When Outcome Already Satisfied", "Avoids clicking when the desired postcondition already exists. Example: true for an already-open panel.", false, false, ref order);
    }
}
