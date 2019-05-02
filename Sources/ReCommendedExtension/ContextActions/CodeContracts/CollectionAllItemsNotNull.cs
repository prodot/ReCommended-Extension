using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Feature.Services.CSharp.Analyses.Bulbs;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;

namespace ReCommendedExtension.ContextActions.CodeContracts
{
    [ContextAction(
        Group = "C#",
        Name = "Add contract: all collection items are not null" + ZoneMarker.Suffix,
        Description = "Adds a contract that all collection items (or dictionary values) are not null.")]
    public sealed class CollectionAllItemsNotNull : AddContractContextAction
    {
        bool isDictionary;

        public CollectionAllItemsNotNull([NotNull] ICSharpContextActionDataProvider provider) : base(provider) { }

        protected override bool IsAvailableForType(IType type)
        {
            var context = Provider.SelectedElement;

            Debug.Assert(context != null);

            if ((type.IsCollectionLike() || type.IsGenericArray(context)) && !type.IsGenericIEnumerable() && !type.IsArray())
            {
                var elementType = CollectionTypeUtil.ElementTypeByCollectionType(type, context, false);

                if (elementType != null)
                {
                    if (elementType.Classify == TypeClassification.REFERENCE_TYPE)
                    {
                        isDictionary = false;
                        return true;
                    }

                    if (type is IDeclaredType declaredType &&
                        (declaredType.GetKeyValueTypesForGenericDictionary() ?? Enumerable.Empty<JetBrains.Util.Pair<IType, IType>>()).Any(
                            pair => pair.Second.Classify == TypeClassification.REFERENCE_TYPE))
                    {
                        isDictionary = true;
                        return true;
                    }
                }
            }

            return false;
        }

        protected override string GetContractTextForUI(string contractIdentifier)
            => isDictionary
                ? string.Format("{0}.{1}(pair => pair.{2} != null)", contractIdentifier, nameof(Enumerable.All), nameof(KeyValuePair<int, int>.Value))
                : string.Format("{0}.{1}(item => item != null)", contractIdentifier, nameof(Enumerable.All));

        protected override IExpression GetExpression(CSharpElementFactory factory, IExpression contractExpression)
        {
            var expression = isDictionary
                ? factory.CreateExpression(
                    string.Format("$0.{0}(pair => pair.{1} != null)", nameof(Enumerable.All), nameof(KeyValuePair<int, int>.Value)),
                    contractExpression)
                : factory.CreateExpression(string.Format("$0.{0}(item => item != null)", nameof(Enumerable.All)), contractExpression);

            var invokedExpression = (IReferenceExpression)((IInvocationExpression)expression).InvokedExpression;

            Debug.Assert(invokedExpression != null);

            var allMethodReference = invokedExpression.Reference;

            var enumerableType = TypeElementUtil.GetTypeElementByClrName(PredefinedType.ENUMERABLE_CLASS, Provider.PsiModule);

            Debug.Assert(enumerableType != null);

            var allMethod = enumerableType.Methods.First(method => method.AssertNotNull().ShortName == nameof(Enumerable.All));

            Debug.Assert(allMethod != null);

            allMethodReference.BindTo(allMethod);

            return expression;
        }

        protected override string TryGetAnnotationAttributeTypeName()
            => isDictionary ? null : ContainerElementNullnessProvider.ItemNotNullAttributeShortName;
    }
}