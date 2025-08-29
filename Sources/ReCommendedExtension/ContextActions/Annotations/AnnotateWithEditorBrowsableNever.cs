using System.ComponentModel;
using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Feature.Services.CSharp.ContextActions;
using JetBrains.ReSharper.Psi;

namespace ReCommendedExtension.ContextActions.Annotations;

[ContextAction(
    GroupType = typeof(CSharpContextActions),
    Name = "Annotate deconstruction method with [EditorBrowsable] attribute" + ZoneMarker.Suffix,
    Description = $"Annotates a deconstruction method with the [EditorBrowsable({
        nameof(EditorBrowsableState)
    }.{
        nameof(EditorBrowsableState.Never)
    })] attribute.")]
public sealed class AnnotateWithEditorBrowsableNever(ICSharpContextActionDataProvider provider) : AnnotateWith<EditorBrowsableAttribute>(provider)
{
    protected override bool CanBeAnnotated(IDeclaredElement? declaredElement)
        => declaredElement is IMethod { ShortName: "Deconstruct" } method
            && method.ReturnType.IsVoid()
            && (method is { IsStatic: false, Parameters: [_, _, ..] } && method.Parameters.All(p => p.Kind == ParameterKind.OUTPUT)
                || method is { IsExtensionMethod: true, Parameters: [_, _, _, ..] }
                && method.Parameters.Skip(1).All(p => p.Kind == ParameterKind.OUTPUT));

    protected override AttributeValue[] GetAnnotationArguments()
        =>
        [
            new(
                ConstantValue.Enum(
                    ConstantValue.Int((int)EditorBrowsableState.Never, PsiModule),
                    TypeFactory.CreateTypeByCLRName(ClrTypeNames.EditorBrowsableState, PsiModule))),
        ];
}