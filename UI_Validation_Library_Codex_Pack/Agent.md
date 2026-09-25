# UI_Validation_Library — Agent Execution Contract

## Purpose

You are implementing a production-grade UiPath custom-activity library. This document is the execution contract. Specifications in this pack are requirements, not suggestions.

The Phase 1 public surface contains only:

- `Validated Get Text`
- `Validated Type Into`
- `Validated Click`

The internal implementation may contain shared services, models, adapters, validators, retry policies, logging helpers, and a deterministic test host.

## 1. Repository-First Rule

Before editing code:

1. inspect solution/project files;
2. determine target .NET/runtime and UiPath package model;
3. determine existing UiPath activity SDK/API versions;
4. determine test framework and test project conventions;
5. determine NuGet/package conventions;
6. determine nullable, analyzers, formatting, warning policy, and CI rules;
7. locate any existing activity base classes, designers, metadata, logging abstraction, or result models;
8. identify whether the repository already provides a controlled test UI.

Do not invent incompatible package versions or introduce a second framework when the repository already has a suitable established one.

When this pack and the repository disagree on a technical mechanism, preserve the behavioral contract in this pack while adapting the mechanism to supported repository/runtime APIs. Document the adaptation.

## 2. Architecture Contract

The implementation must separate public activity orchestration from shared validation logic.

Expected conceptual boundaries:

```text
Public Activities
  -> configuration validation
  -> target resolution / target adapter
  -> validation engine
  -> action executor
  -> postcondition verifier
  -> retry policy
  -> structured logger
  -> result builder
  -> exception mapping
```

Do not duplicate the same target identity, comparison, logging, retry, or result-building logic in all three activities.

Keep pure validation/comparison logic as UI-independent as practical so it can be unit tested without launching an application.

## 3. Core Execution Order

For every activity, preserve this order:

1. create/resolve correlation context;
2. validate configuration;
3. start activity logging;
4. resolve target;
5. validate target uniqueness and identity;
6. validate preconditions;
7. evaluate retry-safe short-circuit conditions where applicable;
8. perform the action/read;
9. validate the output/postcondition;
10. classify success or failure;
11. apply retry policy only if eligible;
12. capture final-failure diagnostics if enabled;
13. finalize structured result;
14. throw/map failure if `ThrowOnFailure = true`, otherwise return failed result.

**Invalid configuration must fail before a UI-changing action.**

## 4. Safe Defaults

Default behavior is strict and safe:

- `ThrowOnFailure = true`
- target ambiguity fails unless explicitly allowed;
- comparisons are strict/case-sensitive unless explicitly normalized;
- `TypeInto` default mode is Replace;
- TypeInto append separator default is empty string;
- sensitive-value logging is disabled/masked;
- full-selector logging is disabled;
- final-failure screenshot is enabled unless explicitly disabled;
- retry-failure and success screenshots are disabled;
- Click action retry is allowed only when configured/defaulted as safe; non-idempotent workflows must be able to disable it;
- Click postcondition is optional but absence produces a diagnostic warning.

## 5. UiPath Integration Rule

Use supported UiPath automation mechanisms. Do not reimplement a selector engine, browser automation stack, desktop automation stack, or accessibility framework.

Exact UiPath APIs differ by package/runtime version. Inspect the repository and use supported APIs for that environment. Preserve the behavior specified here.

The public activities must appear in UiPath Studio with clear names, categories, property descriptions, and safe defaults.

## 6. Public Phase 1 Naming

Recommended Studio display names:

- `Validated Get Text`
- `Validated Type Into`
- `Validated Click`

Recommended category:

- `UI Validation`

Do not publish unrelated backlog activities in Phase 1.

## 7. Failure Classification Contract

Do not collapse every failure into `Exception`.

At minimum support conceptual categories for:

- InvalidConfiguration
- TargetNotFound
- TargetAmbiguous
- TargetIdentityMismatch
- TargetNotReady
- ReadFailed
- ActionFailed
- TextValidationFailed
- PostconditionFailed
- RetryExhausted
- Cancelled
- DiagnosticFailure (secondary only)

Use a library-specific exception hierarchy as described in `EXCEPTION_MODEL.md`.

## 8. Retry Contract

Retry is selective. Do not retry simply because an exception occurred.

Retry potentially transient failures such as:

- target temporarily missing/not ready;
- read-back temporarily unavailable;
- expected text not loaded yet when the rule is configured as waitable;
- pending postcondition.

Do not normally retry:

- invalid configuration;
- invalid selector syntax;
- deterministic identity mismatch;
- deterministic invalid regex;
- conflicting modes;
- deterministic business mismatch unless a specific wait-for-state rule makes it transient.

Click has additional safety rules in `Activities/CLICK.md`.

## 9. Logging Contract

Every meaningful stage must support structured and human-readable logging.

Never log sensitive actual/expected values by default. Prefer lengths, hashes only when useful and safe, masked values, or semantic summaries.

A diagnostic failure such as screenshot capture must never replace the primary exception.

## 10. Testing Execution Mode

The agent must create/adapt tests described under `Testing/`.

Required layers:

1. Unit
2. Component
3. Integration
4. End-to-End

Use a deterministic test UI as specified under `TestHost/` for integration/E2E where technically practical.

If the current environment cannot launch UI automation, still implement the tests and run all non-UI layers. Report the exact UI tests not executed and why. Do not label them passed.

## 11. Test Integrity Rules

Never obtain green status by:

- deleting a required test;
- marking it skipped/ignored/todo without a documented external blocker;
- changing expected output to match incorrect implementation;
- removing assertions;
- reducing strict comparison requirements;
- disabling target identity checks;
- suppressing an exception required by the specification;
- turning a test into a smoke-only assertion.

If a spec appears impossible with the supported UiPath API, document the limitation, implement the strongest truthful behavior, and keep the test/acceptance gap visible.

## 12. Definition of FINISHED

A requested activity/module is `FINISHED` only when:

- repository builds successfully;
- targeted automated tests pass;
- relevant regression tests pass;
- required behavior matches its activity spec;
- logging/redaction behavior is tested;
- exception mapping is tested;
- retry behavior is tested;
- cancellation is respected;
- package metadata/Studio discoverability is correct where relevant;
- documentation is updated;
- no known Critical/High defect remains;
- final report states actual commands/tests and actual results.

If any required item is blocked, report `PARTIAL` or `BLOCKED`, not `FINISHED`.

## 13. Change Discipline

Prefer minimal, cohesive changes. Do not refactor unrelated code merely because it can be improved.

Preserve backwards compatibility unless the repository is explicitly new or the requested change requires a documented breaking change.

Do not modify source input artifacts such as the supplied backlog or sample CRM workflows.

## 14. Final Report

Use `Agent/FINAL_REPORT_TEMPLATE.md`.

The report must include:

- status: FINISHED/PARTIAL/BLOCKED;
- implementation summary;
- files changed;
- architecture decisions/adaptations;
- tests added;
- tests run and exact pass/fail counts;
- package/build result;
- known limitations;
- remaining manual UiPath Studio smoke steps, if any.
