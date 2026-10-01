using System.Globalization;

namespace Vodafone.Robotics.UiValidation.Automation;

internal static class UiPathAttributeParser
{
    internal static bool ResolveEnabled(object? enabledValue, object? accessibilityState)
    {
        if (TryGetBoolean(enabledValue, out var enabled))
        {
            return enabled;
        }

        var state = accessibilityState?.ToString() ?? string.Empty;
        return !state.Contains("disabled", StringComparison.OrdinalIgnoreCase)
            && !state.Contains("unavailable", StringComparison.OrdinalIgnoreCase);
    }

    internal static bool TryGetBoolean(object? value, out bool result)
    {
        if (value is bool boolean)
        {
            result = boolean;
            return true;
        }

        var text = value?.ToString()?.Trim();
        if (string.IsNullOrEmpty(text))
        {
            result = default;
            return false;
        }

        if (bool.TryParse(text, out result))
        {
            return true;
        }

        if (long.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var number)
            && number is 0 or 1)
        {
            result = number == 1;
            return true;
        }

        if (text.Equals("enabled", StringComparison.OrdinalIgnoreCase)
            || text.Equals("available", StringComparison.OrdinalIgnoreCase)
            || text.Equals("yes", StringComparison.OrdinalIgnoreCase)
            || text.Equals("on", StringComparison.OrdinalIgnoreCase))
        {
            result = true;
            return true;
        }

        if (text.Equals("disabled", StringComparison.OrdinalIgnoreCase)
            || text.Equals("unavailable", StringComparison.OrdinalIgnoreCase)
            || text.Equals("no", StringComparison.OrdinalIgnoreCase)
            || text.Equals("off", StringComparison.OrdinalIgnoreCase))
        {
            result = false;
            return true;
        }

        result = default;
        return false;
    }
}
