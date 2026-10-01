# BuildIt

This project is already configured as a .NET library plus a UiPath/NuGet packaging project.

## One-Click Build (Recommended)

Double-click `BuildArtifacts.exe` in the repository root. The launcher automatically:

1. Finds the repository path.
2. Reads and validates `UiValidationVersion` from `Directory.Build.props`.
3. Checks for a .NET 10 or newer SDK.
4. Restores dependencies and builds the matching UiPath package and complete solution.
5. Runs the unit and integration tests.
6. Opens `artifacts/packages` when the build succeeds.

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

The packaging project is built first because `ConsumerSmoke` restores the shared
`UiValidationVersion`
from `artifacts/packages`. On a clean checkout, that package does not exist until the
packaging project has completed once.

## Set The Release Version

`Directory.Build.props` is the single source of truth for the library, NuGet package,
consumer smoke test, and artifact builder. To prepare a future stable release, change
only this value before running the EXE:

```xml
<UiValidationVersion>2.0.0</UiValidationVersion>
```

Use stable `major.minor.patch` format. The launcher stops before building when the value
is missing or invalid, and it never deletes previously generated package versions.

## Rebuild The Launcher EXE

Maintainers can republish the root-level launcher with:

```powershell
dotnet publish tools/UI.Validation.ArtifactBuilder/UI.Validation.ArtifactBuilder.csproj -c Release -r win-x64 --self-contained true -o artifacts/builder
Copy-Item artifacts/builder/BuildArtifacts.exe ./BuildArtifacts.exe -Force
```

## Output

The generated version 2 package is created here:

```text
artifacts/packages/Vodafone.Robotics.UI.Validation.Activities.2.0.0.nupkg
```

The local feed intentionally retains both `1.0.0` and `2.0.0`. UiPath Studio displays
both versions in the Version list after the local source is refreshed. Version `2.0.0`
uses the simplified Target Selector interface; version `1.0.0` remains available for
existing workflows.

## Install In UiPath Studio

1. Open UiPath Studio.
2. Go to Manage Packages.
3. Add `artifacts/packages` as a local package source.
4. Install `Vodafone.Robotics.UI.Validation.Activities`.
5. Confirm the activities appear under the `UI Validation` category.

## Verified Locally

Verified on October 1, 2026 with .NET SDK `10.0.401`:

```text
Packages: Vodafone.Robotics.UI.Validation.Activities.1.0.0.nupkg and 2.0.0.nupkg
Build: succeeded with 0 warnings and 0 errors
Library unit tests: 143 passed, 0 failed, 0 skipped
Artifact-builder tests: 9 passed, 0 failed, 0 skipped
Integration test project: completed successfully
```
