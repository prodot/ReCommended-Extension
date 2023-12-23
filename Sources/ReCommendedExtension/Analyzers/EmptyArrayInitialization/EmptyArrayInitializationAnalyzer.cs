using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.Analyzers.EmptyArrayInitialization;

[ElementProblemAnalyzer(typeof(ICSharpTreeNode), HighlightingTypes = [typeof(EmptyArrayInitializationWarning)])]
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
        if (!ArrayEmptyMethodExists(element.GetPsiModule()))
        {
            return;
        }

        switch (element)
        {
            case IArrayInitializer { InitializerElements: [], Parent: ITypeOwnerDeclaration declaration } arrayInitializer:
            {
                if (declaration.Type.GetScalarType() is { } arrayElementType)
                {
                    // todo: check if R# suggests to use '[]' in this case (C# 12 only)

                    // T[] variable = { }; // variable or type field declaration with initialization
                    // T[] Property { get; } = { };
                    // T[] Property { get; set; } = { };

                    consumer.AddHighlighting(
                        new EmptyArrayInitializationWarning(CreateHighlightingMessage(arrayElementType), arrayInitializer, arrayElementType));
                }
                break;
            }

            // todo: check if R# suggests to use '[]' in this case (C# 12 only)
            // handled by R#: new T[0]
            // handled by R#: new T[0] { }

            case IArrayCreationExpression { Dimensions: [1], DimInits: [], ArrayInitializer.InitializerElements: [] } creationExpression:
                if (creationExpression.GetContainingNode<IAttribute>() is not { })
                {
                    // todo: check if R# suggests to use '[]' when target type is known (C# 12 only) or Array.Empty<T>()

                    // new T[] { }

                    var arrayElementType = creationExpression.GetElementType();

                    consumer.AddHighlighting(
                        new EmptyArrayInitializationWarning(
                            CreateHighlightingMessage(arrayElementType),
                            creationExpression,
                            arrayElementType));
                }
                break;
        }

        // todo: check if R# suggests to replace 'Array.Empty<T>()' with '[]' when target type is known and read-only (T[], IEnumerable<T>, IReadOnlyCollection<T>, IReadOnlyList<T>) (C# 12 only)
    }
}