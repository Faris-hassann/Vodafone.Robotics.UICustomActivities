using System.Windows.Forms;

namespace UI.Validation.TestHost;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        Application.SetHighDpiMode(HighDpiMode.SystemAware);
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new TestHostForm());
    }
}

public sealed class TestHostForm : Form
{
    private readonly TextBox _emptyInput = Input("EmptyInput", "");
    private readonly TextBox _prepopulatedInput = Input("PrepopulatedInput", "Hello");
    private readonly Label _result = Label("ResultLabel", "");
    private readonly Label _persisted = Label("PersistedLabel", "");
    private readonly Label _delayed = Label("DelayedLabel", "Pending");
    private readonly Label _counter = Label("ClickCounter", "0");
    private readonly Label _appearTarget = Label("AppearTarget", "Visible");

    public TestHostForm()
    {
        Name = "UIValidationTestHost";
        Text = "UI Validation Deterministic Test Host";
        Width = 900;
        Height = 700;
        var panel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, WrapContents = true, AutoScroll = true };
        Controls.Add(panel);

        panel.Controls.AddRange(new Control[]
        {
            _emptyInput, _prepopulatedInput,
            Input("DisabledInput", "Disabled", enabled: false),
            UppercaseInput(), DelayedValueInput(), Input("SecureInput", "", password: true),
            Label("PlainLabel", "Ready"), Label("EmptyLabel", ""), Label("WhitespaceLabel", "   "), _delayed, _result, _persisted,
            Button("SearchButton", async () => { await Task.Delay(300); _result.Text = $"Customer:{_emptyInput.Text}"; }),
            Button("SaveButton", () => _persisted.Text = _prepopulatedInput.Text),
            Button("AppearButton", () => _appearTarget.Visible = true),
            Button("DisappearButton", () => _appearTarget.Visible = false),
            Button("StateChangeButton", () => _appearTarget.Enabled = !_appearTarget.Enabled),
            Button("DelayedSuccessButton", async () => { await Task.Delay(400); _delayed.Text = "Loaded"; }),
            Button("NonIdempotentCounterButton", async () =>
            {
                _counter.Text = (int.Parse(_counter.Text, System.Globalization.CultureInfo.InvariantCulture) + 1).ToString(System.Globalization.CultureInfo.InvariantCulture);
                await Task.Delay(350);
                _delayed.Text = "Loaded";
            }),
            Button("DisabledButton", () => { }, enabled: false), _counter, _appearTarget,
            Button("ResetButton", ResetState),
            Label("AmbiguousTarget", "Duplicate A"), Label("AmbiguousTarget", "Duplicate B")
        });
    }

    private void ResetState()
    {
        _emptyInput.Text = string.Empty;
        _prepopulatedInput.Text = "Hello";
        _result.Text = string.Empty;
        _persisted.Text = string.Empty;
        _delayed.Text = "Pending";
        _counter.Text = "0";
        _appearTarget.Visible = true;
        _appearTarget.Enabled = true;
    }

    private static TextBox Input(string name, string text, bool enabled = true, bool password = false) => new() { Name = name, Text = text, Enabled = enabled, UseSystemPasswordChar = password, Width = 260 };
    private static Label Label(string name, string text) => new() { Name = name, Text = text, AutoSize = true };
    private static TextBox UppercaseInput()
    {
        var input = Input("UppercaseInput", "");
        input.CharacterCasing = CharacterCasing.Upper;
        return input;
    }
    private static TextBox DelayedValueInput()
    {
        var input = Input("DelayedValueInput", "");
        input.TextChanged += async (_, _) => { var value = input.Text; await Task.Delay(250); input.Tag = value; };
        return input;
    }
    private static Button Button(string name, Action action, bool enabled = true) => new Button { Name = name, Text = name, Enabled = enabled, AutoSize = true }.Tap(button => button.Click += (_, _) => action());
    private static Button Button(string name, Func<Task> action) => new Button { Name = name, Text = name, AutoSize = true }.Tap(button => button.Click += async (_, _) => await action());
}

internal static class ControlExtensions
{
    public static T Tap<T>(this T value, Action<T> configure) { configure(value); return value; }
}
