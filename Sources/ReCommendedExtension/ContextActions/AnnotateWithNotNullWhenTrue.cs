using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Feature.Services.CSharp.ContextActions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.Modules;

namespace ReCommendedExtension.ContextActions;

[ContextAction(
    Group = "C#",
    Name = "Annotate parameter with [NotNullWhen(true)] attribute" + ZoneMarker.Suffix,
    Description = "Annotates a parameter with the [NotNullWhen(true)] attribute.")]
public sealed class AnnotateWithNotNullWhenTrue(ICSharpContextActionDataProvider provider) : AnnotateWith<NotNullWhenAttribute>(provider)
{
    protected override bool AllowsInheritedMethods => true;

    protected override bool CanBeAnnotated(IDeclaredElement? declaredElement)
        => declaredElement is IParameter
            {
                Kind: ParameterKind.VALUE,
                ContainingParametersOwner: IMethod
                {
                    ShortName: nameof(IEquatable<int>.Equals),
                    TypeParameters: [],
                    Parameters: [var p],
                    ContainingType: IClass type and not IRecord,
                } method,
            } parameter
            && method.ReturnType.IsBool()
            && Equals(p, parameter)
            && TypeEqualityComparer.Default.Equals(p.Type, TypeFactory.CreateType(type))
            && ClrTypeNames.NotNullWhenAttribute.TryGetTypeElement(Provider.PsiModule) is { };

    protected override AttributeValue[] GetAnnotationArguments(IPsiModule psiModule) => [new AttributeValue(ConstantValue.Bool(true, psiModule))];
}