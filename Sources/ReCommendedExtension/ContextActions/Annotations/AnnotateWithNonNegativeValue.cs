using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Feature.Services.CSharp.ContextActions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.DeclaredElements;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.ContextActions.Annotations;

[ContextAction(
    GroupType = typeof(CSharpContextActions),
    Name = "Annotate with [NonNegativeValue] attribute" + ZoneMarker.Suffix,
    Description = "Annotates with the [NonNegativeValue] attribute.")]
public sealed class AnnotateWithNonNegativeValue(ICSharpContextActionDataProvider provider) : AnnotateWithCodeAnnotation(provider)
{
    [Pure]
    static bool IsAvailableForType(IType type) => type.IsInt() || type.IsLong() || type.IsShort() || type.IsSbyte();

    protected override string AnnotationAttributeTypeName => nameof(NonNegativeValueAttribute);

    protected override bool CanBeAnnotated(IDeclaredElement? declaredElement, ITreeNode context)
        => declaredElement switch
        {
            IMethod method => IsAvailableForType(method.ReturnType),
            ILocalFunction localFunction => IsAvailableForType(localFunction.ReturnType),
            IProperty property => IsAvailableForType(property.Type),
            IField { IsConstant: false } field => IsAvailableForType(field.Type),
            IParameter parameter => IsAvailableForType(parameter.Type),
            IDelegate delegateType => IsAvailableForType(delegateType.InvokeMethod.ReturnType),

            _ => false,
        };
}