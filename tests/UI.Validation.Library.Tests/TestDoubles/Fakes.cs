using Vodafone.Robotics.UiValidation.Automation;
using Vodafone.Robotics.UiValidation.Logging;

namespace UI.Validation.Library.Tests.TestDoubles;

internal sealed class FakeTarget : IUiTarget
{
    public string? Text { get; set; }
    public Queue<string?>? TextSequence { get; set; }
    public Dictionary<string, object?> Attributes { get; } = new(StringComparer.OrdinalIgnoreCase);
    public bool IsVisible { get; set; } = true;
    public bool IsEnabled { get; set; } = true;
    public int ClickCount { get; private set; }
    public int SetTextCount { get; private set; }
    public Action<FakeTarget>? OnClick { get; set; }
    public Action<FakeTarget, string>? OnSetText { get; set; }
    public Func<FakeTarget, string?>? OnGetText { get; set; }
    public Func<FakeTarget, string, object?>? OnGetAttribute { get; set; }
    public Exception? ClickException { get; set; }
    public Exception? SetTextException { get; set; }
    public string? GetText()
    {
        if (OnGetText is not null) return OnGetText(this);
        if (TextSequence is { Count: > 0 }) Text = TextSequence.Dequeue();
        return Text;
    }
    public object? GetAttribute(string attributeName) => OnGetAttribute is not null ? OnGetAttribute(this, attributeName) : Attributes.TryGetValue(attributeName, out var value) ? value : null;
    public void Click() { ClickCount++; if (ClickException is not null) throw ClickException; OnClick?.Invoke(this); }
    public void SetText(string value) { SetTextCount++; if (SetTextException is not null) throw SetTextException; Text = value; Attributes["value"] = value; OnSetText?.Invoke(this, value); }
    public void Dispose() { }
}

internal sealed class FakeAdapter : IUiAutomationAdapter
{
    public Dictionary<string, List<IUiTarget>> Targets { get; } = new(StringComparer.Ordinal);
    public Queue<IReadOnlyList<IUiTarget>>? ResolveSequence { get; set; }
    public bool ThrowOnScreenshot { get; set; }
    public int ResolveCount { get; private set; }
    public int ScreenshotCount { get; private set; }
    public IReadOnlyList<IUiTarget> Resolve(string selector, int timeoutMilliseconds, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ResolveCount++;
        if (ResolveSequence is { Count: > 0 }) return ResolveSequence.Dequeue();
        return Targets.TryGetValue(selector, out var targets) ? targets : Array.Empty<IUiTarget>();
    }
    public string? CaptureScreenshot(string selector, string destinationPath, int timeoutMilliseconds, CancellationToken cancellationToken)
    {
        ScreenshotCount++;
        if (ThrowOnScreenshot) throw new IOException("screenshot unavailable");
        return destinationPath;
    }
    public FakeAdapter Add(string selector, params IUiTarget[] targets) { Targets[selector] = targets.ToList(); return this; }
}

internal sealed class RecordingLogger : IValidationLogger
{
    public List<ValidationLogEvent> Events { get; } = new();
    public void Log(ValidationLogEvent logEvent) => Events.Add(logEvent);
}
