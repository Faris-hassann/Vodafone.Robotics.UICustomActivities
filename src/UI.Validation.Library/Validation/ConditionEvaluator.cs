using Vodafone.Robotics.UiValidation.Automation;
using Vodafone.Robotics.UiValidation.Models;

namespace Vodafone.Robotics.UiValidation.Validation;

public sealed class ConditionEvaluator
{
    private readonly IUiAutomationAdapter _adapter;
    public ConditionEvaluator(IUiAutomationAdapter adapter) => _adapter = adapter;

    public bool Evaluate(ConditionDefinition condition, int timeout, TextComparisonOptions comparison, CancellationToken cancellationToken)
    {
        if (condition.Kind == ConditionKind.None) return true;
        var targets = SafeResolve(condition.Selector!, timeout, cancellationToken);
        try
        {
            if (condition.Kind == ConditionKind.Exists) return targets.Count > 0;
            if (condition.Kind == ConditionKind.NotExists) return targets.Count == 0;
            if (targets.Count != 1) return false;
            var target = targets[0];
            return condition.Kind switch
            {
                ConditionKind.TextEquals => TextValidation.Equals(target.GetText(), condition.ExpectedValue, comparison),
                ConditionKind.TextContains => Contains(target.GetText(), condition.ExpectedValue, comparison),
                ConditionKind.AttributeEquals => TextValidation.Equals(target.GetAttribute(condition.AttributeName!)?.ToString(), condition.ExpectedValue, comparison),
                ConditionKind.Enabled => target.IsEnabled,
                ConditionKind.Disabled => !target.IsEnabled,
                ConditionKind.Visible => target.IsVisible,
                ConditionKind.Hidden => !target.IsVisible,
                _ => false
            };
        }
        finally { Dispose(targets); }
    }

    public bool Evaluate(PostConditionDefinition condition, int timeout, TextComparisonOptions comparison, string? priorState, CancellationToken cancellationToken)
    {
        if (condition.Kind == PostConditionKind.None) return true;
        var targets = SafeResolve(condition.Selector!, timeout, cancellationToken);
        try
        {
            if (condition.Kind is PostConditionKind.ElementAppears or PostConditionKind.WindowOrPageAppears) return targets.Count > 0;
            if (condition.Kind == PostConditionKind.ElementDisappears) return targets.Count == 0;
            if (targets.Count != 1) return false;
            var target = targets[0];
            return condition.Kind switch
            {
                PostConditionKind.TextEquals => TextValidation.Equals(target.GetText(), condition.ExpectedValue, comparison),
                PostConditionKind.TextContains => Contains(target.GetText(), condition.ExpectedValue, comparison),
                PostConditionKind.AttributeEquals => TextValidation.Equals(target.GetAttribute(condition.AttributeName!)?.ToString(), condition.ExpectedValue, comparison),
                PostConditionKind.AttributeChanges => !TextValidation.Equals(target.GetAttribute(condition.AttributeName!)?.ToString(), priorState, comparison),
                PostConditionKind.TargetStateChanges => condition.ExpectedValue is null
                    ? !string.Equals(StateSignature(target), priorState, StringComparison.Ordinal)
                    : TextValidation.Equals(target.GetAttribute("state")?.ToString(), condition.ExpectedValue, comparison),
                _ => false
            };
        }
        finally { Dispose(targets); }
    }

    public string? CapturePriorState(PostConditionDefinition condition, int timeout, CancellationToken cancellationToken)
    {
        if (condition.Kind is not (PostConditionKind.AttributeChanges or PostConditionKind.TargetStateChanges)) return null;
        var targets = SafeResolve(condition.Selector!, timeout, cancellationToken);
        try
        {
            if (targets.Count != 1) return null;
            return condition.Kind == PostConditionKind.AttributeChanges
                ? targets[0].GetAttribute(condition.AttributeName!)?.ToString()
                : StateSignature(targets[0]);
        }
        finally { Dispose(targets); }
    }

    private static string StateSignature(IUiTarget target) => $"visible={target.IsVisible};enabled={target.IsEnabled}";

    private IReadOnlyList<IUiTarget> SafeResolve(string selector, int timeout, CancellationToken token)
    {
        try { return _adapter.Resolve(selector, timeout, token); }
        catch (OperationCanceledException) { throw; }
        catch { return Array.Empty<IUiTarget>(); }
    }

    private static void Dispose(IReadOnlyList<IUiTarget> targets)
    {
        foreach (var target in targets) target.Dispose();
    }

    private static bool Contains(string? actual, string? expected, TextComparisonOptions options)
    {
        if (actual is null || expected is null) return false;
        return TextValidation.Normalize(actual, options).Contains(TextValidation.Normalize(expected, options), options.CaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase);
    }
}
