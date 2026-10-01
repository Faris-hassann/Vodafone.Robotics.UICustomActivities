# Activity reference

## Shared stage inputs

All three activities expose:

- `PreConditionSelector`, `PreConditionKind`, `PreConditionAttribute`, `PreConditionExpectedValue`
- required `TargetSelector`, plus `AllowMultipleMatches`, `RequireVisible`, and `RequireEnabled`
- `PostConditionSelector`, `PostConditionKind`, `PostConditionAttribute`, `PostConditionExpectedValue`

Each selector is a complete selector string and is resolved independently. Invalid configuration is rejected before mutation.

Shared operational inputs include timeout, retry count/interval, strict comparison controls, correlation ID, screenshot-on-final-failure, selector/value logging opt-ins, and `ThrowOnFailure`.

## Validated Get Text

Returns the unchanged raw `Text` and a `ValidationResult`. Rules are `RetrievedSuccessfully`, `NotEmpty`, `Exact`, `Contains`, `StartsWith`, `EndsWith`, and `Regex`. `Rule` and `AdditionalRule` use AND semantics. Trim/whitespace normalization affects comparison only.

Version `2.0.0` uses the same simplified selector surface for all three activities.
Selector attributes identify the target directly; the separate Expected Target Name,
Text, Role, ID, Class, and Automation ID arguments were removed. Every visible argument
has a Studio hover description with its purpose and an example.

## Validated Type Into

The required **Target Selector** is a complete UiPath selector XML string whose attributes identify the input control directly.

Modes are `Replace`, `Append`, and `ClearOnly`. Append computes `original + separator + input`. `ExpectedExistingValue` is checked before mutation. `ExpectedFinalValue` supports application-side transformations. Read-back strategies are Auto/Text/ValueAttribute/SpecificAttribute/ActionOnly. Secure input never claims exact read-back and is always redacted from logs.

## Validated Click

The target and optional precondition are validated before click. Supported postconditions cover appears/disappears, text equals/contains, attribute equals/changes, target state, and window/page appearance. `AllowActionRetry=false` is the safe default: the activity polls the postcondition without a second click. With no postcondition, action success is returned with `OutcomeNotIndependentlyVerified`.
