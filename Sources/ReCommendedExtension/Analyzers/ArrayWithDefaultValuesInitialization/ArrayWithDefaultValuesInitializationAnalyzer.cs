using System.Diagnostics;
using System.Text;
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
        static string CreateHighlightingMessage([NotNull] string suggestedCode) => string.Format("Use '{0}'.", suggestedCode);

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

                    var builder = new StringBuilder();
                    builder.Append("new ");

                    Debug.Assert(CSharpLanguage.Instance != null);

                    builder.Append(arrayElementType.GetPresentableName(CSharpLanguage.Instance));

                    if (builder[builder.Length - 1] != '?')
                    {
                        var isNullableReferenceType = element.IsNullableAnnotationsContextEnabled() &&
                            arrayElementType.Classify == TypeClassification.REFERENCE_TYPE &&
                            arrayElementType.NullableAnnotation == NullableAnnotation.NotAnnotated;

                        if (isNullableReferenceType)
                        {
                            builder.Append('?');
                        }
                    }
                    else
                    {
                        // workaround for R# 2020.2

                        if (element.IsNullableAnnotationsContextEnabled())
                        {
                            switch (arrayElementType.Classify)
                            {
                                case TypeClassification.UNKNOWN:
                                case TypeClassification.VALUE_TYPE when !arrayElementType.IsNullable():
                                    builder.Remove(builder.Length - 1, 1);
                                    break;
                            }
                        }
                    }

                    builder.Append('[');
                    builder.Append(element.InitializerElements.Count);
                    builder.Append(']');

                    var suggestedCode = builder.ToString();

                    consumer.AddHighlighting(
                        new ArrayWithDefaultValuesInitializationSuggestion(CreateHighlightingMessage(suggestedCode), suggestedCode, element));
                }
            }
        }
    }
}