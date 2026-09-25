# Test Strategy

## 1. Unit Tests

Target pure logic:

- configuration validators;
- string comparison/normalization;
- raw value state classification;
- TypeInto expected-value calculation;
- mismatch classification;
- retry eligibility;
- click action-retry decision logic;
- exception mapping;
- result builder;
- log redaction/safe summaries.

No real UI should be required.

## 2. Component Tests

Use fake/mock target resolver, action executor, postcondition provider, logger, clock/delay, screenshot service.

Verify:

- lifecycle ordering;
- configuration validation precedes action;
- expected logs/stages are produced;
- retry count and action invocation count;
- final result fields;
- secondary screenshot failure does not replace primary failure;
- cancellation prevents later retries/actions.

## 3. Integration Tests

Use the deterministic test UI in `TestHost/TEST_UI_APP_SPEC.md`.

Verify real target resolution and supported UI automation operations against predictable controls.

## 4. E2E Tests

Execute realistic sequences:

- Type Into an ID → Click Search → Get Text result;
- Append text → Click Save → Get Text persisted result;
- Click delayed action → verify no duplicate click → read final status.

## 5. Test Determinism

Avoid production CRM, public websites, Notepad, Calculator, or uncontrolled browser pages as the primary integration contract.

The test host must expose deterministic states and diagnostics such as click counters so duplicate-action tests are provable.

## 6. Timing

Use short deterministic delays in the test host. Avoid flaky sleeps. Prefer polling with bounded timeout and test-controlled state transitions.

## 7. Logging Assertions

Verify meaningful event names/context, but avoid brittle full-string snapshots for every human-readable message. Assert critical structured fields and redaction.

## 8. Privacy Tests

Tests must prove that:

- TypeInto input is redacted by default;
- password/secure values never appear in logs;
- sensitive expected/actual values are not emitted when sensitive logging is disabled;
- full selector is not emitted by default.
