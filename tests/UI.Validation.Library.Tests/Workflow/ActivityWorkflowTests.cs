using System.Activities;
using UI.Validation.Library.Tests.TestDoubles;
using Vodafone.Robotics.UiValidation.Activities;
using Vodafone.Robotics.UiValidation.Models;

namespace UI.Validation.Library.Tests.Workflow;

public sealed class ActivityWorkflowTests
{
    [Fact]
    public void ValidatedGetText_executes_through_WorkflowInvoker()
    {
        var target = new FakeTarget { Text = "Ready" };
        var outputs = Invoke(new FakeAdapter().Add("work", target), new ValidatedGetText
        {
            DoWorkSelector = "work",
            Rule = TextRule.Exact,
            ExpectedText = "Ready",
            RetryCount = 0,
            ScreenshotOnFinalFailure = false
        });
        Assert.Equal("Ready", outputs["Text"]);
        Assert.True(((UIValidationResult)outputs["ValidationResult"]).Success);
    }

    [Fact]
    public void ValidatedTypeInto_executes_through_WorkflowInvoker()
    {
        var target = new FakeTarget { Text = "Old" };
        var outputs = Invoke(new FakeAdapter().Add("work", target), new ValidatedTypeInto
        {
            DoWorkSelector = "work",
            InputText = "New",
            RetryCount = 0,
            ScreenshotOnFinalFailure = false
        });
        Assert.Equal("New", target.Text);
        Assert.True(((UIValidationResult)outputs["ValidationResult"]).Success);
    }

    [Fact]
    public void ValidatedClick_executes_through_WorkflowInvoker()
    {
        var target = new FakeTarget();
        var outputs = Invoke(new FakeAdapter().Add("work", target), new ValidatedClick
        {
            DoWorkSelector = "work",
            RetryCount = 0,
            ScreenshotOnFinalFailure = false
        });
        Assert.Equal(1, target.ClickCount);
        Assert.True(((UIValidationResult)outputs["ValidationResult"]).Success);
    }

    [Fact]
    public void ThrowOnFailure_false_returns_a_failed_structured_result()
    {
        var outputs = Invoke(new FakeAdapter(), new ValidatedGetText
        {
            DoWorkSelector = "missing",
            ThrowOnFailure = false,
            RetryCount = 0,
            ScreenshotOnFinalFailure = false
        });
        var result = (UIValidationResult)outputs["ValidationResult"];
        Assert.False(result.Success);
        Assert.Equal(FailureCategory.TargetNotFound, result.FailureCategory);
    }

    private static IDictionary<string, object> Invoke(FakeAdapter adapter, Activity activity)
    {
        var original = ValidatedActivityBase.ActivityServices.AdapterFactory;
        ValidatedActivityBase.ActivityServices.AdapterFactory = () => adapter;
        try { return new WorkflowInvoker(activity).Invoke(TimeSpan.FromSeconds(5)); }
        finally { ValidatedActivityBase.ActivityServices.AdapterFactory = original; }
    }
}
