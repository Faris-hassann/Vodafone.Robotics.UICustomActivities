using UI.Validation.Library.Tests.TestDoubles;
using Vodafone.Robotics.UiValidation.Engine;
using Vodafone.Robotics.UiValidation.Models;

namespace UI.Validation.Library.Tests.Component;

public sealed class ValidationEngineTests
{
    private static GetTextConfiguration GetConfig() => new() { ActivityType = "Validated Get Text", ActivityName = "Get", WorkSelector = "work", RetryCount = 0, ScreenshotOnFinalFailure = false };

    [Fact]
    public void GT_010_and_060_Unique_target_succeeds_with_structured_result()
    {
        var logger = new RecordingLogger();
        var engine = new ValidationEngine(new FakeAdapter().Add("work", new FakeTarget { Text = "Ready" }), logger);
        var execution = engine.Execute(GetConfig());
        Assert.True(execution.Result.Success);
        Assert.Equal("Ready", execution.RawText);
        Assert.True(execution.Result.TargetIdentityValidated);
        Assert.Equal(ValidationStage.Completed, execution.Result.ValidationStage);
        Assert.Contains(logger.Events, e => e.EventName == "ActivitySucceeded");
    }

    [Fact]
    public void GT_012_Multiple_targets_fail_by_default()
    {
        var engine = new ValidationEngine(new FakeAdapter().Add("work", new FakeTarget(), new FakeTarget()));
        Assert.Equal(FailureCategory.TargetAmbiguous, engine.Execute(GetConfig()).Result.FailureCategory);
    }

    [Fact]
    public void GT_013_Multiple_targets_can_be_explicitly_allowed()
    {
        var engine = new ValidationEngine(new FakeAdapter().Add("work", new FakeTarget { Text = "first" }, new FakeTarget { Text = "second" }));
        Assert.True(engine.Execute(GetConfig() with { AllowMultipleMatches = true }).Result.Success);
    }

    [Fact]
    public void GT_014_Identity_mismatch_fails()
    {
        var engine = new ValidationEngine(new FakeAdapter().Add("work", new FakeTarget { Text = "Wrong" }));
        var result = engine.Execute(GetConfig() with { Identity = new(ExpectedText: "Right") }).Result;
        Assert.Equal(FailureCategory.TargetIdentityMismatch, result.FailureCategory);
        Assert.False(result.ActionExecuted);
    }

    [Fact]
    public void GT_050_Target_appears_on_retry()
    {
        var target = new FakeTarget { Text = "Loaded" };
        var adapter = new FakeAdapter { ResolveSequence = new Queue<IReadOnlyList<Vodafone.Robotics.UiValidation.Automation.IUiTarget>>(new[] { Array.Empty<Vodafone.Robotics.UiValidation.Automation.IUiTarget>(), new[] { target } }) };
        var result = new ValidationEngine(adapter, delay: (_, _) => { }).Execute(GetConfig() with { RetryCount = 1 }).Result;
        Assert.True(result.Success);
        Assert.Equal(2, result.AttemptCount);
    }

    [Fact]
    public void GT_051_Blank_text_becomes_expected_without_mutation()
    {
        var target = new FakeTarget { TextSequence = new Queue<string?>(new[] { "", "Loaded" }) };
        var result = new ValidationEngine(new FakeAdapter().Add("work", target), delay: (_, _) => { }).Execute(GetConfig() with { RetryCount = 1, Rules = new[] { TextRule.Exact }, ExpectedText = "Loaded" }).Result;
        Assert.True(result.Success);
        Assert.Equal(0, target.SetTextCount);
    }

    [Fact]
    public void GT_053_Cancellation_stops_resolution()
    {
        using var source = new CancellationTokenSource();
        source.Cancel();
        var adapter = new FakeAdapter();
        var result = new ValidationEngine(adapter).Execute(GetConfig(), source.Token).Result;
        Assert.Equal(FailureCategory.Cancelled, result.FailureCategory);
        Assert.Equal(0, adapter.ResolveCount);
    }

    [Fact]
    public void PreCondition_failure_prevents_DoWork()
    {
        var work = new FakeTarget { Text = "Ready" };
        var adapter = new FakeAdapter().Add("work", work).Add("pre", new FakeTarget { Text = "Draft" });
        var result = new ValidationEngine(adapter).Execute(GetConfig() with { PreCondition = new(ConditionKind.TextEquals, "pre", ExpectedValue: "Approved") }).Result;
        Assert.False(result.Success);
        Assert.False(result.ActionExecuted);
    }

    [Fact]
    public void PostCondition_uses_its_own_complete_selector()
    {
        var adapter = new FakeAdapter().Add("work", new FakeTarget { Text = "Ready" }).Add("post", new FakeTarget { Text = "Done" });
        var result = new ValidationEngine(adapter).Execute(GetConfig() with { PostCondition = new(PostConditionKind.TextEquals, "post", ExpectedValue: "Done") }).Result;
        Assert.True(result.Success);
        Assert.True(result.PostConditionValidated);
    }

    [Fact]
    public void Diagnostic_failure_never_replaces_primary_failure()
    {
        var adapter = new FakeAdapter { ThrowOnScreenshot = true };
        var logger = new RecordingLogger();
        var result = new ValidationEngine(adapter, logger).Execute(GetConfig() with { ScreenshotOnFinalFailure = true }).Result;
        Assert.Equal(FailureCategory.TargetNotFound, result.FailureCategory);
        Assert.Contains(logger.Events, e => e.EventName == "DiagnosticFailure");
    }
}
