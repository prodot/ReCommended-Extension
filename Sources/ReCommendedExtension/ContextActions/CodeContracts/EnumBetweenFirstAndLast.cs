using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Feature.Services.CSharp.ContextActions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;
using ReCommendedExtension.ContextActions.CodeContracts.Internal;

namespace ReCommendedExtension.ContextActions.CodeContracts;

[ContextAction(
    Group = "C#",
    Name = "Add contract: enum value is within the valid enum range" + ZoneMarker.Suffix,
    Description = "Adds a contract that the enum value is within the valid enum range.")]
public sealed class EnumBetweenFirstAndLast : AddContractContextAction
{
    internal abstract record EnumContractInfo
    {
        public static EnumContractInfo? TryCreate(IEnum? enumType)
        {
            if (enumType is { } && !enumType.HasAttributeInstance(PredefinedType.FLAGS_ATTRIBUTE_CLASS, false))
            {
                return CSharpNumericTypeInfo
                    .TryCreate(enumType.GetUnderlyingType())
                    ?.TryCreateEnumContractInfoForEnumBetweenFirstAndLast(enumType.EnumMembers.ToList());
            }

            return null;
        }

        public abstract string GetContractTextForUI(string contractIdentifier);

        public abstract IExpression GetExpression(CSharpElementFactory factory, IExpression contractExpression);
    }

    internal sealed record EnumContractInfo<E> : EnumContractInfo where E : struct
    {
        static IEnumerable<E> Range(E min, E max, Func<E, E, bool> isLessOrEquals, Func<E, E> getNext)
        {
            for (var i = min; isLessOrEquals(i, max); i = getNext(i))
            {
                yield return i;
            }
        }

        public static EnumContractInfo<E>? TryCreate(
            IList<IField> members,
            Func<E, E, bool> isLessOrEquals,
            Func<E, E> getNext,
            Func<ConstantValue, E> extractConstantValue)
        {
            var valueMembers = new Dictionary<E, IField>();

            foreach (var field in members.WithoutObsolete())
            {
                if (field.ConstantValue.IsEnum())
                {
                    valueMembers[extractConstantValue(field.ConstantValue.ToEnumUnderlyingType())] = field;
                }
            }

            if (valueMembers.Count > 1)
            {
                var min = valueMembers.Keys.Min();
                var max = valueMembers.Keys.Max();
                if (Range(min, max, isLessOrEquals, getNext).All(valueMembers.ContainsKey))
                {
                    return new EnumContractInfo<E>(valueMembers, min, max);
                }
            }

            return null;
        }

        readonly Dictionary<E, IField> valueMembers;

        readonly E min;
        readonly E max;

        EnumContractInfo(Dictionary<E, IField> valueMembers, E min, E max)
        {
            Debug.Assert(valueMembers.Count > 1);

            this.valueMembers = valueMembers;
            this.min = min;
            this.max = max;
        }

        public override string GetContractTextForUI(string contractIdentifier)
            => $"{contractIdentifier} >= {valueMembers[min].ShortName} && {contractIdentifier} <= {valueMembers[max].ShortName}";

        public override IExpression GetExpression(CSharpElementFactory factory, IExpression contractExpression)
            => factory.CreateExpression("$0 >= $1 && $0 <= $2", contractExpression, valueMembers[min], valueMembers[max]);
    }

    EnumContractInfo? contractInfo;

    public EnumBetweenFirstAndLast(ICSharpContextActionDataProvider provider) : base(provider) { }

    [MemberNotNullWhen(true, nameof(contractInfo))]
    protected override bool IsAvailableForType(IType type)
    {
        contractInfo = EnumContractInfo.TryCreate(type.GetEnumType());

        return contractInfo is { };
    }

    protected override string GetContractTextForUI(string contractIdentifier)
    {
        Debug.Assert(contractInfo is { });

        return contractInfo.GetContractTextForUI(contractIdentifier);
    }

    protected override IExpression GetExpression(CSharpElementFactory factory, IExpression contractExpression)
    {
        Debug.Assert(contractInfo is { });

        return contractInfo.GetExpression(factory, contractExpression);
    }
}