using UI.Validation.Library.Tests.TestDoubles;
using Vodafone.Robotics.UiValidation.Automation;
using Vodafone.Robotics.UiValidation.Engine;
using Vodafone.Robotics.UiValidation.Models;

namespace UI.Validation.Library.Tests.Mandatory;

public sealed class ClickMandatoryTests
{
    private static ClickConfiguration Config() => new()
    {
        ActivityType = "Validated Click", ActivityName = "mandatory-click", WorkSelector = "work",
        RetryCount = 0, RetryIntervalMilliseconds = 0, ScreenshotOnFinalFailure = false
    };

    [Fact]
    public void CL_001_Missing_target_fails_before_click()
    {
        var result = new ValidationEngine(new FakeAdapter()).Execute(Config() with { WorkSelector = "" }).Result;
        Assert.Equal(FailureCategory.InvalidConfiguration, result.FailureCategory);
    }

    [Fact]
    public void CL_003_Invalid_timing_fails_before_click()
    {
        var target = new FakeTarget();
        var result = new ValidationEngine(new FakeAdapter().Add("work", target)).Execute(Config() with { RetryCount = -1 }).Result;
        Assert.Equal(FailureCategory.InvalidConfiguration, result.FailureCategory);
        Assert.Equal(0, target.ClickCount);
    }

    [Fact]
    public void CL_010_Unique_enabled_target_clicks_once()
    {
        var target = new FakeTarget(); var result = new ValidationEngine(new FakeAdapter().Add("work", target)).Execute(Config()).Result;
        Assert.True(result.Success); Assert.Equal(1, target.ClickCount);
    }

    [Fact]
    public void CL_011_Target_resolution_retries_then_clicks_once()
    {
        var target = new FakeTarget();
        var adapter = new FakeAdapter { ResolveSequence = new Queue<IReadOnlyList<IUiTarget>>(new IReadOnlyList<IUiTarget>[] { Array.Empty<IUiTarget>(), new IUiTarget[] { target } }) };
        var result = new ValidationEngine(adapter, delay: (_, _) => { }).Execute(Config() with { RetryCount = 1 }).Result;
        Assert.True(result.Success); Assert.Equal(2, result.AttemptCount); Assert.Equal(1, target.ClickCount);
    }

    [Fact]
    public void CL_012_Multiple_matches_fail_without_click()
    {
        var a = new FakeTarget(); var b = new FakeTarget();
        var result = new ValidationEngine(new FakeAdapter().Add("work", a, b)).Execute(Config()).Result;
        Assert.Equal(FailureCategory.TargetAmbiguous, result.FailureCategory); Assert.Equal(0, a.ClickCount + b.ClickCount);
    }

    [Fact]
    public void CL_013_Identity_mismatch_prevents_click()
    {
        var target = new FakeTarget { Text = "Wrong" };
        var result = new ValidationEngine(new FakeAdapter().Add("work", target)).Execute(Config() with { Identity = new(ExpectedText: "Right") }).Result;
        Assert.Equal(FailureCategory.TargetIdentityMismatch, result.FailureCategory); Assert.Equal(0, target.ClickCount);
    }

    [Fact]
    public void CL_014_Disabled_target_prevents_click()
    {
        var target = new FakeTarget { IsEnabled = false };
        var result = new ValidationEngine(new FakeAdapter().Add("work", target)).Execute(Config()).Result;
        Assert.Equal(FailureCategory.TargetIdentityMismatch, result.FailureCategory); Assert.Equal(0, target.ClickCount);
    }

    [Fact]
    public void CL_015_Precondition_mismatch_prevents_click()
    {
        var work = new FakeTarget(); var adapter = new FakeAdapter().Add("work", work).Add("pre", new FakeTarget { Text = "Draft" });
        var result = new ValidationEngine(adapter).Execute(Config() with { PreCondition = new(ConditionKind.TextEquals, "pre", ExpectedValue: "Approved") }).Result;
        Assert.False(result.Success); Assert.Equal(0, work.ClickCount);
    }

    [Fact]
    public void CL_020_ElementAppears_succeeds()
    {
        var work = new FakeTarget(); var adapter = new FakeAdapter().Add("work", work); work.OnClick = _ => adapter.Add("post", new FakeTarget());
        Assert.True(new ValidationEngine(adapter).Execute(Config() with { PostCondition = new(PostConditionKind.ElementAppears, "post") }).Result.Success);
    }

    [Fact]
    public void CL_021_ElementDisappears_succeeds()
    {
        var work = new FakeTarget(); var adapter = new FakeAdapter().Add("work", work).Add("post", new FakeTarget()); work.OnClick = _ => adapter.Targets.Remove("post");
        Assert.True(new ValidationEngine(adapter).Execute(Config() with { PostCondition = new(PostConditionKind.ElementDisappears, "post") }).Result.Success);
    }

    [Theory]
    [InlineData("CL-022", PostConditionKind.TextEquals, "Done", "Done", true)]
    [InlineData("CL-022", PostConditionKind.TextEquals, "Pending", "Done", false)]
    [InlineData("CL-023", PostConditionKind.TextContains, "Order Done", "Done", true)]
    [InlineData("CL-023", PostConditionKind.TextContains, "Pending", "Done", false)]
    public void Text_postconditions_cover_success_and_failure(string id, PostConditionKind kind, string actual, string expected, bool succeeds)
    {
        _ = id; var work = new FakeTarget(); var post = new FakeTarget { Text = actual };
        var result = new ValidationEngine(new FakeAdapter().Add("work", work).Add("post", post)).Execute(Config() with { PostCondition = new(kind, "post", ExpectedValue: expected), SkipWhenPostConditionAlreadySatisfied = false }).Result;
        Assert.Equal(succeeds, result.Success);
    }

    [Fact]
    public void CL_024_AttributeEquals_covers_success_and_failure()
    {
        var work = new FakeTarget(); var post = new FakeTarget(); post.Attributes["status"] = "done";
        var engine = new ValidationEngine(new FakeAdapter().Add("work", work).Add("post", post));
        Assert.True(engine.Execute(Config() with { PostCondition = new(PostConditionKind.AttributeEquals, "post", "status", "done") }).Result.Success);
        Assert.False(engine.Execute(Config() with { PostCondition = new(PostConditionKind.AttributeEquals, "post", "status", "other"), SkipWhenPostConditionAlreadySatisfied = false }).Result.Success);
    }

    [Fact]
    public void CL_025_AttributeChanges_compares_before_and_after()
    {
        var work = new FakeTarget(); var post = new FakeTarget(); post.Attributes["status"] = "old"; work.OnClick = _ => post.Attributes["status"] = "new";
        Assert.True(new ValidationEngine(new FakeAdapter().Add("work", work).Add("post", post)).Execute(Config() with { PostCondition = new(PostConditionKind.AttributeChanges, "post", "status") }).Result.Success);
    }

    [Fact]
    public void CL_026_TargetStateChanges_compares_before_and_after()
    {
        var work = new FakeTarget(); var post = new FakeTarget { IsEnabled = true }; work.OnClick = _ => post.IsEnabled = false;
        Assert.True(new ValidationEngine(new FakeAdapter().Add("work", work).Add("post", post)).Execute(Config() with { PostCondition = new(PostConditionKind.TargetStateChanges, "post") }).Result.Success);
    }

    [Fact]
    public void CL_027_Window_or_page_appears_uses_existence_semantics()
    {
        var work = new FakeTarget(); var adapter = new FakeAdapter().Add("work", work); work.OnClick = _ => adapter.Add("window", new FakeTarget());
        Assert.True(new ValidationEngine(adapter).Execute(Config() with { PostCondition = new(PostConditionKind.WindowOrPageAppears, "window") }).Result.Success);
    }

    [Fact]
    public void CL_042_Action_retry_disabled_means_exactly_one_click()
    {
        var work = new FakeTarget(); var post = new FakeTarget { Text = "Pending" };
        var result = new ValidationEngine(new FakeAdapter().Add("work", work).Add("post", post), delay: (_, _) => { }).Execute(Config() with { RetryCount = 2, AllowActionRetry = false, PostCondition = new(PostConditionKind.TextEquals, "post", ExpectedValue: "Done") }).Result;
        Assert.False(result.Success); Assert.Equal(1, work.ClickCount);
    }

    [Fact]
    public void CL_044_Non_idempotent_counter_remains_one()
    {
        var count = 0; var work = new FakeTarget { OnClick = _ => count++ }; var post = new FakeTarget { TextSequence = new Queue<string?>(new[] { "Pending", "Pending", "Done" }) };
        var result = new ValidationEngine(new FakeAdapter().Add("work", work).Add("post", post), delay: (_, _) => { }).Execute(Config() with { RetryCount = 2, PostCondition = new(PostConditionKind.TextEquals, "post", ExpectedValue: "Done") }).Result;
        Assert.True(result.Success); Assert.Equal(1, count);
    }

    [Fact]
    public void CL_050_Click_exception_is_ActionFailed()
    {
        var target = new FakeTarget { ClickException = new InvalidOperationException("boom") };
        Assert.Equal(FailureCategory.ActionFailed, new ValidationEngine(new FakeAdapter().Add("work", target)).Execute(Config()).Result.FailureCategory);
    }

    [Fact]
    public void CL_051_Postcondition_timeout_retains_action_executed()
    {
        var result = new ValidationEngine(new FakeAdapter().Add("work", new FakeTarget()).Add("post", new FakeTarget { Text = "Pending" })).Execute(Config() with { PostCondition = new(PostConditionKind.TextEquals, "post", ExpectedValue: "Done") }).Result;
        Assert.Equal(FailureCategory.RetryExhausted, result.FailureCategory); Assert.True(result.ActionExecuted);
    }

    [Fact]
    public void CL_052_Retry_exhaustion_preserves_attempt_and_action_counts()
    {
        var work = new FakeTarget(); var result = new ValidationEngine(new FakeAdapter().Add("work", work).Add("post", new FakeTarget { Text = "Pending" }), delay: (_, _) => { }).Execute(Config() with { RetryCount = 2, PostCondition = new(PostConditionKind.TextEquals, "post", ExpectedValue: "Done") }).Result;
        Assert.Equal(3, result.AttemptCount); Assert.Equal(1, work.ClickCount); Assert.True(result.ActionExecuted);
    }

    [Fact]
    public void CL_053_Cancellation_stops_postcondition_polling_and_additional_clicks()
    {
        using var source = new CancellationTokenSource(); var work = new FakeTarget();
        var result = new ValidationEngine(new FakeAdapter().Add("work", work).Add("post", new FakeTarget { Text = "Pending" }), delay: (_, _) => source.Cancel()).Execute(Config() with { RetryCount = 2, PostCondition = new(PostConditionKind.TextEquals, "post", ExpectedValue: "Done") }, source.Token).Result;
        Assert.Equal(FailureCategory.Cancelled, result.FailureCategory); Assert.Equal(1, work.ClickCount);
    }

    [Fact]
    public void CL_060_Final_failure_records_screenshot()
    {
        var result = new ValidationEngine(new FakeAdapter().Add("work", new FakeTarget()).Add("post", new FakeTarget { Text = "Pending" })).Execute(Config() with { ScreenshotOnFinalFailure = true, PostCondition = new(PostConditionKind.TextEquals, "post", ExpectedValue: "Done") }).Result;
        Assert.EndsWith(".png", result.ScreenshotPath);
    }

    [Fact]
    public void CL_061_Screenshot_failure_preserves_postcondition_failure()
    {
        var adapter = new FakeAdapter { ThrowOnScreenshot = true }.Add("work", new FakeTarget()).Add("post", new FakeTarget { Text = "Pending" });
        var result = new ValidationEngine(adapter).Execute(Config() with { ScreenshotOnFinalFailure = true, PostCondition = new(PostConditionKind.TextEquals, "post", ExpectedValue: "Done") }).Result;
        Assert.Equal(FailureCategory.RetryExhausted, result.FailureCategory); Assert.Null(result.ScreenshotPath);
    }

    [Fact]
    public void CL_062_Selector_is_redacted_by_default()
    {
        var logger = new RecordingLogger(); const string selector = "<ctrl name='secret-account-062' />";
        new ValidationEngine(new FakeAdapter(), logger).Execute(Config() with { WorkSelector = selector });
        Assert.DoesNotContain("secret-account-062", string.Join(" ", logger.Events.SelectMany(e => e.Context.Values).Concat(logger.Events.Select(e => e.Message))));
    }
}
