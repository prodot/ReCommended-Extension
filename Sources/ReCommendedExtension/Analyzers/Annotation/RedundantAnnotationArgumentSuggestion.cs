using JetBrains.Application.Progress;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;

namespace ReCommendedExtension.Analyzers.Annotation;

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.DeclarationRedundancy,
    "Redundant annotation argument" + ZoneMarker.Suffix,
    "",
    Severity.SUGGESTION)]
[ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
public sealed class RedundantAnnotationArgumentSuggestion(string message, ICSharpArgument argument) : AttributeHighlighting(message, false)
{
    const string SeverityId = "RedundantAnnotationArgument";

    public override DocumentRange CalculateRange() => argument.GetDocumentRange();

    [QuickFix]
    public sealed class Fix(RedundantAnnotationArgumentSuggestion highlighting) : QuickFixBase
    {
        public override bool IsAvailable(IUserDataHolder cache) => true;

        public override string Text => "Remove attribute argument";

        protected override Action<ITextControl>? ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
        {
            Debug.Assert(highlighting.Attribute.Arguments is [_]);

            using (WriteLockCookie.Create())
            {
                ModificationUtil.DeleteChildRange(highlighting.Attribute.LPar, highlighting.Attribute.RPar);
            }

            return null;
        }
    }
}