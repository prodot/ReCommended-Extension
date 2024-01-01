using System.Text;
using JetBrains.Metadata.Reader.API;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.Analyzers.Collection;

[ElementProblemAnalyzer(
    typeof(ICSharpTreeNode),
    HighlightingTypes =
    [
        typeof(UseEmptyForArrayInitializationWarning), typeof(UseCollectionExpressionForEmptyInitializationWarning),
        typeof(ArrayWithDefaultValuesInitializationSuggestion),
    ])]
public sealed class CollectionAnalyzer : ElementProblemAnalyzer<ICSharpTreeNode>
{
    [Pure]
    static bool ArrayEmptyMethodExists(IPsiModule psiModule)
        => PredefinedType.ARRAY_FQN.TryGetTypeElement(psiModule) is { } arrayType
            && arrayType.Methods.Any(
                method => method is
                {
                    ShortName: nameof(Array.Empty),
                    IsStatic: true,
                    AccessibilityDomain.DomainType: AccessibilityDomain.AccessibilityDomainType.PUBLIC,
                    TypeParameters: [_],
                    Parameters: [],
                });

    [Pure]
    static IType? TryGetArrayElementType(IArrayInitializer arrayInitializer)
        => arrayInitializer.Parent switch
        {
            ITypeOwnerDeclaration declaration when declaration.Type.GetScalarType() is { } type => type,
            IArrayCreationExpression arrayCreationExpression => arrayCreationExpression.GetElementType(),

            _ => null,
        };

    [Pure]
    static string BuildArrayCreationExpressionCode(IArrayInitializer arrayInitializer, IType arrayElementType)
    {
        Debug.Assert(arrayInitializer.InitializerElements is [_, ..]);

        var builder = new StringBuilder();
        builder.Append("new ");

        Debug.Assert(CSharpLanguage.Instance is { });

        builder.Append(arrayElementType.GetPresentableName(CSharpLanguage.Instance));

        if (builder is [.., not '?'])
        {
            var isNullableReferenceType = arrayInitializer.IsNullableAnnotationsContextEnabled()
                && arrayElementType is { Classify: TypeClassification.REFERENCE_TYPE, NullableAnnotation: NullableAnnotation.NotAnnotated };

            if (isNullableReferenceType)
            {
                builder.Append('?');
            }
        }
        else
        {
            // workaround for R# 2020.2

            if (arrayInitializer.IsNullableAnnotationsContextEnabled())
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
        builder.Append(arrayInitializer.InitializerElements.Count);
        builder.Append(']');

        return builder.ToString();
    }

    protected override void Run(ICSharpTreeNode element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
    {
        if (element.GetCSharpLanguageLevel() >= CSharpLanguageLevel.CSharp120)
        {
            // covered by R#: (target-typed)                      T[] variable = { };                ->  T[] variable = [];
            // covered by R#: (target-typed)                      T[] Property { get; } = { };       ->  T[] Property { get; } = [];
            // covered by R#: (target-typed)                      T[] Property { get; set; } = { };  ->  T[] Property { get; set; } = [];
            // covered by R# (target-typed and non-target-typed): new T[0] { }                       ->  []
            // covered by R# (non-target-typed):                  new T[0]                           ->  []
            // NOT covered by R# (target-typed):                  new T[0]                           ->  []

            switch (element)
            {
                case IArrayInitializer { InitializerElements: [_, ..] } arrayInitializer
                    when TryGetArrayElementType(arrayInitializer) is { } arrayElementType
                    && arrayInitializer.InitializerElements.All(
                        item => item is { FirstChild: { } firstChild } && firstChild.IsDefaultValueOf(arrayElementType)):
                {
                    // { d, default, default(T) }  ->  new T[n] // where d is the default value for the T

                    var arrayCreationExpressionCode = BuildArrayCreationExpressionCode(arrayInitializer, arrayElementType);

                    consumer.AddHighlighting(
                        new ArrayWithDefaultValuesInitializationSuggestion(
                            $"Use '{arrayCreationExpressionCode}'.",
                            arrayCreationExpressionCode,
                            arrayInitializer));
                    break;
                }

                case IArrayCreationExpression
                {
                    Dimensions: [1], DimInits: [{ ConstantValue: { Kind: ConstantValueKind.Int, IntValue: 0 } }], ArrayInitializer: not { },
                } creationExpression when TypeEqualityComparer.Default.Equals(
                    creationExpression.Type(),
                    creationExpression.GetImplicitlyConvertedTo()):
                {
                    // new T[0]  ->  []

                    consumer.AddHighlighting(
                        new UseCollectionExpressionForEmptyInitializationWarning("Use collection expression.", creationExpression));
                    break;
                }
            }

            // todo: check if R# suggests to replace 'Array.Empty<T>()' with '[]' when target type is known and read-only (T[], IEnumerable<T>, IReadOnlyCollection<T>, IReadOnlyList<T>)
        }
        else
        {
            // covered by R#:     new T[0] { }                       ->  Array.Empty<T>()
            // NOT covered by R#: T[] variable = { };                ->  T[] variable = Array.Empty<T>();
            // NOT covered by R#: T[] Property { get; } = { };       ->  T[] Property { get; } = Array.Empty<T>();
            // NOT covered by R#: T[] Property { get; set; } = { };  ->  T[] Property { get; set; } = Array.Empty<T>();

            switch (element)
            {
                case IArrayInitializer { InitializerElements: not { } or [], Parent: ITypeOwnerDeclaration declaration } arrayInitializer
                    when declaration.Type.GetScalarType() is { } arrayElementType && ArrayEmptyMethodExists(element.GetPsiModule()):
                {
                    // T[] variable = { };                ->  T[] variable = Array.Empty<T>();
                    // T[] Property { get; } = { };       ->  T[] Property { get; } = Array.Empty<T>();
                    // T[] Property { get; set; } = { };  ->  T[] Property { get; set; } = Array.Empty<T>();

                    Debug.Assert(CSharpLanguage.Instance is { });

                    consumer.AddHighlighting(
                        new UseEmptyForArrayInitializationWarning(
                            $"Use '{nameof(Array)}.{nameof(Array.Empty)}<{arrayElementType.GetPresentableName(CSharpLanguage.Instance)}>()'.",
                            arrayInitializer,
                            arrayElementType));
                    break;
                }

                case IArrayInitializer { InitializerElements: [_, ..] } arrayInitializer
                    when TryGetArrayElementType(arrayInitializer) is { } arrayElementType
                    && arrayInitializer.InitializerElements.All(
                        item => item is { FirstChild: { } firstChild } && firstChild.IsDefaultValueOf(arrayElementType)):
                {
                    // { d, default, default(T) }  ->  new T[n] // where d is the default value for the T

                    var arrayCreationExpressionCode = BuildArrayCreationExpressionCode(arrayInitializer, arrayElementType);

                    consumer.AddHighlighting(
                        new ArrayWithDefaultValuesInitializationSuggestion(
                            $"Use '{arrayCreationExpressionCode}'.",
                            arrayCreationExpressionCode,
                            arrayInitializer));
                    break;
                }

                case IArrayCreationExpression
                    {
                        Dimensions: [1], DimInits: [], ArrayInitializer.InitializerElements: not { } or [],
                    } creationExpression when creationExpression.GetContainingNode<IAttribute>() is not { }
                    && ArrayEmptyMethodExists(element.GetPsiModule()):
                {
                    // new T[] { }  ->  Array.Empty<T>();

                    var arrayElementType = creationExpression.GetElementType();

                    Debug.Assert(CSharpLanguage.Instance is { });

                    consumer.AddHighlighting(
                        new UseEmptyForArrayInitializationWarning(
                            $"Use '{nameof(Array)}.{nameof(Array.Empty)}<{arrayElementType.GetPresentableName(CSharpLanguage.Instance)}>()'.",
                            creationExpression,
                            arrayElementType));
                    break;
                }
            }
        }
    }
}