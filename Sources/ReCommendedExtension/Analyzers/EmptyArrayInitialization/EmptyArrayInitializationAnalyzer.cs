using System;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;

namespace ReCommendedExtension.Analyzers.EmptyArrayInitialization
{
    [ElementProblemAnalyzer(typeof(ICSharpTreeNode), HighlightingTypes = new[] { typeof(EmptyArrayInitializationWarning) })]
    public sealed class EmptyArrayInitializationAnalyzer : ElementProblemAnalyzer<ICSharpTreeNode>
    {
        [Pure]
        static bool ArrayEmptyMethodExists([NotNull] IPsiModule psiModule)
        {
            var arrayType = TryGetArrayType(psiModule);
            return arrayType != null &&
                arrayType.Methods.Any(method => method.IsStatic && method.ShortName == nameof(Array.Empty) && method.Parameters.Count == 0);
        }

        [Pure]
        [CanBeNull]
        internal static ITypeElement TryGetArrayType([NotNull] IPsiModule psiModule)
            => TypeElementUtil.GetTypeElementByClrName(PredefinedType.ARRAY_FQN, psiModule);

        [NotNull]
        static string CreateHighlightingMessage([NotNull] IType arrayElementType)
        {
            Debug.Assert(CSharpLanguage.Instance != null);

            return string.Format(
                "Use '{0}.{1}<{2}>()'.",
                nameof(Array),
                nameof(Array.Empty),
                arrayElementType.GetPresentableName(CSharpLanguage.Instance));
        }

        protected override void Run(ICSharpTreeNode element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
        {
            if (!ArrayEmptyMethodExists(element.GetPsiModule()))
            {
                return;
            }

            switch (element)
            {
                case IArrayInitializer arrayInitializer:
                    if (arrayInitializer.InitializerElements.Count == 0 && arrayInitializer.Parent is ITypeOwnerDeclaration declaration)
                    {
                        var arrayElementType = declaration.Type.GetScalarType();

                        if (arrayElementType != null)
                        {
                            // T[] variable = { }; // variable or type field declaration with initialization
                            // T[] Property { get; } = { };
                            // T[] Property { get; set; } = { };

                            consumer.AddHighlighting(
                                new EmptyArrayInitializationWarning(CreateHighlightingMessage(arrayElementType), arrayInitializer, arrayElementType));
                        }
                    }
                    break;

                case IArrayCreationExpression creationExpression:
                    if (creationExpression.GetContainingNode<IAttribute>() == null)
                    {
                        var dimensions = creationExpression.Dimensions;
                        if (dimensions.Length == 1 && dimensions[0] == 1)
                        {
                            var arrayElementType = creationExpression.GetElementType();

                            if (creationExpression.DimInits.Count == 0 && creationExpression.ArrayInitializer?.InitializerElements.Count == 0)
                            {
                                // new T[] { }

                                consumer.AddHighlighting(
                                    new EmptyArrayInitializationWarning(
                                        CreateHighlightingMessage(arrayElementType),
                                        creationExpression,
                                        arrayElementType));
                            }

                            if (creationExpression.DimInits.Count == 1 &&
                                creationExpression.DimInits[0] != null &&
                                creationExpression.DimInits[0].Type().IsInt() &&
                                creationExpression.DimInits[0].IsDefaultValueOf(creationExpression.DimInits[0].Type()) &&
                                (creationExpression.ArrayInitializer == null || creationExpression.ArrayInitializer.InitializerElements.Count == 0))
                            {
                                // new T[0]
                                // new T[0] { }

                                consumer.AddHighlighting(
                                    new EmptyArrayInitializationWarning(
                                        CreateHighlightingMessage(arrayElementType),
                                        creationExpression,
                                        arrayElementType));
                            }
                        }
                    }
                    break;
            }
        }
    }
}