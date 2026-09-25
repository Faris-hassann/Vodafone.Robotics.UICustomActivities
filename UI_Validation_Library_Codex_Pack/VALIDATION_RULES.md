# Shared Validation Rules

## 1. Rule Philosophy

Validation is explicit, deterministic, and truthful. A successful UiPath API call alone is not sufficient evidence that the correct target or intended outcome was achieved.

## 2. Configuration Rules

Configuration validation executes first.

Reject invalid states such as:

- negative retry counts;
- invalid timeout values;
- invalid regular expressions;
- unknown comparison/verification modes;
- missing attribute name for attribute validation;
- missing target/value required by a postcondition;
- TypeInto mode conflicts;
- incompatible secure-input and exact-readback requirements;
- an Append separator used with a mode where it has no meaning, if the implementation treats that as invalid rather than ignored.

Prefer explicit validation errors over silently ignoring contradictory configuration.

## 3. Target Identity Rules

Support identity checks as allowed by the target technology. Candidate fields may include:

- name;
- text;
- role/control type;
- ID;
- class;
- automation ID;
- virtual name;
- arbitrary attribute;
- visibility;
- enabled state.

Expected identity conditions must be independently configurable rather than hard-coded to CRM-specific values.

## 4. Target Ambiguity

Default behavior: exactly one intended target.

If more than one match is returned and the configuration does not explicitly allow ambiguity, fail with a target-ambiguity category before executing a mutating action.

Do not silently choose the first match as the general default.

## 5. Comparison Rules

Default string semantics:

- ordinal/exact character comparison unless the runtime requires an equivalent deterministic mode;
- case-sensitive by default;
- no trim by default;
- no whitespace collapse by default;
- no Unicode normalization by default.

Optional normalization may include:

- Trim;
- IgnoreCase;
- whitespace normalization;
- developer-specified expected transformed value.

Normalization applies only to the comparison view unless the activity specification explicitly says otherwise. Never silently alter GetText's returned raw value.

## 6. Null / Empty / Whitespace

Treat these as separate states:

- Null/NoValue
- Empty (`""`)
- WhitespaceOnly
- NonEmpty

Validation rules may choose which states are acceptable.

## 7. Regex

Compile/validate regex configuration before UI action/read where practical. Invalid patterns are configuration errors and are not retryable.

## 8. Precondition Rules

A precondition verifies the state in which an operation is allowed to run. Examples:

- expected existing field value;
- button enabled;
- target text equals `Submit`;
- status equals `Draft`;
- specific attribute equals expected value.

Failure classification depends on whether the configured precondition is expected to become true during a wait window or is a deterministic business mismatch.

## 9. Postcondition Rules

A postcondition proves the outcome. A postcondition should be polled separately from re-executing a non-idempotent action.

Common conditions:

- target appears;
- target disappears;
- text exact/contains;
- attribute exact/changes;
- state changes;
- page/window becomes available.

## 10. Validation Evidence

Results/logs must distinguish:

- target resolved;
- target identity validated;
- action executed;
- readback verified;
- postcondition verified;
- verification unavailable.

Do not combine these into one opaque boolean internally.
