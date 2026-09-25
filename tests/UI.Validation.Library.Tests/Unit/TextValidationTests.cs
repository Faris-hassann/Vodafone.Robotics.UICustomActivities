using Vodafone.Robotics.UiValidation.Models;
using Vodafone.Robotics.UiValidation.Validation;

namespace UI.Validation.Library.Tests.Unit;

public sealed class TextValidationTests
{
    [Theory]
    [InlineData(null, RawValueState.NoValue)]
    [InlineData("", RawValueState.Empty)]
    [InlineData("   ", RawValueState.WhitespaceOnly)]
    [InlineData("Ready", RawValueState.NonEmpty)]
    public void GT_020_to_024_Classifies_raw_states(string? value, RawValueState expected) => Assert.Equal(expected, TextValidation.Classify(value));

    [Fact]
    public void GT_039_Trim_is_validation_only()
    {
        const string raw = "  Ready  ";
        var options = new TextComparisonOptions(TrimForValidation: true);
        Assert.True(TextValidation.Equals(raw, "Ready", options));
        Assert.Equal("  Ready  ", raw);
    }

    [Theory]
    [InlineData(TextRule.Exact, "Ready", "Ready", true)]
    [InlineData(TextRule.Exact, "ready", "Ready", false)]
    [InlineData(TextRule.Contains, "Hello world", "world", true)]
    [InlineData(TextRule.StartsWith, "Hello world", "Hello", true)]
    [InlineData(TextRule.EndsWith, "Hello world", "world", true)]
    [InlineData(TextRule.Regex, "ABC-123", "^[A-Z]+-\\d+$", true)]
    public void GT_030_to_038_Applies_text_rules(TextRule rule, string actual, string expected, bool success)
    {
        var config = new GetTextConfiguration { ActivityType = "test", ActivityName = "test", WorkSelector = "x", Rules = new[] { rule }, ExpectedText = rule == TextRule.Regex ? null : expected, RegexPattern = rule == TextRule.Regex ? expected : null };
        Assert.Equal(success, TextValidation.ValidateRules(actual, config, out _));
    }

    [Fact]
    public void GT_036_Multiple_rules_use_AND()
    {
        var config = new GetTextConfiguration { ActivityType = "test", ActivityName = "test", WorkSelector = "x", Rules = new[] { TextRule.NotEmpty, TextRule.StartsWith, TextRule.EndsWith }, ExpectedText = "A" };
        Assert.False(TextValidation.ValidateRules("ABC", config, out _));
    }

    [Theory]
    [InlineData("abc", "abc", "abc", MismatchKind.ExactMatch)]
    [InlineData("", "abcdef", "abc", MismatchKind.PartialInput)]
    [InlineData("", "abc", "xabc", MismatchKind.UnexpectedPrefix)]
    [InlineData("", "abc", "abcx", MismatchKind.UnexpectedSuffix)]
    [InlineData("", "abc", "abcabc", MismatchKind.DuplicateInput)]
    [InlineData("old", "new", "old", MismatchKind.NoChange)]
    [InlineData("", "abc", "ABC", MismatchKind.UnexpectedTransformation)]
    public void TI_070_to_075_Classifies_mismatches(string original, string expected, string actual, MismatchKind kind) => Assert.Equal(kind, TextValidation.ClassifyMismatch(original, expected, actual));
}
