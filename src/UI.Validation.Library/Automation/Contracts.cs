namespace Vodafone.Robotics.UiValidation.Automation;

public interface IUiAutomationAdapter
{
    IReadOnlyList<IUiTarget> Resolve(string selector, int timeoutMilliseconds, CancellationToken cancellationToken);
    string? CaptureScreenshot(string selector, string destinationPath, int timeoutMilliseconds, CancellationToken cancellationToken);
}

public interface IUiTarget : IDisposable
{
    string? GetText();
    object? GetAttribute(string attributeName);
    bool IsVisible { get; }
    bool IsEnabled { get; }
    void Click();
    void SetText(string value);
}
