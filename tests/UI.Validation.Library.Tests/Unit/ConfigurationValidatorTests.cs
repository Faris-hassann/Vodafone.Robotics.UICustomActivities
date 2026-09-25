using Vodafone.Robotics.UiValidation.Exceptions;
using Vodafone.Robotics.UiValidation.Models;
using Vodafone.Robotics.UiValidation.Validation;

namespace UI.Validation.Library.Tests.Unit;

public sealed class ConfigurationValidatorTests
{
    private static GetTextConfiguration ValidGetText() => new() { ActivityType = "Get", ActivityName = "Get", WorkSelector = "<wnd/><ctrl/>" };

    [Fact] public void GT_001_Invalid_regex_fails_before_resolution() => Assert.Throws<UIConfigurationException>(() => ConfigurationValidator.Validate(ValidGetText() with { Rules = new[] { TextRule.Regex }, RegexPattern = "[" }));
    [Fact] public void GT_002_Negative_retry_fails() => Assert.Throws<UIConfigurationException>(() => ConfigurationValidator.Validate(ValidGetText() with { RetryCount = -1 }));
    [Fact] public void GT_003_Missing_target_fails() => Assert.Throws<UIConfigurationException>(() => ConfigurationValidator.Validate(ValidGetText() with { WorkSelector = "" }));
    [Fact] public void CL_002_Missing_postcondition_selector_fails() => Assert.Throws<UIConfigurationException>(() => ConfigurationValidator.Validate(ValidGetText() with { PostCondition = new(PostConditionKind.TextEquals, ExpectedValue: "Done") }));
    [Fact] public void TI_002_Missing_specific_attribute_fails() => Assert.Throws<UIConfigurationException>(() => ConfigurationValidator.Validate(new TypeIntoConfiguration { ActivityType = "Type", ActivityName = "Type", WorkSelector = "x", VerificationMode = VerificationMode.SpecificAttribute }));
    [Fact] public void TI_004_Secure_exact_readback_fails() => Assert.Throws<UIConfigurationException>(() => ConfigurationValidator.Validate(new TypeIntoConfiguration { ActivityType = "Type", ActivityName = "Type", WorkSelector = "x", IsSecure = true, VerificationMode = VerificationMode.Text }));
    [Fact] public void Common_valid_configuration_passes() => ConfigurationValidator.Validate(ValidGetText());
}
