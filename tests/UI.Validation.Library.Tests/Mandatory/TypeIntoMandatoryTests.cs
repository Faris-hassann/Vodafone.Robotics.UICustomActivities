using UI.Validation.Library.Tests.TestDoubles;
using Vodafone.Robotics.UiValidation.Automation;
using Vodafone.Robotics.UiValidation.Engine;
using Vodafone.Robotics.UiValidation.Models;
using Vodafone.Robotics.UiValidation.Validation;

namespace UI.Validation.Library.Tests.Mandatory;

public sealed class TypeIntoMandatoryTests
{
    private static TypeIntoConfiguration Config() => new()
    {
        ActivityType = "Validated Type Into", ActivityName = "mandatory-type", WorkSelector = "work", InputText = "New",
        RetryCount = 0, RetryIntervalMilliseconds = 0, ScreenshotOnFinalFailure = false
    };

    [Fact]
    public void TI_001_Invalid_mode_fails_before_typing()
    {
        var target = new FakeTarget();
        var result = new ValidationEngine(new FakeAdapter().Add("work", target)).Execute(Config() with { Mode = (TypeIntoMode)999 }).Result;
        Assert.Equal(FailureCategory.InvalidConfiguration, result.FailureCategory);
        Assert.Equal(0, target.SetTextCount);
    }

    [Fact]
    public void TI_003_Invalid_timing_fails_before_typing()
    {
        var target = new FakeTarget();
        var result = new ValidationEngine(new FakeAdapter().Add("work", target)).Execute(Config() with { TimeoutMilliseconds = 0 }).Result;
        Assert.Equal(FailureCategory.InvalidConfiguration, result.FailureCategory);
        Assert.Equal(0, target.SetTextCount);
    }

    [Fact]
    public void TI_010_Unique_enabled_input_succeeds()
    {
        var target = new FakeTarget();
        Assert.True(new ValidationEngine(new FakeAdapter().Add("work", target)).Execute(Config()).Result.Success);
        Assert.Equal(1, target.SetTextCount);
    }

    [Fact]
    public void TI_011_Target_not_found_retries_then_succeeds()
    {
        var target = new FakeTarget();
        var adapter = new FakeAdapter { ResolveSequence = new Queue<IReadOnlyList<IUiTarget>>(new IReadOnlyList<IUiTarget>[] { Array.Empty<IUiTarget>(), new IUiTarget[] { target } }) };
        var result = new ValidationEngine(adapter, delay: (_, _) => { }).Execute(Config() with { RetryCount = 1 }).Result;
        Assert.True(result.Success);
        Assert.Equal(2, result.AttemptCount);
        Assert.Equal(1, target.SetTextCount);
    }

    [Fact]
    public void TI_012_Multiple_matches_fail_without_mutation()
    {
        var first = new FakeTarget(); var second = new FakeTarget();
        var result = new ValidationEngine(new FakeAdapter().Add("work", first, second)).Execute(Config()).Result;
        Assert.Equal(FailureCategory.TargetAmbiguous, result.FailureCategory);
        Assert.Equal(0, first.SetTextCount + second.SetTextCount);
    }

    [Fact]
    public void TI_014_Disabled_field_never_receives_input()
    {
        var target = new FakeTarget { IsEnabled = false };
        var result = new ValidationEngine(new FakeAdapter().Add("work", target)).Execute(Config()).Result;
        Assert.Equal(FailureCategory.TargetIdentityMismatch, result.FailureCategory);
        Assert.Equal(0, target.SetTextCount);
    }

    [Theory]
    [InlineData("TI-021", "Old", "New", "New")]
    [InlineData("TI-022", "Old", "", "")]
    [InlineData("TI-023", "Old", "  New  ", "  New  ")]
    [InlineData("TI-024", "Old", "مرحبا 🌍", "مرحبا 🌍")]
    public void Replace_preserves_exact_input(string id, string original, string input, string expected)
    {
        _ = id;
        var target = new FakeTarget { Text = original };
        var result = new ValidationEngine(new FakeAdapter().Add("work", target)).Execute(Config() with { InputText = input }).Result;
        Assert.True(result.Success);
        Assert.Equal(expected, target.Text);
    }

    [Fact]
    public void TI_025_Long_allowed_input_verifies_exactly()
    {
        var input = new string('x', 20_000); var target = new FakeTarget();
        Assert.True(new ValidationEngine(new FakeAdapter().Add("work", target)).Execute(Config() with { InputText = input }).Result.Success);
        Assert.Equal(input, target.Text);
    }

    [Fact]
    public void TI_031_Append_separator_is_applied_once()
    {
        var target = new FakeTarget { Text = "Hello" };
        Assert.True(new ValidationEngine(new FakeAdapter().Add("work", target)).Execute(Config() with { Mode = TypeIntoMode.Append, InputText = "World", AppendSeparator = " " }).Result.Success);
        Assert.Equal("Hello World", target.Text);
    }

    [Fact]
    public void TI_030_Append_without_separator_produces_original_plus_input()
    {
        var target = new FakeTarget { Text = "Hello" };
        var result = new ValidationEngine(new FakeAdapter().Add("work", target)).Execute(Config() with { Mode = TypeIntoMode.Append, InputText = "World" }).Result;
        Assert.True(result.Success);
        Assert.Equal("HelloWorld", target.Text);
    }

    [Fact]
    public void TI_032_Empty_append_is_a_no_op_when_allowed()
    {
        var target = new FakeTarget { Text = "Hello" };
        var result = new ValidationEngine(new FakeAdapter().Add("work", target)).Execute(Config() with { Mode = TypeIntoMode.Append, InputText = "" }).Result;
        Assert.True(result.Success);
        Assert.Equal(0, target.SetTextCount);
        Assert.Equal("Hello", target.Text);
    }

    [Fact]
    public void TI_033_Existing_value_mismatch_prevents_append()
    {
        var target = new FakeTarget { Text = "Hello" };
        var result = new ValidationEngine(new FakeAdapter().Add("work", target)).Execute(Config() with { Mode = TypeIntoMode.Append, ExpectedExistingValue = "Other" }).Result;
        Assert.False(result.Success);
        Assert.Equal(0, target.SetTextCount);
    }

    [Fact]
    public void TI_040_ClearOnly_clears_and_verifies_empty()
    {
        var target = new FakeTarget { Text = "Populated" };
        var result = new ValidationEngine(new FakeAdapter().Add("work", target)).Execute(Config() with { Mode = TypeIntoMode.ClearOnly }).Result;
        Assert.True(result.Success);
        Assert.Equal(string.Empty, target.Text);
    }

    [Fact]
    public void TI_050_Unexpected_uppercase_transformation_fails_strictly()
    {
        var target = new FakeTarget(); target.OnSetText = (t, value) => t.Text = value.ToUpperInvariant();
        var result = new ValidationEngine(new FakeAdapter().Add("work", target)).Execute(Config() with { InputText = "abc" }).Result;
        Assert.Equal(FailureCategory.RetryExhausted, result.FailureCategory);
        Assert.Equal(MismatchKind.UnexpectedTransformation, result.MismatchKind);
    }

    [Fact]
    public void TI_052_Explicit_expected_formatted_value_succeeds()
    {
        var target = new FakeTarget(); target.OnSetText = (t, _) => t.Text = "1,234.00";
        Assert.True(new ValidationEngine(new FakeAdapter().Add("work", target)).Execute(Config() with { InputText = "1234", ExpectedFinalValue = "1,234.00" }).Result.Success);
    }

    [Fact]
    public void TI_060_Auto_records_the_strategy_actually_used()
    {
        var result = new ValidationEngine(new FakeAdapter().Add("work", new FakeTarget())).Execute(Config()).Result;
        Assert.True(result.Success);
        Assert.Equal("Text", result.VerificationMode);
    }

    [Fact]
    public void TI_061_ValueAttribute_ignores_unrelated_visible_text()
    {
        var target = new FakeTarget(); target.OnSetText = (t, _) => t.Text = "display-mask";
        var result = new ValidationEngine(new FakeAdapter().Add("work", target)).Execute(Config() with { VerificationMode = VerificationMode.ValueAttribute }).Result;
        Assert.True(result.Success);
        Assert.Equal("ValueAttribute", result.VerificationMode);
    }

    [Fact]
    public void TI_062_SpecificAttribute_verifies_configured_attribute()
    {
        var target = new FakeTarget(); target.OnSetText = (t, value) => t.Attributes["data-value"] = value;
        var result = new ValidationEngine(new FakeAdapter().Add("work", target)).Execute(Config() with { VerificationMode = VerificationMode.SpecificAttribute, VerificationAttribute = "data-value" }).Result;
        Assert.True(result.Success);
        Assert.Equal("SpecificAttribute:data-value", result.VerificationMode);
    }

    [Fact]
    public void TI_063_Readback_polling_does_not_repeat_typing()
    {
        var reads = 0;
        var target = new FakeTarget { OnGetText = t => ++reads switch { 1 => "", 2 => null, _ => t.Text } };
        var result = new ValidationEngine(new FakeAdapter().Add("work", target), delay: (_, _) => { }).Execute(Config() with { VerificationMode = VerificationMode.Text, RetryCount = 1 }).Result;
        Assert.True(result.Success);
        Assert.Equal(1, target.SetTextCount);
    }

    [Theory]
    [InlineData("TI-071", "", "abc", "xabc", MismatchKind.UnexpectedPrefix)]
    [InlineData("TI-072", "", "abc", "abcx", MismatchKind.UnexpectedSuffix)]
    [InlineData("TI-073", "", "abc", "abcabc", MismatchKind.DuplicateInput)]
    [InlineData("TI-074", "old", "new", "old", MismatchKind.NoChange)]
    [InlineData("TI-075", "", "abc", "ABC", MismatchKind.UnexpectedTransformation)]
    public void Mismatch_categories_are_deterministic(string id, string original, string expected, string actual, MismatchKind kind)
    {
        _ = id;
        Assert.Equal(kind, TextValidation.ClassifyMismatch(original, expected, actual));
    }

    [Fact]
    public void TI_082_Sensitive_normal_input_is_masked_by_default()
    {
        var logger = new RecordingLogger(); const string secret = "normal-secret-082";
        new ValidationEngine(new FakeAdapter().Add("work", new FakeTarget()), logger).Execute(Config() with { InputText = secret });
        Assert.DoesNotContain(secret, string.Join(" ", logger.Events.SelectMany(e => e.Context.Values).Concat(logger.Events.Select(e => e.Message))));
    }

    [Fact]
    public void TI_090_Transient_disabled_target_can_become_ready()
    {
        var target = new FakeTarget { IsEnabled = false };
        var result = new ValidationEngine(new FakeAdapter().Add("work", target), delay: (_, _) => target.IsEnabled = true).Execute(Config() with { RetryCount = 1 }).Result;
        Assert.True(result.Success);
        Assert.Equal(1, target.SetTextCount);
    }

    [Fact]
    public void TI_091_Target_retry_exhaustion_reports_attempts()
    {
        var result = new ValidationEngine(new FakeAdapter(), delay: (_, _) => { }).Execute(Config() with { RetryCount = 2 }).Result;
        Assert.Equal(FailureCategory.RetryExhausted, result.FailureCategory);
        Assert.Equal(3, result.AttemptCount);
    }

    [Fact]
    public void TI_092_Cancellation_prevents_typing_after_retry_delay()
    {
        using var source = new CancellationTokenSource(); var target = new FakeTarget { IsEnabled = false };
        var result = new ValidationEngine(new FakeAdapter().Add("work", target), delay: (_, _) => source.Cancel()).Execute(Config() with { RetryCount = 2 }, source.Token).Result;
        Assert.Equal(FailureCategory.Cancelled, result.FailureCategory);
        Assert.Equal(0, target.SetTextCount);
    }

    [Fact]
    public void TI_100_Final_mismatch_records_screenshot()
    {
        var target = new FakeTarget(); target.OnSetText = (t, _) => t.Text = "wrong";
        var result = new ValidationEngine(new FakeAdapter().Add("work", target)).Execute(Config() with { ScreenshotOnFinalFailure = true }).Result;
        Assert.Equal(FailureCategory.RetryExhausted, result.FailureCategory);
        Assert.EndsWith(".png", result.ScreenshotPath);
    }

    [Fact]
    public void TI_101_Screenshot_failure_preserves_action_failure()
    {
        var target = new FakeTarget { SetTextException = new InvalidOperationException("cannot type") };
        var adapter = new FakeAdapter { ThrowOnScreenshot = true }.Add("work", target);
        var result = new ValidationEngine(adapter).Execute(Config() with { ScreenshotOnFinalFailure = true }).Result;
        Assert.Equal(FailureCategory.ActionFailed, result.FailureCategory);
        Assert.Null(result.ScreenshotPath);
    }
}
