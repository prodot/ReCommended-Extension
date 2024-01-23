using System.Text;
using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Feature.Services.CSharp.ContextActions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;

namespace ReCommendedExtension.ContextActions.CodeContracts;

[ContextAction(
    Group = "C#",
    Name = "Add contract: enum value has valid values" + ZoneMarker.Suffix,
    Description = "Adds a contract that the enum value has the valid values.")]
public sealed class EnumKnownValues(ICSharpContextActionDataProvider provider) : AddContractContextAction(provider)
{
    IField[]? members;

    [MemberNotNullWhen(true, nameof(members))]
    protected override bool IsAvailableForType(IType type)
    {
        var enumType = type.GetEnumType();

        if (enumType is { } && !enumType.HasAttributeInstance(PredefinedType.FLAGS_ATTRIBUTE_CLASS, false))
        {
            members = [..enumType.EnumMembers.WithoutObsolete()];

            if (members is not [])
            {
                return true;
            }

            members = null;
        }

        return false;
    }

    protected override string GetContractTextForUI(string contractIdentifier)
    {
        Debug.Assert(members is { });

        const int maxItemsToShow = 3;

        return string.Join(" || ", from field in members.Take(maxItemsToShow) select $"{contractIdentifier} == {field.ShortName}")
            + (members.Length > maxItemsToShow ? "..." : "");
    }

    protected override IExpression GetExpression(CSharpElementFactory factory, IExpression contractExpression)
    {
        Debug.Assert(members is { });

        var pattern = new StringBuilder();

        var args = new object[members.Length + 1];
        args[0] = contractExpression;

        for (var i = 0; i < members.Length; i++)
        {
            if (i > 0)
            {
                pattern.Append(" || ");
            }

            var index = i + 1;

            pattern.Append($"$0 == ${index.ToString()}");
            args[index] = members[i];
        }

        return factory.CreateExpression(pattern.ToString(), args);
    }
}