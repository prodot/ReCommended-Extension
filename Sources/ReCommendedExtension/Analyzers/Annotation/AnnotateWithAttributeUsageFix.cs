using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Intentions.Util;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;

namespace ReCommendedExtension.Analyzers.Annotation;

[QuickFix]
public sealed class AnnotateWithAttributeUsageFix : QuickFixBase
{
    readonly MissingAttributeUsageAnnotationWarning highlighting;

    public AnnotateWithAttributeUsageFix(MissingAttributeUsageAnnotationWarning highlighting) => this.highlighting = highlighting;

    public override bool IsAvailable(IUserDataHolder cache) => true;

    public override string Text => $"Annotate with [{nameof(AttributeUsageAttribute)[..^"Attribute".Length]}]";

    protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
    {
        IAttribute attribute;

        using (WriteLockCookie.Create())
        {
            var factory = CSharpElementFactory.GetInstance(highlighting.Declaration);

            var fullName = typeof(AttributeUsageAttribute).FullName;
            Debug.Assert(fullName is { });

            var typeElement = TypesUtil.GetTypeElement(TypeFactory.CreateTypeByCLRName(fullName, highlighting.Declaration.GetPsiModule()));
            Debug.Assert(typeElement is { });

            attribute = factory.CreateAttribute(
                typeElement,
                new[] { new AttributeValue(ConstantValue.NOT_COMPILE_TIME_CONSTANT) },
                Array.Empty<Pair<string, AttributeValue>>());

            attribute = highlighting.Declaration.AddAttributeBefore(attribute, null); // add as last attribute

            ContextActionUtils.FormatWithDefaultProfile(attribute);
        }

        return textControl =>
        {
            Debug.Assert(attribute.Arguments is [{ }]);

            textControl.Caret.MoveTo(attribute.Arguments[0].GetDocumentRange().EndOffset, CaretVisualPlacement.DontScrollIfVisible);
            textControl.EmulateAction("TextControl.Backspace");
        };
    }
}