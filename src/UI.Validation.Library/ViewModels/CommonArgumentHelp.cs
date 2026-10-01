namespace Vodafone.Robotics.UiValidation.ViewModels;

internal static class CommonArgumentHelp
{
    internal static readonly IReadOnlyDictionary<string, string> Entries = new Dictionary<string, string>(StringComparer.Ordinal)
    {
        ["PreConditionSelector"] = "Complete UiPath selector checked before the main action when a precondition is configured. Example: <wnd app='app.exe' /><ctrl name='Ready' />.",
        ["PreConditionKind"] = "Rule that must pass before the main action; None skips this check. Example: ElementExists.",
        ["PreConditionAttribute"] = "Attribute read by an AttributeEquals precondition. Example: enabled.",
        ["PreConditionExpectedValue"] = "Value required by the selected precondition rule. Example: true.",
        ["TargetSelector"] = "Required complete UiPath selector for the element used by the main activity action. Example: <wnd app='app.exe' /><ctrl automationid='Submit' />.",
        ["AllowMultipleMatches"] = "Uses the first match when multiple elements match; false fails safely. Example: false.",
        ["RequireVisible"] = "Requires the selected target to be visible before the action. Example: true.",
        ["RequireEnabled"] = "Requires an enabled target when selected; Type Into and Click always enforce this safety rule. Example: enable it for Get Text when disabled controls must be rejected.",
        ["PostConditionSelector"] = "Complete UiPath selector used to verify the outcome after the action. Example: <wnd app='app.exe' /><ctrl name='Saved' />.",
        ["PostConditionKind"] = "Outcome rule evaluated after the action; None disables outcome verification. Example: ElementAppears.",
        ["PostConditionAttribute"] = "Attribute read by attribute-based postconditions. Example: value.",
        ["PostConditionExpectedValue"] = "Expected value used by the selected postcondition rule. Example: Saved.",
        ["TimeoutMilliseconds"] = "Maximum time allowed for each selector resolution. Example: 10000.",
        ["RetryCount"] = "Additional attempts after the first; total attempts equal one plus this value. Example: 2 gives up to 3 attempts.",
        ["RetryIntervalMilliseconds"] = "Delay between retries or verification polls. Example: 500.",
        ["CaseSensitive"] = "Uses ordinal case-sensitive comparisons when enabled. Example: false treats ready and Ready as equal.",
        ["TrimForValidation"] = "Trims leading and trailing whitespace only during comparison. Example: true compares ' Ready ' with 'Ready'.",
        ["NormalizeWhitespace"] = "Collapses whitespace runs only during comparison. Example: true compares 'A   B' with 'A B'.",
        ["CorrelationId"] = "Optional identifier used to group activity logs with a workflow. Example: Order-4821.",
        ["LogSensitiveValues"] = "Allows non-secure sensitive values in logs; secure values remain redacted. Example: false.",
        ["LogFullSelector"] = "Allows complete selector XML in logs instead of a redacted selector. Example: false.",
        ["ScreenshotOnFinalFailure"] = "Captures a diagnostic screenshot after the final failure. Example: true.",
        ["ThrowOnFailure"] = "Throws a classified exception on failure; false returns a failed Validation Result. Example: true.",
        ["ValidationResult"] = "Returns structured success, stage, retry, verification, and failure details. Example: inspect ValidationResult.Success."
    };

    internal static string Get(string propertyName, string fallback) =>
        Entries.TryGetValue(propertyName, out var help) ? help : fallback;
}
