# Agent Implementation Workflow

Use this loop for each activity or Phase 1.

## Step 1 — Read Contract

Read:

- Agent.md
- Project context
- Architecture
- Shared validation rules
- relevant Activity spec
- relevant Test Cases
- Definition of Done

## Step 2 — Inspect Repository

Determine:

- current projects/solution;
- target framework;
- UiPath activity dependencies;
- how custom activities are authored in this repo;
- test framework;
- package build pipeline;
- existing logging/utilities;
- current naming conventions.

## Step 3 — Gap Analysis

Write a short internal implementation plan mapping each required behavior to:

- existing capability;
- new code needed;
- tests needed;
- known environment limitation.

Do not code around missing understanding of the actual project structure.

## Step 4 — Implement Shared Foundation First Where Needed

Examples:

- failure/stage enums;
- result model;
- config validators;
- logger abstraction;
- retry policy;
- safe comparison utilities;
- target adapter abstraction.

Do not build abstractions not required by the current three activities.

## Step 5 — Implement One Activity Increment

Prefer vertical slices that can be tested.

Example for GetText:

1. config + target resolution;
2. raw read;
3. state classification;
4. validation rules;
5. retry;
6. diagnostics/result;
7. integration coverage.

## Step 6 — Build and Run Targeted Tests

Run the smallest relevant test set after each meaningful increment.

Fix root causes.

## Step 7 — Integration/E2E

Wire deterministic test host and execute UI tests when environment permits.

## Step 8 — Regression

Run all relevant solution tests, not only newly added tests.

## Step 9 — Package / Studio Smoke

Build package and verify Studio discovery where possible.

## Step 10 — Final Report

Use the provided template. Do not state FINISHED when required UI tests or package smoke remain unexecuted.
