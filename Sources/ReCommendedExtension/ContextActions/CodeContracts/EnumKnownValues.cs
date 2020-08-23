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

namespace ReCommendedExtension.ContextActions.CodeContracts
{
    [ContextAction(
        Group = "C#",
        Name = "Add contract: enum value has valid values" + ZoneMarker.Suffix,
        Description = "Adds a contract that the enum value has the valid values.")]
    public sealed class EnumKnownValues : AddContractContextAction
    {
        [CanBeNull]
        IList<IField> members;

        public EnumKnownValues([NotNull] ICSharpContextActionDataProvider provider) : base(provider) { }

        protected override bool IsAvailableForType(IType type)
        {
            var enumType = type.GetEnumType();

            if (enumType != null && !enumType.HasAttributeInstance(PredefinedType.FLAGS_ATTRIBUTE_CLASS, false))
            {
                Debug.Assert(enumType.EnumMembers != null);

                members = enumType.EnumMembers.WithoutObsolete().ToList();

                if (members.Count > 0)
                {
                    return true;
                }

                members = null;
            }

            return false;
        }

        protected override string GetContractTextForUI(string contractIdentifier)
        {
            Debug.Assert(members != null);

            const int maxItemsToShow = 3;

            return string.Join(
                    " || ",
                    from field in members.Take(maxItemsToShow) select $"{contractIdentifier} == {field.AssertNotNull().ShortName}") +
                (members.Count > maxItemsToShow ? "..." : "");
        }

        protected override IExpression GetExpression(CSharpElementFactory factory, IExpression contractExpression)
        {
            Debug.Assert(members != null);

            var pattern = new StringBuilder();

            var args = new object[members.Count + 1];
            args[0] = contractExpression;

            for (var i = 0; i < members.Count; i++)
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
}