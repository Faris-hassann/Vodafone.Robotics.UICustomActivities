using System.Diagnostics;

namespace Vodafone.Robotics.UiValidation.ArtifactBuilder;

internal static class Program
{
    private const int InvalidArgumentsExitCode = 2;
    private const int RepositoryNotFoundExitCode = 10;
    private const int SdkNotFoundExitCode = 11;
    private const int BuildFailedExitCode = 12;
    private const int ArtifactMissingExitCode = 13;
    private const int CancelledExitCode = 130;

    public static async Task<int> Main(string[] args)
    {
        Console.Title = "Vodafone UI Validation - Artifact Builder";
        var nonInteractive = args.Any(argument =>
            argument.Equals("--non-interactive", StringComparison.OrdinalIgnoreCase));

        if (args.Any(argument =>
                !argument.Equals("--non-interactive", StringComparison.OrdinalIgnoreCase)))
        {
            Console.Error.WriteLine("Usage: BuildArtifacts.exe [--non-interactive]");
            return Finish(InvalidArgumentsExitCode, nonInteractive);
        }

        Console.WriteLine("Vodafone UI Validation Artifact Builder");
        Console.WriteLine("========================================");

        try
        {
            var executableDirectory = Path.GetDirectoryName(Environment.ProcessPath) ?? AppContext.BaseDirectory;
            var repositoryRoot = RepositoryLocator.Find(executableDirectory, Environment.CurrentDirectory);
            if (repositoryRoot is null)
            {
                Console.Error.WriteLine();
                Console.Error.WriteLine("ERROR: The repository could not be located.");
                Console.Error.WriteLine("Keep BuildArtifacts.exe inside the UI Validation repository and try again.");
                return Finish(RepositoryNotFoundExitCode, nonInteractive);
            }

            Console.WriteLine($"Repository: {repositoryRoot}");
            Console.WriteLine("Checking for a compatible .NET SDK...");

            var sdk = DotNetSdkLocator.Find();
            if (sdk is null)
            {
                Console.Error.WriteLine();
                Console.Error.WriteLine("ERROR: .NET SDK 10 or newer was not found.");
                Console.Error.WriteLine("Install the .NET 10 SDK, restart this application, and try again.");
                Console.Error.WriteLine("Download: https://dotnet.microsoft.com/download/dotnet/10.0");
                return Finish(SdkNotFoundExitCode, nonInteractive);
            }

            Console.WriteLine($"SDK:        {sdk.Version} ({sdk.ExecutablePath})");

            var packagesDirectory = Path.Combine(repositoryRoot, "artifacts", "packages");
            Directory.CreateDirectory(packagesDirectory);

            using var cancellationSource = new CancellationTokenSource();
            ConsoleCancelEventHandler cancelHandler = (_, eventArgs) =>
            {
                eventArgs.Cancel = true;
                cancellationSource.Cancel();
            };
            Console.CancelKeyPress += cancelHandler;

            BuildResult result;
            try
            {
                var pipeline = new BuildPipeline(new ConsoleCommandRunner(), Console.Out);
                result = await pipeline.RunAsync(
                    sdk.ExecutablePath,
                    repositoryRoot,
                    cancellationSource.Token).ConfigureAwait(false);
            }
            finally
            {
                Console.CancelKeyPress -= cancelHandler;
            }

            if (!result.Succeeded)
            {
                Console.Error.WriteLine();
                Console.Error.WriteLine($"BUILD FAILED: {result.FailedStep!.Name} returned exit code {result.ExitCode}.");
                return Finish(BuildFailedExitCode, nonInteractive);
            }

            var packagePath = ArtifactLocator.FindExpectedPackage(repositoryRoot);
            if (packagePath is null)
            {
                Console.Error.WriteLine();
                Console.Error.WriteLine("BUILD FAILED: The expected NuGet package was not found in artifacts\\packages.");
                return Finish(ArtifactMissingExitCode, nonInteractive);
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("BUILD SUCCEEDED");
            Console.ResetColor();
            Console.WriteLine($"Package: {packagePath}");

            if (!nonInteractive)
            {
                OpenInExplorer(packagesDirectory);
            }

            return Finish(0, nonInteractive);
        }
        catch (OperationCanceledException)
        {
            Console.Error.WriteLine();
            Console.Error.WriteLine("Build cancelled.");
            return Finish(CancelledExitCode, nonInteractive);
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine();
            Console.Error.WriteLine($"UNEXPECTED ERROR: {exception.Message}");
            return Finish(1, nonInteractive);
        }
    }

    private static void OpenInExplorer(string directory)
    {
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "explorer.exe",
                ArgumentList = { directory },
                UseShellExecute = true
            });
        }
        catch (Exception exception) when (exception is System.ComponentModel.Win32Exception or InvalidOperationException)
        {
            Console.Error.WriteLine($"Could not open File Explorer: {exception.Message}");
        }
    }

    private static int Finish(int exitCode, bool nonInteractive)
    {
        if (!nonInteractive)
        {
            Console.WriteLine();
            Console.Write("Press any key to close...");
            try
            {
                Console.ReadKey(intercept: true);
            }
            catch (InvalidOperationException)
            {
                // Input is redirected or no interactive console is attached.
            }

            Console.WriteLine();
        }

        return exitCode;
    }
}
