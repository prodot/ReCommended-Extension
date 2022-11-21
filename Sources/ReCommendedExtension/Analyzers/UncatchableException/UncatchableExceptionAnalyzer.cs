using System;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace ReCommendedExtension.Analyzers.UncatchableException
{
    [ElementProblemAnalyzer(typeof(ISpecificCatchClause), HighlightingTypes = new[] { typeof(UncatchableExceptionWarning) })]
    public sealed class UncatchableExceptionAnalyzer : ElementProblemAnalyzer<ISpecificCatchClause>
    {
        [NotNull]
        static readonly Dictionary<string, string> uncatchableExceptions = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            { "Microsoft.JScript.BreakOutOfFinally", "The exception should not be used in any way." },
            { "Microsoft.JScript.CmdLineException", "The exception should not be used in any way." },
            { "Microsoft.JScript.ContinueOutOfFinally", "The exception should not be used in any way." },
            { "Microsoft.JScript.EndOfFile", "The exception should not be used in any way." },
            { "Microsoft.JScript.JScriptException", "The exception should not be used in any way." },
            { "Microsoft.JScript.NoContextException", "The exception should not be used in any way." },
            { "Microsoft.JScript.ParserException", "The exception should not be used in any way." },
            { "Microsoft.JScript.ReturnOutOfFinally", "The exception should not be used in any way." },
            { "Microsoft.VisualBasic.CompilerServices.IncompleteInitialization", "The exception should not be used in any way." },
            { "Microsoft.VisualBasic.CompilerServices.InternalErrorException", "The exception should not be used in any way." },
            { "System.AccessViolationException", "The exception indicates a bug that can be avoided." },
            { "System.AppDomainUnloadedException", "The exception indicates a bug that can be avoided." },
            { "System.ArgumentException", "The exception indicates a bug that can be avoided." },
            { "System.ArgumentNullException", "The exception indicates a bug that can be avoided." },
            { "System.ArgumentOutOfRangeException", "The exception indicates a bug that can be avoided." },
            { "System.ArrayTypeMismatchException", "The exception indicates a bug that can be avoided." },
            { "System.Collections.Generic.KeyNotFoundException", "The exception indicates a bug that can be avoided." },
            { "System.ComponentModel.InvalidEnumArgumentException", "The exception indicates a bug that can be avoided." },
            { "System.Configuration.SettingsPropertyIsReadOnlyException", "The exception indicates a bug that can be avoided." },
            { "System.Configuration.SettingsPropertyNotFoundException", "The exception indicates a bug that can be avoided." },
            { "System.Configuration.SettingsPropertyWrongTypeException", "The exception indicates a bug that can be avoided." },
            { "System.Data.DeletedRowInaccessibleException", "The exception indicates a bug that can be avoided." },
            { "System.Data.EvaluateException", "The exception indicates a bug that can be avoided." },
            { "System.Data.InRowChangingEventException", "The exception indicates a bug that can be avoided." },
            { "System.Data.InvalidConstraintException", "The exception indicates a bug that can be avoided." },
            { "System.Data.InvalidExpressionException", "The exception indicates a bug that can be avoided." },
            { "System.Data.MissingPrimaryKeyException", "The exception indicates a bug that can be avoided." },
            { "System.Data.NoNullAllowedException", "The exception indicates a bug that can be avoided." },
            { "System.Data.ReadOnlyException", "The exception indicates a bug that can be avoided." },
            { "System.Data.RowNotInTableException", "The exception indicates a bug that can be avoided." },
            { "System.Data.StrongTypingException", "The exception indicates a bug that can be avoided." },
            { "System.Data.SyntaxErrorException", "The exception indicates a bug that can be avoided." },
            { "System.Data.VersionNotFoundException", "The exception indicates a bug that can be avoided." },
            { "System.Diagnostics.Contracts.ContractException", "The exception indicates a bug that can be avoided." },
            { "System.Diagnostics.UnreachableException", "The exception indicates a bug that can be avoided." },
            { "System.DuplicateWaitObjectException", "The exception indicates a bug that can be avoided." },
            { "System.FieldAccessException", "The exception indicates a bug that can be avoided." },
            { "System.IndexOutOfRangeException", "The exception indicates a bug that can be avoided." },
            { "System.InvalidCastException", "The exception indicates a bug that can be avoided." },
            { "System.MemberAccessException", "The exception indicates a bug that can be avoided." },
            { "System.MethodAccessException", "The exception indicates a bug that can be avoided." },
            { "System.MissingFieldException", "The exception indicates a bug that can be avoided." },
            { "System.MissingMemberException", "The exception indicates a bug that can be avoided." },
            { "System.MissingMethodException", "The exception indicates a bug that can be avoided." },
            { @"System.MulticastNotSupportedException", "The exception indicates a bug that can be avoided." },
            { "System.Net.Mail.SmtpFailedRecipientsException", "The exception should not be used in any way." },
            { "System.NotImplementedException", "The exception indicates a bug that can be avoided." },
            { "System.NotSupportedException", "The exception indicates a bug that can be avoided." },
            { "System.NullReferenceException", "The exception indicates a bug that can be avoided." },
            { "System.ObjectDisposedException", "The exception indicates a bug that can be avoided." },
            { "System.OutOfMemoryException", "The exception represents an unrecoverable state." },
            { "System.PlatformNotSupportedException", "The exception indicates a bug that can be avoided." },
            { "System.RankException", "The exception indicates a bug that can be avoided." },
            { "System.Runtime.AmbiguousImplementationException", "The exception indicates a bug that can be avoided." },
            { "System.Runtime.CompilerServices.SwitchExpressionException", "The exception indicates a bug that can be avoided." },
            { "System.Runtime.InteropServices.InvalidComObjectException", "The exception indicates a bug that can be avoided." },
            { "System.Runtime.InteropServices.InvalidOleVariantTypeException", "The exception indicates a bug that can be avoided." },
            { "System.Runtime.InteropServices.MarshalDirectiveException", "The exception indicates a bug that can be avoided." },
            { "System.Runtime.InteropServices.SafeArrayRankMismatchException", "The exception indicates a bug that can be avoided." },
            { "System.Security.HostProtectionException", "The exception cannot be caught." },
            { "System.StackOverflowException", "The exception represents an unrecoverable state." },
            { "System.Threading.ThreadAbortException", "The exception indicates a bug that can be avoided." },
            { "System.Windows.Automation.ElementNotEnabledException", "The exception indicates a bug that can be avoided." },
            { "System.Windows.ResourceReferenceKeyNotFoundException", "The exception indicates a bug that can be avoided." },
        };

        protected override void Run(ISpecificCatchClause element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
        {
            if (uncatchableExceptions.TryGetValue(element.ExceptionType.ToString(), out var reason))
            {
                Debug.Assert(reason != null);

                consumer.AddHighlighting(new UncatchableExceptionWarning(reason, element));
            }
        }
    }
}