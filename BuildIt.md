# BuildIt

This project is already configured as a .NET library plus a UiPath/NuGet packaging project.

## Prerequisite

Install the .NET 10 SDK or newer and make sure `dotnet` is available on `PATH`.
The newer SDK is required because this repository uses the `.slnx` solution format.

Check it with:

```powershell
dotnet --info
```

## Build The Library Package

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

Verified on September 28, 2026 with .NET SDK `10.0.401`:

```text
Package: Vodafone.Robotics.UI.Validation.Activities.1.0.0.nupkg
Build: succeeded with 0 warnings and 0 errors
Unit tests: 139 passed, 0 failed, 0 skipped
Integration test project: completed successfully
```
