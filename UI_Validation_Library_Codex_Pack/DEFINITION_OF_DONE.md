# Definition of Done

An activity or Phase 1 is not complete because code exists or compilation succeeded.

## Build

- restore succeeds;
- build succeeds;
- no compilation errors;
- no unexpected/new warnings, unless documented and accepted;
- package builds successfully.

## Automated Tests

- unit tests pass;
- component tests pass;
- integration tests pass in a supported UI environment;
- E2E tests pass in a supported UI environment;
- relevant regression tests pass;
- no required test was deleted, skipped, weakened, or converted to todo to obtain green status.

If UI execution is unavailable in the current environment, UI tests must exist and be clearly reported as **not executed**, not passed.

## Functional

- configuration validation occurs before UI mutation;
- target identity/ambiguity behavior is correct;
- retry classification is correct;
- cancellation works;
- `ThrowOnFailure` works;
- structured results are correct;
- final failure screenshot behavior is correct;
- primary exceptions survive secondary diagnostic failures.

## Security / Privacy

- sensitive values are masked by default;
- secure text is never exposed through logs;
- full selector logging is opt-in;
- tests verify redaction behavior.

## Activity-Specific

- all acceptance criteria in the relevant activity specification pass;
- all required activity test scenarios are implemented.

## UiPath Studio / Packaging

- activity names/category are correct;
- properties have useful descriptions;
- defaults are safe;
- package installs/restores in a clean/sample environment where available.

## Documentation

- README/update notes are current;
- any deviations from the pack due to UiPath SDK limitations are documented;
- final implementation report is complete.

## Status Rules

Use only:

- `FINISHED` — all required criteria satisfied;
- `PARTIAL` — implementation substantially complete but one or more required criteria/test executions remain;
- `BLOCKED` — an external dependency/environment prevents meaningful completion.
