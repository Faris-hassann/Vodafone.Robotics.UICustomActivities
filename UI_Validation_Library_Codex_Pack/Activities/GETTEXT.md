# Validated Get Text — Activity Specification

## Goal

Read text from the intended UI element, prove target identity as configured, return the original raw text, and optionally validate that text against one or more rules.

## Execution Flow

```text
Validate configuration
→ Resolve target
→ Validate target uniqueness/identity/readiness
→ Read text
→ Classify raw value state
→ Apply enabled validation rules
→ Retry only waitable/transient failures
→ Return raw text + structured result
→ Throw final failure when ThrowOnFailure=true
```

## Inputs / Configuration Concepts

Adapt exact property types/names to UiPath SDK conventions.

### Target

- selector/target descriptor and/or existing UiElement reference;
- timeout profile/custom timeout;
- target identity expectations;
- allow-multiple-matches override.

### Text Rules

Support:

- `RetrievedSuccessfully`
- `NotEmpty`
- `Exact`
- `Contains`
- `StartsWith`
- `EndsWith`
- `Regex`

Multiple enabled rules use AND semantics by default.

### Comparison Configuration

- CaseSensitive: default true
- TrimForValidation: default false
- IgnoreCase or equivalent: explicit only
- NormalizeWhitespace: default false

Never mutate the returned raw text based on validation normalization.

## Raw Value States

Differentiate:

- Null/NoValue
- Empty
- WhitespaceOnly
- NonEmpty
- ReadFailed

`RetrievedSuccessfully` means the read operation succeeded; it does not automatically mean non-empty.

## Retry Behavior

Retry may be appropriate if:

- target is temporarily not found/not ready;
- text read is temporarily unavailable;
- configured rule is explicitly intended to wait for asynchronous UI content.

Invalid regex/configuration and deterministic identity mismatch do not retry.

## Outputs

- raw retrieved text;
- common `UIValidationResult`.

## Logging Examples

Human-readable example:

`[Validated Get Text] Text retrieved and validation passed on attempt 2.`

Structured context should include rule names, attempt, duration, target summary, raw-state classification, and comparison outcome without exposing sensitive text by default.

## Required Edge Cases

- exact empty string is a valid retrieved value when NotEmpty is not enabled;
- whitespace-only can be distinguished from empty;
- Exact must not silently Trim;
- Contains/StartsWith/EndsWith respect CaseSensitive setting;
- invalid Regex fails during config validation;
- raw returned text remains unchanged even when TrimForValidation=true;
- target ambiguity fails by default;
- cancellation stops polling/retry.

## Acceptance Criteria

See global acceptance criteria plus:

1. All seven rule types are supported.
2. Enabled rules combine with AND by default.
3. Raw output is preserved exactly.
4. Null/empty/whitespace/read-failure states are distinguishable.
5. Waitable asynchronous text scenarios can succeed on later attempts without duplicating unrelated UI actions.
