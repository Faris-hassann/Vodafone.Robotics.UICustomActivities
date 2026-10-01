using System.Xml.Linq;
using System.Text.RegularExpressions;

namespace Vodafone.Robotics.UiValidation.ArtifactBuilder;

internal sealed record PackageMetadata(string PackageId, string Version)
{
    public string GetExpectedPackagePath(string repositoryRoot) => Path.Combine(
        repositoryRoot,
        "artifacts",
        "packages",
        $"{PackageId}.{Version}.nupkg");
}

internal sealed class PackageConfigurationException(string message) : Exception(message);

internal static class ArtifactLocator
{
    private static readonly Regex StableVersionPattern = new(
        @"^(0|[1-9][0-9]*)\.(0|[1-9][0-9]*)\.(0|[1-9][0-9]*)$",
        RegexOptions.CultureInvariant);

    public static PackageMetadata ReadMetadata(string repositoryRoot)
    {
        var projectPath = Path.Combine(
            repositoryRoot,
            "src",
            "UI.Validation.Library.Packaging",
            "UI.Validation.Library.Packaging.csproj");
        var buildPropertiesPath = Path.Combine(repositoryRoot, "Directory.Build.props");

        try
        {
            var projectDocument = XDocument.Load(projectPath);
            var buildPropertiesDocument = XDocument.Load(buildPropertiesPath);
            var packageId = FindValue(projectDocument, "PackageId");
            var version = FindValue(buildPropertiesDocument, "UiValidationVersion");

            if (string.IsNullOrWhiteSpace(packageId))
            {
                throw new PackageConfigurationException($"PackageId is missing from '{projectPath}'.");
            }

            if (string.IsNullOrWhiteSpace(version))
            {
                throw new PackageConfigurationException(
                    $"UiValidationVersion is missing from '{buildPropertiesPath}'.");
            }

            if (!StableVersionPattern.IsMatch(version))
            {
                throw new PackageConfigurationException(
                    $"UiValidationVersion '{version}' is invalid. Use stable major.minor.patch format, for example 2.0.0.");
            }

            return new PackageMetadata(packageId, version);
        }
        catch (PackageConfigurationException)
        {
            throw;
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or System.Xml.XmlException)
        {
            throw new PackageConfigurationException($"Could not read package version configuration: {exception.Message}");
        }
    }

    public static string? FindExpectedPackage(string repositoryRoot, PackageMetadata metadata)
    {
        var packagePath = metadata.GetExpectedPackagePath(repositoryRoot);
        return File.Exists(packagePath) ? packagePath : null;
    }

    private static string? FindValue(XDocument document, string propertyName) => document
        .Descendants()
        .FirstOrDefault(element => element.Name.LocalName.Equals(propertyName, StringComparison.Ordinal))?
        .Value
        .Trim();
}
