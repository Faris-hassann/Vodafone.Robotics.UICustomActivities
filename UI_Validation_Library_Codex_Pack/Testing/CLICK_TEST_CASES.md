# Validated Click — Mandatory Test Cases

## Configuration

CL-001 Missing/invalid target configuration fails before clicking.
CL-002 Postcondition type missing required target/value fails before clicking.
CL-003 Invalid timeout/retry configuration fails before clicking.

## Target / Preconditions

CL-010 Correct unique enabled target succeeds.
CL-011 Target not found follows retry policy.
CL-012 Multiple matches fail by default.
CL-013 Identity mismatch prevents click.
CL-014 Disabled target prevents click.
CL-015 Configured precondition mismatch prevents click.

## Postconditions

CL-020 ElementAppears success.
CL-021 ElementDisappears success.
CL-022 TextEquals success/failure.
CL-023 TextContains success/failure.
CL-024 AttributeEquals success/failure.
CL-025 AttributeChanges success.
CL-026 TargetStateChanges success.
CL-027 Window/Page appears success where supported by test environment.

## No Postcondition

CL-030 Click succeeds with no postcondition but result/log includes OutcomeNotIndependentlyVerified warning/state.

## Retry Safety

CL-040 First click succeeds but postcondition is delayed; library polls and does not unnecessarily double-click.
CL-041 Postcondition already true before a potential retry; second click is skipped.
CL-042 AllowActionRetry=false results in exactly one click even when postcondition remains pending.
CL-043 AllowActionRetry=true permits a second click only under a classified safe retry condition.
CL-044 Non-idempotent counter button proves click count remains one when action retry is disabled.

## Failure

CL-050 Click execution exception is classified separately from postcondition failure.
CL-051 Postcondition timeout produces correct category and retains action-executed=true.
CL-052 Retry exhaustion preserves attempt/action counts.
CL-053 Cancellation during postcondition polling prevents additional click/poll attempts as appropriate.

## Diagnostics

CL-060 Final failure screenshot capture succeeds.
CL-061 Screenshot capture failure preserves primary click/postcondition failure.
CL-062 Sensitive selector/value data is not emitted by default.
