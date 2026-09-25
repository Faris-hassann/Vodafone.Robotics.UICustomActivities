using UI.Validation.Library.Tests.TestDoubles;
using Vodafone.Robotics.UiValidation.Engine;
using Vodafone.Robotics.UiValidation.Models;

namespace UI.Validation.Library.Tests.Mandatory;

public sealed class GetTextMandatoryTests
{
    private static GetTextConfiguration Config() => new()
    {
        ActivityType = "Validated Get Text", ActivityName = "mandatory-get", WorkSelector = "work",
        RetryCount = 0, RetryIntervalMilliseconds = 0, ScreenshotOnFinalFailure = false
    };

    [Fact]
    public void GT_011_Target_not_found_exhausts_retry_policy()
    {
        var result = new ValidationEngine(new FakeAdapter(), delay: (_, _) => { }).Execute(Config() with { RetryCount = 2 }).Result;
        Assert.Equal(FailureCategory.RetryExhausted, result.FailureCategory);
        Assert.Equal(3, result.AttemptCount);
    }

    [Fact]
    public void GT_021_Empty_is_successful_without_NotEmpty()
    {
        var execution = new ValidationEngine(new FakeAdapter().Add("work", new FakeTarget { Text = "" })).Execute(Config());
        Assert.True(execution.Result.Success);
        Assert.Equal(string.Empty, execution.RawText);
        Assert.Equal(RawValueState.Empty, execution.Result.RawValueState);
    }

    [Fact]
    public void GT_022_Empty_fails_NotEmpty()
    {
        var result = new ValidationEngine(new FakeAdapter().Add("work", new FakeTarget { Text = "" })).Execute(Config() with { Rules = new[] { TextRule.NotEmpty } }).Result;
        Assert.Equal(FailureCategory.TextValidationFailed, result.FailureCategory);
    }

    [Fact]
    public void GT_023_Whitespace_is_distinct_from_empty()
    {
        var result = new ValidationEngine(new FakeAdapter().Add("work", new FakeTarget { Text = "   " })).Execute(Config() with { Rules = new[] { TextRule.NotEmpty } }).Result;
        Assert.True(result.Success);
        Assert.Equal(RawValueState.WhitespaceOnly, result.RawValueState);
    }

    [Fact]
    public void GT_024_Nonempty_text_succeeds() => Assert.True(new ValidationEngine(new FakeAdapter().Add("work", new FakeTarget { Text = "Ready" })).Execute(Config()).Result.Success);

    [Theory]
    [InlineData("GT-031", TextRule.Exact, "Ready", "Other", false)]
    [InlineData("GT-032", TextRule.Contains, "Hello world", "world", true)]
    [InlineData("GT-032", TextRule.Contains, "Hello world", "mars", false)]
    [InlineData("GT-033", TextRule.StartsWith, "Hello world", "Hello", true)]
    [InlineData("GT-033", TextRule.StartsWith, "Hello world", "world", false)]
    [InlineData("GT-034", TextRule.EndsWith, "Hello world", "world", true)]
    [InlineData("GT-034", TextRule.EndsWith, "Hello world", "Hello", false)]
    public void Text_rule_success_and_failure_cases(string id, TextRule rule, string actual, string expected, bool succeeds)
    {
        _ = id;
        var result = new ValidationEngine(new FakeAdapter().Add("work", new FakeTarget { Text = actual })).Execute(Config() with { Rules = new[] { rule }, ExpectedText = expected }).Result;
        Assert.Equal(succeeds, result.Success);
    }

    [Theory]
    [InlineData("GT-035", "ABC-123", "^[A-Z]+-\\d+$", true)]
    [InlineData("GT-035", "abc", "^[A-Z]+$", false)]
    public void Regex_success_and_failure(string id, string actual, string pattern, bool succeeds)
    {
        _ = id;
        var result = new ValidationEngine(new FakeAdapter().Add("work", new FakeTarget { Text = actual })).Execute(Config() with { Rules = new[] { TextRule.Regex }, RegexPattern = pattern }).Result;
        Assert.Equal(succeeds, result.Success);
    }

    [Fact]
    public void GT_037_Case_sensitive_is_strict()
    {
        var result = new ValidationEngine(new FakeAdapter().Add("work", new FakeTarget { Text = "ready" })).Execute(Config() with { Rules = new[] { TextRule.Exact }, ExpectedText = "Ready" }).Result;
        Assert.False(result.Success);
    }

    [Fact]
    public void GT_038_Case_insensitive_mode_is_documented_and_supported()
    {
        var result = new ValidationEngine(new FakeAdapter().Add("work", new FakeTarget { Text = "ready" })).Execute(Config() with { Rules = new[] { TextRule.Exact }, ExpectedText = "Ready", Comparison = new(CaseSensitive: false) }).Result;
        Assert.True(result.Success);
    }

    [Fact]
    public void GT_040_Whitespace_normalization_only_changes_validation_view()
    {
        const string raw = "A\t  B";
        var execution = new ValidationEngine(new FakeAdapter().Add("work", new FakeTarget { Text = raw })).Execute(Config() with { Rules = new[] { TextRule.Exact }, ExpectedText = "A B", Comparison = new(NormalizeWhitespace: true) });
        Assert.True(execution.Result.Success);
        Assert.Equal(raw, execution.RawText);
    }

    [Fact]
    public void GT_052_Retry_exhaustion_reports_attempt_count()
    {
        var result = new ValidationEngine(new FakeAdapter().Add("work", new FakeTarget { Text = "Pending" }), delay: (_, _) => { }).Execute(Config() with { Rules = new[] { TextRule.Exact }, ExpectedText = "Done", RetryCount = 2 }).Result;
        Assert.Equal(FailureCategory.RetryExhausted, result.FailureCategory);
        Assert.Equal(3, result.AttemptCount);
    }

    [Fact]
    public void GT_060_Result_contains_execution_diagnostics()
    {
        var result = new ValidationEngine(new FakeAdapter().Add("work", new FakeTarget { Text = "Ready" })).Execute(Config() with { CorrelationId = "corr-060" }).Result;
        Assert.True(result.Success);
        Assert.Equal("Validated Get Text", result.ActivityType);
        Assert.Equal(ValidationStage.Completed, result.ValidationStage);
        Assert.Equal(1, result.AttemptCount);
        Assert.Equal("corr-060", result.CorrelationId);
        Assert.True(result.Duration >= TimeSpan.Zero);
    }

    [Fact]
    public void GT_061_Raw_text_is_not_logged_by_default()
    {
        var logger = new RecordingLogger();
        const string secret = "customer-secret-061";
        new ValidationEngine(new FakeAdapter().Add("work", new FakeTarget { Text = secret }), logger).Execute(Config());
        Assert.DoesNotContain(secret, string.Join(" ", logger.Events.SelectMany(e => e.Context.Values).Concat(logger.Events.Select(e => e.Message))));
    }

    [Fact]
    public void GT_062_Final_failure_records_screenshot_path()
    {
        var result = new ValidationEngine(new FakeAdapter()).Execute(Config() with { ScreenshotOnFinalFailure = true }).Result;
        Assert.Equal(FailureCategory.TargetNotFound, result.FailureCategory);
        Assert.EndsWith(".png", result.ScreenshotPath);
    }

    [Fact]
    public void GT_063_Screenshot_failure_preserves_primary_failure()
    {
        var adapter = new FakeAdapter { ThrowOnScreenshot = true };
        var result = new ValidationEngine(adapter).Execute(Config() with { ScreenshotOnFinalFailure = true }).Result;
        Assert.Equal(FailureCategory.TargetNotFound, result.FailureCategory);
        Assert.Null(result.ScreenshotPath);
    }
}
