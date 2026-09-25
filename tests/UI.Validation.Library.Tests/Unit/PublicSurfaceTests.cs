using System.ComponentModel;
using System.Text.Json;
using Vodafone.Robotics.UiValidation.Activities;
using Vodafone.Robotics.UiValidation.Resources;

namespace UI.Validation.Library.Tests.Unit;

public sealed class PublicSurfaceTests
{
    [Fact]
    public void AC_G01_Exactly_three_concrete_public_activities_are_exposed()
    {
        var activities = typeof(ValidatedGetText).Assembly.ExportedTypes
            .Where(t => typeof(ValidatedActivityBase).IsAssignableFrom(t) && !t.IsAbstract)
            .ToArray();
        Assert.Equal(new[] { typeof(ValidatedClick), typeof(ValidatedGetText), typeof(ValidatedTypeInto) }, activities.OrderBy(t => t.Name).ToArray());
    }

    [Theory]
    [InlineData(typeof(ValidatedGetText), "Validated Get Text")]
    [InlineData(typeof(ValidatedTypeInto), "Validated Type Into")]
    [InlineData(typeof(ValidatedClick), "Validated Click")]
    public void AC_T05_Activities_have_professional_Studio_metadata(Type type, string displayName)
    {
        Assert.Equal(displayName, type.GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().Single().DisplayName);
        Assert.Equal("UI Validation", type.GetCustomAttributes(typeof(CategoryAttribute), false).Cast<CategoryAttribute>().Single().Category);
        Assert.NotEmpty(type.GetCustomAttributes(typeof(DescriptionAttribute), false).Cast<DescriptionAttribute>().Single().Description);
    }

    [Fact]
    public void ActivitiesMetadata_declares_three_resolvable_modern_ViewModels()
    {
        var assembly = typeof(ValidatedGetText).Assembly;
        using var stream = assembly.GetManifestResourceStream("Vodafone.Robotics.UiValidation.Resources.ActivitiesMetadata.json");
        Assert.NotNull(stream);
        using var document = JsonDocument.Parse(stream!);
        var activities = document.RootElement.GetProperty("activities").EnumerateArray().ToArray();
        Assert.Equal(3, activities.Length);
        foreach (var activity in activities)
        {
            Assert.NotNull(assembly.GetType(activity.GetProperty("fullName").GetString()!, throwOnError: false));
            Assert.NotNull(assembly.GetType(activity.GetProperty("viewModelType").GetString()!, throwOnError: false));
            Assert.False(string.IsNullOrWhiteSpace(Resources.ResourceManager.GetString(activity.GetProperty("displayNameKey").GetString()!)));
        }
    }
}
