using JetBrains.Application.Progress;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;

namespace ReCommendedExtension.Analyzers.Annotation;

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.ConstraintViolation,
    "Annotation argument is out of range" + ZoneMarker.Suffix,
    "",
    Severity.WARNING)]
[ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
public sealed class InvalidValueRangeBoundaryWarning(string message) : Highlighting(message)
{
    const string SeverityId = "InvalidValueRangeBoundary";

    public required ICSharpExpression PositionParameter { get; init; }

    public required ValueRangeBoundary Boundary { get; init; }

    public required IType Type { get; init; }

    public required bool TypeIsSigned { get; init; }

    public override DocumentRange CalculateRange() => PositionParameter.GetNavigationRange();

    [QuickFix]
    public sealed class Fix(InvalidValueRangeBoundaryWarning highlighting) : QuickFixBase
    {
        public override bool IsAvailable(IUserDataHolder cache) => true;

        public override string Text
        {
            get
            {
                Debug.Assert(CSharpLanguage.Instance is { });

                return highlighting.Boundary switch
                {
                    ValueRangeBoundary.Lower => highlighting.TypeIsSigned
                        ? $"Set the 'from' value to the '{highlighting.Type.GetPresentableName(CSharpLanguage.Instance)}.{nameof(int.MinValue)}'"
                        : "Set the 'from' value to the '0'",

                    ValueRangeBoundary.Higher => 
                        $"Set the 'to' value to the '{highlighting.Type.GetPresentableName(CSharpLanguage.Instance)}.{nameof(int.MaxValue)}'",

                    _ => throw new NotSupportedException(),
                };
            }
        }

        protected override Action<ITextControl>? ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
        {
            using (WriteLockCookie.Create())
            {
                Debug.Assert(CSharpLanguage.Instance is { });

                var factory = CSharpElementFactory.GetInstance(highlighting.PositionParameter);

                var expression = highlighting.Boundary switch
                {
                    ValueRangeBoundary.Lower => highlighting.TypeIsSigned
                        ? $"{highlighting.Type.GetPresentableName(CSharpLanguage.Instance)}.{nameof(int.MinValue)}"
                        : "0",

                    ValueRangeBoundary.Higher => $"{highlighting.Type.GetPresentableName(CSharpLanguage.Instance)}.{nameof(int.MaxValue)}",

                    _ => throw new NotSupportedException(),
                };

                highlighting.PositionParameter.ReplaceBy(factory.CreateExpressionAsIs(expression));
            }

            return null;
        }
    }
}