using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Feature.Services.CSharp.ContextActions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;

namespace ReCommendedExtension.ContextActions.CodeContracts;

[ContextAction(
    GroupType = typeof(CSharpContextActions),
    Name = "Add contract: collection is not empty" + ZoneMarker.Suffix,
    Description = "Adds a contract that the collection is not empty.")]
public sealed class CollectionCountPositive(ICSharpContextActionDataProvider provider) : AddContractContextAction(provider)
{
    bool isArray;

    protected override bool IsAvailableForType(IType type)
    {
        if (!type.IsGenericIEnumerable())
        {
            Debug.Assert(Provider.SelectedElement is { });

            if (type.IsGenericArray(Provider.SelectedElement))
            {
                // type is T[...]
                isArray = true;
                return true;
            }

            if (type.IsCollectionLike())
            {
                isArray = type.IsArray(); // true if type is "Array"
                return true;
            }
        }

        return false;
    }

    protected override string GetContractTextForUI(string contractIdentifier)
        => isArray ? $"{contractIdentifier}.{nameof(Array.Length)} > 0" : $"{contractIdentifier}.{nameof(ICollection<int>.Count)} > 0";

    protected override IExpression GetExpression(CSharpElementFactory factory, IExpression contractExpression)
        => isArray
            ? factory.CreateExpression($"$0.{nameof(Array.Length)} > 0", contractExpression)
            : factory.CreateExpression($"$0.{nameof(ICollection<int>.Count)} > 0", contractExpression);
}