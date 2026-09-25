using System.Activities;
using System.ComponentModel;
using Vodafone.Robotics.UiValidation.Engine;
using Vodafone.Robotics.UiValidation.Models;

namespace Vodafone.Robotics.UiValidation.Activities;

[DisplayName("Validated Click")]
[Description("Validates the selected target before clicking and safely polls a separate postcondition without duplicate clicks by default.")]
[Category("UI Validation")]
public sealed class ValidatedClick : ValidatedActivityBase
{
    public ValidatedClick() => DisplayName = "Validated Click";

    [Category("7. Behavior")]
    [Description("Allows a repeated click only after the configured postcondition remains false. Keep disabled for non-idempotent actions.")]
    public InArgument<bool> AllowActionRetry { get; set; } = new(false);

    [Category("7. Behavior")]
    public InArgument<bool> SkipWhenPostConditionAlreadySatisfied { get; set; } = new(true);

    protected override ActivityConfiguration CreateConfiguration(CodeActivityContext context) => Common(context, new ClickConfiguration
    {
        ActivityType = "Validated Click", ActivityName = DisplayName, WorkSelector = string.Empty,
        AllowActionRetry = AllowActionRetry.Get(context), SkipWhenPostConditionAlreadySatisfied = SkipWhenPostConditionAlreadySatisfied.Get(context)
    });

    protected override ValidationExecutionResult ExecuteCore(ValidationEngine engine, ActivityConfiguration configuration) => engine.Execute((ClickConfiguration)configuration);
}
