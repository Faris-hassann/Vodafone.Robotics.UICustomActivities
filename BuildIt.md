# BuildIt

This project is already configured as a .NET library plus a UiPath/NuGet packaging project.

## One-Click Build (Recommended)

Double-click `BuildArtifacts.exe` in the repository root. The launcher automatically:

1. Finds the repository path.
2. Checks for a .NET 10 or newer SDK.
3. Restores dependencies and builds the UiPath package and complete solution.
4. Runs the unit and integration tests.
5. Opens `artifacts/packages` when the build succeeds.

The console remains open so that the result and any build error can be read. Keep the
EXE somewhere inside the repository if it is moved from the root. The launcher is an
unsigned, self-contained Windows x64 executable; its maintainable source is under
`tools/UI.Validation.ArtifactBuilder`.

For automated use, run the launcher with `--non-interactive`. This prevents File
Explorer from opening and prevents the final key prompt. Exit code `0` means the full
build succeeded; any other exit code means it failed.

## Prerequisite

Install the .NET 10 SDK or newer. The launcher checks `PATH`, `DOTNET_ROOT`, Program
Files, and the current user's local .NET installation automatically. The manual
commands below require `dotnet` to be available on `PATH`. The newer SDK is required
because this repository uses the `.slnx` solution format.

Check it with:

```powershell
dotnet --info
```

## Manual Build (Troubleshooting)

Run these commands from the repository root:

```powershell
New-Item -ItemType Directory -Force -Path artifacts/packages | Out-Null
dotnet restore src/UI.Validation.Library.Packaging/UI.Validation.Library.Packaging.csproj --configfile NuGet.config
dotnet build src/UI.Validation.Library.Packaging/UI.Validation.Library.Packaging.csproj -c Release --no-restore
dotnet restore UI.Validation.Library.slnx --configfile NuGet.config
dotnet build UI.Validation.Library.slnx -c Release --no-restore
dotnet test tests/UI.Validation.Library.Tests/UI.Validation.Library.Tests.csproj -c Release --no-build --no-restore
dotnet test tests/UI.Validation.Library.IntegrationTests/UI.Validation.Library.IntegrationTests.csproj -c Release --no-build --no-restore
```

The packaging project is built first because `ConsumerSmoke` restores version `1.0.0`
from `artifacts/packages`. On a clean checkout, that package does not exist until the
packaging project has completed once.

## Rebuild The Launcher EXE

Maintainers can republish the root-level launcher with:

```powershell
dotnet publish tools/UI.Validation.ArtifactBuilder/UI.Validation.ArtifactBuilder.csproj -c Release -r win-x64 --self-contained true -o artifacts/builder
Copy-Item artifacts/builder/BuildArtifacts.exe ./BuildArtifacts.exe -Force
```

## Output

The generated package should be created here:

```text
artifacts/packages/Vodafone.Robotics.UI.Validation.Activities.1.0.0.nupkg
```

## Install In UiPath Studio

1. Open UiPath Studio.
2. Go to Manage Packages.
3. Add `artifacts/packages` as a local package source.
4. Install `Vodafone.Robotics.UI.Validation.Activities`.
5. Confirm the activities appear under the `UI Validation` category.

## Verified Locally

Verified on October 1, 2026 with .NET SDK `10.0.401`:

```text
Package: Vodafone.Robotics.UI.Validation.Activities.1.0.0.nupkg
Build: succeeded with 0 warnings and 0 errors
Library unit tests: 142 passed, 0 failed, 0 skipped
Artifact-builder tests: 5 passed, 0 failed, 0 skipped
Integration test project: completed successfully
```
