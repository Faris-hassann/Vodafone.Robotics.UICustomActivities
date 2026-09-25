# Deterministic UI Test Host Specification

## Purpose

Provide a local, purpose-built application/page whose states are controlled by the test suite. It must make integration and E2E tests repeatable and make duplicate actions observable.

Choose a technology that the target UiPath Windows environment can automate reliably and that fits the repository. Do not introduce an unnecessarily complex framework.

## Required Controls

### Text Inputs

1. `EmptyInput`
   - starts empty.

2. `PrepopulatedInput`
   - starts with `Hello`.

3. `DisabledInput`
   - disabled.

4. `UppercaseInput`
   - automatically transforms entered text to uppercase.

5. `DelayedValueInput`
   - readback/display becomes available after a short deterministic delay.

6. `SecureInput`
   - password/secure control; value not exposed in a way that would invalidate secure-mode tests.

### Labels / Text

1. `PlainLabel` = `Ready`
2. `EmptyLabel` = empty
3. `WhitespaceLabel` = spaces
4. `DelayedLabel` = becomes `Loaded` after deterministic action/delay
5. `ResultLabel` = changed by Search/Save actions

### Buttons

1. `SearchButton`
   - sets ResultLabel after a short deterministic delay.

2. `SaveButton`
   - copies input to persisted display label.

3. `AppearButton`
   - makes a target appear.

4. `DisappearButton`
   - removes/hides a target.

5. `StateChangeButton`
   - changes an attribute/state.

6. `DelayedSuccessButton`
   - registers click immediately, shows success after a delay.

7. `NonIdempotentCounterButton`
   - increments a visible click counter every click; used to prove retry safety.

8. `DisabledButton`
   - disabled.

## Duplicate/Ambiguous Targets

Provide a controlled screen/state with two elements sharing a deliberately broad selector pattern so target ambiguity behavior can be tested.

## Test Control / Reset

Provide a reliable reset mechanism between tests so state does not leak across cases.

## Timing

Delays should be short and deterministic (for example hundreds of milliseconds to a few seconds) and configurable for tests. Avoid random timing.

## Diagnostics

Expose visible/readable state such as:

- click count;
- last typed/persisted value (except secure value);
- current operation status;
- target visibility state.
