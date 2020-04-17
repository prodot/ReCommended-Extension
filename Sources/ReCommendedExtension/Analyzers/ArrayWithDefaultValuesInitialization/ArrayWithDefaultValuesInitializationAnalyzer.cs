using System.Diagnostics;
using JetBrains.Annotations;
using JetBrains.Metadata.Reader.API;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.Analyzers.ArrayWithDefaultValuesInitialization
{
    [ElementProblemAnalyzer(typeof(IArrayInitializer), HighlightingTypes = new[] { typeof(ArrayWithDefaultValuesInitializationSuggestion) })]
    public sealed class ArrayWithDefaultValuesInitializationAnalyzer : ElementProblemAnalyzer<IArrayInitializer>
    {
        [NotNull]
        static string CreateHighlightingMessage([NotNull] IType arrayElementType, bool isNullableReferenceType, int elementCount)
        {
            Debug.Assert(CSharpLanguage.Instance != null);

            return string.Format(
                "Use 'new {0}{1}[{2}]'.",
                arrayElementType.GetPresentableName(CSharpLanguage.Instance),
                isNullableReferenceType ? "?" : "",
                elementCount.ToString());
        }

        protected override void Run(IArrayInitializer element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
        {
            if (element.InitializerElements.Count > 0)
            {
                IType arrayElementType;
                switch (element.Parent)
                {
                    case ITypeOwnerDeclaration declaration:
                        arrayElementType = declaration.Type.GetScalarType();

                        if (arrayElementType == null)
                        {
                            return;
                        }
                        break;

                    case IArrayCreationExpression creationExpression:
                        arrayElementType = creationExpression.GetElementType();
                        break;

                    default: return;
                }

                if (element.InitializerElements.All(
                    initializerElement => initializerElement?.FirstChild != null && initializerElement.FirstChild.IsDefaultValueOf(arrayElementType)))
                {
                    // { d, default, default(T) } // where d is the default value for the T

                    var isNullableReferenceType = element.IsNullableAnnotationsContextEnabled() &&
                        arrayElementType.Classify == TypeClassification.REFERENCE_TYPE &&
                        arrayElementType.NullableAnnotation == NullableAnnotation.NotAnnotated;

                    consumer.AddHighlighting(
                        new ArrayWithDefaultValuesInitializationSuggestion(
                            CreateHighlightingMessage(arrayElementType, isNullableReferenceType, element.InitializerElements.Count),
                            element,
                            isNullableReferenceType,
                            arrayElementType,
                            element.InitializerElements.Count));
                }
            }
        }
    }
}