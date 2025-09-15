using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace ReCommendedExtension.Analyzers.UncatchableException;

[ElementProblemAnalyzer(typeof(ISpecificCatchClause), HighlightingTypes = [typeof(UncatchableExceptionWarning)])]
public sealed class UncatchableExceptionAnalyzer : ElementProblemAnalyzer<ISpecificCatchClause>
{
    enum Reason
    {
        ShouldNotBeUsed,
        IndicatesAvoidableBug,
        UnrecoverableState,
        CannotBeCaught,
    }

    static readonly Dictionary<string, Reason> uncatchableExceptions = new(StringComparer.Ordinal)
    {
        { "Microsoft.JScript.BreakOutOfFinally", Reason.ShouldNotBeUsed },
        { "Microsoft.JScript.CmdLineException", Reason.ShouldNotBeUsed },
        { "Microsoft.JScript.ContinueOutOfFinally", Reason.ShouldNotBeUsed },
        { "Microsoft.JScript.EndOfFile", Reason.ShouldNotBeUsed },
        { "Microsoft.JScript.JScriptException", Reason.ShouldNotBeUsed },
        { "Microsoft.JScript.NoContextException", Reason.ShouldNotBeUsed },
        { "Microsoft.JScript.ParserException", Reason.ShouldNotBeUsed },
        { "Microsoft.JScript.ReturnOutOfFinally", Reason.ShouldNotBeUsed },
        { "Microsoft.VisualBasic.CompilerServices.IncompleteInitialization", Reason.ShouldNotBeUsed },
        { "Microsoft.VisualBasic.CompilerServices.InternalErrorException", Reason.ShouldNotBeUsed },
        { "System.AccessViolationException", Reason.IndicatesAvoidableBug },
        { "System.AppDomainUnloadedException", Reason.IndicatesAvoidableBug },
        { "System.ArgumentException", Reason.IndicatesAvoidableBug },
        { "System.ArgumentNullException", Reason.IndicatesAvoidableBug },
        { "System.ArgumentOutOfRangeException", Reason.IndicatesAvoidableBug },
        { "System.ArrayTypeMismatchException", Reason.IndicatesAvoidableBug },
        { "System.Collections.Generic.KeyNotFoundException", Reason.IndicatesAvoidableBug },
        { "System.ComponentModel.InvalidEnumArgumentException", Reason.IndicatesAvoidableBug },
        { "System.Configuration.SettingsPropertyIsReadOnlyException", Reason.IndicatesAvoidableBug },
        { "System.Configuration.SettingsPropertyNotFoundException", Reason.IndicatesAvoidableBug },
        { "System.Configuration.SettingsPropertyWrongTypeException", Reason.IndicatesAvoidableBug },
        { "System.Data.DeletedRowInaccessibleException", Reason.IndicatesAvoidableBug },
        { "System.Data.EvaluateException", Reason.IndicatesAvoidableBug },
        { "System.Data.InRowChangingEventException", Reason.IndicatesAvoidableBug },
        { "System.Data.InvalidConstraintException", Reason.IndicatesAvoidableBug },
        { "System.Data.InvalidExpressionException", Reason.IndicatesAvoidableBug },
        { "System.Data.MissingPrimaryKeyException", Reason.IndicatesAvoidableBug },
        { "System.Data.NoNullAllowedException", Reason.IndicatesAvoidableBug },
        { "System.Data.ReadOnlyException", Reason.IndicatesAvoidableBug },
        { "System.Data.RowNotInTableException", Reason.IndicatesAvoidableBug },
        { "System.Data.StrongTypingException", Reason.IndicatesAvoidableBug },
        { "System.Data.SyntaxErrorException", Reason.IndicatesAvoidableBug },
        { "System.Data.VersionNotFoundException", Reason.IndicatesAvoidableBug },
        { "System.Diagnostics.Contracts.ContractException", Reason.IndicatesAvoidableBug },
        { "System.Diagnostics.UnreachableException", Reason.IndicatesAvoidableBug },
        { "System.DuplicateWaitObjectException", Reason.IndicatesAvoidableBug },
        { "System.FieldAccessException", Reason.IndicatesAvoidableBug },
        { "System.IndexOutOfRangeException", Reason.IndicatesAvoidableBug },
        { "System.InvalidCastException", Reason.IndicatesAvoidableBug },
        { "System.MemberAccessException", Reason.IndicatesAvoidableBug },
        { "System.MethodAccessException", Reason.IndicatesAvoidableBug },
        { "System.MissingFieldException", Reason.IndicatesAvoidableBug },
        { "System.MissingMemberException", Reason.IndicatesAvoidableBug },
        { "System.MissingMethodException", Reason.IndicatesAvoidableBug },
        { "System.MulticastNotSupportedException", Reason.IndicatesAvoidableBug },
        { "System.Net.Mail.SmtpFailedRecipientsException", Reason.ShouldNotBeUsed },
        { "System.NotImplementedException", Reason.IndicatesAvoidableBug },
        { "System.NotSupportedException", Reason.IndicatesAvoidableBug },
        { "System.NullReferenceException", Reason.IndicatesAvoidableBug },
        { "System.ObjectDisposedException", Reason.IndicatesAvoidableBug },
        { "System.OutOfMemoryException", Reason.UnrecoverableState },
        { "System.PlatformNotSupportedException", Reason.IndicatesAvoidableBug },
        { "System.RankException", Reason.IndicatesAvoidableBug },
        { "System.Runtime.AmbiguousImplementationException", Reason.IndicatesAvoidableBug },
        { "System.Runtime.CompilerServices.SwitchExpressionException", Reason.IndicatesAvoidableBug },
        { "System.Runtime.InteropServices.InvalidComObjectException", Reason.IndicatesAvoidableBug },
        { "System.Runtime.InteropServices.InvalidOleVariantTypeException", Reason.IndicatesAvoidableBug },
        { "System.Runtime.InteropServices.MarshalDirectiveException", Reason.IndicatesAvoidableBug },
        { "System.Runtime.InteropServices.SafeArrayRankMismatchException", Reason.IndicatesAvoidableBug },
        { "System.Security.HostProtectionException", Reason.CannotBeCaught },
        { "System.StackOverflowException", Reason.UnrecoverableState },
        { "System.Threading.ThreadAbortException", Reason.IndicatesAvoidableBug },
        { "System.Windows.Automation.ElementNotEnabledException", Reason.IndicatesAvoidableBug },
        { "System.Windows.ResourceReferenceKeyNotFoundException", Reason.IndicatesAvoidableBug },
    };

    protected override void Run(ISpecificCatchClause element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
    {
        if (uncatchableExceptions.TryGetValue(element.ExceptionType.GetClrName().FullName, out var reason))
        {
            consumer.AddHighlighting(
                new UncatchableExceptionWarning(
                    reason switch
                    {
                        Reason.ShouldNotBeUsed => "The exception should not be used in any way.",
                        Reason.IndicatesAvoidableBug => "The exception indicates a bug that can be avoided.",
                        Reason.UnrecoverableState => "The exception represents an unrecoverable state.",
                        Reason.CannotBeCaught => "The exception cannot be caught.",

                        _ => throw new NotSupportedException(),
                    },
                    element));
        }
    }
}