namespace Vodafone.Robotics.UiValidation.ViewModels;

internal static class TypeIntoArgumentHelp
{
    internal static readonly IReadOnlyDictionary<string, string> Entries = new Dictionary<string, string>(StringComparer.Ordinal)
    {
        ["PreConditionSelector"] = "Complete UiPath selector checked before typing when a precondition is configured. Example: <wnd app='notepad.exe' /><ctrl name='Status' role='text' />.",
        ["PreConditionKind"] = "Rule that must pass before the target is changed; None skips the check. Example: TextEquals.",
        ["PreConditionAttribute"] = "Target attribute read by an AttributeEquals precondition. Example: enabled.",
        ["PreConditionExpectedValue"] = "Value required by the selected precondition rule. Example: Ready.",
        ["TargetSelector"] = "Required complete UiPath selector XML for the input control; selector attributes identify the target directly. Example: <wnd app='notepad.exe' /><ctrl automationid='SearchBox' role='editable text' />.",
        ["AllowMultipleMatches"] = "Allows the first selector match to be used when more than one element matches; false fails safely. Example: false.",
        ["RequireVisible"] = "Requires the selected input control to be visible before typing. Example: true.",
        ["RequireEnabled"] = "Type Into always requires an enabled input control for safe mutation; this shared option cannot disable that rule. Example: leave the default false because Type Into enforces it automatically.",
        ["Mode"] = "Controls whether text replaces, appends to, or clears the current value; the default is Replace. Example: Append.",
        ["InputText"] = "Text written in Replace or Append mode and redacted from logs by default. Example: CUST-001.",
        ["AppendSeparator"] = "Text inserted between the existing value and Input Text in Append mode; the default is empty. Example: a single space.",
        ["ExpectedExistingValue"] = "Optional value that must already be present before any typing occurs. Example: Customer:.",
        ["ExpectedFinalValue"] = "Optional final-value override used when the application intentionally transforms the typed text. Example: ABC for input abc.",
        ["VerificationMode"] = "Selects how the final value is read back; Auto chooses the strongest supported strategy. Example: ValueAttribute.",
        ["VerificationAttribute"] = "Attribute read when Verification Mode is SpecificAttribute; it is required in that mode. Example: value.",
        ["IsSensitive"] = "Redacts ordinary input text from logs unless sensitive logging is explicitly enabled. Example: true for customer data.",
        ["IsSecure"] = "Marks password-like input so its exact value is never logged or claimed as readable. Example: true for a password field.",
        ["DisallowEmptyInput"] = "Rejects empty Input Text in Replace or Append mode; ClearOnly remains valid. Example: true.",
        ["PostConditionSelector"] = "Complete UiPath selector used to verify an observable result after typing. Example: <wnd app='crm.exe' /><ctrl name='Saved' role='text' />.",
        ["PostConditionKind"] = "Outcome rule evaluated after typing; None disables external outcome verification. Example: TextEquals.",
        ["PostConditionAttribute"] = "Attribute read by attribute-based postconditions. Example: value.",
        ["PostConditionExpectedValue"] = "Expected value used by the selected postcondition rule. Example: Saved.",
        ["TimeoutMilliseconds"] = "Maximum time allowed for each selector resolution before it fails. Example: 10000.",
        ["RetryCount"] = "Additional attempts after the first attempt; total attempts equal one plus this value. Example: 2 gives up to 3 attempts.",
        ["RetryIntervalMilliseconds"] = "Delay between retries or verification polls. Example: 500.",
        ["CaseSensitive"] = "Uses ordinal case-sensitive value comparisons when enabled. Example: false treats abc and ABC as equal.",
        ["TrimForValidation"] = "Trims leading and trailing whitespace only for comparisons, without changing typed text. Example: true compares ' Ready ' with 'Ready'.",
        ["NormalizeWhitespace"] = "Collapses whitespace runs only for comparisons, without changing typed text. Example: true compares 'A   B' with 'A B'.",
        ["CorrelationId"] = "Optional identifier that groups this activity's logs with the surrounding workflow. Example: Order-4821.",
        ["LogSensitiveValues"] = "Explicitly permits non-secure sensitive values in logs; secure values remain redacted. Example: false.",
        ["LogFullSelector"] = "Explicitly permits complete selector XML in logs instead of a redacted selector. Example: false.",
        ["ScreenshotOnFinalFailure"] = "Captures a diagnostic screenshot after the final failure while preserving the primary error. Example: true.",
        ["ThrowOnFailure"] = "Throws a classified exception on failure when true; otherwise returns a failed Validation Result. Example: false.",
        ["ValidationResult"] = "Returns structured success, stage, attempts, verification, and failure details. Example: inspect ValidationResult.Success after execution."
    };

    internal static string Get(string propertyName) => Entries.TryGetValue(propertyName, out var help)
        ? help
        : throw new InvalidOperationException($"Type Into help is missing for '{propertyName}'.");
}
