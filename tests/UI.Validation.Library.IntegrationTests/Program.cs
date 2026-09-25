using System.Diagnostics;
using Vodafone.Robotics.UiValidation.Automation;
using Vodafone.Robotics.UiValidation.Engine;
using Vodafone.Robotics.UiValidation.Logging;
using Vodafone.Robotics.UiValidation.Models;

namespace UI.Validation.Library.IntegrationTests;

internal static class Program
{
    private const string App = "<wnd app='UI.Validation.TestHost.exe' title='UI Validation Deterministic Test Host' />";
    private static string Selector(string name, string role)
    {
        return role == "push button"
            ? $"{App}<ctrl name='{name}' role='push button' />"
            : $"{App}<ctrl automationid='{name}' role='{role}' />";
    }

    [STAThread]
    private static int Main(string[] args)
    {
        var hostPath = args.Length > 0
            ? Path.GetFullPath(args[0])
            : Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "testhost", "UI.Validation.TestHost", "bin", "Release", "net6.0-windows", "UI.Validation.TestHost.exe"));
        if (!File.Exists(hostPath))
        {
            Console.Error.WriteLine($"NOT EXECUTED: test host was not found at {hostPath}");
            return 2;
        }

        using var host = Process.Start(new ProcessStartInfo(hostPath) { UseShellExecute = true, WindowStyle = ProcessWindowStyle.Normal });
        if (host is null) { Console.Error.WriteLine("NOT EXECUTED: test host could not be started."); return 2; }
        try
        {
            Thread.Sleep(1_000);
            var adapter = new UiPathAutomationAdapter();
            var logger = new ConsoleLogger();
            var engine = new ValidationEngine(adapter, logger);
            const string searchCorrelation = "E2E-001-correlation";

            var searchType = engine.Execute(new TypeIntoConfiguration
            {
                ActivityType = "Validated Type Into", ActivityName = "E2E-001 Type Customer", WorkSelector = Selector("EmptyInput", "editable text"),
                InputText = "CUST-001", Mode = TypeIntoMode.Replace, RetryCount = 1, ScreenshotOnFinalFailure = false, CorrelationId = searchCorrelation
            }).Result;
            Check(searchType, "E2E-001/type");

            var searchClick = engine.Execute(new ClickConfiguration
            {
                ActivityType = "Validated Click", ActivityName = "E2E-001 Search", WorkSelector = Selector("SearchButton", "push button"),
                PostCondition = new(PostConditionKind.TextEquals, Selector("ResultLabel", "text"), ExpectedValue: "Customer:CUST-001"),
                RetryCount = 4, RetryIntervalMilliseconds = 150, AllowActionRetry = false, ScreenshotOnFinalFailure = false, CorrelationId = searchCorrelation
            }).Result;
            Check(searchClick, "E2E-001/click");

            var result = engine.Execute(new GetTextConfiguration
            {
                ActivityType = "Validated Get Text", ActivityName = "E2E-001 Verify Result", WorkSelector = Selector("ResultLabel", "text"),
                Rules = new[] { TextRule.Exact, TextRule.Contains }, ExpectedText = "Customer:CUST-001", RetryCount = 1, ScreenshotOnFinalFailure = false, CorrelationId = searchCorrelation
            });
            Check(result.Result, "E2E-001/get");
            Require(searchType.CorrelationId == searchCorrelation && searchClick.CorrelationId == searchCorrelation && result.Result.CorrelationId == searchCorrelation, "E2E-001 correlation was not propagated.");
            Require(!logger.Flattened.Contains("CUST-001", StringComparison.Ordinal), "E2E-001 sensitive input leaked into logs.");
            Console.WriteLine("E2E-001 PASS");

            Check(engine.Execute(new TypeIntoConfiguration
            {
                ActivityType = "Validated Type Into", ActivityName = "E2E-002 Append", WorkSelector = Selector("PrepopulatedInput", "editable text"),
                InputText = "World", Mode = TypeIntoMode.Append, AppendSeparator = " ", RetryCount = 1, ScreenshotOnFinalFailure = false
            }).Result, "E2E-002/append");
            Check(engine.Execute(new ClickConfiguration
            {
                ActivityType = "Validated Click", ActivityName = "E2E-002 Save", WorkSelector = Selector("SaveButton", "push button"),
                PostCondition = new(PostConditionKind.TextEquals, Selector("PersistedLabel", "text"), ExpectedValue: "Hello World"),
                RetryCount = 2, RetryIntervalMilliseconds = 100, ScreenshotOnFinalFailure = false
            }).Result, "E2E-002/save");
            var persisted = engine.Execute(new GetTextConfiguration
            {
                ActivityType = "Validated Get Text", ActivityName = "E2E-002 Persisted Value", WorkSelector = Selector("PersistedLabel", "text"),
                Rules = new[] { TextRule.Exact }, ExpectedText = "Hello World", RetryCount = 1, ScreenshotOnFinalFailure = false
            });
            Check(persisted.Result, "E2E-002/get");
            Require(persisted.RawText == "Hello World", "E2E-002 persisted value was incorrect.");
            Console.WriteLine("E2E-002 PASS");

            Check(engine.Execute(new ClickConfiguration
            {
                ActivityType = "Validated Click", ActivityName = "E2E-003 Non-Idempotent", WorkSelector = Selector("NonIdempotentCounterButton", "push button"),
                PostCondition = new(PostConditionKind.TextEquals, Selector("DelayedLabel", "text"), ExpectedValue: "Loaded"),
                RetryCount = 5, RetryIntervalMilliseconds = 100, AllowActionRetry = false, ScreenshotOnFinalFailure = false
            }).Result, "E2E-003/click");
            var counter = engine.Execute(new GetTextConfiguration
            {
                ActivityType = "Validated Get Text", ActivityName = "E2E-003 Verify Counter", WorkSelector = Selector("ClickCounter", "text"),
                Rules = new[] { TextRule.Exact }, ExpectedText = "1", RetryCount = 1, ScreenshotOnFinalFailure = false
            });
            Check(counter.Result, "E2E-003/counter");
            Console.WriteLine("E2E-003 PASS");

            var diagnosticFailure = engine.Execute(new ClickConfiguration
            {
                ActivityType = "Validated Click", ActivityName = "E2E-004 Failure Diagnostics", WorkSelector = Selector("SaveButton", "push button"),
                PostCondition = new(PostConditionKind.TextEquals, Selector("PersistedLabel", "text"), ExpectedValue: "IMPOSSIBLE-E2E-004"),
                RetryCount = 1, RetryIntervalMilliseconds = 50, AllowActionRetry = false, ScreenshotOnFinalFailure = true
            }).Result;
            Require(!diagnosticFailure.Success && diagnosticFailure.FailureCategory == FailureCategory.RetryExhausted, "E2E-004 did not preserve retry-exhaustion failure.");
            Require(diagnosticFailure.ActionExecuted, "E2E-004 did not record the click.");
            Require(diagnosticFailure.ScreenshotPath is not null && File.Exists(diagnosticFailure.ScreenshotPath), "E2E-004 screenshot was not captured.");
            Console.WriteLine("E2E-004 PASS");

            Check(engine.Execute(new ClickConfiguration
            {
                ActivityType = "Validated Click", ActivityName = "Reset before E2E-005", WorkSelector = Selector("ResetButton", "push button"),
                RetryCount = 0, ScreenshotOnFinalFailure = false
            }).Result, "E2E-005/reset");
            var invalid = engine.Execute(new ClickConfiguration
            {
                ActivityType = "Validated Click", ActivityName = "E2E-005 Configuration Safety", WorkSelector = Selector("NonIdempotentCounterButton", "push button"),
                PostCondition = new(PostConditionKind.TextEquals, null, ExpectedValue: "Done"), RetryCount = 0, ScreenshotOnFinalFailure = false
            }).Result;
            Require(!invalid.Success && invalid.FailureCategory == FailureCategory.InvalidConfiguration && !invalid.ActionExecuted, "E2E-005 invalid configuration mutated UI or returned the wrong category.");
            var zeroCounter = engine.Execute(new GetTextConfiguration
            {
                ActivityType = "Validated Get Text", ActivityName = "E2E-005 Verify No Mutation", WorkSelector = Selector("ClickCounter", "text"),
                Rules = new[] { TextRule.Exact }, ExpectedText = "0", RetryCount = 0, ScreenshotOnFinalFailure = false
            });
            Check(zeroCounter.Result, "E2E-005/counter");
            Console.WriteLine("E2E-005 PASS");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"NOT EXECUTED / ENVIRONMENT FAILURE: {ex}");
            return 2;
        }
        finally
        {
            if (!host.HasExited) host.Kill(entireProcessTree: true);
        }
    }

    private static void Check(UIValidationResult result, string id)
    {
        if (!result.Success) throw new InvalidOperationException($"{id} failed: {result.FailureCategory} / {result.FailureReason}");
        Console.WriteLine($"{id} PASS (attempts={result.AttemptCount}, stage={result.ValidationStage})");
    }

    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }

    private sealed class ConsoleLogger : IValidationLogger
    {
        private readonly List<ValidationLogEvent> _events = new();
        public string Flattened => string.Join(" ", _events.SelectMany(e => e.Context.Values).Concat(_events.Select(e => e.Message)));
        public void Log(ValidationLogEvent logEvent)
        {
            _events.Add(logEvent);
            Console.WriteLine($"[{logEvent.Level}] {logEvent.EventName}: {logEvent.Message}");
        }
    }
}
