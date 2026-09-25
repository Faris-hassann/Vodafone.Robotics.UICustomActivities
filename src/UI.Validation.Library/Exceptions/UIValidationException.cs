using Vodafone.Robotics.UiValidation.Models;

namespace Vodafone.Robotics.UiValidation.Exceptions;

public class UIValidationException : Exception
{
    public FailureCategory Category { get; }
    public ValidationStage Stage { get; }
    public UIValidationResult? ValidationResult { get; }

    public UIValidationException(string message, FailureCategory category, ValidationStage stage, Exception? inner = null, UIValidationResult? result = null) : base(message, inner)
    {
        Category = category;
        Stage = stage;
        ValidationResult = result;
    }
}

public sealed class UIConfigurationException : UIValidationException
{
    public UIConfigurationException(string message) : base(message, FailureCategory.InvalidConfiguration, ValidationStage.ConfigurationValidation) { }
}

public sealed class UITargetNotFoundException : UIValidationException
{
    public UITargetNotFoundException(string message, Exception? inner = null) : base(message, FailureCategory.TargetNotFound, ValidationStage.TargetResolution, inner) { }
}

public sealed class UITargetAmbiguousException : UIValidationException
{
    public UITargetAmbiguousException(string message) : base(message, FailureCategory.TargetAmbiguous, ValidationStage.TargetResolution) { }
}

public sealed class UITargetIdentityException : UIValidationException
{
    public UITargetIdentityException(string message) : base(message, FailureCategory.TargetIdentityMismatch, ValidationStage.TargetIdentityValidation) { }
}

public sealed class UIActionException : UIValidationException
{
    public UIActionException(string message, Exception? inner = null) : base(message, FailureCategory.ActionFailed, ValidationStage.DoWork, inner) { }
}

public sealed class UITextValidationException : UIValidationException
{
    public UITextValidationException(string message) : base(message, FailureCategory.TextValidationFailed, ValidationStage.PostCondition) { }
}

public sealed class UIPostConditionException : UIValidationException
{
    public UIPostConditionException(string message) : base(message, FailureCategory.PostConditionFailed, ValidationStage.PostCondition) { }
}
