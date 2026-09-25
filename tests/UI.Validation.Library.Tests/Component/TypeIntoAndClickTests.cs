using UI.Validation.Library.Tests.TestDoubles;
using Vodafone.Robotics.UiValidation.Engine;
using Vodafone.Robotics.UiValidation.Models;

namespace UI.Validation.Library.Tests.Component;

public sealed class TypeIntoAndClickTests
{
    private static TypeIntoConfiguration TypeConfig() => new() { ActivityType = "Validated Type Into", ActivityName = "Type", WorkSelector = "work", RetryCount = 0, ScreenshotOnFinalFailure = false, InputText = "World", IsSensitive = true };
    private static ClickConfiguration ClickConfig() => new() { ActivityType = "Validated Click", ActivityName = "Click", WorkSelector = "work", RetryCount = 2, RetryIntervalMilliseconds = 0, ScreenshotOnFinalFailure = false };

    [Theory]
    [InlineData(TypeIntoMode.Replace, "Hello", "World", "World")]
    [InlineData(TypeIntoMode.Append, "Hello", "World", "Hello World")]
    [InlineData(TypeIntoMode.ClearOnly, "Hello", "ignored", "")]
    public void TI_020_030_040_Modes_compute_and_verify_final_value(TypeIntoMode mode, string original, string input, string expected)
    {
        var target = new FakeTarget { Text = original };
        var config = TypeConfig() with { Mode = mode, InputText = input, AppendSeparator = mode == TypeIntoMode.Append ? " " : string.Empty };
        var result = new ValidationEngine(new FakeAdapter().Add("work", target)).Execute(config).Result;
        Assert.True(result.Success);
        Assert.Equal(expected, target.Text);
    }

    [Fact]
    public void TI_013_Existing_value_mismatch_prevents_typing()
    {
        var target = new FakeTarget { Text = "Account:" };
        var result = new ValidationEngine(new FakeAdapter().Add("work", target)).Execute(TypeConfig() with { ExpectedExistingValue = "Customer:" }).Result;
        Assert.False(result.Success);
        Assert.Equal(0, target.SetTextCount);
    }

    [Fact]
    public void TI_051_Expected_final_override_accepts_application_transformation()
    {
        var target = new FakeTarget();
        target.OnSetText = (t, _) => t.Text = "ABC";
        var result = new ValidationEngine(new FakeAdapter().Add("work", target)).Execute(TypeConfig() with { InputText = "abc", ExpectedFinalValue = "ABC" }).Result;
        Assert.True(result.Success);
    }

    [Fact]
    public void TI_080_Sensitive_input_is_redacted_in_logs()
    {
        var logger = new RecordingLogger();
        var result = new ValidationEngine(new FakeAdapter().Add("work", new FakeTarget()), logger).Execute(TypeConfig() with { InputText = "secret-value" }).Result;
        Assert.True(result.Success);
        Assert.DoesNotContain("secret-value", string.Join(" ", logger.Events.SelectMany(e => e.Context.Values).Concat(logger.Events.Select(e => e.Message))));
    }

    [Fact]
    public void TI_081_Secure_auto_mode_is_truthfully_action_only()
    {
        var result = new ValidationEngine(new FakeAdapter().Add("work", new FakeTarget())).Execute(TypeConfig() with { IsSecure = true, VerificationMode = VerificationMode.Auto }).Result;
        Assert.True(result.Success);
        Assert.False(result.VerificationAvailable);
        Assert.Equal("ActionOnly", result.VerificationMode);
    }

    [Fact]
    public void CL_030_No_postcondition_warns_but_succeeds()
    {
        var target = new FakeTarget();
        var logger = new RecordingLogger();
        var result = new ValidationEngine(new FakeAdapter().Add("work", target), logger).Execute(ClickConfig()).Result;
        Assert.True(result.Success);
        Assert.Equal(1, target.ClickCount);
        Assert.False(result.OutcomeIndependentlyVerified);
        Assert.Contains(logger.Events, e => e.EventName == "OutcomeNotIndependentlyVerified");
    }

    [Fact]
    public void CL_040_042_Delayed_postcondition_does_not_duplicate_click_by_default()
    {
        var work = new FakeTarget();
        var post = new FakeTarget { TextSequence = new Queue<string?>(new[] { "Pending", "Pending", "Done" }) };
        var adapter = new FakeAdapter().Add("work", work).Add("post", post);
        var config = ClickConfig() with { PostCondition = new(PostConditionKind.TextEquals, "post", ExpectedValue: "Done"), AllowActionRetry = false };
        var result = new ValidationEngine(adapter, delay: (_, _) => { }).Execute(config).Result;
        Assert.True(result.Success);
        Assert.Equal(1, work.ClickCount);
    }

    [Fact]
    public void CL_041_Already_satisfied_postcondition_skips_click()
    {
        var work = new FakeTarget();
        var adapter = new FakeAdapter().Add("work", work).Add("post", new FakeTarget { Text = "Done" });
        var result = new ValidationEngine(adapter).Execute(ClickConfig() with { PostCondition = new(PostConditionKind.TextEquals, "post", ExpectedValue: "Done") }).Result;
        Assert.True(result.Success);
        Assert.Equal(0, work.ClickCount);
    }

    [Fact]
    public void CL_043_Explicit_action_retry_can_click_again()
    {
        var work = new FakeTarget();
        var post = new FakeTarget { Text = "Pending" };
        work.OnClick = _ => { if (work.ClickCount == 2) post.Text = "Done"; };
        var adapter = new FakeAdapter().Add("work", work).Add("post", post);
        var result = new ValidationEngine(adapter, delay: (_, _) => { }).Execute(ClickConfig() with { PostCondition = new(PostConditionKind.TextEquals, "post", ExpectedValue: "Done"), AllowActionRetry = true }).Result;
        Assert.True(result.Success);
        Assert.Equal(2, work.ClickCount);
    }

    [Fact]
    public void CL_002_Invalid_postcondition_never_clicks()
    {
        var work = new FakeTarget();
        var result = new ValidationEngine(new FakeAdapter().Add("work", work)).Execute(ClickConfig() with { PostCondition = new(PostConditionKind.TextEquals, null, ExpectedValue: "Done") }).Result;
        Assert.Equal(FailureCategory.InvalidConfiguration, result.FailureCategory);
        Assert.Equal(0, work.ClickCount);
    }
}
