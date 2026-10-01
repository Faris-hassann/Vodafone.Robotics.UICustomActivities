# UI Validation Library for UiPath

This repository builds one UiPath Windows activity package containing exactly three public activities:

- **Validated Get Text**
- **Validated Type Into**
- **Validated Click**

Every activity follows the same logged lifecycle:

```text
Configuration validation
  -> PreCondition (optional complete selector)
  -> Target (required complete selector)
  -> PostCondition (optional complete selector)
  -> structured UIValidationResult
```

The implementation follows UiPath's code-activity template conventions: activity logic and embedded metadata live in the activity assembly, packaging is isolated in its own project, and pure validation/orchestration logic is independently testable.

## Build and test

```powershell
dotnet restore UI.Validation.Library.slnx --configfile NuGet.config
dotnet build UI.Validation.Library.slnx -c Release --no-restore
dotnet test tests/UI.Validation.Library.Tests/UI.Validation.Library.Tests.csproj -c Release --no-restore
```

The current package is written to `artifacts/packages/Vodafone.Robotics.UI.Validation.Activities.2.0.1.nupkg`.
The same local feed retains `1.0.0` and `2.0.0` for existing workflows, so UiPath Studio
can offer all three versions.

## Install in UiPath Studio

1. Add `artifacts/packages` as a local package source in **Manage Packages > Settings**.
2. Install `Vodafone.Robotics.UI.Validation.Activities` in a modern **Windows** project.
3. Find the three activities under the **UI Validation** category.
4. Provide the required `TargetSelector` and, when configured, complete selector XML strings for `PreConditionSelector` and `PostConditionSelector`.
5. Keep `LogFullSelector=false` and `LogSensitiveValues=false` unless an approved diagnostic policy explicitly permits those values.

`RetryCount` means retries after the first attempt, so `MaxAttempts = 1 + RetryCount`. Click action retry is disabled by default; postcondition polling can continue without issuing a second click.

## Projects

- `src/UI.Validation.Library` — activities, UiPath adapter, validation engine, result/exception/logging models.
- `src/UI.Validation.Library.Packaging` — NuGet activity package.
- `tests/UI.Validation.Library.Tests` — unit and component tests.
- `testhost/UI.Validation.TestHost` — deterministic WinForms UI for attended integration/E2E tests.
- `tests/UI.Validation.Library.IntegrationTests` — attended harness and complete-selector catalog.
- `samples/ConsumerSmoke` — clean package-consumer compilation smoke test.

The current verified baseline is 162 library tests with zero failures/skips, complete traceability for all 108 mandatory scenario IDs in the supplied test pack, and passing deterministic real-UI flows E2E-001 through E2E-005.

## Runtime note

The production adapter uses UiPath's supported `Selector` and `UiElement` APIs. Full selectors with at least an application/window tag plus a target tag are split into a parent scope and final target query so zero/one/many matches can be classified. The deterministic UI host is intended to be driven from an attended UiPath Robot/Studio session; UI execution must not be claimed when such a session is unavailable.
