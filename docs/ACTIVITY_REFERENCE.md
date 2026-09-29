# Activity reference

## Shared stage inputs

All three activities expose:

- `PreConditionSelector`, `PreConditionKind`, `PreConditionAttribute`, `PreConditionExpectedValue`
- `DoWorkSelector` plus optional target identity fields (`ExpectedTargetName`, `ExpectedTargetText`, role, ID, class, automation ID, visible, enabled)
- `PostConditionSelector`, `PostConditionKind`, `PostConditionAttribute`, `PostConditionExpectedValue`

Each selector is a complete selector string and is resolved independently. Invalid configuration is rejected before mutation.

Shared operational inputs include timeout, retry count/interval, strict comparison controls, correlation ID, screenshot-on-final-failure, selector/value logging opt-ins, and `ThrowOnFailure`.

## Validated Get Text

Returns the unchanged raw `Text` and a `ValidationResult`. Rules are `RetrievedSuccessfully`, `NotEmpty`, `Exact`, `Contains`, `StartsWith`, `EndsWith`, and `Regex`. `Rule` and `AdditionalRule` use AND semantics. Trim/whitespace normalization affects comparison only.

## Validated Type Into

The required **Target Selector** is a complete UiPath selector XML string. Its selector attributes identify the input control directly, so new Type Into configurations do not show the separate Expected Target Name, Text, Role, ID, Class, or Automation ID fields. The serialized property remains `DoWorkSelector` for compatibility with existing workflows.

Modes are `Replace`, `Append`, and `ClearOnly`. Append computes `original + separator + input`. `ExpectedExistingValue` is checked before mutation. `ExpectedFinalValue` supports application-side transformations. Read-back strategies are Auto/Text/ValueAttribute/SpecificAttribute/ActionOnly. Secure input never claims exact read-back and is always redacted from logs. Every Type Into argument includes a Studio tooltip describing its purpose and a concrete example.

## Validated Click

The target and optional precondition are validated before click. Supported postconditions cover appears/disappears, text equals/contains, attribute equals/changes, target state, and window/page appearance. `AllowActionRetry=false` is the safe default: the activity polls the postcondition without a second click. With no postcondition, action success is returned with `OutcomeNotIndependentlyVerified`.
