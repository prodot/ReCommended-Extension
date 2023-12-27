using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Intentions.Util;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;

namespace ReCommendedExtension.Analyzers.Annotation;

[QuickFix]
public sealed class AnnotateWithNotNullWhenTrueFix(MissingNotNullWhenAnnotationSuggestion highlighting) : QuickFixBase // todo: make it ContextAction
{
    public override bool IsAvailable(IUserDataHolder cache) => true;

    public override string Text => $"Annotate with [{nameof(NotNullWhenAttribute).WithoutSuffix()}(true)]";

    protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
    {
        using (WriteLockCookie.Create())
        {
            var factory = CSharpElementFactory.GetInstance(highlighting.Declaration);

            var fullName = typeof(NotNullWhenAttribute).FullName;
            Debug.Assert(fullName is { });

            var psiModule = highlighting.Declaration.GetPsiModule();

            var typeElement = TypesUtil.GetTypeElement(TypeFactory.CreateTypeByCLRName(fullName, psiModule));
            Debug.Assert(typeElement is { });

            var attribute = factory.CreateAttribute(
                typeElement,
                [new AttributeValue(ConstantValue.Bool(true, psiModule))],
                Array.Empty<Pair<string, AttributeValue>>());

            attribute = highlighting.Declaration.AddAttributeBefore(attribute, null); // add as last attribute

            ContextActionUtils.FormatWithDefaultProfile(attribute);
        }

        return _ => { };
    }
}