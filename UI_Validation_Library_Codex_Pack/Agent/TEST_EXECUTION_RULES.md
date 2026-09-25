# Agent Test Execution Rules

## Mandatory

- Generate executable tests from all mandatory test-case IDs relevant to the implemented scope.
- Preserve test IDs in test names/comments where practical for traceability.
- Use the repository's existing runner/framework.
- Run targeted tests before regression.
- Record exact command and result.

## Never

- skip a failing required test merely to proceed;
- delete a scenario because it is inconvenient;
- replace exact assertions with `NotNull`/`NoException` smoke assertions;
- make sensitive logging tests opt-out;
- make duplicate-click safety tests non-observable;
- claim UI tests passed when the execution environment could not launch them.

## Flaky Tests

If a test is flaky:

1. identify nondeterminism;
2. remove sleeps/random timing where possible;
3. make test-host state explicit;
4. use bounded polling;
5. fix production/test-host race rather than increasing timeouts blindly.

## Final Test Order

Recommended:

1. unit tests;
2. component tests;
3. activity-specific integration tests;
4. E2E tests;
5. full relevant regression suite;
6. package/consumer smoke test.
