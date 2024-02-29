using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Feature.Services.CSharp.ContextActions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.DeclaredElements;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.ContextActions.Annotations;

[ContextAction(
    Group = "C#",
    Name = "Annotate methods with [LinqTunnel] attribute" + ZoneMarker.Suffix,
    Description = "Annotates a method with the [LinqTunnel] attribute.")]
public sealed class AnnotateWithLinqTunnel(ICSharpContextActionDataProvider provider) : AnnotateWithCodeAnnotation(provider)
{
    [Pure]
    static bool CanBeTunneling(IParametersOwner parametersOwner)
        => parametersOwner.ReturnType.IsGenericIEnumerable() && parametersOwner.Parameters.Any(p => p.Type.IsGenericIEnumerable())
            || parametersOwner.ReturnType.IsIAsyncEnumerable() && parametersOwner.Parameters.Any(p => p.Type.IsIAsyncEnumerable());

    protected override string AnnotationAttributeTypeName => nameof(LinqTunnelAttribute);

    protected override bool CanBeAnnotated(IDeclaredElement? declaredElement, ITreeNode context)
        => declaredElement switch
        {
            IMethod method => CanBeTunneling(method),
            ILocalFunction localFunction => CanBeTunneling(localFunction),

            _ => false,
        };
}