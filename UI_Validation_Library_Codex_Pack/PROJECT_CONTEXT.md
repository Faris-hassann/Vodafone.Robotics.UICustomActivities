# Project Context

## Project Name

`UI_Validation_Library`

## Phase 1 Purpose

Create a reusable UiPath custom-activity library that wraps common UI operations with strong validation before and after the operation.

The library must answer three questions for every activity:

1. **Did we find the correct target?**
2. **Did we perform the intended operation safely?**
3. **Can we prove the expected outcome occurred?**

## Phase 1 Public Activities

| Priority | Activity | Public Studio Name | Core Goal |
|---|---|---|---|
| High | Get Text | Validated Get Text | Read and validate text from the intended target |
| High | Type Into | Validated Type Into | Write and verify the correct final value in the intended input |
| High | Click | Validated Click | Click the intended control and verify the resulting state when configured |

## Why These Activities

The source backlog identifies Get Text, Type Into, and Click as High-priority UI Automation activities. The provided CRM sample workflows also demonstrate the current pattern the library is intended to generalize:

- precondition/element-exists check;
- attach/activate UI context where needed;
- perform the UiPath action;
- postcondition validation;
- Retry Scope;
- success/failure logging;
- throw/rethrow behavior;
- process/component monitoring.

The library must preserve the strengths of that pattern while removing CRM-specific selectors and duplicated validation sequences.

## Target Technology Direction

- Modern UiPath **Windows** projects are the Phase 1 target.
- Implementation is primarily C#/.NET.
- Use supported UiPath UI automation APIs/activities appropriate to the repository's installed versions.
- Keep validation/business logic sufficiently decoupled to make future runtime/technology adapters possible.

## Package Direction

Ship the three public activities in one package/library. Do not create three independent NuGet packages.

## Phase 1 Non-Goals

The backlog contains many additional activities, but they are not public Phase 1 deliverables. Examples include Get Attribute, Take Screenshot, Select Item, Element Exists, Attach Window, Activate, Maximize Window, Double Click, Hover, browser navigation, and application lifecycle activities.

They may be used internally where needed through supported UiPath functionality, but do not expose them as separate public validated activities unless the scope is explicitly expanded.
