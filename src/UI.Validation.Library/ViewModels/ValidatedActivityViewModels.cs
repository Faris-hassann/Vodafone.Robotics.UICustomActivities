using System.Activities.DesignViewModels;
using Vodafone.Robotics.UiValidation.Models;

namespace Vodafone.Robotics.UiValidation.ViewModels;

public abstract class ValidatedActivityViewModelBase : DesignPropertiesViewModel
{
    public DesignInArgument<string?> PreConditionSelector { get; set; } = null!;
    public DesignInArgument<ConditionKind> PreConditionKind { get; set; } = null!;
    public DesignInArgument<string?> PreConditionAttribute { get; set; } = null!;
    public DesignInArgument<string?> PreConditionExpectedValue { get; set; } = null!;
    public DesignInArgument<string> DoWorkSelector { get; set; } = null!;
    public DesignInArgument<bool> AllowMultipleMatches { get; set; } = null!;
    public DesignInArgument<string?> ExpectedTargetName { get; set; } = null!;
    public DesignInArgument<string?> ExpectedTargetText { get; set; } = null!;
    public DesignInArgument<string?> ExpectedTargetRole { get; set; } = null!;
    public DesignInArgument<string?> ExpectedTargetId { get; set; } = null!;
    public DesignInArgument<string?> ExpectedTargetClass { get; set; } = null!;
    public DesignInArgument<string?> ExpectedAutomationId { get; set; } = null!;
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
        Configure(PreConditionSelector, "PreCondition Selector", "Complete selector evaluated before Do Work.", false, false, ref order);
        Configure(PreConditionKind, "PreCondition Kind", "Rule that gates Do Work.", false, false, ref order);
        Configure(PreConditionAttribute, "PreCondition Attribute", "Attribute used by the precondition rule.", false, false, ref order);
        Configure(PreConditionExpectedValue, "PreCondition Expected Value", "Expected state or value for the precondition.", false, false, ref order);
        Configure(DoWorkSelector, "Do Work Selector", "Complete UiPath selector for the main operation.", true, true, ref order);
        Configure(AllowMultipleMatches, "Allow Multiple Matches", "Explicitly permits selection of the first match.", false, false, ref order);
        Configure(ExpectedTargetName, "Expected Target Name", "Optional identity check.", false, false, ref order);
        Configure(ExpectedTargetText, "Expected Target Text", "Optional identity check.", false, false, ref order);
        Configure(ExpectedTargetRole, "Expected Target Role", "Optional identity check.", false, false, ref order);
        Configure(ExpectedTargetId, "Expected Target ID", "Optional identity check.", false, false, ref order);
        Configure(ExpectedTargetClass, "Expected Target Class", "Optional identity check.", false, false, ref order);
        Configure(ExpectedAutomationId, "Expected Automation ID", "Optional identity check.", false, false, ref order);
        Configure(RequireVisible, "Require Visible", "Rejects a target that is not visible.", false, false, ref order);
        Configure(RequireEnabled, "Require Enabled", "Rejects a target that is disabled.", false, false, ref order);
        ConfigureSpecific(ref order);
        Configure(PostConditionSelector, "PostCondition Selector", "Complete selector used to prove the outcome.", false, false, ref order);
        Configure(PostConditionKind, "PostCondition Kind", "Outcome rule evaluated after Do Work.", false, false, ref order);
        Configure(PostConditionAttribute, "PostCondition Attribute", "Attribute used by the outcome rule.", false, false, ref order);
        Configure(PostConditionExpectedValue, "PostCondition Expected Value", "Expected outcome value.", false, false, ref order);
        Configure(TimeoutMilliseconds, "Timeout (ms)", "Per-resolution timeout in milliseconds.", false, false, ref order);
        Configure(RetryCount, "Retry Count", "Additional attempts after the first attempt.", false, false, ref order);
        Configure(RetryIntervalMilliseconds, "Retry Interval (ms)", "Fixed delay between retries or polls.", false, false, ref order);
        Configure(CaseSensitive, "Case Sensitive", "Uses ordinal case-sensitive comparison when enabled.", false, false, ref order);
        Configure(TrimForValidation, "Trim For Validation", "Trims only the comparison view, never returned raw text.", false, false, ref order);
        Configure(NormalizeWhitespace, "Normalize Whitespace", "Normalizes only the comparison view.", false, false, ref order);
        Configure(CorrelationId, "Correlation ID", "Optional workflow-wide correlation identifier.", false, false, ref order);
        Configure(LogSensitiveValues, "Log Sensitive Values", "Explicit opt-in; secure text remains redacted.", false, false, ref order);
        Configure(LogFullSelector, "Log Full Selector", "Explicit opt-in to full selector logging.", false, false, ref order);
        Configure(ScreenshotOnFinalFailure, "Screenshot On Final Failure", "Captures a diagnostic image after the primary failure.", false, false, ref order);
        Configure(ThrowOnFailure, "Throw On Failure", "Throws a classified exception instead of returning only a failed result.", false, false, ref order);
        Configure(ValidationResult, "Validation Result", "Structured lifecycle, verification, retry, and failure result.", false, ref order);
        PersistValuesChangedDuringInit();
    }

    protected abstract void ConfigureSpecific(ref int order);

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
        Configure(Rule, "Text Rule", "Primary validation rule.", false, true, ref order);
        Configure(AdditionalRule, "Additional Text Rule", "Optional second rule combined with AND semantics.", false, false, ref order);
        Configure(ExpectedText, "Expected Text", "Value used by exact/contains/prefix/suffix rules.", false, true, ref order);
        Configure(RegexPattern, "Regex Pattern", "Pattern used by the Regex rule.", false, false, ref order);
        Configure(WaitForRules, "Wait For Rules", "Treats text-rule mismatch as waitable within retry limits.", false, false, ref order);
        Configure(Text, "Raw Text", "Unchanged text returned by the target.", false, ref order);
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
    protected override void ConfigureSpecific(ref int order)
    {
        Configure(Mode, "Mode", "Replace, Append, or ClearOnly.", false, true, ref order);
        Configure(InputText, "Input Text", "Text to write; redacted in logs by default.", false, true, ref order);
        Configure(AppendSeparator, "Append Separator", "Separator inserted between original and appended text.", false, false, ref order);
        Configure(ExpectedExistingValue, "Expected Existing Value", "Optional pre-mutation value check.", false, false, ref order);
        Configure(ExpectedFinalValue, "Expected Final Value", "Override for intentional application transformations.", false, false, ref order);
        Configure(VerificationMode, "Verification Mode", "Read-back strategy used after typing.", false, false, ref order);
        Configure(VerificationAttribute, "Verification Attribute", "Required for SpecificAttribute mode.", false, false, ref order);
        Configure(IsSensitive, "Is Sensitive", "Marks ordinary input as sensitive.", false, false, ref order);
        Configure(IsSecure, "Is Secure", "Prevents exact value logging/read-back claims.", false, false, ref order);
        Configure(DisallowEmptyInput, "Disallow Empty Input", "Rejects an empty non-ClearOnly request.", false, false, ref order);
    }
}

public sealed class ValidatedClickViewModel : ValidatedActivityViewModelBase
{
    public DesignInArgument<bool> AllowActionRetry { get; set; } = null!;
    public DesignInArgument<bool> SkipWhenPostConditionAlreadySatisfied { get; set; } = null!;
    public ValidatedClickViewModel(IDesignServices services) : base(services) { }
    protected override void ConfigureSpecific(ref int order)
    {
        Configure(AllowActionRetry, "Allow Action Retry", "Allows another click only after the postcondition remains false.", false, false, ref order);
        Configure(SkipWhenPostConditionAlreadySatisfied, "Skip When Outcome Already Satisfied", "Avoids a click when the desired state already exists.", false, false, ref order);
    }
}
