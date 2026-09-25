# Validated Type Into — Activity Specification

## Goal

Safely write text into the intended UI control and verify the final value whenever reliable read-back is technically available.

## Execution Flow

```text
Validate configuration
→ Resolve target
→ Validate target uniqueness/identity/visibility/enabled/interactable state
→ Read/capture original value when required/available
→ Validate optional expected existing value
→ Calculate expected final value
→ Perform Replace / Append / ClearOnly
→ Read back using configured/auto strategy
→ Compare expected vs actual
→ Classify mismatch
→ Retry only safe/transient failures
→ Return structured result
```

## Modes

### Replace (default)

Expected final value:

`ExpectedFinal = InputText`

Implementation must ensure previous content is cleared/replaced rather than accidentally appended.

### Append

Expected final value:

`ExpectedFinal = OriginalValue + AppendSeparator + InputText`

`AppendSeparator` defaults to empty string.

### ClearOnly

Expected final value is empty when the target supports value verification.

## Existing Value Validation

Optional precondition:

- expected current value;
- configured comparison normalization;
- fail before typing when deterministic mismatch occurs.

Example:

Expected current: `Customer:`
Actual current: `Account:`

Result: do not type; return/throw precondition/validation failure.

## Final Value Override

Support `ExpectedFinalValue` or equivalent explicit override for application-side transformations.

Example:

Input: `abc`
Application normalizes to: `ABC`
ExpectedFinalValue: `ABC`

This is a valid success if configured.

## Read-Back Strategies

Support concepts equivalent to:

- Auto
- Text
- ValueAttribute
- SpecificAttribute

Auto may inspect target technology/capabilities and choose the strongest reliable readback.

The result must state which verification mode was actually used.

## Secure Input

When the target does not expose the value:

- do not log secret input;
- do not claim exact value verification;
- mark exact readback unavailable;
- verify safe observable evidence where possible (interaction success, target state, length if genuinely available/safe, external postcondition if supported by implementation);
- make the limitation explicit in result/logs.

## Mismatch Classification

Where determinable, classify:

- ExactMatch
- PartialInput
- UnexpectedPrefix
- UnexpectedSuffix
- DuplicateInput
- NoChange
- UnexpectedTransformation
- UnclassifiedMismatch

Classification improves diagnostics but does not replace the exact expected/actual comparison.

## Empty Input

Recommended semantics:

- Replace + empty → clear field;
- Append + empty → no-op unless DisallowEmptyInput is configured;
- ClearOnly → clear field.

## Safety Rules

- Validate configuration before mutation.
- Validate target identity before mutation.
- Do not silently trim values.
- Do not log the input in clear text by default.
- Do not convert a failed exact verification into success because the action call itself succeeded.

## Required Edge Cases

- pre-populated field Replace;
- pre-populated field Append;
- whitespace-sensitive value;
- Unicode text;
- long input;
- wrong target identity;
- multiple target matches;
- disabled/non-interactable target;
- partial entry;
- duplicate input;
- no-change condition;
- intentional uppercase transformation;
- delayed read-back;
- retry exhaustion;
- secure field;
- cancellation;
- sensitive logging disabled.
