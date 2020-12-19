using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Feature.Services.CSharp.Analyses.Bulbs;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;
using ReCommendedExtension.ContextActions.CodeContracts.Internal;

namespace ReCommendedExtension.ContextActions.CodeContracts
{
    [ContextAction(
        Group = "C#",
        Name = "Add contract: enum value has the valid flags" + ZoneMarker.Suffix,
        Description = "Adds a contract that the enum value has the valid flags.")]
    public sealed class EnumFlags : AddContractContextAction
    {
        internal abstract class EnumContractInfo
        {
            [CanBeNull]
            public static EnumContractInfo TryCreate([CanBeNull] IEnum enumType)
            {
                if (enumType != null && enumType.HasAttributeInstance(PredefinedType.FLAGS_ATTRIBUTE_CLASS, false))
                {
                    return CSharpNumericTypeInfo.TryCreate(enumType.GetUnderlyingType())?.TryCreateEnumFlags(enumType.EnumMembers.ToList());
                }

                return null;
            }

            [NotNull]
            public abstract string GetContractTextForUI([NotNull] string contractIdentifier);

            [NotNull]
            public abstract IExpression GetExpression([NotNull] CSharpElementFactory factory, [NotNull] IExpression contractExpression);
        }

        internal sealed class EnumContractInfo<E> : EnumContractInfo where E : struct
        {
            [NotNull]
            static IEnumerable<E> Range(E min, E max, [NotNull] Func<E, E, bool> isLessOrEquals, [NotNull] Func<E, E> getMultipliedWithTwo)
            {
                for (var i = min; isLessOrEquals(i, max); i = getMultipliedWithTwo(i))
                {
                    yield return i;
                }
            }

            [CanBeNull]
            public static EnumContractInfo<E> TryCreate(
                [NotNull][ItemNotNull] IList<IField> members,
                E one,
                [NotNull] Func<E, double> convertToDouble,
                [NotNull] Func<E, bool> isZero,
                [NotNull] Func<E, E, bool> isLessOrEquals,
                [NotNull] Func<E, E> getMultipliedWithTwo,
                [NotNull] string cSharpLiteralSuffix)
            {
                var valueMembers = new Dictionary<E, IField>();
                foreach (var member in members.WithoutObsolete())
                {
                    var value = member.ConstantValue.Value;
                    if (value != null)
                    {
                        var t = (E)value;
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

            [CanBeNull]
            readonly IField zeroMember;

            [NotNull]
            readonly Dictionary<E, IField> valueMembers;

            [NotNull]
            readonly string cSharpLiteralSuffix;

            EnumContractInfo([CanBeNull] IField zeroMember, [NotNull] Dictionary<E, IField> valueMembers, [NotNull] string cSharpLiteralSuffix)
            {
                Debug.Assert(!valueMembers.ContainsKey(default));
                Debug.Assert(zeroMember != null || valueMembers.Count > 0);

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
                    from value in valueMembers.Keys.Take(maxItemsToShow) orderby value select valueMembers[value].AssertNotNull().ShortName);

                return string.Format(
                    "{0} >= {1} && {0} <= {2}",
                    contractIdentifier,
                    zeroExpression,
                    totalExpression + (valueMembers.Count > maxItemsToShow ? "..." : ""));
            }

            public override IExpression GetExpression(CSharpElementFactory factory, IExpression contractExpression)
            {
                var zeroExpression = zeroMember != null
                    ? factory.CreateExpression("$0", zeroMember)
                    : factory.CreateExpression(string.Format("0{0}", cSharpLiteralSuffix));

                if (valueMembers.Count == 0)
                {
                    return factory.CreateExpression("$0 == $1", contractExpression, zeroExpression);
                }

                var fields = new List<IField>(valueMembers.Count);
                fields.AddRange(from value in valueMembers.Keys orderby value select valueMembers[value]);

                var pattern = new StringBuilder("$0 >= $1 && $0 <= ");
                if (fields.Count > 1)
                {
                    pattern.Append("(");
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
                if (fields.Count > 1)
                {
                    pattern.Append(")");
                }

                return factory.CreateExpression(pattern.ToString(), args);
            }
        }

        [CanBeNull]
        EnumContractInfo contractInfo;

        public EnumFlags([NotNull] ICSharpContextActionDataProvider provider) : base(provider) { }

        protected override bool IsAvailableForType(IType type)
        {
            contractInfo = EnumContractInfo.TryCreate(type.GetEnumType());

            return contractInfo != null;
        }

        protected override string GetContractTextForUI(string contractIdentifier)
        {
            Debug.Assert(contractInfo != null);

            return contractInfo.GetContractTextForUI(contractIdentifier);
        }

        protected override IExpression GetExpression(CSharpElementFactory factory, IExpression contractExpression)
        {
            Debug.Assert(contractInfo != null);

            return contractInfo.GetExpression(factory, contractExpression);
        }
    }
}