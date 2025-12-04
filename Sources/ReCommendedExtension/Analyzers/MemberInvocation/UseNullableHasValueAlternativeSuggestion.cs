using JetBrains.Application.Progress;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;
using ReCommendedExtension.Extensions;

namespace ReCommendedExtension.Analyzers.MemberInvocation;

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.BestPractice,
    $"Replace '{nameof(Nullable<>)}<T>.{nameof(Nullable<>.HasValue)}' with a pattern or a null check" + ZoneMarker.Suffix,
    "",
    Severity.SUGGESTION)]
[ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
public sealed class UseNullableHasValueAlternativeSuggestion(string message) : Highlighting(message)
{
    const string SeverityId = "UseNullableHasValueAlternative";

    public required IReferenceExpression ReferenceExpression { get; init; }

    public override DocumentRange CalculateRange()
        => ReferenceExpression.GetDocumentRange().SetStartTo(ReferenceExpression.Reference.GetDocumentRange().StartOffset);

    [QuickFix]
    public sealed class ObjectPatternFix(UseNullableHasValueAlternativeSuggestion highlighting) : QuickFixBase
    {
        public override bool IsAvailable(IUserDataHolder cache)
            => highlighting.ReferenceExpression.GetCSharpLanguageLevel() >= CSharpLanguageLevel.CSharp70;

        public override string Text
        {
            get
            {
                Debug.Assert(highlighting.ReferenceExpression.QualifierExpression is { });

                var expression = highlighting.ReferenceExpression.QualifierExpression.GetText().TrimToSingleLineWithMaxLength(120);

                return $$"""Replace with '{{expression}} is { }'""";
            }
        }

        protected override Action<ITextControl>? ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
        {
            using (WriteLockCookie.Create())
            {
                var factory = CSharpElementFactory.GetInstance(highlighting.ReferenceExpression);

                ModificationUtil
                    .ReplaceChild(
                        highlighting.ReferenceExpression,
                        factory.CreateExpression("($0 is { })", highlighting.ReferenceExpression.QualifierExpression))
                    .TryRemoveParentheses(factory);
            }

            return null;
        }
    }

    [QuickFix]
    public sealed class NotNullFix(UseNullableHasValueAlternativeSuggestion highlighting) : QuickFixBase
    {
        public override bool IsAvailable(IUserDataHolder cache) => true;

        public override string Text
        {
            get
            {
                Debug.Assert(highlighting.ReferenceExpression.QualifierExpression is { });

                var expression = highlighting.ReferenceExpression.QualifierExpression.GetText().TrimToSingleLineWithMaxLength(120);

                return $"Replace with '{expression} != null'";
            }
        }

        protected override Action<ITextControl>? ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
        {
            using (WriteLockCookie.Create())
            {
                var factory = CSharpElementFactory.GetInstance(highlighting.ReferenceExpression);

                ModificationUtil
                    .ReplaceChild(
                        highlighting.ReferenceExpression,
                        factory.CreateExpression("($0 != null)", highlighting.ReferenceExpression.QualifierExpression))
                    .TryRemoveParentheses(factory);
            }

            return null;
        }
    }

    [QuickFix]
    public sealed class NotNullPatternFix(UseNullableHasValueAlternativeSuggestion highlighting) : QuickFixBase
    {
        public override bool IsAvailable(IUserDataHolder cache)
            => highlighting.ReferenceExpression.GetCSharpLanguageLevel() >= CSharpLanguageLevel.CSharp90;

        public override string Text
        {
            get
            {
                Debug.Assert(highlighting.ReferenceExpression.QualifierExpression is { });

                var expression = highlighting.ReferenceExpression.QualifierExpression.GetText().TrimToSingleLineWithMaxLength(120);

                return $"Replace with '{expression} is not null'";
            }
        }

        protected override Action<ITextControl>? ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
        {
            using (WriteLockCookie.Create())
            {
                var factory = CSharpElementFactory.GetInstance(highlighting.ReferenceExpression);

                ModificationUtil
                    .ReplaceChild(
                        highlighting.ReferenceExpression,
                        factory.CreateExpression("($0 is not null)", highlighting.ReferenceExpression.QualifierExpression))
                    .TryRemoveParentheses(factory);
            }

            return null;
        }
    }

    [QuickFix]
    public sealed class TuplePatternFix(UseNullableHasValueAlternativeSuggestion highlighting) : QuickFixBase
    {
        [NonNegativeValue]
        int tupleLength;

        public override bool IsAvailable(IUserDataHolder cache)
        {
            Debug.Assert(highlighting.ReferenceExpression.QualifierExpression is { });

            return highlighting.ReferenceExpression.GetCSharpLanguageLevel() >= CSharpLanguageLevel.CSharp80
                && highlighting.ReferenceExpression.QualifierExpression.Type().Unlift().IsValueTuple(out tupleLength)
                && tupleLength >= 2;
        }

        public override string Text
        {
            get
            {
                Debug.Assert(highlighting.ReferenceExpression.QualifierExpression is { });
                Debug.Assert(tupleLength >= 2);

                var expression = highlighting.ReferenceExpression.QualifierExpression.GetText().TrimToSingleLineWithMaxLength(120);
                var pattern = $"({string.Join(", ", Enumerable.Repeat('_', tupleLength))})";

                return $"Replace with '{expression} is {pattern}'";
            }
        }

        protected override Action<ITextControl>? ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
        {
            using (WriteLockCookie.Create())
            {
                var factory = CSharpElementFactory.GetInstance(highlighting.ReferenceExpression);

                var pattern = $"({string.Join(", ", Enumerable.Repeat('_', tupleLength))})";

                ModificationUtil
                    .ReplaceChild(
                        highlighting.ReferenceExpression,
                        factory.CreateExpression($"($0 is {pattern})", highlighting.ReferenceExpression.QualifierExpression))
                    .TryRemoveParentheses(factory);
            }

            return null;
        }
    }
}