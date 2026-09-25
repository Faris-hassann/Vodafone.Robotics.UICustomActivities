# Architecture Specification

## 1. Architectural Goal

Build three small public activities over a shared internal validation platform.

Do not implement each activity as a monolithic class containing duplicate selector resolution, logging, retry, screenshot, comparison, result, and exception code.

## 2. Conceptual Module Layout

Adapt names to repository conventions, but preserve responsibilities:

```text
UI_Validation_Library
├── Activities
│   ├── ValidatedGetText
│   ├── ValidatedTypeInto
│   └── ValidatedClick
├── Validation
│   ├── ConfigurationValidator
│   ├── TargetValidator
│   ├── TextValidator
│   ├── StateValidator
│   ├── PostconditionValidator
│   └── ValidationEngine
├── Ui
│   ├── TargetResolver
│   ├── UiElementAdapter
│   └── ActionExecutor(s)
├── Retry
│   └── RetryPolicy
├── Logging
│   ├── IValidationLogger
│   └── ValidationLogger
├── Diagnostics
│   └── ScreenshotService
├── Exceptions
├── Models
│   ├── UIValidationResult
│   ├── FailureCategory
│   ├── ValidationStage
│   └── configuration/condition models
└── Internal
```

This is conceptual. Match the actual repository and UiPath activity SDK style.

## 3. Separation of Concerns

### Public Activity Layer

Responsible for:

- exposing Studio properties;
- coordinating execution;
- adapting workflow cancellation/context;
- returning public outputs;
- mapping final failures according to `ThrowOnFailure`.

Not responsible for implementing every validator inline.

### Configuration Validation

Pure or mostly pure validation of property combinations. Must run before mutation.

### Target Resolution

Responsible for locating the UI target through supported UiPath mechanisms and reporting zero/one/many matches where available.

### Target Validation

Validates identity and state:

- expected role/control type;
- text/name/id/class/attributes;
- visible/enabled/interactable state;
- uniqueness policy.

### Action Executor

Performs the read/type/click operation using supported UiPath technology-specific APIs.

### Postcondition Validator

Evaluates the configured outcome without assuming action success.

### Retry Policy

Takes classified failures/states and decides whether to retry, delay, poll only, or stop.

### Logger

Produces structured + human-readable events. Logging is cross-cutting but should not determine core business behavior.

### Result Builder

Builds one consistent structured result for all three activities.

## 4. Dependency Direction

Prefer this direction:

```text
Activities -> interfaces/services -> UiPath adapters
           -> pure validation models/functions
```

Pure validation code must not require a live UI where unnecessary.

## 5. Extensibility

Design shared validation so later activities such as Get Attribute, Select Item, Double Click, Element Exists, Navigate To, or Open Application can reuse it.

Do not over-engineer Phase 1 into a generic framework that delays delivery. Extract only abstractions justified by the three activities and their tests.

## 6. Threading and Cancellation

Respect the activity execution model used by the target UiPath SDK. Long waits/retry delays must observe cancellation where supported. Do not swallow cancellation and continue retries.

## 7. Truthful Verification

A core architectural rule is that the library must report what it actually verified.

Examples:

- if exact secure-text readback is impossible, do not set `ExactValueVerified = true`;
- if Click has no postcondition, do not imply outcome verification;
- if a target technology cannot expose uniqueness, report the strongest supported target confidence and document the limitation;
- if screenshot capture fails, preserve primary failure.
