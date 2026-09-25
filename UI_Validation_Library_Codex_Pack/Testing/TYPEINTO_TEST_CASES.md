# Validated Type Into — Mandatory Test Cases

## Configuration

TI-001 Invalid mode/configuration fails before typing.
TI-002 Invalid verification attribute configuration fails before typing.
TI-003 Invalid timeout/retry configuration fails.
TI-004 Secure mode + impossible exact-readback requirement is rejected or truthfully downgraded according to documented policy before mutation.

## Target

TI-010 Correct unique input succeeds.
TI-011 Target not found follows retry policy.
TI-012 Multiple matches fail by default.
TI-013 Identity mismatch fails before mutation.
TI-014 Disabled/not-interactable field fails or retries according to classification.

## Replace

TI-020 Empty field + Replace writes input exactly.
TI-021 Pre-populated field + Replace removes old value and writes only input.
TI-022 Replace with empty input clears field.
TI-023 Replace preserves meaningful leading/trailing whitespace unless explicitly normalized.
TI-024 Unicode input verifies exactly.
TI-025 Long allowed input verifies exactly.

## Append

TI-030 Pre-populated field + Append produces Original + Input.
TI-031 Append with separator produces Original + Separator + Input.
TI-032 Empty Append input is a no-op when allowed.
TI-033 Existing-value validation mismatch prevents typing.

## ClearOnly

TI-040 ClearOnly clears a populated field and verifies empty result when available.

## Application Transformations

TI-050 Automatic uppercase without ExpectedFinalValue fails strict comparison.
TI-051 Automatic uppercase with ExpectedFinalValue=uppercase succeeds.
TI-052 Formatting transformation (e.g. numeric display) follows explicit expected value policy.

## Read-Back

TI-060 Auto selects a supported verification strategy and records which one.
TI-061 ValueAttribute strategy succeeds for a control whose visible text is not the entered value.
TI-062 SpecificAttribute verifies configured attribute.
TI-063 Readback temporarily unavailable retries safely without duplicate unexpected typing when implementation can poll readback separately.

## Mismatch Classification

TI-070 PartialInput classified correctly.
TI-071 UnexpectedPrefix classified correctly.
TI-072 UnexpectedSuffix classified correctly.
TI-073 DuplicateInput classified correctly.
TI-074 NoChange classified correctly.
TI-075 UnexpectedTransformation classified correctly where determinable.

## Secure / Privacy

TI-080 Secure input does not emit clear-text value in logs.
TI-081 Secure mode does not claim exact verification if unavailable.
TI-082 Sensitive normal input is masked by default.

## Retry / Cancellation

TI-090 Transient target-not-ready may succeed on later attempt.
TI-091 Retry exhaustion returns correct attempt count/category.
TI-092 Cancellation prevents further typing attempts.

## Diagnostics

TI-100 Final failure screenshot captured by default.
TI-101 Screenshot failure preserves primary mismatch/action failure.
