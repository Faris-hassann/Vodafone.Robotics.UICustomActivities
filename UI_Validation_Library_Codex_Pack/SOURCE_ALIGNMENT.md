# Source Alignment

This specification pack was derived from the supplied project artifacts and the agreed design decisions.

## 1. Backlog Alignment

The backlog identifies the following as **High** priority:

- Get Text
- Type Into
- Click

It also identifies supporting UI concepts such as Element Exist, Attach Window, Activate, Maximize Window, Open/Attach Browser, and application/browser actions. Phase 1 deliberately publishes only the three selected activities while allowing supported internal dependencies as needed.

## 2. Existing CRM Workflow Patterns

Three supplied UiPath workflows demonstrate existing validation practices:

### CRM_AccountHomePage_ReadLastBillAmount

Observed pattern:

- element existence as precondition;
- Retry Scope with configurable retry count/interval;
- attach/activate/maximize application;
- read value;
- validate non-empty result;
- success/failure logs;
- throw/rethrow and component monitoring.

### CRM_HomePage_ClickSearchIcon

Observed pattern:

- search-button existence precondition;
- attach/activate application;
- click action;
- postcondition checks for the next target (Customer ID field);
- Retry Scope;
- logging and throw/rethrow.

### CRM_SearchPage_EnterCustomerID

Observed pattern:

- input existence precondition;
- attach/activate application;
- Type Into with field clearing;
- read-back validation;
- retry;
- logging and throw/rethrow.

## 3. What This Library Improves

The library generalizes those workflow-specific patterns and adds:

- shared validators rather than copied workflow sequences;
- target identity and ambiguity checks, not existence only;
- configuration validation before UI mutation;
- structured results and failure categories;
- sensitive-data-aware logging;
- safe click retry/idempotency controls;
- exact Append/Replace semantics;
- multiple GetText validation modes;
- explicit application-side transformation handling;
- deterministic automated test host;
- unit/component/integration/E2E test layers.

## 4. Reference Codex Pack Pattern

The implementation pack follows the supplied agent-pack philosophy:

- one mandatory agent execution contract;
- module/activity-specific specifications;
- repository-first behavior;
- executable-test intent;
- prohibition on deleting/skipping required tests merely to obtain green status;
- strict Definition of Done;
- full regression before declaring FINISHED.

This pack is MD-only. The coding agent is expected to generate/adapt executable tests inside the actual repository.
