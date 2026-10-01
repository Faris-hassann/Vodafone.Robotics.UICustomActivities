# Vodafone Robotics UI Validation Activities

Version 2 of the UiPath Windows activities for **Validated Get Text**, **Validated Type Into**, and **Validated Click**.

Each activity exposes independent complete-selector inputs for PreCondition, Target, and PostCondition stages, privacy-safe structured logging, classified failures, bounded retry behavior, and a common `UIValidationResult` output.

Each activity now has one required **Target Selector**, plus optional **PreCondition
Selector** and **PostCondition Selector** inputs. Selector attributes identify the target
directly, and every visible argument provides hover help in UiPath Studio.

Install this package in a modern UiPath Windows project and find the activities under **UI Validation**.
