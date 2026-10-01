using Vodafone.Robotics.UiValidation.Automation;

namespace UI.Validation.Library.Tests.Unit;

public sealed class UiPathAttributeParserTests
{
    [Theory]
    [InlineData(true, true)]
    [InlineData(false, false)]
    [InlineData(" true ", true)]
    [InlineData("FALSE", false)]
    [InlineData("1", true)]
    [InlineData("0", false)]
    [InlineData(1, true)]
    [InlineData(0, false)]
    [InlineData("enabled", true)]
    [InlineData("available", true)]
    [InlineData("disabled", false)]
    [InlineData("unavailable", false)]
    public void Supported_enabled_values_are_parsed_without_conversion_errors(object value, bool expected)
    {
        var parsed = UiPathAttributeParser.TryGetBoolean(value, out var actual);

        Assert.True(parsed);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(null, null, true)]
    [InlineData("", "", true)]
    [InlineData("   ", "focusable", true)]
    [InlineData("provider-specific", "focusable", true)]
    [InlineData("", "disabled, focusable", false)]
    [InlineData(null, "unavailable", false)]
    public void Inconclusive_enabled_value_falls_back_to_accessibility_state(
        object? enabledValue,
        object? accessibilityState,
        bool expected)
    {
        var actual = UiPathAttributeParser.ResolveEnabled(enabledValue, accessibilityState);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Empty_and_unknown_values_are_inconclusive_instead_of_throwing()
    {
        Assert.False(UiPathAttributeParser.TryGetBoolean(string.Empty, out _));
        Assert.False(UiPathAttributeParser.TryGetBoolean("provider-specific", out _));
    }
}
