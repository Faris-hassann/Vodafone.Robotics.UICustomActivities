using Vodafone.Robotics.UiValidation.ArtifactBuilder;
using Xunit;

namespace UI.Validation.ArtifactBuilder.Tests;

public sealed class ArtifactBuilderTests
{
    [Fact]
    public void RepositoryLocatorFindsRootFromNestedDirectory()
    {
        using var repository = TemporaryRepository.Create();
        var nestedDirectory = Directory.CreateDirectory(Path.Combine(repository.Path, "tools", "publish")).FullName;

        var result = RepositoryLocator.Find(nestedDirectory);

        Assert.Equal(repository.Path, result);
    }

    [Fact]
    public void RepositoryLocatorReturnsNullWhenMarkersAreMissing()
    {
        using var directory = TemporaryDirectory.Create();

        var result = RepositoryLocator.Find(directory.Path);

        Assert.Null(result);
    }

    [Fact]
    public void SdkParserChoosesHighestCompatibleVersionAndAcceptsPreviewText()
    {
        const string output = "8.0.425 [C:\\dotnet\\sdk]\n10.0.100-preview.1 [C:\\dotnet\\sdk]\n10.0.401 [C:\\dotnet\\sdk]";

        var result = DotNetSdkLocator.ParseHighestSupportedSdk(output);

        Assert.Equal(new Version(10, 0, 401), result);
    }

    [Fact]
    public void SdkSelectionSkipsInvalidAndOldCandidates()
    {
        var candidates = new[] { "invalid", "old", "compatible" };
        var responses = new Dictionary<string, string?>
        {
            ["invalid"] = "not an SDK response",
            ["old"] = "8.0.425 [C:\\dotnet\\sdk]",
            ["compatible"] = "10.0.401 [C:\\dotnet\\sdk]"
        };

        var result = DotNetSdkLocator.SelectFirstCompatible(candidates, candidate => responses[candidate]);

        Assert.NotNull(result);
        Assert.Equal("compatible", result.ExecutablePath);
        Assert.Equal(new Version(10, 0, 401), result.Version);
    }

    [Fact]
    public async Task PipelineStopsAfterFirstFailedCommand()
    {
        var runner = new FakeCommandRunner(0, 0, 7, 0);
        var pipeline = new BuildPipeline(runner, TextWriter.Null);

        var result = await pipeline.RunAsync("dotnet.exe", "C:\\repo", CancellationToken.None);

        Assert.False(result.Succeeded);
        Assert.Equal("Restore solution", result.FailedStep?.Name);
        Assert.Equal(7, result.ExitCode);
        Assert.Equal(3, runner.CallCount);
    }

    private sealed class FakeCommandRunner(params int[] exitCodes) : ICommandRunner
    {
        private readonly Queue<int> exitCodes = new(exitCodes);

        public int CallCount { get; private set; }

        public Task<int> RunAsync(
            string executablePath,
            IReadOnlyList<string> arguments,
            string workingDirectory,
            CancellationToken cancellationToken)
        {
            CallCount++;
            return Task.FromResult(exitCodes.Dequeue());
        }
    }

    private class TemporaryDirectory : IDisposable
    {
        protected TemporaryDirectory()
        {
            Path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"artifact-builder-{Guid.NewGuid():N}");
            Directory.CreateDirectory(Path);
        }

        public string Path { get; }

        public static TemporaryDirectory Create() => new();

        public void Dispose() => Directory.Delete(Path, recursive: true);
    }

    private sealed class TemporaryRepository : TemporaryDirectory
    {
        private TemporaryRepository()
        {
            File.WriteAllText(System.IO.Path.Combine(Path, "UI.Validation.Library.slnx"), string.Empty);
            File.WriteAllText(System.IO.Path.Combine(Path, "NuGet.config"), string.Empty);
            var packagingDirectory = Directory.CreateDirectory(
                System.IO.Path.Combine(Path, "src", "UI.Validation.Library.Packaging"));
            File.WriteAllText(
                System.IO.Path.Combine(packagingDirectory.FullName, "UI.Validation.Library.Packaging.csproj"),
                string.Empty);
        }

        public static new TemporaryRepository Create() => new();
    }
}
