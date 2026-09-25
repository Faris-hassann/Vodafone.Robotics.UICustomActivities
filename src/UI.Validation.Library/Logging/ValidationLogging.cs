using System.Text.Json;
using Vodafone.Robotics.UiValidation.Models;

namespace Vodafone.Robotics.UiValidation.Logging;

public sealed record ValidationLogEvent(
    ValidationLogLevel Level,
    string EventName,
    string Message,
    string ActivityType,
    ValidationStage Stage,
    int Attempt,
    int MaxAttempts,
    string CorrelationId,
    IReadOnlyDictionary<string, string> Context);

public interface IValidationLogger { void Log(ValidationLogEvent logEvent); }

public sealed class NullValidationLogger : IValidationLogger
{
    public static NullValidationLogger Instance { get; } = new();
    private NullValidationLogger() { }
    public void Log(ValidationLogEvent logEvent) { }
}

public static class SafeLog
{
    public static string Value(string? value, bool reveal, bool secure = false) => secure || !reveal ? $"<redacted:length={value?.Length ?? 0}>" : value ?? "<null>";

    public static string Selector(string selector, bool reveal)
    {
        if (reveal) return selector;
        var tagCount = selector.Count(c => c == '<');
        return $"<selector:redacted;length={selector.Length};tags={tagCount}>";
    }

    public static string SerializeContext(IReadOnlyDictionary<string, string> context) => JsonSerializer.Serialize(context);
}
