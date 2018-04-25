using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReCommendedExtension.Highlightings;

namespace ReCommendedExtension.Analyzers
{
    [ElementProblemAnalyzer(typeof(ICSharpTreeNode), HighlightingTypes = new[] { typeof(UnthrowableExceptionHighlighting) })]
    public sealed class UnthrowableExceptionAnalyzer : ElementProblemAnalyzer<ICSharpTreeNode>
    {
        [NotNull]
        static readonly Dictionary<string, string> unthrowableExceptions = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            { "System.Exception", "A more specific exception should be used." },
            { "Microsoft.Build.BuildEngine.InternalLoggerException", "The exception should only be thrown by the MSBuild engine." },
            { "Microsoft.Build.BuildEngine.RemoteErrorException", "The exception should only be thrown by the MSBuild engine." },
            { "Microsoft.Build.Exceptions.InternalLoggerException", "The exception should only be thrown by the MSBuild engine." },
            { "Microsoft.CSharp.RuntimeBinder.RuntimeBinderException", "The exception should only be thrown by the CLR." },
            { "Microsoft.CSharp.RuntimeBinder.RuntimeBinderInternalCompilerException", "The exception should only be thrown by the CLR." },
            { "Microsoft.JScript.CmdLineException", "The exception should not be used in any way." },
            { "Microsoft.JScript.ParserException", "The exception should not be used in any way." },
            { "Microsoft.JScript.EndOfFile", "The exception should not be used in any way." },
            { "Microsoft.VisualBasic.CompilerServices.IncompleteInitialization", "The exception should not be used in any way." },
            { "Microsoft.VisualBasic.CompilerServices.InternalErrorException", "The exception should not be used in any way." },
            { "System.AggregateException", "The exception should only be thrown by the TPL API." },
            { "System.ApplicationException", "A more specific exception should be used." },
            { "Microsoft.JScript.BreakOutOfFinally", "The exception should not be used in any way." },
            { "Microsoft.JScript.ContinueOutOfFinally", "The exception should not be used in any way." },
            { "Microsoft.JScript.JScriptException", "The exception should not be used in any way." },
            { "Microsoft.JScript.NoContextException", "The exception should not be used in any way." },
            { "Microsoft.JScript.ReturnOutOfFinally", "The exception should not be used in any way." },
            { "System.Reflection.TargetException", "The exception should only be thrown by the Reflection API." },
            { "System.Reflection.TargetInvocationException", "The exception should only be thrown by the Reflection API." },
            { "System.Reflection.TargetParameterCountException", "The exception should only be thrown by the Reflection API." },
            { "System.ComponentModel.Composition.CompositionContractMismatchException", "The exception should only be thrown by the MEF API." },
            { "System.ComponentModel.Composition.CompositionException", "The exception should only be thrown by the MEF API." },
            { "System.ComponentModel.Composition.ChangeRejectedException", "The exception should only be thrown by the MEF API." },
            { "System.ComponentModel.Composition.ImportCardinalityMismatchException", "The exception should only be thrown by the MEF API." },
            { @"System.ComponentModel.Composition.Primitives.ComposablePartException", "The exception should only be thrown by the MEF API." },
            { "System.ComponentModel.Design.ExceptionCollection", "The exception uses non-generic collections." },
            { "System.Configuration.Provider.ProviderException", "The exception should only be thrown by the .NET Configuration API." },
            { "System.Configuration.SettingsPropertyIsReadOnlyException", "The exception should only be thrown by the .NET Configuration API." },
            { "System.Configuration.SettingsPropertyNotFoundException", "The exception should only be thrown by the .NET Configuration API." },
            { "System.Configuration.SettingsPropertyWrongTypeException", "The exception should only be thrown by the .NET Configuration API." },
            { "System.DirectoryServices.Protocols.DirectoryException", "A more specific exception should be used." },
            { "System.IdentityModel.RequestException", "A more specific exception should be used." },
            { "System.IdentityModel.Selectors.CardSpaceException", "The exception should only be thrown by the WCF API." },
            { "System.IdentityModel.Services.FederationException", "A more specific exception should be used." },
            { "System.Management.Instrumentation.InstrumentationBaseException", "A more specific exception should be used." },
            { "System.Net.Http.HttpRequestException", "A more specific exception should be used." },
            { "System.Net.Mail.SmtpFailedRecipientsException", "The exception should not be used in any way." },
            { "System.Runtime.CompilerServices.RuntimeWrappedException", "The exception should only be thrown by the CLR." },
            { "System.Runtime.DurableInstancing.InstancePersistenceException", "A more specific exception should be used." },
            { @"System.Runtime.Remoting.MetadataServices.SUDSGeneratorException", @"The exception should only be thrown by the Remoting API." },
            { @"System.Runtime.Remoting.MetadataServices.SUDSParserException", @"The exception should only be thrown by the Remoting API." },
            { "System.SystemException", "A more specific exception should be used." },
            { "Microsoft.SqlServer.Server.InvalidUdtException", "The exception should only be thrown by the SQL Server API." },
            { "System.AccessViolationException", "The exception should only be thrown by the CLR." },
            { "System.AppDomainUnloadedException", "The exception should only be thrown by the CLR." },
            { "System.ArrayTypeMismatchException", "The exception should only be thrown by the CLR." },
            { "System.BadImageFormatException", "The exception should only be thrown by the CLR." },
            { "System.CannotUnloadAppDomainException", "The exception should only be thrown by the CLR." },
            { "System.ComponentModel.WarningException", "A more specific exception should be used." },
            { "System.Configuration.ConfigurationException", "The exception should only be thrown by the .NET Configuration API." },
            { "System.Configuration.ConfigurationErrorsException", "The exception should only be thrown by the .NET Configuration API." },
            { "System.ContextMarshalException", "The exception should only be thrown by the CLR." },
            { "System.Data.DataException", "The exception should only be thrown by the ADO API." },
            { "System.Data.EntityException", "A more specific exception should be used." },
            { "System.Data.TypedDataSetGeneratorException", "A more specific exception should be used." },
            { "System.Data.SqlTypes.SqlTypeException", "A more specific exception should be used." },
            { "System.DataMisalignedException", "The exception should only be thrown by the CLR." },
            { "System.Deployment.Application.DeploymentException", "The exception should only be thrown by the ClickOnce API." },
            { "System.Deployment.Application.DependentPlatformMissingException", "The exception should only be thrown by the ClickOnce API." },
            { "System.Deployment.Application.CompatibleFrameworkMissingException", "The exception should only be thrown by the ClickOnce API." },
            { "System.Deployment.Application.SupportedRuntimeMissingException", "The exception should only be thrown by the ClickOnce API." },
            { "System.Deployment.Application.DeploymentDownloadException", "The exception should only be thrown by the ClickOnce API." },
            { "System.Deployment.Application.InvalidDeploymentException", "The exception should only be thrown by the ClickOnce API." },
            { "System.Deployment.Application.TrustNotGrantedException", "The exception should only be thrown by the ClickOnce API." },
            { "System.DirectoryServices.AccountManagement.PrincipalException", "A more specific exception should be used." },
            { "System.IndexOutOfRangeException", "The exception should only be thrown by the CLR." },
            { "System.InsufficientExecutionStackException", "The exception should only be thrown by the CLR." },
            { "System.InvalidProgramException", "The exception should only be thrown by the CLR." },
            { "System.IO.FileLoadException", "The exception should only be thrown by the CLR." },
            { "System.MemberAccessException", "The exception should only be thrown by the CLR." },
            { "System.FieldAccessException", "The exception should only be thrown by the CLR." },
            { "System.MethodAccessException", "The exception should only be thrown by the CLR." },
            { "System.MissingMemberException", "The exception should only be thrown by the CLR." },
            { "System.MissingFieldException", "The exception should only be thrown by the CLR." },
            { "System.MissingMethodException", "The exception should only be thrown by the CLR." },
            { @"System.MulticastNotSupportedException", "The exception should only be thrown by the CLR." },
            { "System.NullReferenceException", "The exception should only be thrown by the CLR." },
            { "System.OutOfMemoryException", "The exception should only be thrown by the CLR." },
            { "System.InsufficientMemoryException", "The exception should only be thrown by the CLR." },
            { "System.RankException", "The exception should only be thrown by the CLR." },
            { "System.Reflection.AmbiguousMatchException", "The exception should only be thrown by the Reflection API." },
            { "System.Reflection.ReflectionTypeLoadException", "The exception should only be thrown by the Reflection API." },
            { "System.Resources.MissingManifestResourceException", "The exception should only be thrown by the CLR." },
            { "System.Resources.MissingSatelliteAssemblyException", "The exception should only be thrown by the CLR." },
            { @"System.Runtime.InteropServices.ExternalException", "The exception should only be thrown by the CLR." },
            { @"System.Data.Odbc.OdbcException", "The exception should only be thrown by the ADO API." },
            { "System.Data.OleDb.OleDbException", "The exception should only be thrown by the ADO API." },
            { "System.Data.OracleClient.OracleException", "The exception should only be thrown by the ADO API." },
            { "System.Data.SqlClient.SqlException", "The exception should only be thrown by the ADO API." },
            { "System.Messaging.MessageQueueException", "The exception should only be thrown by the Message Queue API." },
            { @"System.Runtime.InteropServices.SEHException", "The exception should only be thrown by the CLR." },
            { @"System.Runtime.InteropServices.InvalidComObjectException", "The exception should only be thrown by the CLR." },
            { @"System.Runtime.InteropServices.InvalidOleVariantTypeException", "The exception should only be thrown by the CLR." },
            { @"System.Runtime.InteropServices.MarshalDirectiveException", "The exception should only be thrown by the CLR." },
            { @"System.Runtime.InteropServices.SafeArrayRankMismatchException", "The exception should only be thrown by the CLR." },
            { "System.Security.HostProtectionException", "The exception should only be thrown by the CLR." },
            { "System.StackOverflowException", "The exception should only be thrown by the CLR." },
            { "System.Threading.ThreadAbortException", "The exception should only be thrown by the CLR." },
            { "System.Threading.ThreadInterruptedException", "The exception should only be thrown by the CLR." },
            { "System.Threading.ThreadStartException", "The exception should only be thrown by the CLR." },
            { "System.Threading.ThreadStateException", "The exception should only be thrown by the CLR." },
            { "System.TypeInitializationException", "The exception should only be thrown by the CLR." },
            { "System.TypeLoadException", "The exception should only be thrown by the CLR." },
            { "System.DllNotFoundException", "The exception should only be thrown by the CLR." },
            { "System.EntryPointNotFoundException", "The exception should only be thrown by the CLR." },
            { "System.TypeAccessException", "The exception should only be thrown by the CLR." },
            { "System.TypeUnloadedException", "The exception should only be thrown by the CLR." },
            { "System.Windows.Markup.XamlParseException", "The exception should only be thrown by the WPF API." },
            { "System.Windows.Media.Animation.AnimationException", "The exception should only be thrown by the WPF API." },
            { "System.Threading.BarrierPostPhaseException", "The exception should only be thrown by the CLR." },
        };

        protected override void Run(ICSharpTreeNode element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
        {
            ICSharpExpression exception;
            switch (element)
            {
                case IThrowStatement throwStatement:
                    exception = throwStatement.Exception;
                    break;

                case IThrowExpression throwExpression:
                    exception = throwExpression.Exception;
                    break;

                default: return;
            }

            if (exception == null)
            {
                return;
            }

            if (unthrowableExceptions.TryGetValue(exception.GetExpressionType().ToString(), out var reason))
            {
                consumer.AddHighlighting(new UnthrowableExceptionHighlighting(reason, exception));
            }
        }
    }
}