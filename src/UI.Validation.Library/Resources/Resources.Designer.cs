#nullable enable
using System.Globalization;
using System.Resources;

namespace Vodafone.Robotics.UiValidation.Resources;

public static class Resources
{
    private static readonly ResourceManager Manager = new("Vodafone.Robotics.UiValidation.Resources.Resources", typeof(Resources).Assembly);
    public static ResourceManager ResourceManager => Manager;
    public static CultureInfo? Culture { get; set; }
    public static string Category_UIValidation => Manager.GetString(nameof(Category_UIValidation), Culture) ?? "UI Validation";
    public static string ValidatedGetText_DisplayName => Manager.GetString(nameof(ValidatedGetText_DisplayName), Culture) ?? "Validated Get Text";
    public static string ValidatedGetText_Description => Manager.GetString(nameof(ValidatedGetText_Description), Culture) ?? string.Empty;
    public static string ValidatedTypeInto_DisplayName => Manager.GetString(nameof(ValidatedTypeInto_DisplayName), Culture) ?? "Validated Type Into";
    public static string ValidatedTypeInto_Description => Manager.GetString(nameof(ValidatedTypeInto_Description), Culture) ?? string.Empty;
    public static string ValidatedClick_DisplayName => Manager.GetString(nameof(ValidatedClick_DisplayName), Culture) ?? "Validated Click";
    public static string ValidatedClick_Description => Manager.GetString(nameof(ValidatedClick_Description), Culture) ?? string.Empty;
}
