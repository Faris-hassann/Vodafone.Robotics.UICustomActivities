# BuildIt

This project is already configured as a .NET library plus a UiPath/NuGet packaging project.

## Prerequisite

Install the .NET SDK and make sure `dotnet` is available on `PATH`.

Check it with:

```powershell
dotnet --info
```

## Build The Library Package

Run these commands from the repository root:

```powershell
dotnet restore UI.Validation.Library.slnx --configfile NuGet.config
dotnet build UI.Validation.Library.slnx -c Release --no-restore
dotnet build src/UI.Validation.Library.Packaging/UI.Validation.Library.Packaging.csproj -c Release --no-restore
dotnet test tests/UI.Validation.Library.Tests/UI.Validation.Library.Tests.csproj -c Release --no-restore
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

## Attempted Locally

I attempted to run the build commands in this workspace, but this shell currently cannot find the .NET CLI:

```text
dotnet : The term 'dotnet' is not recognized as the name of a cmdlet, function, script file, or operable program.
```

After installing the .NET SDK or fixing `PATH`, rerun the commands above.
