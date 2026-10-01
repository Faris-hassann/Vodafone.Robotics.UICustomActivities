using System.Xml.Linq;

namespace Vodafone.Robotics.UiValidation.ArtifactBuilder;

internal static class ArtifactLocator
{
    public static string? FindExpectedPackage(string repositoryRoot)
    {
        var projectPath = Path.Combine(
            repositoryRoot,
            "src",
            "UI.Validation.Library.Packaging",
            "UI.Validation.Library.Packaging.csproj");

        try
        {
            var document = XDocument.Load(projectPath);
            var packageId = document.Descendants("PackageId").Select(element => element.Value).FirstOrDefault();
            var version = document.Descendants("Version").Select(element => element.Value).FirstOrDefault();
            if (string.IsNullOrWhiteSpace(packageId) || string.IsNullOrWhiteSpace(version))
            {
                return null;
            }

            var packagePath = Path.Combine(repositoryRoot, "artifacts", "packages", $"{packageId}.{version}.nupkg");
            return File.Exists(packagePath) ? packagePath : null;
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or System.Xml.XmlException)
        {
            return null;
        }
    }
}
