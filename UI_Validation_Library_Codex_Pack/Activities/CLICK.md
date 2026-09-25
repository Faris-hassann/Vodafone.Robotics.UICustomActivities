# Validated Click — Activity Specification

## Goal

Click the intended UI element only after validation, then verify the expected outcome when configured, while preventing unsafe duplicate actions during retries.

## Execution Flow

```text
Validate configuration
→ Resolve target
→ Validate target uniqueness/identity/visibility/enabled/interactable state
→ Validate optional precondition
→ Evaluate postcondition before action when retry-safe short-circuit is relevant
→ Click
→ Validate/poll postcondition
→ Before re-click: test whether outcome already succeeded
→ Apply action-retry safety policy
→ Return structured result or final failure
```

## Preconditions

Examples:

- button text equals `Submit`;
- enabled = true;
- aria-disabled = false;
- current business status = `Draft`;
- required attribute equals expected value.

## Postconditions

Support concepts equivalent to:

- ElementAppears
- ElementDisappears
- TextEquals
- TextContains
- AttributeEquals
- AttributeChanges
- TargetStateChanges
- WindowOrPageAppears

Exact supported shapes may follow UiPath target APIs available in the repository.

## Success Definition

With postcondition:

`TargetValidated AND ClickExecuted AND PostconditionValidated`

Without postcondition:

`TargetValidated AND ClickExecuted`

and record a warning such as `OutcomeNotIndependentlyVerified`.

## Already-Satisfied Outcome

Before a retry/re-click, evaluate the configured postcondition. If already true, consider the intended operation satisfied and do not issue another click.

For the initial attempt, an optional configurable policy may choose whether an already-satisfied postcondition means SkipAsAlreadySatisfied, FailBecauseUnexpectedState, or ClickAnyway. Default recommendation: SkipAsAlreadySatisfied for retry-safe workflows.

## Non-Idempotent Safety

Expose/configure `AllowActionRetry` (or equivalent).

When false:

- first click may execute once;
- library may continue postcondition polling;
- library must not issue a second click automatically.

Use this for:

- payments;
- delete/confirm;
- submit order;
- send email/message;
- finalize transaction;
- any action with real-world side effects.

## Retry Classification

Postcondition pending is not the same as click failure.

Keep separate concepts for:

- click execution failure;
- target becoming unavailable;
- postcondition pending;
- postcondition deterministic failure;
- outcome already satisfied.

## Configuration Safety

All required postcondition target/value fields must be validated before the first click.

Bad example that must never happen:

`Click Submit Payment → discover postcondition selector was missing → throw configuration error`

Correct behavior:

`detect invalid postcondition configuration → fail without clicking`

## Required Edge Cases

- successful click + element appears;
- successful click + element disappears;
- delayed postcondition;
- postcondition already true before retry;
- click action throws;
- target wrong/ambiguous;
- button disabled;
- no postcondition configured;
- non-idempotent action with AllowActionRetry=false;
- retry-safe click where first action succeeded but response was delayed;
- cancellation during postcondition polling;
- screenshot failure after a primary click/postcondition failure.
