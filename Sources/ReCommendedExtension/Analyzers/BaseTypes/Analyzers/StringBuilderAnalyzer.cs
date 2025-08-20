using System.Text;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Impl.ControlFlow.NullableAnalysis.Runner;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using ReCommendedExtension.Analyzers.BaseTypes.Collections;
using ReCommendedExtension.Extensions;
using ReCommendedExtension.Extensions.MethodFinding;

namespace ReCommendedExtension.Analyzers.BaseTypes.Analyzers;

/// <remarks>
/// C# language version checks are only done when a quick fix would require it.
/// </remarks>
[ElementProblemAnalyzer(
    typeof(IInvocationExpression),
    HighlightingTypes =
    [
        typeof(PassSingleCharacterSuggestion),
        typeof(PassSingleCharactersSuggestion),
        typeof(UseOtherMethodSuggestion),
        typeof(RedundantArgumentHint),
        typeof(RedundantMethodInvocationHint),
    ])]
public sealed class StringBuilderAnalyzer(NullableReferenceTypesDataFlowAnalysisRunSynchronizer nullableReferenceTypesDataFlowAnalysisRunSynchronizer)
    : ElementProblemAnalyzer<IInvocationExpression>
{
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Underscore character used intentionally as a separator.")]
    static class ParameterTypes
    {
        public static IReadOnlyList<ParameterType> Char { get; } = [new() { ClrTypeName = PredefinedType.CHAR_FQN }];

        public static IReadOnlyList<ParameterType> Char_Char { get; } =
        [
            new() { ClrTypeName = PredefinedType.CHAR_FQN }, new() { ClrTypeName = PredefinedType.CHAR_FQN },
        ];

        public static IReadOnlyList<ParameterType> Char_Char_Int32_Int32 { get; } =
        [
            new() { ClrTypeName = PredefinedType.CHAR_FQN },
            new() { ClrTypeName = PredefinedType.CHAR_FQN },
            new() { ClrTypeName = PredefinedType.INT_FQN },
            new() { ClrTypeName = PredefinedType.INT_FQN },
        ];

        public static IReadOnlyList<ParameterType> Char_ObjectArray { get; } =
        [
            new() { ClrTypeName = PredefinedType.CHAR_FQN }, new ArrayParameterType { ClrTypeName = PredefinedType.OBJECT_FQN },
        ];

        public static IReadOnlyList<ParameterType> Char_StringArray { get; } =
        [
            new() { ClrTypeName = PredefinedType.CHAR_FQN }, new ArrayParameterType { ClrTypeName = PredefinedType.STRING_FQN },
        ];

        public static IReadOnlyList<ParameterType> Char_IEnumerableOfT { get; } =
        [
            new() { ClrTypeName = PredefinedType.CHAR_FQN }, new GenericParameterType { ClrTypeName = PredefinedType.GENERIC_IENUMERABLE_FQN },
        ];

        public static IReadOnlyList<ParameterType> Char_ReadOnlySpanOfT { get; } =
        [
            new() { ClrTypeName = PredefinedType.CHAR_FQN }, new GenericParameterType { ClrTypeName = PredefinedType.SYSTEM_READ_ONLY_SPAN_FQN },
        ];

        public static IReadOnlyList<ParameterType> Object { get; } = [new() { ClrTypeName = PredefinedType.OBJECT_FQN }];

        public static IReadOnlyList<ParameterType> String { get; } = [new() { ClrTypeName = PredefinedType.STRING_FQN }];

        public static IReadOnlyList<ParameterType> Int32_String { get; } =
        [
            new() { ClrTypeName = PredefinedType.INT_FQN }, new() { ClrTypeName = PredefinedType.STRING_FQN },
        ];

        public static IReadOnlyList<ParameterType> Int32_Char { get; } =
        [
            new() { ClrTypeName = PredefinedType.INT_FQN }, new() { ClrTypeName = PredefinedType.CHAR_FQN },
        ];
    }

    /// <remarks>
    /// <c>builder.Append(value, 0)</c> → <c>builder</c><para/>
    /// <c>builder.Append(value, 1)</c> → <c>builder.Append(value)</c>
    /// </remarks>
    static void AnalyzeAppend_Char_Int32(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument repeatCountArgument)
    {
        switch (repeatCountArgument.Value.TryGetInt32Constant())
        {
            case 0:
                consumer.AddHighlighting(
                    new RedundantMethodInvocationHint(
                        $"Calling '{nameof(StringBuilder.Append)}' with the repeat count 0 is redundant.",
                        invocationExpression,
                        invokedExpression));
                break;

            case 1 when PredefinedType.STRING_BUILDER_FQN.HasMethod(
                new MethodSignature { Name = nameof(StringBuilder.Append), ParameterTypes = ParameterTypes.Char },
                invocationExpression.PsiModule):

                consumer.AddHighlighting(new RedundantArgumentHint("Passing 1 is redundant.", repeatCountArgument));
                break;
        }
    }

    /// <remarks>
    /// <c>builder.Append(null)</c> → <c>builder</c><para/>
    /// <c>builder.Append([])</c> → <c>builder</c>
    /// </remarks>
    static void AnalyzeAppend_CharArray(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument valueArgument)
    {
        if (valueArgument.Value.IsDefaultValue())
        {
            consumer.AddHighlighting(
                new RedundantMethodInvocationHint(
                    $"Calling '{nameof(StringBuilder.Append)}' with null is redundant.",
                    invocationExpression,
                    invokedExpression));
            return;
        }

        if (CollectionCreation.TryFrom(valueArgument.Value) is { Count: 0 })
        {
            consumer.AddHighlighting(
                new RedundantMethodInvocationHint(
                    $"Calling '{nameof(StringBuilder.Append)}' with an empty array is redundant.",
                    invocationExpression,
                    invokedExpression));
        }
    }

    /// <remarks>
    /// <c>builder.Append(null, 0, 0)</c> → <c>builder</c><para/>
    /// <c>builder.Append([], 0, 0)</c> → <c>builder</c>
    /// </remarks>
    static void AnalyzeAppend_CharArray_Int32_Int32(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument valueArgument,
        ICSharpArgument startIndexArgument,
        ICSharpArgument charCountArgument)
    {
        if (startIndexArgument.Value.TryGetInt32Constant() == 0 && charCountArgument.Value.TryGetInt32Constant() == 0)
        {
            if (valueArgument.Value.IsDefaultValue())
            {
                consumer.AddHighlighting(
                    new RedundantMethodInvocationHint(
                        $"Calling '{nameof(StringBuilder.Append)}' with null is redundant.",
                        invocationExpression,
                        invokedExpression));
                return;
            }

            if (CollectionCreation.TryFrom(valueArgument.Value) is { Count: 0 })
            {
                consumer.AddHighlighting(
                    new RedundantMethodInvocationHint(
                        $"Calling '{nameof(StringBuilder.Append)}' with an empty array is redundant.",
                        invocationExpression,
                        invokedExpression));
            }
        }
    }

    /// <remarks>
    /// <c>builder.Append(null)</c> → <c>builder</c><para/>
    /// <c>builder.Append("")</c> → <c>builder</c><para/>
    /// <c>builder.Append("a")</c> → <c>builder.Append('a')</c>
    /// </remarks>
    static void AnalyzeAppend_String(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument valueArgument)
    {
        if (valueArgument.Value.IsDefaultValue())
        {
            consumer.AddHighlighting(
                new RedundantMethodInvocationHint(
                    $"Calling '{nameof(StringBuilder.Append)}' with null is redundant.",
                    invocationExpression,
                    invokedExpression));
            return;
        }

        switch (valueArgument.Value.TryGetStringConstant())
        {
            case "":
                consumer.AddHighlighting(
                    new RedundantMethodInvocationHint(
                        $"Calling '{nameof(StringBuilder.Append)}' with an empty string is redundant.",
                        invocationExpression,
                        invokedExpression));
                break;

            case [var character] when PredefinedType.STRING_BUILDER_FQN.HasMethod(
                new MethodSignature { Name = nameof(StringBuilder.Append), ParameterTypes = ParameterTypes.Char },
                valueArgument.NameIdentifier is { },
                out var parameterNames,
                invocationExpression.PsiModule):

                consumer.AddHighlighting(
                    new PassSingleCharacterSuggestion(
                        "Pass the single character.",
                        valueArgument,
                        parameterNames is [var valueParameterName] ? valueParameterName : null,
                        character));
                break;
        }
    }

    /// <remarks>
    /// <c>builder.Append(null, 0, 0)</c> → <c>builder</c><para/>
    /// <c>builder.Append(value, startIndex, 0)</c> → <c>builder</c><para/>
    /// <c>builder.Append("abc", 2, 1)</c> → <c>builder.Append('c')</c>
    /// </remarks>
    void AnalyzeAppend_String_Int32_Int32(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument valueArgument,
        ICSharpArgument startIndexArgument,
        ICSharpArgument countArgument)
    {
        switch (valueArgument.Value, startIndexArgument.Value.TryGetInt32Constant(), countArgument.Value.TryGetInt32Constant())
        {
            case (var value, 0, 0) when value.IsDefaultValue():
                consumer.AddHighlighting(
                    new RedundantMethodInvocationHint(
                        $"Calling '{nameof(StringBuilder.Append)}' with (null, 0, 0) is redundant.",
                        invocationExpression,
                        invokedExpression));
                break;

            case (var value, >= 0 and var startIndex, 1) when value.TryGetStringConstant() is { } s
                && startIndex < s.Length
                && PredefinedType.STRING_BUILDER_FQN.HasMethod(
                    new MethodSignature { Name = nameof(StringBuilder.Append), ParameterTypes = ParameterTypes.Char },
                    valueArgument.NameIdentifier is { },
                    out var parameterNames,
                    invocationExpression.PsiModule):

                consumer.AddHighlighting(
                    new PassSingleCharacterSuggestion(
                        "Pass the single character.",
                        valueArgument,
                        parameterNames is [var valueParameterName] ? valueParameterName : null,
                        s[startIndex],
                        redundantArguments: [startIndexArgument, countArgument]));
                break;

            case ({ } value, >= 0, 0) when value.IsNotNullHere(nullableReferenceTypesDataFlowAnalysisRunSynchronizer):
                consumer.AddHighlighting(
                    new RedundantMethodInvocationHint(
                        $"Calling '{nameof(StringBuilder.Append)}' with the count 0 is redundant.",
                        invocationExpression,
                        invokedExpression));
                break;
        }
    }

    /// <remarks>
    /// <c>builder.Append(null)</c> → <c>builder</c>
    /// </remarks>
    static void AnalyzeAppend_StringBuilder(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument valueArgument)
    {
        if (valueArgument.Value.IsDefaultValue())
        {
            consumer.AddHighlighting(
                new RedundantMethodInvocationHint(
                    $"Calling '{nameof(StringBuilder.Append)}' with null is redundant.",
                    invocationExpression,
                    invokedExpression));
        }
    }

    /// <remarks>
    /// <c>builder.Append(null, 0, 0)</c> → <c>builder</c><para/>
    /// <c>builder.Append(value, startIndex, 0)</c> → <c>builder</c>
    /// </remarks>
    void AnalyzeAppend_StringBuilder_Int32_Int32(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument valueArgument,
        ICSharpArgument startIndexArgument,
        ICSharpArgument countArgument)
    {
        switch (valueArgument.Value, startIndexArgument.Value.TryGetInt32Constant(), countArgument.Value.TryGetInt32Constant())
        {
            case (var value, 0, 0) when value.IsDefaultValue():
                consumer.AddHighlighting(
                    new RedundantMethodInvocationHint(
                        $"Calling '{nameof(StringBuilder.Append)}' with (null, 0, 0) is redundant.",
                        invocationExpression,
                        invokedExpression));
                break;

            case ({ } value, >= 0, 0) when value.IsNotNullHere(nullableReferenceTypesDataFlowAnalysisRunSynchronizer):
                consumer.AddHighlighting(
                    new RedundantMethodInvocationHint(
                        $"Calling '{nameof(StringBuilder.Append)}' with the count 0 is redundant.",
                        invocationExpression,
                        invokedExpression));
                break;
        }
    }

    /// <remarks>
    /// <c>builder.Append(null)</c> → <c>builder</c>
    /// </remarks>
    static void AnalyzeAppend_Object(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument valueArgument)
    {
        if (valueArgument.Value.IsDefaultValue())
        {
            consumer.AddHighlighting(
                new RedundantMethodInvocationHint(
                    $"Calling '{nameof(StringBuilder.Append)}' with null is redundant.",
                    invocationExpression,
                    invokedExpression));
        }
    }

    /// <remarks>
    /// <c>builder.AppendJoin(separator, [])</c> → <c>builder</c><para/>
    /// <c>builder.AppendJoin(separator, [item])</c> → <c>builder.Append(item)</c><para/>
    /// <c>builder.AppendJoin(",", values)</c> → <c>builder.AppendJoin(',', values)</c>
    /// </remarks>
    static void AnalyzeAppendJoin_String_ObjectArray(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        TreeNodeCollection<ICSharpArgument?> arguments)
    {
        [Pure]
        bool MethodExists()
            => PredefinedType.STRING_BUILDER_FQN.HasMethod(
                new MethodSignature { Name = nameof(StringBuilder.Append), ParameterTypes = ParameterTypes.Object },
                invocationExpression.PsiModule);

        if (arguments.TrySplit(out var separatorArgument, out var valuesArguments))
        {
            switch (valuesArguments)
            {
                case []:
                    consumer.AddHighlighting(
                        new RedundantMethodInvocationHint(
                            "Calling 'AppendJoin' with an empty array is redundant.", // todo: use 'nameof(StringBuilder.AppendJoin)'
                            invocationExpression,
                            invokedExpression));
                    return;

                case [{ Value: { } } argument]:
                    if (CollectionCreation.TryFrom(argument.Value) is { } collectionCreation)
                    {
                        switch (collectionCreation.Count)
                        {
                            case 0:
                                consumer.AddHighlighting(
                                    new RedundantMethodInvocationHint(
                                        "Calling 'AppendJoin' with an empty array is redundant.", // todo: use 'nameof(StringBuilder.AppendJoin)'
                                        invocationExpression,
                                        invokedExpression));
                                return;

                            case 1 when MethodExists():
                                consumer.AddHighlighting(
                                    new UseOtherMethodSuggestion(
                                        $"Use the '{nameof(StringBuilder.Append)}' method.",
                                        invocationExpression,
                                        invokedExpression,
                                        nameof(StringBuilder.Append),
                                        false,
                                        [collectionCreation.SingleElement.GetText()]));
                                return;
                        }
                    }
                    else
                    {
                        if (argument.Value.GetExpressionType().ToIType() is { } argumentType
                            && !argumentType.IsGenericArrayOfObject()
                            && MethodExists())
                        {
                            consumer.AddHighlighting(
                                new UseOtherMethodSuggestion(
                                    $"Use the '{nameof(StringBuilder.Append)}' method.",
                                    invocationExpression,
                                    invokedExpression,
                                    nameof(StringBuilder.Append),
                                    false,
                                    [argument.Value.GetText()]));
                            return;
                        }
                    }
                    break;
            }

            if (separatorArgument.Value.TryGetStringConstant() is [var character]
                && PredefinedType.STRING_BUILDER_FQN.HasMethod(
                    new MethodSignature
                    {
                        Name = "AppendJoin", // todo: use 'nameof(StringBuilder.AppendJoin)'
                        ParameterTypes = ParameterTypes.Char_ObjectArray,
                    },
                    separatorArgument.NameIdentifier is { },
                    out var parameterNames,
                    invocationExpression.PsiModule))
            {
                consumer.AddHighlighting(
                    new PassSingleCharacterSuggestion(
                        "Pass the single character.",
                        separatorArgument,
                        parameterNames is [var separatorParameterName, _] ? separatorParameterName : null,
                        character));
            }
        }
    }

    /// <remarks>
    /// <c>builder.AppendJoin(separator, default(ReadOnlySpan&lt;object?&gt;))</c> → <c>builder</c><para/>
    /// <c>builder.AppendJoin(separator, new ReadOnlySpan&lt;object?&gt;())</c> → <c>builder</c><para/>
    /// <c>builder.AppendJoin(separator, [item])</c> → <c>builder.Append(item)</c><para/>
    /// <c>builder.AppendJoin(separator, item)</c> → <c>builder.Append(item)</c><para/>
    /// <c>builder.AppendJoin(",", values)</c> → <c>builder.AppendJoin(',', values)</c>
    /// </remarks>
    static void AnalyzeAppendJoin_String_ReadOnlySpanOfObject(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        TreeNodeCollection<ICSharpArgument?> arguments)
    {
        [Pure]
        bool MethodExists()
            => PredefinedType.STRING_BUILDER_FQN.HasMethod(
                new MethodSignature { Name = nameof(StringBuilder.Append), ParameterTypes = ParameterTypes.Object },
                invocationExpression.PsiModule);

        if (arguments.TrySplit(out var separatorArgument, out var valuesArguments))
        {
            switch (valuesArguments)
            {
                case []:
                    consumer.AddHighlighting(
                        new RedundantMethodInvocationHint(
                            "Calling 'AppendJoin' with an empty span is redundant.", // todo: use 'nameof(StringBuilder.AppendJoin)'
                            invocationExpression,
                            invokedExpression));
                    return;

                case [{ Value: { } } argument]:
                    if (CollectionCreation.TryFrom(argument.Value) is { } collectionCreation)
                    {
                        switch (collectionCreation.Count)
                        {
                            case 0:
                                consumer.AddHighlighting(
                                    new RedundantMethodInvocationHint(
                                        "Calling 'AppendJoin' with an empty span is redundant.", // todo: use 'nameof(StringBuilder.AppendJoin)'
                                        invocationExpression,
                                        invokedExpression));
                                return;

                            case 1 when MethodExists():
                                consumer.AddHighlighting(
                                    new UseOtherMethodSuggestion(
                                        $"Use the '{nameof(StringBuilder.Append)}' method.",
                                        invocationExpression,
                                        invokedExpression,
                                        nameof(StringBuilder.Append),
                                        false,
                                        [collectionCreation.SingleElement.GetText()]));
                                return;
                        }
                    }
                    else
                    {
                        if (argument.Value.GetExpressionType().ToIType() is { } argumentType
                            && !argumentType.IsReadOnlySpanOfObject()
                            && MethodExists())
                        {
                            consumer.AddHighlighting(
                                new UseOtherMethodSuggestion(
                                    $"Use the '{nameof(StringBuilder.Append)}' method.",
                                    invocationExpression,
                                    invokedExpression,
                                    nameof(StringBuilder.Append),
                                    false,
                                    [argument.Value.GetText()]));
                            return;
                        }
                    }
                    break;
            }

            if (separatorArgument.Value.TryGetStringConstant() is [var character]
                && PredefinedType.STRING_BUILDER_FQN.HasMethod(
                    new MethodSignature
                    {
                        Name = "AppendJoin", // todo: use 'nameof(StringBuilder.AppendJoin)'
                        ParameterTypes = ParameterTypes.Char_ReadOnlySpanOfT,
                    },
                    separatorArgument.NameIdentifier is { },
                    out var parameterNames,
                    invocationExpression.PsiModule))
            {
                consumer.AddHighlighting(
                    new PassSingleCharacterSuggestion(
                        "Pass the single character.",
                        separatorArgument,
                        parameterNames is [var separatorParameterName, _] ? separatorParameterName : null,
                        character));
            }
        }
    }

    /// <remarks>
    /// <c>builder.AppendJoin(separator, [])</c> → <c>builder</c><para/>
    /// <c>builder.AppendJoin(separator, [item])</c> → <c>builder.Append(item)</c><para/>
    /// <c>builder.AppendJoin(",", values)</c> → <c>builder.AppendJoin(',', values)</c>
    /// </remarks>
    static void AnalyzeAppendJoin_String_IEnumerableOfT(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument separatorArgument,
        ICSharpArgument valuesArgument)
    {
        if (CollectionCreation.TryFrom(valuesArgument.Value) is { } collectionCreation)
        {
            switch (collectionCreation.Count)
            {
                case 0:
                    consumer.AddHighlighting(
                        new RedundantMethodInvocationHint(
                            "Calling 'AppendJoin' with an empty array is redundant.", // todo: use 'nameof(StringBuilder.AppendJoin)'
                            invocationExpression,
                            invokedExpression));
                    return;

                case 1 when PredefinedType.STRING_BUILDER_FQN.HasMethod(
                    new MethodSignature { Name = nameof(StringBuilder.Append), ParameterTypes = ParameterTypes.Object },
                    invocationExpression.PsiModule):

                    consumer.AddHighlighting(
                        new UseOtherMethodSuggestion(
                            $"Use the '{nameof(StringBuilder.Append)}' method.",
                            invocationExpression,
                            invokedExpression,
                            nameof(StringBuilder.Append),
                            false,
                            [collectionCreation.SingleElement.GetText()]));
                    return;
            }
        }

        if (separatorArgument.Value.TryGetStringConstant() is [var character]
            && PredefinedType.STRING_BUILDER_FQN.HasMethod(
                new MethodSignature
                {
                    Name = "AppendJoin", // todo: use 'nameof(StringBuilder.AppendJoin)'
                    ParameterTypes = ParameterTypes.Char_IEnumerableOfT,
                    GenericParametersCount = 1,
                },
                separatorArgument.NameIdentifier is { },
                out var parameterNames,
                invocationExpression.PsiModule))
        {
            consumer.AddHighlighting(
                new PassSingleCharacterSuggestion(
                    "Pass the single character.",
                    separatorArgument,
                    parameterNames is [var separatorParameterName, _] ? separatorParameterName : null,
                    character));
        }
    }

    /// <remarks>
    /// <c>builder.AppendJoin(separator, [])</c> → <c>builder</c><para/>
    /// <c>builder.AppendJoin(separator, [item])</c> → <c>builder.Append(item)</c><para/>
    /// <c>builder.AppendJoin(",", values)</c> → <c>builder.AppendJoin(',', values)</c>
    /// </remarks>
    static void AnalyzeAppendJoin_String_StringArray(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        TreeNodeCollection<ICSharpArgument?> arguments)
    {
        [Pure]
        bool MethodExists()
            => PredefinedType.STRING_BUILDER_FQN.HasMethod(
                new MethodSignature { Name = nameof(StringBuilder.Append), ParameterTypes = ParameterTypes.String },
                invocationExpression.PsiModule);

        if (arguments.TrySplit(out var separatorArgument, out var valuesArguments))
        {
            switch (valuesArguments)
            {
                case []:
                    consumer.AddHighlighting(
                        new RedundantMethodInvocationHint(
                            "Calling 'AppendJoin' with an empty array is redundant.", // todo: use 'nameof(StringBuilder.AppendJoin)'
                            invocationExpression,
                            invokedExpression));
                    return;

                case [{ Value: { } } argument]:
                    if (CollectionCreation.TryFrom(argument.Value) is { } collectionCreation)
                    {
                        switch (collectionCreation.Count)
                        {
                            case 0:
                                consumer.AddHighlighting(
                                    new RedundantMethodInvocationHint(
                                        "Calling 'AppendJoin' with an empty array is redundant.", // todo: use 'nameof(StringBuilder.AppendJoin)'
                                        invocationExpression,
                                        invokedExpression));
                                return;

                            case 1 when MethodExists():
                                consumer.AddHighlighting(
                                    new UseOtherMethodSuggestion(
                                        $"Use the '{nameof(StringBuilder.Append)}' method.",
                                        invocationExpression,
                                        invokedExpression,
                                        nameof(StringBuilder.Append),
                                        false,
                                        [collectionCreation.SingleElement.GetText()]));
                                return;
                        }
                    }
                    else
                    {
                        if (argument.Value.GetExpressionType().ToIType() is { } argumentType
                            && !argumentType.IsGenericArrayOfString()
                            && MethodExists())
                        {
                            consumer.AddHighlighting(
                                new UseOtherMethodSuggestion(
                                    $"Use the '{nameof(StringBuilder.Append)}' method.",
                                    invocationExpression,
                                    invokedExpression,
                                    nameof(StringBuilder.Append),
                                    false,
                                    [argument.Value.GetText()]));
                            return;
                        }
                    }
                    break;
            }

            if (separatorArgument.Value.TryGetStringConstant() is [var character]
                && PredefinedType.STRING_BUILDER_FQN.HasMethod(
                    new MethodSignature
                    {
                        Name = "AppendJoin", // todo: use 'nameof(StringBuilder.AppendJoin)'
                        ParameterTypes = ParameterTypes.Char_StringArray,
                    },
                    separatorArgument.NameIdentifier is { },
                    out var parameterNames,
                    invocationExpression.PsiModule))
            {
                consumer.AddHighlighting(
                    new PassSingleCharacterSuggestion(
                        "Pass the single character.",
                        separatorArgument,
                        parameterNames is [var separatorParameterName, _] ? separatorParameterName : null,
                        character));
            }
        }
    }

    /// <remarks>
    /// <c>builder.AppendJoin(separator, default(ReadOnlySpan&lt;string?&gt;))</c> → <c>builder</c><para/>
    /// <c>builder.AppendJoin(separator, new ReadOnlySpan&lt;string?&gt;())</c> → <c>builder</c><para/>
    /// <c>builder.AppendJoin(separator, [item])</c> → <c>builder.Append(item)</c><para/>
    /// <c>builder.AppendJoin(separator, item)</c> → <c>builder.Append(item)</c><para/>
    /// <c>builder.AppendJoin(",", values)</c> → <c>builder.AppendJoin(',', values)</c>
    /// </remarks>
    static void AnalyzeAppendJoin_String_ReadOnlySpanOfString(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        TreeNodeCollection<ICSharpArgument?> arguments)
    {
        [Pure]
        bool MethodExists()
            => PredefinedType.STRING_BUILDER_FQN.HasMethod(
                new MethodSignature { Name = nameof(StringBuilder.Append), ParameterTypes = ParameterTypes.String },
                invocationExpression.PsiModule);

        if (arguments.TrySplit(out var separatorArgument, out var valuesArguments))
        {
            switch (valuesArguments)
            {
                case []:
                    consumer.AddHighlighting(
                        new RedundantMethodInvocationHint(
                            "Calling 'AppendJoin' with an empty span is redundant.", // todo: use 'nameof(StringBuilder.AppendJoin)'
                            invocationExpression,
                            invokedExpression));
                    return;

                case [{ Value: { } } argument]:
                    if (CollectionCreation.TryFrom(argument.Value) is { } collectionCreation)
                    {
                        switch (collectionCreation.Count)
                        {
                            case 0:
                                consumer.AddHighlighting(
                                    new RedundantMethodInvocationHint(
                                        "Calling 'AppendJoin' with an empty span is redundant.", // todo: use 'nameof(StringBuilder.AppendJoin)'
                                        invocationExpression,
                                        invokedExpression));
                                return;

                            case 1 when MethodExists():
                                consumer.AddHighlighting(
                                    new UseOtherMethodSuggestion(
                                        $"Use the '{nameof(StringBuilder.Append)}' method.",
                                        invocationExpression,
                                        invokedExpression,
                                        nameof(StringBuilder.Append),
                                        false,
                                        [collectionCreation.SingleElement.GetText()]));
                                return;
                        }
                    }
                    else
                    {
                        if (argument.Value.GetExpressionType().ToIType() is { } argumentType
                            && !argumentType.IsReadOnlySpanOfString()
                            && MethodExists())
                        {
                            consumer.AddHighlighting(
                                new UseOtherMethodSuggestion(
                                    $"Use the '{nameof(StringBuilder.Append)}' method.",
                                    invocationExpression,
                                    invokedExpression,
                                    nameof(StringBuilder.Append),
                                    false,
                                    [argument.Value.GetText()]));
                            return;
                        }
                    }
                    break;
            }

            if (separatorArgument.Value.TryGetStringConstant() is [var character]
                && PredefinedType.STRING_BUILDER_FQN.HasMethod(
                    new MethodSignature
                    {
                        Name = "AppendJoin", // todo: use 'nameof(StringBuilder.AppendJoin)'
                        ParameterTypes = ParameterTypes.Char_ReadOnlySpanOfT,
                    },
                    separatorArgument.NameIdentifier is { },
                    out var parameterNames,
                    invocationExpression.PsiModule))
            {
                consumer.AddHighlighting(
                    new PassSingleCharacterSuggestion(
                        "Pass the single character.",
                        separatorArgument,
                        parameterNames is [var separatorParameterName, _] ? separatorParameterName : null,
                        character));
            }
        }
    }

    /// <remarks>
    /// <c>builder.AppendJoin(separator, [])</c> → <c>builder</c><para/>
    /// <c>builder.AppendJoin(separator, [item])</c> → <c>builder.Append(item)</c>
    /// </remarks>
    static void AnalyzeAppendJoin_Char_ObjectArray(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        TreeNodeCollection<ICSharpArgument?> arguments)
    {
        [Pure]
        bool MethodExists()
            => PredefinedType.STRING_BUILDER_FQN.HasMethod(
                new MethodSignature { Name = nameof(StringBuilder.Append), ParameterTypes = ParameterTypes.Object },
                invocationExpression.PsiModule);

        if (arguments.TrySplit(out _, out var valuesArguments))
        {
            switch (valuesArguments)
            {
                case []:
                    consumer.AddHighlighting(
                        new RedundantMethodInvocationHint(
                            "Calling 'AppendJoin' with an empty array is redundant.", // todo: use 'nameof(StringBuilder.AppendJoin)'
                            invocationExpression,
                            invokedExpression));
                    break;

                case [{ Value: { } } argument]:
                    if (CollectionCreation.TryFrom(argument.Value) is { } collectionCreation)
                    {
                        switch (collectionCreation.Count)
                        {
                            case 0:
                                consumer.AddHighlighting(
                                    new RedundantMethodInvocationHint(
                                        "Calling 'AppendJoin' with an empty array is redundant.", // todo: use 'nameof(StringBuilder.AppendJoin)'
                                        invocationExpression,
                                        invokedExpression));
                                break;

                            case 1 when MethodExists():
                                consumer.AddHighlighting(
                                    new UseOtherMethodSuggestion(
                                        $"Use the '{nameof(StringBuilder.Append)}' method.",
                                        invocationExpression,
                                        invokedExpression,
                                        nameof(StringBuilder.Append),
                                        false,
                                        [collectionCreation.SingleElement.GetText()]));
                                break;
                        }
                    }
                    else
                    {
                        if (argument.Value.GetExpressionType().ToIType() is { } argumentType
                            && !argumentType.IsGenericArrayOfObject()
                            && MethodExists())
                        {
                            consumer.AddHighlighting(
                                new UseOtherMethodSuggestion(
                                    $"Use the '{nameof(StringBuilder.Append)}' method.",
                                    invocationExpression,
                                    invokedExpression,
                                    nameof(StringBuilder.Append),
                                    false,
                                    [argument.Value.GetText()]));
                        }
                    }
                    break;
            }
        }
    }

    /// <remarks>
    /// <c>builder.AppendJoin(separator, default(ReadOnlySpan&lt;object?&gt;))</c> → <c>builder</c><para/>
    /// <c>builder.AppendJoin(separator, new ReadOnlySpan&lt;object?&gt;())</c> → <c>builder</c><para/>
    /// <c>builder.AppendJoin(separator, [item])</c> → <c>builder.Append(item)</c><para/>
    /// <c>builder.AppendJoin(separator, item)</c> → <c>builder.Append(item)</c>
    /// </remarks>
    static void AnalyzeAppendJoin_Char_ReadOnlySpanOfObject(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        TreeNodeCollection<ICSharpArgument?> arguments)
    {
        [Pure]
        bool MethodExists()
            => PredefinedType.STRING_BUILDER_FQN.HasMethod(
                new MethodSignature { Name = nameof(StringBuilder.Append), ParameterTypes = ParameterTypes.Object },
                invocationExpression.PsiModule);

        if (arguments.TrySplit(out _, out var valuesArguments))
        {
            switch (valuesArguments)
            {
                case []:
                    consumer.AddHighlighting(
                        new RedundantMethodInvocationHint(
                            "Calling 'AppendJoin' with an empty span is redundant.", // todo: use 'nameof(StringBuilder.AppendJoin)'
                            invocationExpression,
                            invokedExpression));
                    break;

                case [{ Value: { } } argument]:
                    if (CollectionCreation.TryFrom(argument.Value) is { } collectionCreation)
                    {
                        switch (collectionCreation.Count)
                        {
                            case 0:
                                consumer.AddHighlighting(
                                    new RedundantMethodInvocationHint(
                                        "Calling 'AppendJoin' with an empty span is redundant.", // todo: use 'nameof(StringBuilder.AppendJoin)'
                                        invocationExpression,
                                        invokedExpression));
                                break;

                            case 1 when MethodExists():
                                consumer.AddHighlighting(
                                    new UseOtherMethodSuggestion(
                                        $"Use the '{nameof(StringBuilder.Append)}' method.",
                                        invocationExpression,
                                        invokedExpression,
                                        nameof(StringBuilder.Append),
                                        false,
                                        [collectionCreation.SingleElement.GetText()]));
                                break;
                        }
                    }
                    else
                    {
                        if (argument.Value.GetExpressionType().ToIType() is { } argumentType
                            && !argumentType.IsReadOnlySpanOfObject()
                            && MethodExists())
                        {
                            consumer.AddHighlighting(
                                new UseOtherMethodSuggestion(
                                    $"Use the '{nameof(StringBuilder.Append)}' method.",
                                    invocationExpression,
                                    invokedExpression,
                                    nameof(StringBuilder.Append),
                                    false,
                                    [argument.Value.GetText()]));
                        }
                    }
                    break;
            }
        }
    }

    /// <remarks>
    /// <c>builder.AppendJoin(separator, [])</c> → <c>builder</c><para/>
    /// <c>builder.AppendJoin(separator, [item])</c> → <c>builder.Append(item)</c>
    /// </remarks>
    static void AnalyzeAppendJoin_Char_IEnumerableOfT(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument valuesArgument)
    {
        if (CollectionCreation.TryFrom(valuesArgument.Value) is { } collectionCreation)
        {
            switch (collectionCreation.Count)
            {
                case 0:
                    consumer.AddHighlighting(
                        new RedundantMethodInvocationHint(
                            "Calling 'AppendJoin' with an empty array is redundant.", // todo: use 'nameof(StringBuilder.AppendJoin)'
                            invocationExpression,
                            invokedExpression));
                    return;

                case 1 when PredefinedType.STRING_BUILDER_FQN.HasMethod(
                    new MethodSignature { Name = nameof(StringBuilder.Append), ParameterTypes = ParameterTypes.Object },
                    invocationExpression.PsiModule):

                    consumer.AddHighlighting(
                        new UseOtherMethodSuggestion(
                            $"Use the '{nameof(StringBuilder.Append)}' method.",
                            invocationExpression,
                            invokedExpression,
                            nameof(StringBuilder.Append),
                            false,
                            [collectionCreation.SingleElement.GetText()]));
                    return;
            }
        }
    }

    /// <remarks>
    /// <c>builder.AppendJoin(separator, [])</c> → <c>builder</c><para/>
    /// <c>builder.AppendJoin(separator, [item])</c> → <c>builder.Append(item)</c>
    /// </remarks>
    static void AnalyzeAppendJoin_Char_StringArray(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        TreeNodeCollection<ICSharpArgument?> arguments)
    {
        [Pure]
        bool MethodExists()
            => PredefinedType.STRING_BUILDER_FQN.HasMethod(
                new MethodSignature { Name = nameof(StringBuilder.Append), ParameterTypes = ParameterTypes.String },
                invocationExpression.PsiModule);

        if (arguments.TrySplit(out _, out var valuesArguments))
        {
            switch (valuesArguments)
            {
                case []:
                    consumer.AddHighlighting(
                        new RedundantMethodInvocationHint(
                            "Calling 'AppendJoin' with an empty array is redundant.", // todo: use 'nameof(StringBuilder.AppendJoin)'
                            invocationExpression,
                            invokedExpression));
                    break;

                case [{ Value: { } } argument]:
                    if (CollectionCreation.TryFrom(argument.Value) is { } collectionCreation)
                    {
                        switch (collectionCreation.Count)
                        {
                            case 0:
                                consumer.AddHighlighting(
                                    new RedundantMethodInvocationHint(
                                        "Calling 'AppendJoin' with an empty array is redundant.", // todo: use 'nameof(StringBuilder.AppendJoin)'
                                        invocationExpression,
                                        invokedExpression));
                                break;

                            case 1 when MethodExists():
                                consumer.AddHighlighting(
                                    new UseOtherMethodSuggestion(
                                        $"Use the '{nameof(StringBuilder.Append)}' method.",
                                        invocationExpression,
                                        invokedExpression,
                                        nameof(StringBuilder.Append),
                                        false,
                                        [collectionCreation.SingleElement.GetText()]));
                                break;
                        }
                    }
                    else
                    {
                        if (argument.Value.GetExpressionType().ToIType() is { } argumentType
                            && !argumentType.IsGenericArrayOfString()
                            && MethodExists())
                        {
                            consumer.AddHighlighting(
                                new UseOtherMethodSuggestion(
                                    $"Use the '{nameof(StringBuilder.Append)}' method.",
                                    invocationExpression,
                                    invokedExpression,
                                    nameof(StringBuilder.Append),
                                    false,
                                    [argument.Value.GetText()]));
                        }
                    }
                    break;
            }
        }
    }

    /// <remarks>
    /// <c>builder.AppendJoin(separator, default(ReadOnlySpan&lt;string?&gt;))</c> → <c>builder</c><para/>
    /// <c>builder.AppendJoin(separator, new ReadOnlySpan&lt;string?&gt;())</c> → <c>builder</c><para/>
    /// <c>builder.AppendJoin(separator, [item])</c> → <c>builder.Append(item)</c><para/>
    /// <c>builder.AppendJoin(separator, item)</c> → <c>builder.Append(item)</c>
    /// </remarks>
    static void AnalyzeAppendJoin_Char_ReadOnlySpanOfString(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        TreeNodeCollection<ICSharpArgument?> arguments)
    {
        [Pure]
        bool MethodExists()
            => PredefinedType.STRING_BUILDER_FQN.HasMethod(
                new MethodSignature { Name = nameof(StringBuilder.Append), ParameterTypes = ParameterTypes.String },
                invocationExpression.PsiModule);

        if (arguments.TrySplit(out _, out var valuesArguments))
        {
            switch (valuesArguments)
            {
                case []:
                    consumer.AddHighlighting(
                        new RedundantMethodInvocationHint(
                            "Calling 'AppendJoin' with an empty span is redundant.", // todo: use 'nameof(StringBuilder.AppendJoin)'
                            invocationExpression,
                            invokedExpression));
                    break;

                case [{ Value: { } } argument]:
                    if (CollectionCreation.TryFrom(argument.Value) is { } collectionCreation)
                    {
                        switch (collectionCreation.Count)
                        {
                            case 0:
                                consumer.AddHighlighting(
                                    new RedundantMethodInvocationHint(
                                        "Calling 'AppendJoin' with an empty span is redundant.", // todo: use 'nameof(StringBuilder.AppendJoin)'
                                        invocationExpression,
                                        invokedExpression));
                                break;

                            case 1 when MethodExists():
                                consumer.AddHighlighting(
                                    new UseOtherMethodSuggestion(
                                        $"Use the '{nameof(StringBuilder.Append)}' method.",
                                        invocationExpression,
                                        invokedExpression,
                                        nameof(StringBuilder.Append),
                                        false,
                                        [collectionCreation.SingleElement.GetText()]));
                                break;
                        }
                    }
                    else
                    {
                        if (argument.Value.GetExpressionType().ToIType() is { } argumentType
                            && !argumentType.IsReadOnlySpanOfString()
                            && MethodExists())
                        {
                            consumer.AddHighlighting(
                                new UseOtherMethodSuggestion(
                                    $"Use the '{nameof(StringBuilder.Append)}' method.",
                                    invocationExpression,
                                    invokedExpression,
                                    nameof(StringBuilder.Append),
                                    false,
                                    [argument.Value.GetText()]));
                        }
                    }
                    break;
            }
        }
    }

    /// <remarks>
    /// <c>builder.Insert(index, value, 1)</c> → <c>builder.Insert(index, value)</c><para/>
    /// <c>builder.Insert(index, "a", 1)</c> → <c>builder.Insert(index, 'a')</c>
    /// </remarks>
    static void AnalyzeInsert_Int32_String_Int32(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument valueArgument,
        ICSharpArgument countArgument)
    {
        switch (valueArgument.Value.TryGetStringConstant(), countArgument.Value.TryGetInt32Constant())
        {
            case (null, 1) when PredefinedType.STRING_BUILDER_FQN.HasMethod(
                new MethodSignature { Name = nameof(StringBuilder.Insert), ParameterTypes = ParameterTypes.Int32_String },
                invocationExpression.PsiModule):

                consumer.AddHighlighting(new RedundantArgumentHint("Passing 1 is redundant.", countArgument));
                break;

            case ([var character], 1) when PredefinedType.STRING_BUILDER_FQN.HasMethod(
                new MethodSignature { Name = nameof(StringBuilder.Insert), ParameterTypes = ParameterTypes.Int32_Char },
                valueArgument.NameIdentifier is { },
                out var parameterNames,
                invocationExpression.PsiModule):

                consumer.AddHighlighting(
                    new PassSingleCharacterSuggestion(
                        "Pass the single character.",
                        valueArgument,
                        parameterNames is [_, var valueParameterName] ? valueParameterName : null,
                        character,
                        redundantArguments: [countArgument]));
                break;
        }
    }

    /// <remarks>
    /// <c>builder.Insert(index, "a")</c> → <c>builder.Insert(int, 'a')</c>
    /// </remarks>
    static void AnalyzeInsert_Int32_String(IHighlightingConsumer consumer, IInvocationExpression invocationExpression, ICSharpArgument valueArgument)
    {
        if (valueArgument.Value.TryGetStringConstant() is [var character]
            && PredefinedType.STRING_BUILDER_FQN.HasMethod(
                new MethodSignature { Name = nameof(StringBuilder.Insert), ParameterTypes = ParameterTypes.Int32_Char },
                valueArgument.NameIdentifier is { },
                out var parameterNames,
                invocationExpression.PsiModule))
        {
            consumer.AddHighlighting(
                new PassSingleCharacterSuggestion(
                    "Pass the single character.",
                    valueArgument,
                    parameterNames is [_, var valueParameterName] ? valueParameterName : null,
                    character));
        }
    }

    /// <remarks>
    /// <c>builder.Insert(index, null)</c> → <c>builder</c>
    /// </remarks>
    static void AnalyzeInsert_Int32_Object(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument valueArgument)
    {
        if (valueArgument.Value.IsDefaultValue())
        {
            consumer.AddHighlighting(
                new RedundantMethodInvocationHint(
                    $"Calling '{nameof(StringBuilder.Insert)}' with null is redundant.",
                    invocationExpression,
                    invokedExpression));
        }
    }

    /// <remarks>
    /// <c>builder.Replace("abc", "abc")</c> → <c>text</c><para/>
    /// <c>builder.Replace("a", "b")</c> → <c>text.Replace('a', 'b')</c>
    /// </remarks>
    static void AnalyzeReplace_String_String(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument oldValueArgument,
        ICSharpArgument newValueArgument)
    {
        if (oldValueArgument.Value.TryGetStringConstant() is { } oldValue and not "" && newValueArgument.Value.TryGetStringConstant() is { } newValue)
        {
            if (oldValue == newValue)
            {
                consumer.AddHighlighting(
                    new RedundantMethodInvocationHint(
                        $"Calling '{nameof(StringBuilder.Replace)}' with identical values is redundant.",
                        invocationExpression,
                        invokedExpression));
                return;
            }

            if (oldValue is [var oldCharacter]
                && newValue is [var newCharacter]
                && PredefinedType.STRING_BUILDER_FQN.HasMethod(
                    new MethodSignature { Name = nameof(StringBuilder.Replace), ParameterTypes = ParameterTypes.Char_Char },
                    oldValueArgument.NameIdentifier is { } || newValueArgument.NameIdentifier is { },
                    out var parameterNames,
                    invocationExpression.PsiModule))
            {
                var highlighting = new PassSingleCharactersSuggestion(
                    "Pass the single character.",
                    [oldValueArgument, newValueArgument],
                    parameterNames is [var oldCharParameterName, var newCharParameterName]
                        ?
                        [
                            oldValueArgument.NameIdentifier is { } ? oldCharParameterName : null,
                            newValueArgument.NameIdentifier is { } ? newCharParameterName : null,
                        ]
                        : new string?[2],
                    [oldCharacter, newCharacter]);

                consumer.AddHighlighting(highlighting, oldValueArgument.Value.GetDocumentRange());
                consumer.AddHighlighting(highlighting, newValueArgument.Value.GetDocumentRange());
            }
        }
    }

    /// <remarks>
    /// <c>builder.Replace('a', 'a')</c> → <c>text</c>
    /// </remarks>
    static void AnalyzeReplace_Char_Char(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument oldCharArgument,
        ICSharpArgument newCharArgument)
    {
        if (oldCharArgument.Value.TryGetCharConstant() is { } oldCharacter
            && newCharArgument.Value.TryGetCharConstant() is { } newCharacter
            && oldCharacter == newCharacter)
        {
            consumer.AddHighlighting(
                new RedundantMethodInvocationHint(
                    $"Calling '{nameof(StringBuilder.Replace)}' with identical characters is redundant.",
                    invocationExpression,
                    invokedExpression));
        }
    }

    /// <remarks>
    /// <c>builder.Replace("a", "b", startIndex, count)</c> → <c>text.Replace('a', 'b', startIndex, count)</c>
    /// </remarks>
    static void AnalyzeReplace_String_String_Int32_Int32(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument oldValueArgument,
        ICSharpArgument newValueArgument)
    {
        if (oldValueArgument.Value.TryGetStringConstant() is [var oldCharacter]
            && newValueArgument.Value.TryGetStringConstant() is [var newCharacter]
            && PredefinedType.STRING_BUILDER_FQN.HasMethod(
                new MethodSignature { Name = nameof(StringBuilder.Replace), ParameterTypes = ParameterTypes.Char_Char_Int32_Int32 },
                oldValueArgument.NameIdentifier is { } || newValueArgument.NameIdentifier is { },
                out var parameterNames,
                invocationExpression.PsiModule))
        {
            var highlighting = new PassSingleCharactersSuggestion(
                "Pass the single character.",
                [oldValueArgument, newValueArgument],
                parameterNames is [var oldCharParameterName, var newCharParameterName]
                    ?
                    [
                        oldValueArgument.NameIdentifier is { } ? oldCharParameterName : null,
                        newValueArgument.NameIdentifier is { } ? newCharParameterName : null,
                    ]
                    : new string?[2],
                [oldCharacter, newCharacter]);

            consumer.AddHighlighting(highlighting, oldValueArgument.Value.GetDocumentRange());
            consumer.AddHighlighting(highlighting, newValueArgument.Value.GetDocumentRange());
        }
    }

    protected override void Run(IInvocationExpression element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
    {
        if (element is { InvokedExpression: IReferenceExpression { QualifierExpression: { }, Reference: var reference } invokedExpression }
            && reference.Resolve().DeclaredElement is IMethod
            {
                IsStatic: false, AccessibilityDomain.DomainType: AccessibilityDomain.AccessibilityDomainType.PUBLIC,
            } method
            && method.ContainingType.IsClrType(PredefinedType.STRING_BUILDER_FQN))
        {
            switch (method.ShortName, method.TypeParameters)
            {
                case (nameof(StringBuilder.Append), []):
                    switch (method.Parameters, element.TryGetArgumentsInDeclarationOrder())
                    {
                        case ([{ Type: var valueType }, { Type: var repeatCountType }], [_, { } repeatCountArgument])
                            when valueType.IsChar() && repeatCountType.IsInt():

                            AnalyzeAppend_Char_Int32(consumer, element, invokedExpression, repeatCountArgument);
                            break;

                        case ([{ Type: var valueType }], [{ } valueArgument]) when valueType.IsGenericArrayOfChar():
                            AnalyzeAppend_CharArray(consumer, element, invokedExpression, valueArgument);
                            break;

                        case ([{ Type: var valueType }, { Type: var startIndexType }, { Type: var charCountType }], [
                            { } valueArgument, { } startIndexArgument, { } charCountArgument,
                        ]) when valueType.IsGenericArrayOfChar() && startIndexType.IsInt() && charCountType.IsInt():
                            AnalyzeAppend_CharArray_Int32_Int32(
                                consumer,
                                element,
                                invokedExpression,
                                valueArgument,
                                startIndexArgument,
                                charCountArgument);
                            break;

                        case ([{ Type: var valueType }], [{ } valueArgument]) when valueType.IsString():
                            AnalyzeAppend_String(consumer, element, invokedExpression, valueArgument);
                            break;

                        case ([{ Type: var valueType }, { Type: var startIndexType }, { Type: var countType }], [
                            { } valueArgument, { } startIndexArgument, { } countArgument,
                        ]) when valueType.IsString() && startIndexType.IsInt() && countType.IsInt():
                            AnalyzeAppend_String_Int32_Int32(consumer, element, invokedExpression, valueArgument, startIndexArgument, countArgument);
                            break;

                        case ([{ Type: var valueType }], [{ } valueArgument]) when valueType.IsClrType(PredefinedType.STRING_BUILDER_FQN):
                            AnalyzeAppend_StringBuilder(consumer, element, invokedExpression, valueArgument);
                            break;

                        case ([{ Type: var valueType }, { Type: var startIndexType }, { Type: var countType }], [
                            { } valueArgument, { } startIndexArgument, { } countArgument,
                        ]) when valueType.IsClrType(PredefinedType.STRING_BUILDER_FQN) && startIndexType.IsInt() && countType.IsInt():
                            AnalyzeAppend_StringBuilder_Int32_Int32(
                                consumer,
                                element,
                                invokedExpression,
                                valueArgument,
                                startIndexArgument,
                                countArgument);
                            break;

                        case ([{ Type: var valueType }], [{ } valueArgument]) when valueType.IsObject():
                            AnalyzeAppend_Object(consumer, element, invokedExpression, valueArgument);
                            break;
                    }
                    break;

                case ("AppendJoin", _): // todo: use 'nameof(StringBuilder.AppendJoin)' when available
                    switch (method.TypeParameters, method.Parameters, element.TryGetArgumentsInDeclarationOrder())
                    {
                        case ([], [{ Type: var separatorType }, { Type: var valuesType }], { } arguments)
                            when separatorType.IsString() && valuesType.IsGenericArrayOfObject():

                            AnalyzeAppendJoin_String_ObjectArray(consumer, element, invokedExpression, arguments);
                            break;

                        case ([], [{ Type: var separatorType }, { Type: var valuesType }], { } arguments)
                            when separatorType.IsString() && valuesType.IsReadOnlySpanOfObject():

                            AnalyzeAppendJoin_String_ReadOnlySpanOfObject(consumer, element, invokedExpression, arguments);
                            break;

                        case ([_], [{ Type: var separatorType }, { Type: var valuesType }], [
                            { } separatorArgument, { } valuesArgument,
                        ]) when separatorType.IsString() && valuesType.IsGenericIEnumerable():

                            AnalyzeAppendJoin_String_IEnumerableOfT(consumer, element, invokedExpression, separatorArgument, valuesArgument);
                            break;

                        case ([], [{ Type: var separatorType }, { Type: var valuesType }], { } arguments)
                            when separatorType.IsString() && valuesType.IsGenericArrayOfString():

                            AnalyzeAppendJoin_String_StringArray(consumer, element, invokedExpression, arguments);
                            break;

                        case ([], [{ Type: var separatorType }, { Type: var valuesType }], { } arguments)
                            when separatorType.IsString() && valuesType.IsReadOnlySpanOfString():

                            AnalyzeAppendJoin_String_ReadOnlySpanOfString(consumer, element, invokedExpression, arguments);
                            break;

                        case ([], [{ Type: var separatorType }, { Type: var valuesType }], { } arguments)
                            when separatorType.IsChar() && valuesType.IsGenericArrayOfObject():

                            AnalyzeAppendJoin_Char_ObjectArray(consumer, element, invokedExpression, arguments);
                            break;

                        case ([], [{ Type: var separatorType }, { Type: var valuesType }], { } arguments)
                            when separatorType.IsChar() && valuesType.IsReadOnlySpanOfObject():

                            AnalyzeAppendJoin_Char_ReadOnlySpanOfObject(consumer, element, invokedExpression, arguments);
                            break;

                        case ([_], [{ Type: var separatorType }, { Type: var valuesType }], [_, { } valuesArgument])
                            when separatorType.IsChar() && valuesType.IsGenericIEnumerable():

                            AnalyzeAppendJoin_Char_IEnumerableOfT(consumer, element, invokedExpression, valuesArgument);
                            break;

                        case ([], [{ Type: var separatorType }, { Type: var valuesType }], { } arguments)
                            when separatorType.IsChar() && valuesType.IsGenericArrayOfString():

                            AnalyzeAppendJoin_Char_StringArray(consumer, element, invokedExpression, arguments);
                            break;

                        case ([], [{ Type: var separatorType }, { Type: var valuesType }], { } arguments)
                            when separatorType.IsChar() && valuesType.IsReadOnlySpanOfString():

                            AnalyzeAppendJoin_Char_ReadOnlySpanOfString(consumer, element, invokedExpression, arguments);
                            break;
                    }
                    break;

                case (nameof(StringBuilder.Insert), []):
                    switch (method.Parameters, element.TryGetArgumentsInDeclarationOrder())
                    {
                        case ([{ Type: var indexType }, { Type: var valueType }, { Type: var countType }], [_, { } valueArgument, { } countArgument])
                            when indexType.IsInt() && valueType.IsString() && countType.IsInt():

                            AnalyzeInsert_Int32_String_Int32(consumer, element, valueArgument, countArgument);
                            break;

                        case ([{ Type: var indexType }, { Type: var valueType }], [_, { } valueArgument])
                            when indexType.IsInt() && valueType.IsString():

                            AnalyzeInsert_Int32_String(consumer, element, valueArgument);
                            break;

                        case ([{ Type: var indexType }, { Type: var valueType }], [_, { } valueArgument])
                            when indexType.IsInt() && valueType.IsObject():

                            AnalyzeInsert_Int32_Object(consumer, element, invokedExpression, valueArgument);
                            break;
                    }
                    break;

                case (nameof(StringBuilder.Replace), []):
                    switch (method.Parameters, element.TryGetArgumentsInDeclarationOrder())
                    {
                        case ([{ Type: var oldValueType }, { Type: var newValueType }], [{ } oldValueArgument, { } newValueArgument])
                            when oldValueType.IsString() && newValueType.IsString():

                            AnalyzeReplace_String_String(consumer, element, invokedExpression, oldValueArgument, newValueArgument);
                            break;

                        case ([{ Type: var oldCharType }, { Type: var newCharType }], [{ } oldCharArgument, { } newCharArgument])
                            when oldCharType.IsChar() && newCharType.IsChar():

                            AnalyzeReplace_Char_Char(consumer, element, invokedExpression, oldCharArgument, newCharArgument);
                            break;

                        case ([{ Type: var oldValueType }, { Type: var newValueType }, { Type: var startIndexType }, { Type: var countType }], [
                            { } oldValueArgument, { } newValueArgument, _, _,
                        ]) when oldValueType.IsString() && newValueType.IsString() && startIndexType.IsInt() && countType.IsInt():
                            AnalyzeReplace_String_String_Int32_Int32(consumer, element, oldValueArgument, newValueArgument);
                            break;
                    }
                    break;
            }
        }
    }
}