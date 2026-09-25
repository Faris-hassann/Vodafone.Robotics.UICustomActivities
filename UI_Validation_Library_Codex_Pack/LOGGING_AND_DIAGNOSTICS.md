# Logging and Diagnostics

## 1. Logging Goals

Logs must explain where the validation lifecycle is, what was attempted, and why a failure occurred without exposing sensitive data by default.

## 2. Logging Abstraction

Do not hard-couple the library to the CRM sample's `IAP_Logging` implementation.

Provide/use an internal logging abstraction that can route to standard UiPath logging and can be extended/adapted to project-specific logging later.

## 3. Lifecycle Events

Support events equivalent to:

- `ActivityStarted`
- `ConfigurationValidated`
- `TargetSearchStarted`
- `TargetFound`
- `TargetIdentityValidated`
- `PreconditionValidationStarted`
- `PreconditionValidated`
- `ActionStarted`
- `ActionCompleted`
- `PostconditionValidationStarted`
- `PostconditionValidated`
- `RetryScheduled`
- `ScreenshotCaptured`
- `ActivitySucceeded`
- `ActivityFailed`

Use appropriate levels. Detailed lifecycle can be Debug/Trace; key milestones Info; recoverable retries Warning/Debug depending policy; final failures Error.

## 4. Structured Context

Where available and safe, include:

- activity type;
- display name;
- workflow/process name;
- application/window summary;
- target description;
- safe selector summary;
- validation stage;
- attempt number / max attempts;
- timeout;
- verification mode;
- expected/actual summary;
- failure category;
- elapsed duration;
- correlation ID.

## 5. Sensitive Data

Default: do not log clear-text TypeInto input, passwords, account IDs, emails, names, or other potentially sensitive values.

Prefer:

- `<redacted>`;
- length;
- whether a comparison matched;
- semantic mismatch classification;
- safe partial metadata only when policy explicitly permits it.

`LogSensitiveValues = true` must be an explicit opt-in and still must not override secure/password protections.

## 6. Selector Logging

Default to a safe selector summary. Do not dump a full selector unless explicitly enabled.

If the selector contains potentially sensitive dynamic values, redaction must apply even when richer diagnostics are enabled unless the caller knowingly enables unsafe logging and policy permits it.

## 7. Correlation

If caller supplies CorrelationId, preserve it. Otherwise generate one for the activity execution. Allow a workflow to reuse one correlation ID across multiple validated activities.

## 8. Screenshots

Default behavior:

- final failure: capture;
- retry failure: do not capture;
- success: do not capture.

Recommended file naming concept:

`<Workflow>_<Activity>_<FailureCategory>_<CorrelationId>_<Timestamp>.png`

Sanitize filename components.

A screenshot failure is logged as a secondary diagnostic and must never replace the root automation failure.
