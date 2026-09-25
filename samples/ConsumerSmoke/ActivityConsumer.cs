using Vodafone.Robotics.UiValidation.Activities;
using Vodafone.Robotics.UiValidation.Models;

namespace ConsumerSmoke;

public static class ActivityConsumer
{
    public static Type[] DiscoverableActivities => new[]
    {
        typeof(ValidatedGetText),
        typeof(ValidatedTypeInto),
        typeof(ValidatedClick)
    };

    public static bool SafeDefaultsAreAvailable() =>
        new ValidatedClick().AllowActionRetry.Expression is not null &&
        new ValidatedTypeInto().Mode.Expression is not null &&
        Enum.IsDefined(typeof(PostConditionKind), PostConditionKind.ElementAppears);
}
