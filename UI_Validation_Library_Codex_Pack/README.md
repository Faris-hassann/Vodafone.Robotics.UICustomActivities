# UI_Validation_Library — Codex / Claude Implementation Pack

This MD-only pack is intended to be copied into the target repository and used directly by Codex, Claude, or another coding agent.

## Phase 1 Goal

Build a reusable UiPath custom-activity library named `UI_Validation_Library` containing exactly these public Phase 1 activities:

1. `Validated Get Text`
2. `Validated Type Into`
3. `Validated Click`

The library must validate the target and outcome rather than assuming that a UI action succeeded merely because the UiPath activity executed.

## Start Here

The coding agent must read, in order:

1. `Agent.md`
2. `PROJECT_CONTEXT.md`
3. `ARCHITECTURE.md`
4. `VALIDATION_RULES.md`
5. the relevant file under `Activities/`
6. `Testing/README.md`
7. `DEFINITION_OF_DONE.md`

Then it must inspect the real repository before choosing exact package versions, SDK APIs, project layout, or namespaces.

## Example Agent Goals

- `Complete Validated Get Text`
- `Complete Validated Type Into`
- `Complete Validated Click`
- `Complete UI_Validation_Library Phase 1`

## Non-negotiable Agent Behavior

The agent must:

- inspect the real repository first;
- reuse the repository's established build/test/package conventions when compatible;
- implement the smallest safe increment;
- build after meaningful changes;
- generate and execute the required tests;
- repair implementation defects instead of weakening tests;
- run targeted tests, then the full relevant regression suite;
- document any environment-dependent tests that cannot run;
- never report `FINISHED` unless the Definition of Done is satisfied.

The agent must never:

- delete or skip required tests to obtain green status;
- weaken acceptance criteria;
- silently change public semantics;
- claim an exact UI verification when the technology does not expose the required value;
- log secrets or sensitive input values by default;
- retry a dangerous click in a way that can duplicate a real-world action.

## Pack Structure

```text
Agent.md
PROJECT_CONTEXT.md
SOURCE_ALIGNMENT.md
ARCHITECTURE.md
ACCEPTANCE_CRITERIA.md
VALIDATION_RULES.md
CONFIGURATION.md
EXCEPTION_MODEL.md
LOGGING_AND_DIAGNOSTICS.md
RETRY_AND_TIMEOUT_POLICY.md
RESULT_MODEL.md
PACKAGE_AND_STUDIO.md
DEFINITION_OF_DONE.md
Activities/
  GETTEXT.md
  TYPEINTO.md
  CLICK.md
Testing/
  README.md
  TEST_STRATEGY.md
  GETTEXT_TEST_CASES.md
  TYPEINTO_TEST_CASES.md
  CLICK_TEST_CASES.md
  E2E_TEST_CASES.md
TestHost/
  TEST_UI_APP_SPEC.md
Agent/
  IMPLEMENTATION_WORKFLOW.md
  TEST_EXECUTION_RULES.md
  FINAL_REPORT_TEMPLATE.md
Prompts/
  START_HERE.md
```
