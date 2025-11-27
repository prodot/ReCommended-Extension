using System.Text;
using JetBrains.Application.Progress;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Parsing;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;
using ReCommendedExtension.Extensions;

namespace ReCommendedExtension.Analyzers.Argument;

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.BestPractice,
    "Use another argument" + ZoneMarker.Suffix,
    "",
    Severity.SUGGESTION)]
[ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
public sealed class UseOtherArgumentSuggestion(string message) : Highlighting(message)
{
    const string SeverityId = "UseOtherArgument";

    public required ArgumentReplacement ArgumentReplacement { get; init; }

    internal UpcomingArgument? AdditionalArgument { get; init; }

    internal ICSharpArgument? RedundantArgument { get; init; }

    public override DocumentRange CalculateRange() => ArgumentReplacement.Argument.Value.GetDocumentRange();

    [QuickFix]
    public sealed class Fix(UseOtherArgumentSuggestion highlighting) : QuickFixBase
    {
        public override bool IsAvailable(IUserDataHolder cache) => true;

        public override string Text
        {
            get
            {
                var builder = new StringBuilder();

                builder.Append($"Replace with '{highlighting.ArgumentReplacement.Replacement.Value.TrimToSingleLineWithMaxLength(120)}'");

                if (highlighting is { AdditionalArgument: { } } or { RedundantArgument: { } })
                {
                    builder.Append(" (and ");

                    if (highlighting.AdditionalArgument is { } additionalArgument)
                    {
                        builder.Append($"add '{additionalArgument.Value}'");
                    }

                    if (highlighting is { AdditionalArgument: { }, RedundantArgument: { } })
                    {
                        builder.Append(", ");
                    }

                    if (highlighting.RedundantArgument is { })
                    {
                        builder.Append($"remove '{highlighting.RedundantArgument.Value?.GetText()}'");
                    }

                    builder.Append(')');
                }

                return builder.ToString();
            }
        }

        protected override Action<ITextControl>? ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
        {
            using (WriteLockCookie.Create())
            {
                var factory = CSharpElementFactory.GetInstance(highlighting.ArgumentReplacement.Argument);

                var argument = ModificationUtil.ReplaceChild(
                    highlighting.ArgumentReplacement.Argument,
                    factory.CreateArgument(
                        highlighting.ArgumentReplacement.Replacement.ParameterKind,
                        highlighting.ArgumentReplacement.Replacement.ParameterName,
                        factory.CreateExpression(highlighting.ArgumentReplacement.Replacement.Value)));

                highlighting.RedundantArgument?.Remove();

                if (highlighting.AdditionalArgument is { } additionalArgument)
                {
                    var comma = ModificationUtil.AddChildAfter(argument, CSharpTokenType.COMMA.CreateTreeElement());

                    ModificationUtil.AddChildAfter(
                        comma,
                        factory.CreateArgument(
                            additionalArgument.ParameterKind,
                            additionalArgument.ParameterName,
                            factory.CreateExpression(additionalArgument.Value)));
                }
            }

            return null;
        }
    }
}