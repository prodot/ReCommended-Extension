using JetBrains.Application.Progress;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Feature.Services.Daemon.Attributes;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;

namespace ReCommendedExtension.Analyzers.Annotation;

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.DeclarationRedundancy,
    "Redundant nullable annotation" + ZoneMarker.Suffix,
    "",
    Severity.HINT)]
[ConfigurableSeverityHighlighting(
    SeverityId,
    CSharpLanguage.Name,
    AttributeId = AnalysisHighlightingAttributeIds.DEADCODE,
    OverlapResolve = OverlapResolveKind.DEADCODE)]
public sealed class RedundantNullableAnnotationHint(string message) : Highlighting(message)
{
    const string SeverityId = "RedundantNullableAnnotation";

    public required INullableTypeUsage NullableTypeUsage { get; init; }

    public override DocumentRange CalculateRange() => NullableTypeUsage.NullableMark.GetDocumentRange();

    [QuickFix]
    public sealed class Fix(RedundantNullableAnnotationHint highlighting) : QuickFixBase
    {
        public override bool IsAvailable(IUserDataHolder cache) => true;

        public override string Text => "Make method return type not nullable";

        protected override Action<ITextControl>? ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
        {
            using (WriteLockCookie.Create())
            {
                ModificationUtil.ReplaceChild(highlighting.NullableTypeUsage, highlighting.NullableTypeUsage.UnderlyingType);
            }

            return null;
        }
    }
}