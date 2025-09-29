using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Impl.ControlFlow.NullableAnalysis.Runner;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Util;
using ReCommendedExtension.Analyzers.BaseTypes.NumberInfos;
using ReCommendedExtension.Extensions;
using ReCommendedExtension.Extensions.MethodFinding;

namespace ReCommendedExtension.Analyzers.BaseTypes.Analyzers;

/// <remarks>
/// C# language version checks are only done when a quick fix would require it.
/// </remarks>
[ElementProblemAnalyzer(typeof(IInvocationExpression), HighlightingTypes = [typeof(UseExpressionResultSuggestion), typeof(RedundantArgumentHint)])]
public sealed class RandomAnalyzer(NullableReferenceTypesDataFlowAnalysisRunSynchronizer nullableReferenceTypesDataFlowAnalysisRunSynchronizer)
    : ElementProblemAnalyzer<IInvocationExpression>
{
    static class ParameterTypes
    {
        public static IReadOnlyList<Func<IType, bool>> Int32 { get; } = [t => t.IsInt()];

        public static IReadOnlyList<Func<IType, bool>> Int64 { get; } = [t => t.IsLong()];
    }

    [Pure]
    static string CreateEmptyArray(IType itemType, IInvocationExpression context)
    {
        if (context.GetCSharpLanguageLevel() >= CSharpLanguageLevel.CSharp120 && context.TryGetTargetType(true) is { })
        {
            return "[]";
        }

        Debug.Assert(CSharpLanguage.Instance is { });

        var type = itemType.GetPresentableName(CSharpLanguage.Instance);

        if (PredefinedType.ARRAY_FQN.HasMethod(
            new MethodSignature { Name = nameof(Array.Empty), ParameterTypes = [], GenericParametersCount = 1, IsStatic = true },
            context.GetPsiModule()))
        {
            return $"{nameof(Array)}.{nameof(Array.Empty)}<{type}>()";
        }

        return $"new {type}[0]";
    }

    /// <remarks>
    /// <c>random.GetItems(choices, 0)</c> → <c>Array.Empty&lt;T&gt;()</c> or <c>[]</c> (C# 12)
    /// </remarks>
    void AnalyzeGetItems_Array(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        IType? typeArgument,
        ICSharpArgument choicesArgument,
        ICSharpArgument lengthArgument)
    {
        Debug.Assert(invokedExpression.QualifierExpression is { });

        if (lengthArgument.Value.TryGetInt32Constant() == 0
            && !invocationExpression.IsUsedAsStatement()
            && invokedExpression.QualifierExpression.IsNotNullHere(nullableReferenceTypesDataFlowAnalysisRunSynchronizer)
            && choicesArgument.Value is { }
            && choicesArgument.Value.IsNotNullHere(nullableReferenceTypesDataFlowAnalysisRunSynchronizer))
        {
            var elementType = typeArgument
                ?? CollectionTypeUtil.ElementTypeByCollectionType(choicesArgument.Value.Type(), invocationExpression, false);

            if (elementType is { })
            {
                consumer.AddHighlighting(
                    new UseExpressionResultSuggestion(
                        "The expression is always an empty array.",
                        invocationExpression,
                        CreateEmptyArray(elementType, invocationExpression)));
            }
        }
    }

    /// <remarks>
    /// <c>random.GetItems(choices, 0)</c> → <c>Array.Empty&lt;T&gt;()</c> or <c>[]</c> (C# 12)
    /// </remarks>
    void AnalyzeGetItems_ReadOnlySpan(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        IType? typeArgument,
        ICSharpArgument choicesArgument,
        ICSharpArgument lengthArgument)
    {
        Debug.Assert(invokedExpression.QualifierExpression is { });

        if (lengthArgument.Value.TryGetInt32Constant() == 0
            && !invocationExpression.IsUsedAsStatement()
            && invokedExpression.QualifierExpression.IsNotNullHere(nullableReferenceTypesDataFlowAnalysisRunSynchronizer))
        {
            var elementType = typeArgument ?? (choicesArgument.Value is { } ? TypesUtil.GetTypeArgumentValue(choicesArgument.Value.Type(), 0) : null);

            if (elementType is { })
            {
                consumer.AddHighlighting(
                    new UseExpressionResultSuggestion(
                        "The expression is always an empty array.",
                        invocationExpression,
                        CreateEmptyArray(elementType, invocationExpression)));
            }
        }
    }

    /// <remarks>
    /// <c>random.Next(int.MaxValue)</c> → <c>random.Next()</c><para/>
    /// <c>random.Next(0)</c> → <c>0</c><para/>
    /// <c>random.Next(1)</c> → <c>0</c>
    /// </remarks>
    void AnalyzeNext_Int32(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument maxValueArgument)
    {
        Debug.Assert(invokedExpression.QualifierExpression is { });

        switch (maxValueArgument.Value.TryGetInt32Constant())
        {
            case int.MaxValue
                when ClrTypeNames.Random.HasMethod(
                    new MethodSignature { Name = nameof(Random.Next), ParameterTypes = [] },
                    invocationExpression.PsiModule):
                consumer.AddHighlighting(new RedundantArgumentHint($"Passing the int.{nameof(int.MaxValue)} is redundant.", maxValueArgument));
                break;

            case 0 or 1 when !invocationExpression.IsUsedAsStatement()
                && invokedExpression.QualifierExpression.IsNotNullHere(nullableReferenceTypesDataFlowAnalysisRunSynchronizer):

                consumer.AddHighlighting(new UseExpressionResultSuggestion("The expression is always 0.", invocationExpression, "0"));
                break;
        }
    }

    /// <remarks>
    /// <c>random.Next(0, maxValue)</c> → <c>random.Next(maxValue)</c><para/>
    /// <c>random.Next(n, n)</c> → <c>n</c><para/>
    /// <c>random.Next(n, n + 1)</c> → <c>n</c>
    /// </remarks>
    void AnalyzeNext_Int32_Int32(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument minValueArgument,
        ICSharpArgument maxValueArgument)
    {
        Debug.Assert(invokedExpression.QualifierExpression is { });

        switch (minValueArgument.Value.TryGetInt32Constant())
        {
            case 0 when ClrTypeNames.Random.HasMethod(
                new MethodSignature { Name = nameof(Random.Next), ParameterTypes = ParameterTypes.Int32 },
                invocationExpression.PsiModule):

                consumer.AddHighlighting(new RedundantArgumentHint("Passing 0 is redundant.", minValueArgument));
                break;

            case var minValue when maxValueArgument.Value.TryGetInt32Constant() is { } maxValue
                && (minValue == maxValue || minValue == unchecked(maxValue - 1))
                && !invocationExpression.IsUsedAsStatement()
                && invokedExpression.QualifierExpression.IsNotNullHere(nullableReferenceTypesDataFlowAnalysisRunSynchronizer):

                Debug.Assert(minValueArgument.Value is { });

                consumer.AddHighlighting(
                    new UseExpressionResultSuggestion(
                        "The expression is the same as the first argument.",
                        invocationExpression,
                        NumberInfo.Int32.GetReplacementFromArgument(invocationExpression, minValueArgument.Value)));
                break;
        }
    }

    /// <remarks>
    /// <c>random.NextInt64(long.MaxValue)</c> → <c>random.NextInt64()</c> (.NET 6)<para/>
    /// <c>random.NextInt64(0)</c> → <c>0</c><para/>
    /// <c>random.NextInt64(1)</c> → <c>0</c>
    /// </remarks>
    void AnalyzeNextInt64_Int64(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument maxValueArgument)
    {
        Debug.Assert(invokedExpression.QualifierExpression is { });

        switch (maxValueArgument.Value.TryGetInt64Constant())
        {
            case long.MaxValue when ClrTypeNames.Random.HasMethod(
                new MethodSignature { Name = "NextInt64", ParameterTypes = [] }, // todo: nameof(Random.NextInt64) when available
                invocationExpression.PsiModule):

                consumer.AddHighlighting(new RedundantArgumentHint($"Passing the long.{nameof(long.MaxValue)} is redundant.", maxValueArgument));
                break;

            case 0 or 1 when !invocationExpression.IsUsedAsStatement()
                && invokedExpression.QualifierExpression.IsNotNullHere(nullableReferenceTypesDataFlowAnalysisRunSynchronizer):

                consumer.AddHighlighting(new UseExpressionResultSuggestion("The expression is always 0.", invocationExpression, "0L"));
                break;
        }
    }

    /// <remarks>
    /// <c>random.NextInt64(0, maxValue)</c> → <c>random.NextInt64(maxValue)</c> (.NET 6)<para/>
    /// <c>random.Next(n, n)</c> → <c>n</c><para/>
    /// <c>random.Next(n, n + 1)</c> → <c>n</c>
    /// </remarks>
    void AnalyzeNextInt64_Int64_Int64(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument minValueArgument,
        ICSharpArgument maxValueArgument)
    {
        Debug.Assert(invokedExpression.QualifierExpression is { });

        switch (minValueArgument.Value.TryGetInt64Constant())
        {
            case 0 when ClrTypeNames.Random.HasMethod(
                new MethodSignature { Name = "NextInt64", ParameterTypes = ParameterTypes.Int64 }, // todo: nameof(Random.NextInt64) when available
                invocationExpression.PsiModule):

                consumer.AddHighlighting(new RedundantArgumentHint("Passing 0 is redundant.", minValueArgument));
                break;

            case var minValue when maxValueArgument.Value.TryGetInt32Constant() is { } maxValue
                && (minValue == maxValue || minValue == unchecked(maxValue - 1))
                && !invocationExpression.IsUsedAsStatement()
                && invokedExpression.QualifierExpression.IsNotNullHere(nullableReferenceTypesDataFlowAnalysisRunSynchronizer):

                Debug.Assert(minValueArgument.Value is { });

                consumer.AddHighlighting(
                    new UseExpressionResultSuggestion(
                        "The expression is the same as the first argument.",
                        invocationExpression,
                        NumberInfo.Int64.GetReplacementFromArgument(invocationExpression, minValueArgument.Value)));
                break;
        }
    }

    protected override void Run(IInvocationExpression element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
    {
        if (element is { InvokedExpression: IReferenceExpression { QualifierExpression: { }, Reference: var reference } invokedExpression }
            && reference.Resolve().DeclaredElement is IMethod
            {
                AccessibilityDomain.DomainType: AccessibilityDomain.AccessibilityDomainType.PUBLIC, IsStatic: false,
            } method
            && method.ContainingType.IsClrType(ClrTypeNames.Random))
        {
            switch (method.ShortName)
            {
                case "GetItems": // todo: nameof(Random.GetItems) when available
                    switch (method.TypeParameters, method.Parameters, element.TypeArguments, element.TryGetArgumentsInDeclarationOrder())
                    {
                        case ([_], [{ Type: var choicesType }, { Type: var lengthType }], [_] or [], [{ } choicesArgument, { } lengthArgument])
                            when choicesType.IsGenericArray() && lengthType.IsInt():
                        {
                            AnalyzeGetItems_Array(
                                consumer,
                                element,
                                invokedExpression,
                                element.TypeArguments is [var typeArgument] ? typeArgument : null,
                                choicesArgument,
                                lengthArgument);
                            break;
                        }

                        case ([_], [{ Type: var choicesType }, { Type: var lengthType }], [_] or [], [{ } choicesArgument, { } lengthArgument])
                            when choicesType.IsReadOnlySpan() && lengthType.IsInt():
                        {
                            AnalyzeGetItems_ReadOnlySpan(
                                consumer,
                                element,
                                invokedExpression,
                                element.TypeArguments is [var typeArgument] ? typeArgument : null,
                                choicesArgument,
                                lengthArgument);
                            break;
                        }
                    }
                    break;

                case nameof(Random.Next):
                    switch (method.TypeParameters, method.Parameters, element.TryGetArgumentsInDeclarationOrder())
                    {
                        case ([], [{ Type: var maxValueType }], [{ } maxValueArgument]) when maxValueType.IsInt():
                            AnalyzeNext_Int32(consumer, element, invokedExpression, maxValueArgument);
                            break;

                        case ([], [{ Type: var minValueType }, { Type: var maxValueType }], [{ } minValueArgument, { } maxValueArgument])
                            when minValueType.IsInt() && maxValueType.IsInt():

                            AnalyzeNext_Int32_Int32(consumer, element, invokedExpression, minValueArgument, maxValueArgument);
                            break;
                    }
                    break;

                case "NextInt64": // todo: nameof(Random.NextInt64) when available
                    switch (method.TypeParameters, method.Parameters, element.TryGetArgumentsInDeclarationOrder())
                    {
                        case ([], [{ Type: var maxValueType }], [{ } maxValueArgument]) when maxValueType.IsLong():
                            AnalyzeNextInt64_Int64(consumer, element, invokedExpression, maxValueArgument);
                            break;

                        case ([], [{ Type: var minValueType }, { Type: var maxValueType }], [{ } minValueArgument, { } maxValueArgument])
                            when minValueType.IsLong() && maxValueType.IsLong():

                            AnalyzeNextInt64_Int64_Int64(consumer, element, invokedExpression, minValueArgument, maxValueArgument);
                            break;
                    }
                    break;
            }
        }
    }
}