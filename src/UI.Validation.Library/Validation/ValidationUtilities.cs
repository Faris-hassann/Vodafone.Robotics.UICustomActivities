using System.Text.RegularExpressions;
using Vodafone.Robotics.UiValidation.Automation;
using Vodafone.Robotics.UiValidation.Exceptions;
using Vodafone.Robotics.UiValidation.Models;

namespace Vodafone.Robotics.UiValidation.Validation;

public static class ConfigurationValidator
{
    public static void Validate(ActivityConfiguration configuration)
    {
        if (string.IsNullOrWhiteSpace(configuration.WorkSelector)) throw new UIConfigurationException("Do Work selector is required.");
        if (configuration.TimeoutMilliseconds <= 0) throw new UIConfigurationException("TimeoutMilliseconds must be greater than zero.");
        if (configuration.RetryCount < 0) throw new UIConfigurationException("RetryCount cannot be negative.");
        if (configuration.RetryIntervalMilliseconds < 0) throw new UIConfigurationException("RetryIntervalMilliseconds cannot be negative.");
        Validate(configuration.PreCondition);
        Validate(configuration.PostCondition);

        if (configuration is GetTextConfiguration getText)
        {
            if (getText.Rules.Count == 0) throw new UIConfigurationException("At least one Get Text rule is required.");
            if (getText.Rules.Contains(TextRule.Regex))
            {
                if (string.IsNullOrEmpty(getText.RegexPattern)) throw new UIConfigurationException("RegexPattern is required when the Regex rule is enabled.");
                try { _ = new Regex(getText.RegexPattern, RegexOptions.CultureInvariant); }
                catch (ArgumentException ex) { throw new UIConfigurationException($"RegexPattern is invalid: {ex.Message}"); }
            }
            if (getText.Rules.Any(r => r is TextRule.Exact or TextRule.Contains or TextRule.StartsWith or TextRule.EndsWith) && getText.ExpectedText is null)
                throw new UIConfigurationException("ExpectedText is required by the selected text rule.");
        }

        if (configuration is TypeIntoConfiguration typeInto)
        {
            if (!Enum.IsDefined(typeof(TypeIntoMode), typeInto.Mode)) throw new UIConfigurationException("Type Into mode is invalid.");
            if (!Enum.IsDefined(typeof(VerificationMode), typeInto.VerificationMode)) throw new UIConfigurationException("Verification mode is invalid.");
            if (typeInto.DisallowEmptyInput && typeInto.Mode != TypeIntoMode.ClearOnly && string.IsNullOrEmpty(typeInto.InputText))
                throw new UIConfigurationException("InputText cannot be empty when DisallowEmptyInput is enabled.");
            if (typeInto.VerificationMode == VerificationMode.SpecificAttribute && string.IsNullOrWhiteSpace(typeInto.VerificationAttribute))
                throw new UIConfigurationException("VerificationAttribute is required for SpecificAttribute verification.");
            if (typeInto.IsSecure && typeInto.VerificationMode is VerificationMode.Text or VerificationMode.ValueAttribute or VerificationMode.SpecificAttribute)
                throw new UIConfigurationException("Secure input cannot require exact control read-back. Use Auto or ActionOnly with an external postcondition.");
        }
    }

    private static void Validate(ConditionDefinition condition)
    {
        if (condition.Kind == ConditionKind.None) return;
        if (string.IsNullOrWhiteSpace(condition.Selector)) throw new UIConfigurationException("PreCondition selector is required when a precondition is configured.");
        if (condition.Kind == ConditionKind.AttributeEquals && string.IsNullOrWhiteSpace(condition.AttributeName)) throw new UIConfigurationException("PreCondition AttributeName is required for AttributeEquals.");
        if (condition.Kind is ConditionKind.TextEquals or ConditionKind.TextContains or ConditionKind.AttributeEquals && condition.ExpectedValue is null)
            throw new UIConfigurationException("PreCondition ExpectedValue is required by the selected condition.");
    }

    private static void Validate(PostConditionDefinition condition)
    {
        if (condition.Kind == PostConditionKind.None) return;
        if (string.IsNullOrWhiteSpace(condition.Selector)) throw new UIConfigurationException("PostCondition selector is required when a postcondition is configured.");
        if (condition.Kind is PostConditionKind.AttributeEquals or PostConditionKind.AttributeChanges && string.IsNullOrWhiteSpace(condition.AttributeName))
            throw new UIConfigurationException("PostCondition AttributeName is required by the selected condition.");
        if (condition.Kind is PostConditionKind.TextEquals or PostConditionKind.TextContains or PostConditionKind.AttributeEquals && condition.ExpectedValue is null)
            throw new UIConfigurationException("PostCondition ExpectedValue is required by the selected condition.");
    }
}

public static class TextValidation
{
    public static RawValueState Classify(string? value, bool readSucceeded = true)
    {
        if (!readSucceeded) return RawValueState.ReadFailed;
        if (value is null) return RawValueState.NoValue;
        if (value.Length == 0) return RawValueState.Empty;
        return string.IsNullOrWhiteSpace(value) ? RawValueState.WhitespaceOnly : RawValueState.NonEmpty;
    }

    public static string Normalize(string value, TextComparisonOptions options)
    {
        var normalized = options.TrimForValidation ? value.Trim() : value;
        return options.NormalizeWhitespace ? Regex.Replace(normalized, @"\s+", " ") : normalized;
    }

    public static bool Equals(string? actual, string? expected, TextComparisonOptions options)
    {
        if (actual is null || expected is null) return actual == expected;
        var comparison = options.CaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
        return string.Equals(Normalize(actual, options), Normalize(expected, options), comparison);
    }

    public static bool ValidateRules(string? raw, GetTextConfiguration configuration, out string reason)
    {
        var actual = raw is null ? null : Normalize(raw, configuration.Comparison);
        var expected = configuration.ExpectedText is null ? null : Normalize(configuration.ExpectedText, configuration.Comparison);
        var comparison = configuration.Comparison.CaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
        foreach (var rule in configuration.Rules)
        {
            var passed = rule switch
            {
                TextRule.None => true,
                TextRule.RetrievedSuccessfully => true,
                TextRule.NotEmpty => !string.IsNullOrEmpty(raw),
                TextRule.Exact => string.Equals(actual, expected, comparison),
                TextRule.Contains => actual?.Contains(expected!, comparison) == true,
                TextRule.StartsWith => actual?.StartsWith(expected!, comparison) == true,
                TextRule.EndsWith => actual?.EndsWith(expected!, comparison) == true,
                TextRule.Regex => actual is not null && Regex.IsMatch(actual, configuration.RegexPattern!, configuration.Comparison.CaseSensitive ? RegexOptions.CultureInvariant : RegexOptions.CultureInvariant | RegexOptions.IgnoreCase),
                _ => false
            };
            if (!passed) { reason = $"Text rule '{rule}' failed."; return false; }
        }
        reason = string.Empty;
        return true;
    }

    public static MismatchKind ClassifyMismatch(string? original, string expected, string? actual)
    {
        if (actual == expected) return MismatchKind.ExactMatch;
        if (actual == original) return MismatchKind.NoChange;
        if (!string.IsNullOrEmpty(expected) && actual == expected + expected) return MismatchKind.DuplicateInput;
        if (actual is not null && expected.StartsWith(actual, StringComparison.Ordinal)) return MismatchKind.PartialInput;
        if (actual is not null && actual.EndsWith(expected, StringComparison.Ordinal)) return MismatchKind.UnexpectedPrefix;
        if (actual is not null && actual.StartsWith(expected, StringComparison.Ordinal)) return MismatchKind.UnexpectedSuffix;
        if (actual is not null && string.Equals(actual, expected, StringComparison.OrdinalIgnoreCase)) return MismatchKind.UnexpectedTransformation;
        return MismatchKind.UnclassifiedMismatch;
    }
}

public static class TargetValidation
{
    public static void Validate(IUiTarget target, TargetIdentityOptions identity, TextComparisonOptions comparison)
    {
        if (identity.RequireVisible && !target.IsVisible) throw new UITargetIdentityException("Target is not visible.");
        if (identity.RequireEnabled && !target.IsEnabled) throw new UITargetIdentityException("Target is not enabled.");
        Check(target, "name", identity.ExpectedName, comparison);
        if (identity.ExpectedText is not null && !TextValidation.Equals(target.GetText(), identity.ExpectedText, comparison)) throw new UITargetIdentityException("Target text does not match the configured identity.");
        Check(target, "role", identity.ExpectedRole, comparison);
        Check(target, "id", identity.ExpectedId, comparison);
        Check(target, "class", identity.ExpectedClass, comparison);
        Check(target, "automationid", identity.ExpectedAutomationId, comparison);
    }

    private static void Check(IUiTarget target, string attribute, string? expected, TextComparisonOptions comparison)
    {
        if (expected is null) return;
        var actual = target.GetAttribute(attribute)?.ToString();
        if (!TextValidation.Equals(actual, expected, comparison)) throw new UITargetIdentityException($"Target attribute '{attribute}' does not match the configured identity.");
    }
}
