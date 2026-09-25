# Retry and Timeout Policy

## 1. Principle

Retry only conditions that may reasonably change with time.

## 2. Retryable Examples

Potentially retryable:

- target temporarily missing;
- target present but temporarily not ready/interactable;
- text temporarily blank/pending while page loads;
- final TypeInto value not yet observable due to asynchronous UI update;
- Click postcondition pending;
- transient read-back failure.

## 3. Non-Retryable Examples

Normally stop immediately:

- invalid configuration;
- invalid regex;
- unsupported mode combination;
- invalid selector syntax;
- deterministic target identity mismatch;
- explicit business precondition mismatch not configured as waitable;
- exact final value mismatch caused by a deterministic transformation when no alternate expected value is configured.

## 4. Attempts

Define and document whether `RetryCount` means additional retries after the first attempt or total attempts. Use one convention consistently across all activities and tests.

Recommended semantic:

`MaxAttempts = 1 + RetryCount`

## 5. Delay Strategy

Phase 1 default: fixed delay.

Internal design may permit future Linear or Exponential strategies.

## 6. Cancellation

Cancellation must prevent future retries/actions and must not be converted into `RetryExhausted`.

## 7. Click Safety

Click separates **action retry** from **postcondition polling**.

Before any repeated click:

1. evaluate configured postcondition;
2. if already true, return success without another click;
3. if action retry is disabled, do not click again; continue only safe postcondition polling until timeout/retry exhaustion;
4. if action retry is allowed and the classified failure indicates re-clicking is safe, perform another click.

This separation is mandatory for non-idempotent actions.
