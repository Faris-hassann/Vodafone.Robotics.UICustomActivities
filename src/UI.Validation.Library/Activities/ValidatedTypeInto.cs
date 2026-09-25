using System.Activities;
using System.ComponentModel;
using Vodafone.Robotics.UiValidation.Engine;
using Vodafone.Robotics.UiValidation.Models;

namespace Vodafone.Robotics.UiValidation.Activities;

[DisplayName("Validated Type Into")]
[Description("Writes text safely with Replace, Append, or ClearOnly semantics and independently verifies the final value and optional postcondition.")]
[Category("UI Validation")]
public sealed class ValidatedTypeInto : ValidatedActivityBase
{
    public ValidatedTypeInto() => DisplayName = "Validated Type Into";

    [Category("2. Do Work - Type Into")]
    public InArgument<TypeIntoMode> Mode { get; set; } = new(TypeIntoMode.Replace);

    [Category("2. Do Work - Type Into")]
    public InArgument<string?> InputText { get; set; } = new(string.Empty);

    [Category("2. Do Work - Type Into")]
    public InArgument<string?> AppendSeparator { get; set; } = new(string.Empty);

    [Category("1. PreCondition")]
    public InArgument<string?> ExpectedExistingValue { get; set; } = new();

    [Category("3. PostCondition")]
    public InArgument<string?> ExpectedFinalValue { get; set; } = new();

    [Category("3. PostCondition")]
    public InArgument<VerificationMode> VerificationMode { get; set; } = new(Models.VerificationMode.Auto);

    [Category("3. PostCondition")]
    public InArgument<string?> VerificationAttribute { get; set; } = new();

    [Category("6. Logging and Diagnostics")]
    public InArgument<bool> IsSensitive { get; set; } = new(true);

    [Category("7. Behavior")]
    public InArgument<bool> IsSecure { get; set; } = new(false);

    [Category("7. Behavior")]
    public InArgument<bool> DisallowEmptyInput { get; set; } = new(false);

    protected override ActivityConfiguration CreateConfiguration(CodeActivityContext context) => Common(context, new TypeIntoConfiguration
    {
        ActivityType = "Validated Type Into", ActivityName = DisplayName, WorkSelector = string.Empty,
        Mode = Mode.Get(context), InputText = InputText.Get(context) ?? string.Empty, AppendSeparator = AppendSeparator.Get(context) ?? string.Empty,
        ExpectedExistingValue = ExpectedExistingValue.Get(context), ExpectedFinalValue = ExpectedFinalValue.Get(context),
        VerificationMode = VerificationMode.Get(context), VerificationAttribute = VerificationAttribute.Get(context),
        IsSensitive = IsSensitive.Get(context), IsSecure = IsSecure.Get(context), DisallowEmptyInput = DisallowEmptyInput.Get(context)
    });

    protected override ValidationExecutionResult ExecuteCore(ValidationEngine engine, ActivityConfiguration configuration) => engine.Execute((TypeIntoConfiguration)configuration);
}
