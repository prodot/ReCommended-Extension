using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Feature.Services.CSharp.ContextActions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Util;

namespace ReCommendedExtension.ContextActions.Annotations;

[ContextAction(
    GroupType = typeof(CSharpContextActions),
    Name = "Annotate attribute type with [AttributeUsage] attribute" + ZoneMarker.Suffix,
    Description = "Annotates an attribute type with the [AttributeUsage] attribute.")]

public sealed class AnnotateWithAttributeUsage(ICSharpContextActionDataProvider provider) : AnnotateWith<AttributeUsageAttribute>(provider)
{
    protected override bool CanBeAnnotated(IDeclaredElement? declaredElement)
        => declaredElement is ITypeElement typeElement && typeElement.IsAttribute();

    protected override AttributeValue[] GetAnnotationArguments(IPsiModule psiModule) => [new AttributeValue(ConstantValue.NOT_COMPILE_TIME_CONSTANT)];
}