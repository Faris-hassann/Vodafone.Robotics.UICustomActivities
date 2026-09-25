namespace Vodafone.Robotics.UiValidation.Models;

public sealed record TextComparisonOptions(bool CaseSensitive = true, bool TrimForValidation = false, bool NormalizeWhitespace = false);

public sealed record TargetIdentityOptions(
    string? ExpectedName = null,
    string? ExpectedText = null,
    string? ExpectedRole = null,
    string? ExpectedId = null,
    string? ExpectedClass = null,
    string? ExpectedAutomationId = null,
    bool RequireVisible = true,
    bool RequireEnabled = false);

public sealed record ConditionDefinition(
    ConditionKind Kind = ConditionKind.None,
    string? Selector = null,
    string? AttributeName = null,
    string? ExpectedValue = null,
    bool Waitable = true);

public sealed record PostConditionDefinition(
    PostConditionKind Kind = PostConditionKind.None,
    string? Selector = null,
    string? AttributeName = null,
    string? ExpectedValue = null,
    bool Waitable = true);

public abstract record ActivityConfiguration
{
    public string ActivityType { get; init; } = string.Empty;
    public string ActivityName { get; init; } = string.Empty;
    public string WorkSelector { get; init; } = string.Empty;
    public string? CorrelationId { get; init; }
    public int TimeoutMilliseconds { get; init; } = 10_000;
    public int RetryCount { get; init; } = 2;
    public int RetryIntervalMilliseconds { get; init; } = 500;
    public bool AllowMultipleMatches { get; init; }
    public bool ThrowOnFailure { get; init; } = true;
    public bool ScreenshotOnFinalFailure { get; init; } = true;
    public bool LogSensitiveValues { get; init; }
    public bool LogFullSelector { get; init; }
    public TargetIdentityOptions Identity { get; init; } = new();
    public ConditionDefinition PreCondition { get; init; } = new();
    public PostConditionDefinition PostCondition { get; init; } = new();
    public TextComparisonOptions Comparison { get; init; } = new();
}

public sealed record GetTextConfiguration : ActivityConfiguration
{
    public IReadOnlyList<TextRule> Rules { get; init; } = new[] { TextRule.RetrievedSuccessfully };
    public string? ExpectedText { get; init; }
    public string? RegexPattern { get; init; }
    public bool WaitForRules { get; init; } = true;
}

public sealed record TypeIntoConfiguration : ActivityConfiguration
{
    public TypeIntoMode Mode { get; init; } = TypeIntoMode.Replace;
    public string InputText { get; init; } = string.Empty;
    public string AppendSeparator { get; init; } = string.Empty;
    public string? ExpectedExistingValue { get; init; }
    public string? ExpectedFinalValue { get; init; }
    public VerificationMode VerificationMode { get; init; } = VerificationMode.Auto;
    public string? VerificationAttribute { get; init; }
    public bool IsSensitive { get; init; } = true;
    public bool IsSecure { get; init; }
    public bool DisallowEmptyInput { get; init; }
}

public sealed record ClickConfiguration : ActivityConfiguration
{
    public bool AllowActionRetry { get; init; }
    public bool SkipWhenPostConditionAlreadySatisfied { get; init; } = true;
}
