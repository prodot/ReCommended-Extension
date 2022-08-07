using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Feature.Services.CSharp.ContextActions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;
using ReCommendedExtension.ContextActions.CodeContracts.Internal;

namespace ReCommendedExtension.ContextActions.CodeContracts
{
    [ContextAction(
        Group = "C#",
        Name = "Add contract: enum value is within the valid enum range" + ZoneMarker.Suffix,
        Description = "Adds a contract that the enum value is within the valid enum range.")]
    public sealed class EnumBetweenFirstAndLast : AddContractContextAction
    {
        internal abstract class EnumContractInfo
        {
            [CanBeNull]
            public static EnumContractInfo TryCreate([CanBeNull] IEnum enumType)
            {
                if (enumType != null && !enumType.HasAttributeInstance(PredefinedType.FLAGS_ATTRIBUTE_CLASS, false))
                {
                    return CSharpNumericTypeInfo.TryCreate(enumType.GetUnderlyingType())
                        ?.TryCreateEnumContractInfoForEnumBetweenFirstAndLast(enumType.EnumMembers.ToList());
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
            static IEnumerable<E> Range(E min, E max, [NotNull] Func<E, E, bool> isLessOrEquals, [NotNull] Func<E, E> getNext)
            {
                for (var i = min; isLessOrEquals(i, max); i = getNext(i))
                {
                    yield return i;
                }
            }

            [CanBeNull]
            public static EnumContractInfo<E> TryCreate(
                [NotNull][ItemNotNull] IList<IField> members,
                [NotNull] Func<E, E, bool> isLessOrEquals,
                [NotNull] Func<E, E> getNext,
                [NotNull] Func<ConstantValue, E> extractConstantValue)
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

            [NotNull]
            readonly Dictionary<E, IField> valueMembers;

            readonly E min;
            readonly E max;

            EnumContractInfo([NotNull] Dictionary<E, IField> valueMembers, E min, E max)
            {
                Debug.Assert(valueMembers.Count > 1);

                this.valueMembers = valueMembers;
                this.min = min;
                this.max = max;
            }

            public override string GetContractTextForUI(string contractIdentifier)
            {
                Debug.Assert(valueMembers[min] != null);
                Debug.Assert(valueMembers[max] != null);

                return string.Format("{0} >= {1} && {0} <= {2}", contractIdentifier, valueMembers[min].ShortName, valueMembers[max].ShortName);
            }

            public override IExpression GetExpression(CSharpElementFactory factory, IExpression contractExpression)
                => factory.CreateExpression("$0 >= $1 && $0 <= $2", contractExpression, valueMembers[min], valueMembers[max]);
        }

        [CanBeNull]
        EnumContractInfo contractInfo;

        public EnumBetweenFirstAndLast([NotNull] ICSharpContextActionDataProvider provider) : base(provider) { }

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