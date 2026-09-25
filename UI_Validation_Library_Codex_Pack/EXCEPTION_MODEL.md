# Exception and Failure Model

## 1. Goals

Failures must be machine-classifiable and human-readable.

## 2. Conceptual Exception Hierarchy

Adapt exact class names to repository conventions:

```text
UIValidationException
├── UIConfigurationException
├── UITargetNotFoundException
├── UITargetAmbiguousException
├── UITargetIdentityException
├── UITargetNotReadyException
├── UIReadException
├── UIActionException
├── UITextValidationException
├── UIPostconditionException
└── UIRetryExhaustedException
```

Cancellation should use the runtime's normal cancellation semantics rather than being disguised as a validation exception.

## 3. BusinessRuleException Mapping

A consuming UiPath workflow may expect `BusinessRuleException` for a deterministic business/validation mismatch. The implementation may map/wrap suitable failures when required, but must preserve the original category and diagnostic context.

Examples commonly suitable for business-rule style handling:

- expected target state does not match a configured business precondition;
- typed value was executed but violates the required final value after retries;
- required postcondition definitively fails.

Examples commonly technical/system:

- selector parsing failure;
- UI automation technology unavailable;
- application not reachable;
- target resolution infrastructure failure.

Do not force every error into BusinessRuleException.

## 4. Retry Exhaustion

When retries are exhausted, preserve the last meaningful root failure as inner/context data and expose:

- attempts;
- elapsed duration;
- last validation stage;
- last failure category;
- last safe diagnostic message.

## 5. ThrowOnFailure

When `true`, throw/mapped exception after final diagnostics/result completion.

When `false`, return `UIValidationResult.Success = false` and do not throw the validation failure. Runtime cancellation and catastrophic platform exceptions may still follow platform semantics.

## 6. Secondary Failures

Failures in logging, screenshot capture, or optional telemetry must not replace the primary automation failure. Record them as secondary diagnostics where possible.
