using System.Text;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using ReCommendedExtension.Analyzers.BaseTypes.Collections;
using ReCommendedExtension.Extensions;
using ReCommendedExtension.Extensions.MethodFinding;

namespace ReCommendedExtension.Analyzers.BaseTypes.Analyzers;

/// <remarks>
/// C# language version checks are only done when a quick fix would require it.
/// </remarks>
[ElementProblemAnalyzer(typeof(IInvocationExpression), HighlightingTypes = [typeof(UseOtherMethodSuggestion)])]
public sealed class StringBuilderAnalyzer : ElementProblemAnalyzer<IInvocationExpression>
{
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Underscore character used intentionally as a separator.")]
    static class Parameters
    {
        public static IReadOnlyList<Parameter> Object { get; } = [Parameter.Object];

        public static IReadOnlyList<Parameter> String { get; } = [Parameter.String];
    }

    /// <remarks>
    /// <c>builder.AppendJoin(separator, [item])</c> → <c>builder.Append(item)</c>
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
                new MethodSignature { Name = nameof(StringBuilder.Append), Parameters = Parameters.Object },
                invocationExpression.PsiModule);

        if (arguments.TrySplit(out _, out var valuesArguments) && valuesArguments is [{ Value: { } } argument])
        {
            if (CollectionCreation.TryFrom(argument.Value) is { } collectionCreation)
            {
                if (collectionCreation.Count == 1 && MethodExists())
                {
                    consumer.AddHighlighting(
                        new UseOtherMethodSuggestion(
                            $"Use the '{nameof(StringBuilder.Append)}' method.",
                            invocationExpression,
                            invokedExpression,
                            nameof(StringBuilder.Append),
                            false,
                            [collectionCreation.SingleElement.GetText()]));
                }
            }
            else
            {
                if (argument.Value.GetExpressionType().ToIType() is { } argumentType && !argumentType.IsGenericArrayOfObject() && MethodExists())
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
        }
    }

    /// <remarks>
    /// <c>builder.AppendJoin(separator, [item])</c> → <c>builder.Append(item)</c><para/>
    /// <c>builder.AppendJoin(separator, item)</c> → <c>builder.Append(item)</c>
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
                new MethodSignature { Name = nameof(StringBuilder.Append), Parameters = Parameters.Object },
                invocationExpression.PsiModule);

        if (arguments.TrySplit(out _, out var valuesArguments) && valuesArguments is [{ Value: { } } argument])
        {
            if (CollectionCreation.TryFrom(argument.Value) is { } collectionCreation)
            {
                if (collectionCreation.Count == 1 && MethodExists())
                {
                    consumer.AddHighlighting(
                        new UseOtherMethodSuggestion(
                            $"Use the '{nameof(StringBuilder.Append)}' method.",
                            invocationExpression,
                            invokedExpression,
                            nameof(StringBuilder.Append),
                            false,
                            [collectionCreation.SingleElement.GetText()]));
                }
            }
            else
            {
                if (argument.Value.GetExpressionType().ToIType() is { } argumentType && !argumentType.IsReadOnlySpanOfObject() && MethodExists())
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
        }
    }

    /// <remarks>
    /// <c>builder.AppendJoin(separator, [item])</c> → <c>builder.Append(item)</c>
    /// </remarks>
    static void AnalyzeAppendJoin_String_IEnumerableOfT(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument valuesArgument)
    {
        if (CollectionCreation.TryFrom(valuesArgument.Value) is { Count: 1 } collectionCreation
            && PredefinedType.STRING_BUILDER_FQN.HasMethod(
                new MethodSignature { Name = nameof(StringBuilder.Append), Parameters = Parameters.Object },
                invocationExpression.PsiModule))
        {
            consumer.AddHighlighting(
                new UseOtherMethodSuggestion(
                    $"Use the '{nameof(StringBuilder.Append)}' method.",
                    invocationExpression,
                    invokedExpression,
                    nameof(StringBuilder.Append),
                    false,
                    [collectionCreation.SingleElement.GetText()]));
        }
    }

    /// <remarks>
    /// <c>builder.AppendJoin(separator, [item])</c> → <c>builder.Append(item)</c>
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
                new MethodSignature { Name = nameof(StringBuilder.Append), Parameters = Parameters.String },
                invocationExpression.PsiModule);

        if (arguments.TrySplit(out _, out var valuesArguments) && valuesArguments is [{ Value: { } } argument])
        {
            if (CollectionCreation.TryFrom(argument.Value) is { } collectionCreation)
            {
                if (collectionCreation.Count == 1 && MethodExists())
                {
                    consumer.AddHighlighting(
                        new UseOtherMethodSuggestion(
                            $"Use the '{nameof(StringBuilder.Append)}' method.",
                            invocationExpression,
                            invokedExpression,
                            nameof(StringBuilder.Append),
                            false,
                            [collectionCreation.SingleElement.GetText()]));
                }
            }
            else
            {
                if (argument.Value.GetExpressionType().ToIType() is { } argumentType && !argumentType.IsGenericArrayOfString() && MethodExists())
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
        }
    }

    /// <remarks>
    /// <c>builder.AppendJoin(separator, [item])</c> → <c>builder.Append(item)</c><para/>
    /// <c>builder.AppendJoin(separator, item)</c> → <c>builder.Append(item)</c>
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
                new MethodSignature { Name = nameof(StringBuilder.Append), Parameters = Parameters.String },
                invocationExpression.PsiModule);

        if (arguments.TrySplit(out _, out var valuesArguments) && valuesArguments is [{ Value: { } } argument])
        {
            if (CollectionCreation.TryFrom(argument.Value) is { } collectionCreation)
            {
                if (collectionCreation.Count == 1 && MethodExists())
                {
                    consumer.AddHighlighting(
                        new UseOtherMethodSuggestion(
                            $"Use the '{nameof(StringBuilder.Append)}' method.",
                            invocationExpression,
                            invokedExpression,
                            nameof(StringBuilder.Append),
                            false,
                            [collectionCreation.SingleElement.GetText()]));
                }
            }
            else
            {
                if (argument.Value.GetExpressionType().ToIType() is { } argumentType && !argumentType.IsReadOnlySpanOfString() && MethodExists())
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
        }
    }

    /// <remarks>
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
                new MethodSignature { Name = nameof(StringBuilder.Append), Parameters = Parameters.Object },
                invocationExpression.PsiModule);

        if (arguments.TrySplit(out _, out var valuesArguments) && valuesArguments is [{ Value: { } } argument])
        {
            if (CollectionCreation.TryFrom(argument.Value) is { } collectionCreation)
            {
                if (collectionCreation.Count == 1 && MethodExists())
                {
                    consumer.AddHighlighting(
                        new UseOtherMethodSuggestion(
                            $"Use the '{nameof(StringBuilder.Append)}' method.",
                            invocationExpression,
                            invokedExpression,
                            nameof(StringBuilder.Append),
                            false,
                            [collectionCreation.SingleElement.GetText()]));
                }
            }
            else
            {
                if (argument.Value.GetExpressionType().ToIType() is { } argumentType && !argumentType.IsGenericArrayOfObject() && MethodExists())
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
        }
    }

    /// <remarks>
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
                new MethodSignature { Name = nameof(StringBuilder.Append), Parameters = Parameters.Object },
                invocationExpression.PsiModule);

        if (arguments.TrySplit(out _, out var valuesArguments) && valuesArguments is [{ Value: { } } argument])
        {
            if (CollectionCreation.TryFrom(argument.Value) is { } collectionCreation)
            {
                if (collectionCreation.Count == 1 && MethodExists())
                {
                    consumer.AddHighlighting(
                        new UseOtherMethodSuggestion(
                            $"Use the '{nameof(StringBuilder.Append)}' method.",
                            invocationExpression,
                            invokedExpression,
                            nameof(StringBuilder.Append),
                            false,
                            [collectionCreation.SingleElement.GetText()]));
                }
            }
            else
            {
                if (argument.Value.GetExpressionType().ToIType() is { } argumentType && !argumentType.IsReadOnlySpanOfObject() && MethodExists())
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
        }
    }

    /// <remarks>
    /// <c>builder.AppendJoin(separator, [item])</c> → <c>builder.Append(item)</c>
    /// </remarks>
    static void AnalyzeAppendJoin_Char_IEnumerableOfT(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument valuesArgument)
    {
        if (CollectionCreation.TryFrom(valuesArgument.Value) is { Count: 1 } collectionCreation
            && PredefinedType.STRING_BUILDER_FQN.HasMethod(
                new MethodSignature { Name = nameof(StringBuilder.Append), Parameters = Parameters.Object },
                invocationExpression.PsiModule))
        {
            consumer.AddHighlighting(
                new UseOtherMethodSuggestion(
                    $"Use the '{nameof(StringBuilder.Append)}' method.",
                    invocationExpression,
                    invokedExpression,
                    nameof(StringBuilder.Append),
                    false,
                    [collectionCreation.SingleElement.GetText()]));
        }
    }

    /// <remarks>
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
                new MethodSignature { Name = nameof(StringBuilder.Append), Parameters = Parameters.String },
                invocationExpression.PsiModule);

        if (arguments.TrySplit(out _, out var valuesArguments) && valuesArguments is [{ Value: { } } argument])
        {
            if (CollectionCreation.TryFrom(argument.Value) is { } collectionCreation)
            {
                if (collectionCreation.Count == 1 && MethodExists())
                {
                    consumer.AddHighlighting(
                        new UseOtherMethodSuggestion(
                            $"Use the '{nameof(StringBuilder.Append)}' method.",
                            invocationExpression,
                            invokedExpression,
                            nameof(StringBuilder.Append),
                            false,
                            [collectionCreation.SingleElement.GetText()]));
                }
            }
            else
            {
                if (argument.Value.GetExpressionType().ToIType() is { } argumentType && !argumentType.IsGenericArrayOfString() && MethodExists())
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
        }
    }

    /// <remarks>
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
                new MethodSignature { Name = nameof(StringBuilder.Append), Parameters = Parameters.String },
                invocationExpression.PsiModule);

        if (arguments.TrySplit(out _, out var valuesArguments) && valuesArguments is [{ Value: { } } argument])
        {
            if (CollectionCreation.TryFrom(argument.Value) is { } collectionCreation)
            {
                if (collectionCreation.Count == 1 && MethodExists())
                {
                    consumer.AddHighlighting(
                        new UseOtherMethodSuggestion(
                            $"Use the '{nameof(StringBuilder.Append)}' method.",
                            invocationExpression,
                            invokedExpression,
                            nameof(StringBuilder.Append),
                            false,
                            [collectionCreation.SingleElement.GetText()]));
                }
            }
            else
            {
                if (argument.Value.GetExpressionType().ToIType() is { } argumentType && !argumentType.IsReadOnlySpanOfString() && MethodExists())
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

                        case ([_], [{ Type: var separatorType }, { Type: var valuesType }], [_, { } valuesArgument])
                            when separatorType.IsString() && valuesType.IsGenericIEnumerable():

                            AnalyzeAppendJoin_String_IEnumerableOfT(consumer, element, invokedExpression, valuesArgument);
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
            }
        }
    }
}