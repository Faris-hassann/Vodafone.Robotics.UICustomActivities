# Structured Result Model

## 1. Common Result

Provide a common result model for all three activities. Exact language/type names may match repository conventions.

Recommended fields:

```text
UIValidationResult
- Success : bool
- ActivityType
- ActivityName
- ValidationStage
- FailureCategory
- FailureReason
- AttemptCount
- MaxAttempts
- TargetFound
- TargetIdentityValidated
- ActionExecuted
- PostconditionValidated
- VerificationAvailable
- VerificationMode
- ExpectedValue or SafeExpectedSummary
- ActualValue or SafeActualSummary
- StartedAt
- CompletedAt
- Duration
- CorrelationId
- ScreenshotPath
- ExceptionType
- ExceptionMessage
- Metadata
```

## 2. Sensitive Values

The result object may need actual/expected values for workflow logic, but do not automatically mirror sensitive result fields into logs/telemetry.

If the project requires stronger privacy guarantees, provide safe-summary fields and make raw result values optional/configurable.

## 3. Activity-Specific Outputs

### Get Text

In addition to `UIValidationResult`, expose raw retrieved text.

### Type Into

Expose result plus useful safe verification status. Raw input already exists as input and does not need to be redundantly exposed unless a repository pattern requires it.

### Click

Expose result plus postcondition verification state.

## 4. Stage Model

Recommended stages:

- NotStarted
- ConfigurationValidation
- TargetResolution
- TargetIdentityValidation
- PreconditionValidation
- ActionExecution
- PostconditionValidation
- Diagnostics
- Completed

## 5. Failure Category Model

Recommended categories:

- None
- InvalidConfiguration
- TargetNotFound
- TargetAmbiguous
- TargetIdentityMismatch
- TargetNotReady
- ReadFailed
- ActionFailed
- TextValidationFailed
- PostconditionFailed
- RetryExhausted
- Cancelled
- Unknown
