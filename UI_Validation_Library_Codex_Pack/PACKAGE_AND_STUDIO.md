# Package and UiPath Studio Contract

## 1. Package

Ship Phase 1 as one NuGet/custom activity package. Match repository naming/versioning conventions.

Suggested product identity:

- Library: `UI_Validation_Library`
- Studio category: `UI Validation`

Do not split GetText, TypeInto, and Click into separate packages.

## 2. Runtime

Target modern UiPath Windows projects for Phase 1.

Do not add Windows-Legacy compatibility unless explicitly required by the repository/client environment.

## 3. Studio Experience

Each public activity must have:

- clear display name;
- concise description;
- logically grouped properties;
- safe defaults;
- input/output descriptions;
- category metadata;
- no CRM-specific naming.

## 4. Dependency Policy

Use versions compatible with the existing repository. Do not blindly upgrade the project to the newest UiPath packages merely because newer versions exist.

Avoid unnecessary dependencies.

## 5. Package Verification

Before FINISHED:

- restore succeeds;
- build succeeds;
- package generation succeeds;
- package can be consumed by a clean/sample UiPath Windows project where the environment permits;
- all three activities appear and can be configured;
- public metadata is understandable.
