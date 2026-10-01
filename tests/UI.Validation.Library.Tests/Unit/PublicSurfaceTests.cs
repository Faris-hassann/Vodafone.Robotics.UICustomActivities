using System.ComponentModel;
using System.Text.Json;
using Vodafone.Robotics.UiValidation.Activities;
using Vodafone.Robotics.UiValidation.Resources;
using Vodafone.Robotics.UiValidation.ViewModels;

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

    [Fact]
    public void Every_designer_uses_one_target_selector_and_has_no_identity_fields()
    {
        var identityProperties = new[]
        {
            "ExpectedTargetName", "ExpectedTargetText", "ExpectedTargetRole",
            "ExpectedTargetId", "ExpectedTargetClass", "ExpectedAutomationId"
        };

        foreach (var viewModelType in new[]
                 {
                     typeof(ValidatedGetTextViewModel),
                     typeof(ValidatedTypeIntoViewModel),
                     typeof(ValidatedClickViewModel)
                 })
        {
            var properties = DesignerArgumentNames(viewModelType);
            Assert.Contains(nameof(ValidatedActivityBase.TargetSelector), properties);
            Assert.DoesNotContain("DoWorkSelector", properties);
            Assert.DoesNotContain(properties, identityProperties.Contains);
        }
    }

    [Fact]
    public void TypeInto_every_visible_argument_has_purpose_and_example_help()
    {
        var visibleArguments = DesignerArgumentNames(typeof(ValidatedTypeIntoViewModel));

        Assert.Equal(visibleArguments.OrderBy(name => name), TypeIntoArgumentHelp.Entries.Keys.OrderBy(name => name));
        Assert.All(TypeIntoArgumentHelp.Entries.Values, tooltip =>
        {
            Assert.False(string.IsNullOrWhiteSpace(tooltip));
            Assert.Contains("Example:", tooltip, StringComparison.Ordinal);
            Assert.True(tooltip.IndexOf("Example:", StringComparison.Ordinal) > 0);
        });
    }

    [Fact]
    public void Runtime_exposes_only_TargetSelector_and_removes_legacy_target_arguments()
    {
        var runtimeProperties = typeof(ValidatedActivityBase).GetProperties().Select(property => property.Name).ToHashSet(StringComparer.Ordinal);
        var removedProperties = new[]
        {
            "DoWorkSelector", "ExpectedTargetName", "ExpectedTargetText", "ExpectedTargetRole",
            "ExpectedTargetId", "ExpectedTargetClass", "ExpectedAutomationId"
        };

        Assert.Contains(nameof(ValidatedActivityBase.TargetSelector), runtimeProperties);
        Assert.DoesNotContain(runtimeProperties, removedProperties.Contains);
    }

    [Fact]
    public void Every_common_designer_argument_has_purpose_and_example_help()
    {
        var commonArguments = DesignerArgumentNames(typeof(ValidatedActivityViewModelBase));

        Assert.Equal(commonArguments.OrderBy(name => name), CommonArgumentHelp.Entries.Keys.OrderBy(name => name));
        Assert.All(CommonArgumentHelp.Entries.Values, AssertPurposeAndExample);
    }

    private static void AssertPurposeAndExample(string tooltip)
    {
        Assert.False(string.IsNullOrWhiteSpace(tooltip));
        Assert.Contains("Example:", tooltip, StringComparison.Ordinal);
        Assert.True(tooltip.IndexOf("Example:", StringComparison.Ordinal) > 0);
    }

    private static HashSet<string> DesignerArgumentNames(Type viewModelType)
    {
        return viewModelType.GetProperties()
            .Where(property => property.DeclaringType?.Namespace == typeof(ValidatedTypeIntoViewModel).Namespace)
            .Where(property => property.PropertyType.Name.StartsWith("DesignInArgument", StringComparison.Ordinal)
                || property.PropertyType.Name.StartsWith("DesignOutArgument", StringComparison.Ordinal))
            .Select(property => property.Name)
            .ToHashSet(StringComparer.Ordinal);
    }
}
