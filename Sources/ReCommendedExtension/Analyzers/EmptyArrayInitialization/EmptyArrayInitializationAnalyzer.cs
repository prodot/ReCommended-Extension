using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.Analyzers.EmptyArrayInitialization;

[ElementProblemAnalyzer(
    typeof(ICSharpTreeNode),
    HighlightingTypes = [typeof(UseEmptyForArrayInitializationWarning), typeof(UseCollectionExpressionForEmptyInitializationWarning)])]
public sealed class EmptyArrayInitializationAnalyzer : ElementProblemAnalyzer<ICSharpTreeNode>
{
    [Pure]
    static bool ArrayEmptyMethodExists(IPsiModule psiModule)
        => PredefinedType.ARRAY_FQN.TryGetTypeElement(psiModule) is { } arrayType
            && arrayType.Methods.Any(method => method is { IsStatic: true, ShortName: nameof(Array.Empty), TypeParameters: [_], Parameters: [] });

    [Pure]
    static string CreateHighlightingMessage(IType arrayElementType)
    {
        Debug.Assert(CSharpLanguage.Instance is { });

        return $"Use '{nameof(Array)}.{nameof(Array.Empty)}<{arrayElementType.GetPresentableName(CSharpLanguage.Instance)}>()'.";
    }

    protected override void Run(ICSharpTreeNode element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
    {
        if (element.GetCSharpLanguageLevel() >= CSharpLanguageLevel.CSharp120)
        {
            // covered by R#: (target-typed)                      T[] variable = { };
            // covered by R#: (target-typed)                      T[] Property { get; } = { };
            // covered by R#: (target-typed)                      T[] Property { get; set; } = { };
            // covered by R# (target-typed and non-target-typed): new T[0] { }
            // covered by R# (non-target-typed):                  new T[0]
            // NOT covered by R# (target-typed):                  new T[0]

            switch (element)
            {
                case IArrayCreationExpression
                {
                    Dimensions: [1], DimInits: [{ ConstantValue: { Kind: ConstantValueKind.Int, IntValue: 0 } }], ArrayInitializer: not { },
                } creationExpression when TypeEqualityComparer.Default.Equals(
                    creationExpression.Type(),
                    creationExpression.GetImplicitlyConvertedTo()):
                {
                    // new T[0]

                    consumer.AddHighlighting(
                        new UseCollectionExpressionForEmptyInitializationWarning("Use collection expression.", creationExpression));
                    break;
                }
            }
        }
        else
        {
            if (!ArrayEmptyMethodExists(element.GetPsiModule()))
            {
                return;
            }

            // covered by R#:     new T[0] { }
            // NOT covered by R#: T[] variable = { };
            // NOT covered by R#: T[] Property { get; } = { };
            // NOT covered by R#: T[] Property { get; set; } = { };

            switch (element)
            {
                case IArrayInitializer { InitializerElements: [], Parent: ITypeOwnerDeclaration declaration } arrayInitializer
                    when declaration.Type.GetScalarType() is { } arrayElementType:
                {
                    // T[] variable = { }; // variable or type field declaration with initialization
                    // T[] Property { get; } = { };
                    // T[] Property { get; set; } = { };

                    consumer.AddHighlighting(
                        new UseEmptyForArrayInitializationWarning(CreateHighlightingMessage(arrayElementType), arrayInitializer, arrayElementType));
                    break;
                }

                case IArrayCreationExpression { Dimensions: [1], DimInits: [], ArrayInitializer.InitializerElements: [] } creationExpression
                    when creationExpression.GetContainingNode<IAttribute>() is not { }:
                {
                    // new T[] { }

                    var arrayElementType = creationExpression.GetElementType();

                    consumer.AddHighlighting(
                        new UseEmptyForArrayInitializationWarning(CreateHighlightingMessage(arrayElementType), creationExpression, arrayElementType));
                    break;
                }
            }
        }

        // todo: check if R# suggests to replace 'Array.Empty<T>()' with '[]' when target type is known and read-only (T[], IEnumerable<T>, IReadOnlyCollection<T>, IReadOnlyList<T>) (C# 12 only)
    }
}