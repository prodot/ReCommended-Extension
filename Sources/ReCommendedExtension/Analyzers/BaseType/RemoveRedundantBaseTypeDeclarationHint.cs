using JetBrains.Application.Progress;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Feature.Services.Daemon.Attributes;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;

namespace ReCommendedExtension.Analyzers.BaseType;

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.BestPractice,
    "Redundant base type declaration" + ZoneMarker.Suffix,
    "",
    Severity.SUGGESTION)]
[ConfigurableSeverityHighlighting(
    SeverityId,
    CSharpLanguage.Name,
    AttributeId = AnalysisHighlightingAttributeIds.DEADCODE,
    OverlapResolve = OverlapResolveKind.DEADCODE)]
public sealed class RemoveRedundantBaseTypeDeclarationHint(string message) : Highlighting(message)
{
    const string SeverityId = "RemoveRedundantBaseTypeDeclaration";

    public required IExtendsList BaseTypes { get; init; }

    public override DocumentRange CalculateRange()
    {
        Debug.Assert(BaseTypes.ExtendedTypes is [_, ..]);

        return BaseTypes.ExtendedTypes[0].GetDocumentRange();
    }

    [QuickFix]
    public sealed class Fix(RemoveRedundantBaseTypeDeclarationHint highlighting) : QuickFixBase
    {
        public override bool IsAvailable(IUserDataHolder cache) => true;

        public override string Text => "Remove redundant 'object'";

        protected override Action<ITextControl>? ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
        {
            using (WriteLockCookie.Create())
            {
                Debug.Assert(highlighting.BaseTypes.ExtendedTypes is [_, ..]);

                highlighting.BaseTypes.RemoveExtendedType(highlighting.BaseTypes.ExtendedTypes[0]);
            }

            return null;
        }
    }
}