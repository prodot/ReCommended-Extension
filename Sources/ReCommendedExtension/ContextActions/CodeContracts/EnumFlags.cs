using System.Text;
using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Feature.Services.CSharp.ContextActions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;
using ReCommendedExtension.ContextActions.CodeContracts.Internal;

namespace ReCommendedExtension.ContextActions.CodeContracts;

[ContextAction(
    GroupType = typeof(CSharpContextActions),
    Name = "Add contract: enum value has the valid flags" + ZoneMarker.Suffix,
    Description = "Adds a contract that the enum value has the valid flags.")]
public sealed class EnumFlags(ICSharpContextActionDataProvider provider) : AddContractContextAction(provider)
{
    internal abstract record EnumContractInfo
    {
        [Pure]
        public static EnumContractInfo? TryCreate(IEnum? enumType)
        {
            if (enumType is { } && enumType.HasAttributeInstance(PredefinedType.FLAGS_ATTRIBUTE_CLASS, false))
            {
                return CSharpNumericTypeInfo.TryCreate(enumType.GetUnderlyingType())?.TryCreateEnumFlags([..enumType.EnumMembers]);
            }

            return null;
        }

        [Pure]
        public abstract string GetContractTextForUI(string contractIdentifier);

        [Pure]
        public abstract IExpression GetExpression(CSharpElementFactory factory, IExpression contractExpression);
    }

    internal sealed record EnumContractInfo<E> : EnumContractInfo where E : struct
    {
        [Pure]
        static IEnumerable<E> Range(E min, E max, Func<E, E, bool> isLessOrEquals, Func<E, E> getMultipliedWithTwo)
        {
            for (var i = min; isLessOrEquals(i, max); i = getMultipliedWithTwo(i))
            {
                yield return i;
            }
        }

        [Pure]
        public static EnumContractInfo<E>? TryCreate(
            IField[] members,
            E one,
            Func<E, double> convertToDouble,
            Func<E, bool> isZero,
            Func<E, E, bool> isLessOrEquals,
            Func<E, E> getMultipliedWithTwo,
            string cSharpLiteralSuffix,
            Func<ConstantValue, E> extractConstantValue)
        {
            var valueMembers = new Dictionary<E, IField>();

            foreach (var member in members.WithoutObsolete())
            {
                if (member.ConstantValue.IsEnum())
                {
                    var t = extractConstantValue(member.ConstantValue.ToEnumUnderlyingType());
                    if (isLessOrEquals(t, default) && !isZero(t))
                    {
                        return null;
                    }

                    if (isZero(t))
                    {
                        valueMembers[t] = member;
                    }
                    else
                    {
                        var log = Math.Log(convertToDouble(t), 2);
                        if (Math.Abs(log - Math.Floor(log)) < double.Epsilon)
                        {
                            valueMembers[t] = member;
                        }
                    }
                }
            }

            if (valueMembers.Count > 0)
            {
                var max = valueMembers.Keys.Max();

                if (isZero(max))
                {
                    var zeroMember = valueMembers[default];
                    valueMembers.Remove(default);
                    return new EnumContractInfo<E>(zeroMember, valueMembers, cSharpLiteralSuffix);
                }

                if (Range(one, max, isLessOrEquals, getMultipliedWithTwo).All(valueMembers.ContainsKey))
                {
                    valueMembers.TryGetValue(default, out var zeroMember);
                    valueMembers.Remove(default);
                    return new EnumContractInfo<E>(zeroMember, valueMembers, cSharpLiteralSuffix);
                }
            }

            return null;
        }

        readonly IField? zeroMember;

        readonly Dictionary<E, IField> valueMembers;

        readonly string cSharpLiteralSuffix;

        EnumContractInfo(IField? zeroMember, Dictionary<E, IField> valueMembers, string cSharpLiteralSuffix)
        {
            Debug.Assert(!valueMembers.ContainsKey(default));
            Debug.Assert(zeroMember is { } || valueMembers.Count > 0);

            this.zeroMember = zeroMember;
            this.valueMembers = valueMembers;
            this.cSharpLiteralSuffix = cSharpLiteralSuffix;
        }

        public override string GetContractTextForUI(string contractIdentifier)
        {
            var zeroExpression = zeroMember?.ShortName ?? "0";

            if (valueMembers.Count == 0)
            {
                return $"{contractIdentifier} == {zeroExpression}";
            }

            const int maxItemsToShow = 3;

            var totalExpression = string.Join(
                " | ",
                from value in valueMembers.Keys.Take(maxItemsToShow) orderby value select valueMembers[value].ShortName);

            return string.Format(
                "{0} >= {1} && {0} <= {2}",
                contractIdentifier,
                zeroExpression,
                totalExpression + (valueMembers.Count > maxItemsToShow ? "..." : ""));
        }

        public override IExpression GetExpression(CSharpElementFactory factory, IExpression contractExpression)
        {
            var zeroExpression = zeroMember is { }
                ? factory.CreateExpression("$0", zeroMember)
                : factory.CreateExpression($"0{cSharpLiteralSuffix}");

            if (valueMembers.Count == 0)
            {
                return factory.CreateExpression("$0 == $1", contractExpression, zeroExpression);
            }

            var fields = new List<IField>(valueMembers.Count);
            fields.AddRange(from value in valueMembers.Keys orderby value select valueMembers[value]);

            var pattern = new StringBuilder("$0 >= $1 && $0 <= ");
            if (fields is [_, _, ..])
            {
                pattern.Append('(');
            }

            var args = new object[fields.Count + 2];
            args[0] = contractExpression;
            args[1] = zeroExpression;

            for (var i = 0; i < fields.Count; i++)
            {
                if (i > 0)
                {
                    pattern.Append(" | ");
                }

                var index = i + 2;

                pattern.Append($"${index.ToString()}");
                args[index] = fields[i];
            }
            if (fields is [_, _, ..])
            {
                pattern.Append(')');
            }

            return factory.CreateExpression(pattern.ToString(), args);
        }
    }

    EnumContractInfo? contractInfo;

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