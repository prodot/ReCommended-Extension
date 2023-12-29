using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Feature.Services.CSharp.ContextActions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.ContextActions;

[ContextAction(
    Group = "C#",
    Name = "Annotate methods, parameters, properties, and fields with [HandlesResourceDisposal] attribute" + ZoneMarker.Suffix,
    Description = "Annotates a method, a parameter, a property, or a field with the [HandlesResourceDisposal] attribute.")]

public sealed class AnnotateWithHandlesResourceDisposal(ICSharpContextActionDataProvider provider) : AnnotateWithCodeAnnotation(provider)
{
    protected override string AnnotationAttributeTypeName => nameof(HandlesResourceDisposalAttribute);

    protected override bool CanBeAnnotated(IDeclaredElement? declaredElement, ITreeNode context)
    {
        switch (declaredElement)
        {
            case IMethod
            {
                IsStatic: false,
                AccessibilityDomain.DomainType: not (AccessibilityDomain.AccessibilityDomainType.PRIVATE
                or AccessibilityDomain.AccessibilityDomainType.NONE),
                ContainingType: { },
            } method:
                var disposableInterface = PredefinedType.IDISPOSABLE_FQN.TryGetTypeElement(context.GetPsiModule());
                var disposeMethod = disposableInterface?.Methods.FirstOrDefault(m => m.ShortName == nameof(IDisposable.Dispose));

                var asyncDisposableInterface = PredefinedType.IASYNCDISPOSABLE_FQN.TryGetTypeElement(context.GetPsiModule());
                var disposeAsyncMethod = asyncDisposableInterface?.Methods.FirstOrDefault(m => m.ShortName == "DisposeAsync"); // todo: use nameof(IAsyncDisposable.DisposeAsync)

                return method.ContainingType.IsDescendantOf(disposableInterface)
                    && disposeMethod is { }
                    && !method.OverridesOrImplements(disposeMethod)
                    || method.ContainingType.IsDescendantOf(asyncDisposableInterface)
                    && disposeAsyncMethod is { }
                    && !method.OverridesOrImplements(disposeAsyncMethod)
                    || method.ContainingType is IStruct { IsByRefLike: true } && !method.IsDisposeMethod() && !method.IsDisposeAsyncMethod();

            case IParameter
            {
                Kind: ParameterKind.VALUE or ParameterKind.INPUT or ParameterKind.READONLY_REFERENCE or ParameterKind.REFERENCE,
            } parameter:
                return parameter.Type.IsDisposable(context);

            case IProperty property: return property.Type.IsDisposable(context);

            case IField field: return field.Type.IsDisposable(context);

            default: return false;
        }
    }
}