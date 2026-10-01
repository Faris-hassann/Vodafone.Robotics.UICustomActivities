using UiPath.Core;
using Vodafone.Robotics.UiValidation.Exceptions;

namespace Vodafone.Robotics.UiValidation.Automation;

public sealed class UiPathAutomationAdapter : IUiAutomationAdapter
{
    public IReadOnlyList<IUiTarget> Resolve(string selector, int timeoutMilliseconds, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        try
        {
            var parsed = new Selector(selector);
            var count = parsed.GetTagCount();
            if (count < 2)
            {
                return new IUiTarget[] { new UiPathTarget(CreateElement(parsed, timeoutMilliseconds, cancellationToken)) };
            }

            var parentSelector = parsed.Subselector(0, count - 1);
            var targetSelector = parsed.Subselector(count - 1, count);
            using var parent = CreateElement(parentSelector, timeoutMilliseconds, cancellationToken);
            var elements = parent.FindAll(FindScope.FIND_DESCENDANTS, targetSelector, null!);
            return elements.Select(e => (IUiTarget)new UiPathTarget(e)).ToArray();
        }
        catch (OperationCanceledException) { throw; }
        catch (SelectorNotFoundException) { return Array.Empty<IUiTarget>(); }
        catch (InvalidSelectorException ex) { throw new UIConfigurationException($"Selector syntax is invalid: {ex.Message}"); }
        catch (FindElementException) { return Array.Empty<IUiTarget>(); }
    }

    public string? CaptureScreenshot(string selector, string destinationPath, int timeoutMilliseconds, CancellationToken cancellationToken)
    {
        var targets = Resolve(selector, timeoutMilliseconds, cancellationToken);
        try
        {
            if (targets.Count == 0 || targets[0] is not UiPathTarget target) return null;
            Directory.CreateDirectory(Path.GetDirectoryName(destinationPath)!);
            using var image = target.Element.ScreenshotUnsafe();
            image.SaveFile(destinationPath);
            return destinationPath;
        }
        finally
        {
            foreach (var target in targets) target.Dispose();
        }
    }

    private static UiElement CreateElement(Selector selector, int timeoutMilliseconds, CancellationToken cancellationToken)
    {
        var element = new UiElement(selector, timeoutMilliseconds);
        element.Cancel += (out bool cancel) => cancel = cancellationToken.IsCancellationRequested;
        return element;
    }

    private sealed class UiPathTarget : IUiTarget
    {
        internal UiElement Element { get; }
        public UiPathTarget(UiElement element) => Element = element;
        public string? GetText()
        {
            string? attributeText = null;
            try { attributeText = Element.Get("text", true)?.ToString(); }
            catch (ElementOperationException) { }
            if (!string.IsNullOrEmpty(attributeText)) return attributeText;
            try
            {
                var scraped = Element.Scrape(new ScrapeOptions { ScrapingMethod = ScrapingMethod.AUTOMATIC }).Text;
                if (scraped is not null) return scraped;
            }
            catch
            {
                // Attribute/value fallback below remains truthful when the target does not support scraping.
            }
            try { return Element.Get("value", true)?.ToString() ?? attributeText; }
            catch (ElementOperationException) { return attributeText; }
        }
        public object? GetAttribute(string attributeName) => Element.Get(attributeName, true);
        public bool IsVisible => Element.IsVisible() || Element.IsHighlightable();
        public bool IsEnabled
        {
            get
            {
                object? enabledValue = null;
                try { enabledValue = Element.Get("enabled", true); }
                catch (ElementOperationException) { }

                if (UiPathAttributeParser.TryGetBoolean(enabledValue, out var enabled))
                {
                    return enabled;
                }

                object? accessibilityState = null;
                try { accessibilityState = Element.Get("aastate", true); }
                catch (ElementOperationException) { }

                // Some providers expose an empty enabled attribute. Fall back to accessibility state;
                // if neither property is decisive, the action itself remains the final authority.
                return UiPathAttributeParser.ResolveEnabled(enabledValue, accessibilityState);
            }
        }
        public void Click() => Element.Click(ClickType.CLICK_SINGLE, MouseButton.BTN_LEFT, InputMethod.API);
        public void SetText(string value)
        {
            Element.SetFocus();
            Element.WriteText("[k(end)d(shift)k(home)u(shift)k(del)]", InputMethod.WINDOW_MESSAGES);
            if (value.Length > 0) Element.WriteText(value, InputMethod.WINDOW_MESSAGES);
        }
        public void Dispose() => Element.Dispose();
    }
}
