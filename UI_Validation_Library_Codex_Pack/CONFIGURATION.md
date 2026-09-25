# Configuration Contract

## 1. General Principle

Activities must be easy to use with safe defaults while exposing advanced overrides for enterprise workflows.

Exact property types/names may be adapted to the UiPath SDK conventions in the real repository, but preserve the semantics below.

## 2. Common Properties

Recommended common concepts:

| Property | Default | Meaning |
|---|---:|---|
| DisplayName | activity default | Studio display name |
| Target / Selector / UiElement | required as appropriate | Target identification |
| TimeoutProfile | Medium | Short/Medium/Long profile |
| CustomTimeout | null | Overrides profile when supplied |
| RetryCount | library default | Number of retry opportunities |
| RetryInterval | library default | Delay between retry/poll attempts |
| ValidateTargetIdentity | true when identity rules supplied | Whether to enforce identity rules |
| ExpectedTargetAttributes | none | Identity/state rules |
| AllowMultipleMatches | false | Explicit ambiguity override |
| ScreenshotOnFinalFailure | true | Diagnostic screenshot |
| ScreenshotOnRetryFailure | false | Avoid image spam |
| ScreenshotOnSuccess | false | Avoid image spam |
| LogSensitiveValues | false | Clear-text value logging is opt-in |
| LogFullSelector | false | Full selector logging is opt-in |
| CorrelationId | auto if absent | Correlates multiple activities |
| ThrowOnFailure | true | Throw vs return failed result |

## 3. Timeout Profiles

Initial recommended profiles are configurable centrally. Do not hard-code them so deeply that consumers cannot override them.

Concept:

- Short
- Medium
- Long
- Custom

The sample CRM workflows already use short/medium/long timeout ideas. The library turns this into a reusable configuration model.

## 4. Retry Settings

Phase 1 default retry strategy: Fixed interval.

Architecture may allow later Linear/Exponential strategies without exposing unnecessary complexity in the first release.

## 5. Target Input Flexibility

Support both where the chosen UiPath SDK allows it:

- selector/target descriptor; and
- existing UiElement/target reference.

Avoid resolving the same target again when a valid existing target object is intentionally supplied.

## 6. Property Validation

All public property combinations must have deterministic validation with actionable error messages.
