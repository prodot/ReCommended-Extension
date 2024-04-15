using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Feature.Services.CSharp.ContextActions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.DeclaredElements;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.ContextActions.Annotations;

[ContextAction(
    GroupType = typeof(CSharpContextActions),
    Name = "Annotate with [ValueRange(...)] attribute" + ZoneMarker.Suffix,
    Description = "Annotates with the [ValueRange(...)] attribute.")]
public sealed class AnnotateWithValueRange(ICSharpContextActionDataProvider provider) : AnnotateWithCodeAnnotation(provider)
{
    [Pure]
    static bool IsAvailableForType(IType type)
        => type.IsInt() || type.IsLong() || type.IsShort() || type.IsSbyte() || type.IsUint() || type.IsUlong() || type.IsUshort() || type.IsByte();

    protected override bool AllowsMultiple => true;

    protected override string AnnotationAttributeTypeName => nameof(ValueRangeAttribute);

    protected override AttributeValue[] GetAnnotationArguments(IPsiModule psiModule) => [new AttributeValue(ConstantValue.NOT_COMPILE_TIME_CONSTANT)];

    protected override bool CanBeAnnotated(IDeclaredElement? declaredElement, ITreeNode context)
        => declaredElement switch
        {
            IMethod method => IsAvailableForType(method.ReturnType),
            ILocalFunction localFunction => IsAvailableForType(localFunction.ReturnType),
            IProperty property => IsAvailableForType(property.Type),
            IField { IsConstant: false } field => IsAvailableForType(field.Type),
            IParameter parameter => IsAvailableForType(parameter.Type),
            IDelegate @delegate => IsAvailableForType(@delegate.InvokeMethod.ReturnType),

            _ => false,
        };
}