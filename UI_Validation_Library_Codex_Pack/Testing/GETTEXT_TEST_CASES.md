# Validated Get Text — Mandatory Test Cases

Implement executable tests for the following scenarios.

## Configuration

GT-001 Invalid regex fails before target resolution/read.
GT-002 Invalid timeout/retry configuration fails deterministically.
GT-003 Missing target configuration fails clearly.

## Target

GT-010 Correct unique target succeeds.
GT-011 Target not found follows retry policy then fails.
GT-012 Multiple matches fail by default.
GT-013 Explicitly allowed multiple-match policy behaves exactly as documented.
GT-014 Target identity mismatch fails before read result is accepted.

## Raw Text States

GT-020 Null/no-value state is classified separately.
GT-021 Empty string is retrieved successfully when NotEmpty is not enabled.
GT-022 Empty string fails when NotEmpty is enabled.
GT-023 Whitespace-only is distinguishable from empty.
GT-024 Non-empty text succeeds.

## Rules

GT-030 Exact match succeeds.
GT-031 Exact mismatch fails.
GT-032 Contains succeeds/fails correctly.
GT-033 StartsWith succeeds/fails correctly.
GT-034 EndsWith succeeds/fails correctly.
GT-035 Regex succeeds/fails correctly.
GT-036 Multiple enabled rules require all rules to pass.
GT-037 CaseSensitive=true is strict.
GT-038 IgnoreCase/configured non-sensitive mode behaves as documented.
GT-039 TrimForValidation does not modify returned raw text.
GT-040 Whitespace normalization, if implemented, affects validation view only.

## Retry / Timing

GT-050 Element appears on later attempt and succeeds.
GT-051 Text is blank initially then becomes expected value and succeeds for a waitable rule.
GT-052 Retry exhaustion returns/throws correct category and attempt count.
GT-053 Cancellation stops further polling.

## Logging / Results

GT-060 Result includes actual activity/stage/attempt/duration/correlation information.
GT-061 Sensitive raw text is not logged when sensitive logging is disabled.
GT-062 Final failure screenshot path is recorded when capture succeeds.
GT-063 Screenshot failure preserves primary failure.
