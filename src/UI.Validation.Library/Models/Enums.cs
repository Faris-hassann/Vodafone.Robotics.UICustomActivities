namespace Vodafone.Robotics.UiValidation.Models;

public enum ValidationStage { NotStarted, ConfigurationValidation, TargetResolution, TargetIdentityValidation, PreCondition, DoWork, PostCondition, Diagnostics, Completed }
public enum FailureCategory { None, InvalidConfiguration, TargetNotFound, TargetAmbiguous, TargetIdentityMismatch, TargetNotReady, ReadFailed, ActionFailed, TextValidationFailed, PostConditionFailed, RetryExhausted, Cancelled, Unknown }
public enum RawValueState { NoValue, Empty, WhitespaceOnly, NonEmpty, ReadFailed }
public enum TextRule { None, RetrievedSuccessfully, NotEmpty, Exact, Contains, StartsWith, EndsWith, Regex }
public enum TypeIntoMode { Replace, Append, ClearOnly }
public enum VerificationMode { Auto, Text, ValueAttribute, SpecificAttribute, ActionOnly }
public enum PostConditionKind { None, ElementAppears, ElementDisappears, TextEquals, TextContains, AttributeEquals, AttributeChanges, TargetStateChanges, WindowOrPageAppears }
public enum ConditionKind { None, Exists, NotExists, TextEquals, TextContains, AttributeEquals, Enabled, Disabled, Visible, Hidden }
public enum MismatchKind { None, ExactMatch, PartialInput, UnexpectedPrefix, UnexpectedSuffix, DuplicateInput, NoChange, UnexpectedTransformation, UnclassifiedMismatch }
public enum ValidationLogLevel { Trace, Information, Warning, Error }
