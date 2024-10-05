using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Feature.Services.CSharp.ContextActions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace ReCommendedExtension.ContextActions.Annotations;

[ContextAction(
    GroupType = typeof(CSharpContextActions),
    Name = "Annotate types, methods, and parameters with [MustDisposeResource(false)] attribute" + ZoneMarker.Suffix,
    Description = "Annotates a type, a method, or a parameter with the [MustDisposeResource(false)] attribute.")]
public sealed class AnnotateWithMustDisposeResourceFalse(ICSharpContextActionDataProvider provider) : AnnotateWithMustDisposeResourceBase(provider)
{
    protected override bool IsAttribute(IAttribute attribute)
        => base.IsAttribute(attribute) && attribute.Arguments is [{ Value.ConstantValue: { Kind: ConstantValueKind.Bool, BoolValue: false } }];

    protected override AttributeValue[] GetAnnotationArguments() => [new(ConstantValue.Bool(false, PsiModule))];

    protected override bool IsTypeAnnotated(ITypeElement type) => false;

    protected override bool IsAnyBaseTypeAnnotated(ITypeElement type) => false;

    protected override bool IsAnyBaseMethodAnnotated(IMethod method) => false;

    protected override bool IsParameterOfAnyBaseMethodAnnotated(IParameter parameter) => false;
}