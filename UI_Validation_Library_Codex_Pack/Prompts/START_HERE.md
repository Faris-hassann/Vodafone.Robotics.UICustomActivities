# Start Here — Prompt for Codex / Claude

Copy this pack into the repository, then use a prompt like the following.

```text
You are implementing UI_Validation_Library.

First read UI_Validation_Library_Codex_Pack/Agent.md and follow it as the execution contract. Then read PROJECT_CONTEXT.md, ARCHITECTURE.md, VALIDATION_RULES.md, DEFINITION_OF_DONE.md, and all activity/testing specs relevant to the requested scope.

Goal: Complete UI_Validation_Library Phase 1.

Phase 1 public activities are only:
1. Validated Get Text
2. Validated Type Into
3. Validated Click

Before coding, inspect the real repository, target framework, installed UiPath packages, activity SDK patterns, test framework, package conventions, and existing utilities. Adapt the technical mechanism to the repository while preserving the behavioral contract in the pack.

Implement shared validation infrastructure without unnecessary over-engineering. Generate executable unit, component, integration, and E2E tests from the mandatory test-case Markdown files. Use or build the deterministic test host specified by the pack.

Run builds and tests yourself. Fix root causes. Never delete, skip, weaken, or rewrite required tests/acceptance criteria merely to get green. Never claim a test passed if it was not executed. Preserve privacy-safe logging and click retry safety.

At the end, produce the exact report defined in Agent/FINAL_REPORT_TEMPLATE.md and mark status FINISHED only if DEFINITION_OF_DONE.md is satisfied.
```

## Narrower Goals

You may replace the goal with:

- `Complete Validated Get Text`
- `Complete Validated Type Into`
- `Complete Validated Click`

Even for a narrow goal, shared infrastructure must remain compatible with the whole Phase 1 architecture.
