# Executable Test Contract — Instructions

The Markdown files in this folder define mandatory test behavior. The coding agent must translate them into the repository's real test framework and UI test harness.

## Required Test Layers

1. Unit tests — pure validation/comparison/configuration/retry/result logic.
2. Component tests — activity orchestration with mocks/fakes/adapters.
3. Integration tests — real activities against the deterministic test UI.
4. End-to-End tests — combined TypeInto → Click → GetText flows.

## Agent Rules

The agent must:

1. inspect the repository's existing test framework;
2. use that framework unless technically unsuitable;
3. generate/adapt executable tests from these contracts;
4. run targeted tests after implementation changes;
5. fix implementation defects rather than weakening expected behavior;
6. run the relevant regression suite before FINISHED;
7. report exact counts and any tests not executed.

The agent must not:

- delete required scenarios;
- mark required cases skipped merely to pass CI;
- convert assertions to TODO;
- change expected values to match buggy code;
- test only that no exception was thrown when the contract requires outcome validation.

## Environment Limitation

If the current agent environment cannot run UiPath UI automation, it must still:

- implement the test host/project;
- implement integration/E2E tests;
- run unit/component tests;
- compile/package all test projects if possible;
- explicitly label UI tests `NOT EXECUTED` in the final report.
