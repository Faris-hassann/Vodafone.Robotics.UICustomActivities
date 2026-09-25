using System.Collections.ObjectModel;

namespace Vodafone.Robotics.UiValidation.Models;

public sealed class UIValidationResult
{
    public bool Success { get; init; }
    public string ActivityType { get; init; } = string.Empty;
    public string ActivityName { get; init; } = string.Empty;
    public ValidationStage ValidationStage { get; init; }
    public FailureCategory FailureCategory { get; init; }
    public string FailureReason { get; init; } = string.Empty;
    public int AttemptCount { get; init; }
    public int MaxAttempts { get; init; }
    public bool TargetFound { get; init; }
    public bool TargetIdentityValidated { get; init; }
    public bool ActionExecuted { get; init; }
    public bool PostConditionValidated { get; init; }
    public bool VerificationAvailable { get; init; }
    public string VerificationMode { get; init; } = string.Empty;
    public string SafeExpectedSummary { get; init; } = string.Empty;
    public string SafeActualSummary { get; init; } = string.Empty;
    public DateTimeOffset StartedAt { get; init; }
    public DateTimeOffset CompletedAt { get; init; }
    public TimeSpan Duration => CompletedAt - StartedAt;
    public string CorrelationId { get; init; } = string.Empty;
    public string? ScreenshotPath { get; init; }
    public string? ExceptionType { get; init; }
    public string? ExceptionMessage { get; init; }
    public RawValueState? RawValueState { get; init; }
    public MismatchKind MismatchKind { get; init; }
    public bool OutcomeIndependentlyVerified { get; init; }
    public IReadOnlyDictionary<string, string> Metadata { get; init; } = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>());
}

public sealed record ValidationExecutionResult(UIValidationResult Result, string? RawText = null);
