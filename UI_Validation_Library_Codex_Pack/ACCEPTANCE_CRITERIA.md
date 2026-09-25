# Phase 1 Acceptance Criteria

## Global

AC-G01. The solution exposes exactly three public Phase 1 activities: Validated Get Text, Validated Type Into, Validated Click.

AC-G02. They ship in one installable library/package for modern UiPath Windows projects.

AC-G03. Implementation is primarily C#/.NET and uses supported UiPath UI automation APIs appropriate to the actual repository environment.

AC-G04. Shared validation/retry/logging/result infrastructure is reused across activities.

AC-G05. Invalid activity configuration fails before any UI-changing action is performed.

AC-G06. Target validation supports more than element existence and can validate identity/state where supported.

AC-G07. Unexpected multiple matches fail by default unless explicitly allowed.

AC-G08. `ThrowOnFailure` defaults to true and can be explicitly disabled to return a failed structured result.

AC-G09. Activities provide safe timeout/retry defaults with activity-level overrides.

AC-G10. Cancellation stops further retry/action attempts cleanly.

## Get Text

AC-GT01. Raw retrieved text is returned without silent trim/case/whitespace mutation.

AC-GT02. The activity differentiates target-not-found, read failure, null, empty, and whitespace-only results.

AC-GT03. Supported rules include RetrievedSuccessfully, NotEmpty, Exact, Contains, StartsWith, EndsWith, Regex.

AC-GT04. Multiple enabled rules use AND logic by default.

AC-GT05. Case sensitivity is configurable and defaults to strict/case-sensitive.

AC-GT06. Optional normalization affects validation only, not returned raw text.

AC-GT07. The activity can retry/poll when target text is temporarily unavailable or has not yet met a waitable rule.

## Type Into

AC-TI01. Modes include Replace, Append, ClearOnly; Replace is default.

AC-TI02. Replace produces the intended replacement rather than accidental concatenation.

AC-TI03. Append expected value equals original + configured separator + input.

AC-TI04. Optional existing-value validation runs before mutation.

AC-TI05. Final value verification supports Auto, Text, ValueAttribute, SpecificAttribute or equivalent supported strategies.

AC-TI06. The activity supports explicit ExpectedFinalValue for intentional application transformations.

AC-TI07. Where determinable, mismatches classify PartialInput, UnexpectedPrefix, UnexpectedSuffix, DuplicateInput, NoChange, or UnexpectedTransformation.

AC-TI08. Secure/password fields never falsely report exact readback verification if unavailable.

AC-TI09. Sensitive input is not logged in clear text by default.

## Click

AC-CL01. The target is validated before clicking.

AC-CL02. Optional preconditions can validate target state/attributes before action.

AC-CL03. Supported postconditions include appear/disappear, text condition, attribute/state change, and window/page availability where supported.

AC-CL04. With a configured postcondition, success requires both click execution and postcondition success.

AC-CL05. Without a configured postcondition, success requires target validation + click execution and emits an `OutcomeNotIndependentlyVerified` style warning.

AC-CL06. Before repeating a click, the library checks whether the configured success condition is already true.

AC-CL07. `AllowActionRetry = false` prevents another click while allowing safe postcondition polling/waiting.

AC-CL08. Configuration defects in postconditions are detected before the first click.

## Logging / Diagnostics

AC-L01. Activities produce structured and human-readable lifecycle logs.

AC-L02. Logs include dynamic context such as activity, stage, attempt, duration, validation mode, and correlation ID.

AC-L03. Sensitive values are masked/redacted by default.

AC-L04. Full selector logging is off by default; safe selector summary is preferred.

AC-L05. Final failure screenshot is enabled by default, success/retry screenshots disabled by default.

AC-L06. Screenshot failure is secondary and cannot replace the primary activity failure.

## Testing / Delivery

AC-T01. Unit, component, integration, and E2E tests are implemented.

AC-T02. Integration/E2E tests use a deterministic test UI rather than relying on production CRM or another uncontrolled external app.

AC-T03. Positive, negative, boundary, retry, cancellation, logging, redaction, and diagnostic scenarios are covered.

AC-T04. Targeted tests and relevant regression tests pass before FINISHED.

AC-T05. Activities are discoverable in UiPath Studio with clear property names/descriptions.

AC-T06. NuGet/package restore/install and build are verified.
