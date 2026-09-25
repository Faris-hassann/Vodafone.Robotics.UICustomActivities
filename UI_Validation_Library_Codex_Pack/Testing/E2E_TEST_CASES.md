# End-to-End Test Cases

Use the deterministic test UI.

## E2E-001 Search Flow

1. Validated Type Into writes a customer-like ID into an empty field.
2. Exact readback passes.
3. Validated Click presses Search.
4. Postcondition waits for result label to appear/change.
5. Validated Get Text reads the result label.
6. Exact/Contains validation passes.
7. One correlation ID can be propagated across the three activities.

Assert:

- no duplicate click;
- all structured results succeed;
- logs contain consistent correlation;
- no sensitive input appears in logs by default.

## E2E-002 Append + Save

1. Field begins with `Hello`.
2. Validated Type Into Append with separator ` ` and input `World`.
3. Expected final value is `Hello World`.
4. Validated Click Save.
5. GetText reads persisted display value.
6. Exact equals `Hello World`.

## E2E-003 Delayed Click Outcome / Non-Idempotent Safety

1. Click a test-host button that increments a hidden/visible click counter once and shows success after a delay.
2. `AllowActionRetry=false`.
3. Outcome is not immediately visible.
4. Library polls postcondition.
5. Success eventually appears.

Assert click count is exactly 1.

## E2E-004 Failure Diagnostics

1. Configure a valid target but an intentionally impossible postcondition.
2. Click executes once.
3. Postcondition exhausts.
4. Result identifies PostconditionFailed/RetryExhausted as designed.
5. Screenshot is captured.
6. Primary exception remains correct.

## E2E-005 Configuration Safety

Configure Click postcondition with missing required target/value.

Assert:

- activity fails configuration validation;
- click count remains 0;
- no mutating UI action occurs.
