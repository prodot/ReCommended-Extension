using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Feature.Services.CSharp.ContextActions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;

namespace ReCommendedExtension.ContextActions.CodeContracts;

[ContextAction(
    GroupType = typeof(CSharpContextActions),
    Name = "Add contract: all collection items are not null" + ZoneMarker.Suffix,
    Description = "Adds a contract that all collection items (or dictionary values) are not null.")]
public sealed class CollectionAllItemsNotNull(ICSharpContextActionDataProvider provider) : AddContractContextAction(provider)
{
    [Pure]
    static bool IsGenericDictionaryWithReferenceTypeValues(IDeclaredType declaredType)
    {
        if (declaredType.GetTypeElement() is { } typeElement
            && declaredType.Module.GetPredefinedType().GenericIDictionary.GetTypeElement() is { } genericInterfaceTypeElement
            && typeElement.IsDescendantOf(genericInterfaceTypeElement))
        {
            foreach (var substitution in typeElement.GetAncestorSubstitution(genericInterfaceTypeElement))
            {
                var secondTypeParameter = declaredType.GetSubstitution().Apply(substitution)[genericInterfaceTypeElement.TypeParameters[1]];
                if (secondTypeParameter.Classify == TypeClassification.REFERENCE_TYPE)
                {
                    return true;
                }
            }
        }

        return false;
    }

    bool isDictionary;

    protected override bool IsAvailableForType(IType type)
    {
        var context = Provider.SelectedElement;
        Debug.Assert(context is { });

        if ((type.IsCollectionLike() || type.IsGenericArray(context))
            && !type.IsGenericIEnumerable()
            && !type.IsArray()
            && CollectionTypeUtil.ElementTypeByCollectionType(type, context, false) is { } elementType)
        {
            if (elementType.Classify == TypeClassification.REFERENCE_TYPE)
            {
                isDictionary = false;
                return true;
            }

            if (type is IDeclaredType declaredType && IsGenericDictionaryWithReferenceTypeValues(declaredType))
            {
                isDictionary = true;
                return true;
            }
        }

        return false;
    }

    protected override string GetContractTextForUI(string contractIdentifier)
        => isDictionary
            ? $"{contractIdentifier}.{nameof(Enumerable.All)}(pair => pair.{nameof(KeyValuePair<int, int>.Value)} != null)"
            : $"{contractIdentifier}.{nameof(Enumerable.All)}(item => item != null)";

    protected override IExpression GetExpression(CSharpElementFactory factory, IExpression contractExpression)
    {
        var expression = isDictionary
            ? factory.CreateExpression(
                $"$0.{nameof(Enumerable.All)}(pair => pair.{nameof(KeyValuePair<int, int>.Value)} != null)",
                contractExpression)
            : factory.CreateExpression($"$0.{nameof(Enumerable.All)}(item => item != null)", contractExpression);

        var invokedExpression = (IReferenceExpression)((IInvocationExpression)expression).InvokedExpression;

        var allMethodReference = invokedExpression.Reference;

        var enumerableType = PredefinedType.ENUMERABLE_CLASS.TryGetTypeElement(Provider.PsiModule);
        Debug.Assert(enumerableType is { });

        var allMethod = enumerableType.Methods.First(method => method.ShortName == nameof(Enumerable.All));

        allMethodReference.BindTo(allMethod);

        return expression;
    }

    protected override string? TryGetAnnotationAttributeTypeName()
        => isDictionary ? null : ContainerElementNullnessProvider.ItemNotNullAttributeShortName;
}