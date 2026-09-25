using System.Activities;
using System.ComponentModel;
using Vodafone.Robotics.UiValidation.Engine;
using Vodafone.Robotics.UiValidation.Models;

namespace Vodafone.Robotics.UiValidation.Activities;

[DisplayName("Validated Get Text")]
[Description("Reads text using a complete UiPath selector and validates PreCondition, Do Work, and PostCondition stages with structured logs.")]
[Category("UI Validation")]
public sealed class ValidatedGetText : ValidatedActivityBase
{
    public ValidatedGetText() => DisplayName = "Validated Get Text";

    [Category("2. Do Work - Text Rules")]
    public InArgument<TextRule> Rule { get; set; } = new(TextRule.RetrievedSuccessfully);

    [Category("2. Do Work - Text Rules")]
    public InArgument<TextRule> AdditionalRule { get; set; } = new(TextRule.None);

    [Category("2. Do Work - Text Rules")]
    public InArgument<string?> ExpectedText { get; set; } = new();

    [Category("2. Do Work - Text Rules")]
    public InArgument<string?> RegexPattern { get; set; } = new();

    [Category("2. Do Work - Text Rules")]
    public InArgument<bool> WaitForRules { get; set; } = new(true);

    [Category("8. Output")]
    public OutArgument<string?> Text { get; set; } = null!;

    protected override ActivityConfiguration CreateConfiguration(CodeActivityContext context)
    {
        var rules = new[] { Rule.Get(context), AdditionalRule.Get(context) }.Where(r => r != TextRule.None).ToArray();
        return Common(context, new GetTextConfiguration
        {
            ActivityType = "Validated Get Text", ActivityName = DisplayName, WorkSelector = string.Empty,
            Rules = rules, ExpectedText = ExpectedText.Get(context), RegexPattern = RegexPattern.Get(context), WaitForRules = WaitForRules.Get(context)
        });
    }

    protected override ValidationExecutionResult ExecuteCore(ValidationEngine engine, ActivityConfiguration configuration) => engine.Execute((GetTextConfiguration)configuration);
    protected override void SetActivityOutputs(CodeActivityContext context, ValidationExecutionResult execution) => Text.Set(context, execution.RawText);
}
