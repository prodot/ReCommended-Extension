using System.Text;
using JetBrains.Metadata.Reader.API;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.Analyzers.ArrayWithDefaultValuesInitialization;

[ElementProblemAnalyzer(typeof(IArrayInitializer), HighlightingTypes = new[] { typeof(ArrayWithDefaultValuesInitializationSuggestion) })]
public sealed class ArrayWithDefaultValuesInitializationAnalyzer : ElementProblemAnalyzer<IArrayInitializer>
{
    static string CreateHighlightingMessage(string suggestedCode) => $"Use '{suggestedCode}'.";

    protected override void Run(IArrayInitializer element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
    {
        if (element.InitializerElements is not [])
        {
            IType arrayElementType;
            switch (element.Parent)
            {
                case ITypeOwnerDeclaration declaration:
                    if (declaration.Type.GetScalarType() is { } type)
                    {
                        arrayElementType = type;
                        break;
                    }

                    return;

                case IArrayCreationExpression creationExpression:
                    arrayElementType = creationExpression.GetElementType();
                    break;

                default: return;
            }

            if (element.InitializerElements.All(
                initializerElement => initializerElement is { FirstChild: { } firstChild } && firstChild.IsDefaultValueOf(arrayElementType)))
            {
                // { d, default, default(T) } // where d is the default value for the T

                var builder = new StringBuilder();
                builder.Append("new ");

                Debug.Assert(CSharpLanguage.Instance is { });

                builder.Append(arrayElementType.GetPresentableName(CSharpLanguage.Instance));

                if (builder is [.., not '?'])
                {
                    var isNullableReferenceType = element.IsNullableAnnotationsContextEnabled()
                        && arrayElementType is { Classify: TypeClassification.REFERENCE_TYPE, NullableAnnotation: NullableAnnotation.NotAnnotated };

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