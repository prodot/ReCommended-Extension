using JetBrains.Metadata.Reader.API;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Parsing;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;

namespace ReCommendedExtension.Analyzers.Strings;

/// <remarks>
/// C# language version checks are only done when a quick fix would require it.
/// </remarks>
[ElementProblemAnalyzer(
    typeof(IInvocationExpression),
    HighlightingTypes =
    [
        typeof(UseExpressionResultSuggestion),
        typeof(PassSingleCharacterSuggestion),
        typeof(PassSingleCharactersSuggestion),
        typeof(UseStringListPatternSuggestion),
        typeof(UseOtherMethodSuggestion),
        typeof(RedundantArgumentHint),
        typeof(RedundantElementHint),
        typeof(UseStringPropertySuggestion),
        typeof(RedundantMethodInvocationHint),
        typeof(UseRangeIndexerSuggestion),
    ])]
public sealed class StringAnalyzer : ElementProblemAnalyzer<IInvocationExpression>
{
    abstract class Collection
    {
        [Pure]
        public static Collection? TryFrom(ICSharpExpression? expression)
            => expression switch
            {
                ICollectionExpression collectionExpression => new CollectionExpressionCollection(collectionExpression),

                IArrayCreationExpression { ArrayInitializer: { } } arrayCreationExpression => new ArrayCreationExpressionCollection(
                    arrayCreationExpression),

                _ => null,
            };

        IReadOnlyList<string?>? stringConstants;

        protected abstract IEnumerable<(IInitializerElement, ICSharpExpression)> ElementsWithExpressions { get; }

        public abstract ICSharpExpression Expression { get; }

        [NonNegativeValue]
        public abstract int Count { get; }

        public abstract IEnumerable<IInitializerElement> Elements { get; }

        public IEnumerable<(IInitializerElement, char)> ElementsWithCharConstants
        {
            get
            {
                foreach (var (element, expression) in ElementsWithExpressions)
                {
                    if (TryGetCharConstant(expression) is { } charConstant)
                    {
                        yield return (element, charConstant);
                    }
                }
            }
        }

        public IEnumerable<(IInitializerElement, string)> ElementsWithStringConstants
        {
            get
            {
                foreach (var (element, expression) in ElementsWithExpressions)
                {
                    if (TryGetStringConstant(expression) is { } stringConstant)
                    {
                        yield return (element, stringConstant);
                    }
                }
            }
        }

        public IReadOnlyList<string?> StringConstants
        {
            get
            {
                if (stringConstants is not { })
                {
                    var array = new string?[Count];

                    var i = 0;
                    foreach (var (_, expression) in ElementsWithExpressions)
                    {
                        if (TryGetStringConstant(expression) is { } stringConstant)
                        {
                            array[i++] = stringConstant;
                        }
                    }

                    stringConstants = [..array];

                    Debug.Assert(StringConstants.Count == Count);
                }

                return stringConstants;
            }
        }
    }

    sealed class CollectionExpressionCollection : Collection
    {
        readonly ICollectionExpression collectionExpression;

        internal CollectionExpressionCollection(ICollectionExpression collectionExpression) => this.collectionExpression = collectionExpression;

        protected override IEnumerable<(IInitializerElement, ICSharpExpression)> ElementsWithExpressions
        {
            get
            {
                foreach (var element in collectionExpression.CollectionElements)
                {
                    if (element is IExpressionElement expressionElement)
                    {
                        yield return (element, expressionElement.Expression);
                    }
                }
            }
        }

        public override ICSharpExpression Expression => collectionExpression;

        public override int Count => collectionExpression.CollectionElements.Count;

        public override IEnumerable<IInitializerElement> Elements => collectionExpression.CollectionElements;
    }

    sealed class ArrayCreationExpressionCollection : Collection
    {
        readonly IArrayCreationExpression arrayCreationExpression;

        internal ArrayCreationExpressionCollection(IArrayCreationExpression arrayCreationExpression)
        {
            Debug.Assert(arrayCreationExpression.ArrayInitializer is { });

            this.arrayCreationExpression = arrayCreationExpression;
        }

        protected override IEnumerable<(IInitializerElement, ICSharpExpression)> ElementsWithExpressions
        {
            get
            {
                foreach (var element in arrayCreationExpression.ArrayInitializer.ElementInitializers)
                {
                    if (element is IExpressionInitializer elementInitializer)
                    {
                        yield return (element, elementInitializer.Value);
                    }
                }
            }
        }

        public override ICSharpExpression Expression => arrayCreationExpression;

        public override int Count => arrayCreationExpression.ArrayInitializer.ElementInitializers.Count;

        public override IEnumerable<IInitializerElement> Elements => arrayCreationExpression.ArrayInitializer.ElementInitializers;
    }

    [Pure]
    static bool IsStringComparison(IType type) => type.IsClrType(PredefinedType.STRING_COMPARISON_CLASS);

    [Pure]
    static bool IsStringSplitOptions(IType type) => type.IsClrType(ClrTypeNames.StringSplitOptions);

    [Pure]
    static string[]? TryGetParameterNamesIfMethodExists(string methodName, IClrTypeName[] parameterTypes, IPsiModule psiModule)
    {
        if (PredefinedType.STRING_FQN.TryGetTypeElement(psiModule) is { } stringType)
        {
            foreach (var method in stringType.Methods)
            {
                if (method is { TypeParameters: [] } && method.ShortName == methodName && method.Parameters.Count == parameterTypes.Length)
                {
                    if (parameterTypes is [])
                    {
                        return [];
                    }

                    var parameterNames = new string[parameterTypes.Length];

                    for (var i = 0; i < parameterNames.Length; i++)
                    {
                        if (parameterTypes[i].TryGetTypeElement(psiModule) is { } expectedType
                            && TypeEqualityComparer.Default.Equals(TypeFactory.CreateType(expectedType), method.Parameters[i].Type))
                        {
                            parameterNames[i] = method.Parameters[i].ShortName;
                        }
                    }

                    if (parameterNames.All(n => n is { }))
                    {
                        return parameterNames;
                    }
                }
            }
        }

        return null;
    }

    [Pure]
    static bool IsDefaultValue(ICSharpExpression? expression) => expression is { } && expression.IsDefaultValueOf(expression.Type());

    [Pure]
    static string? TryGetStringConstant(ICSharpExpression? expression)
        => expression is IConstantValueOwner { ConstantValue: { Kind: ConstantValueKind.String, StringValue: var value } } ? value : null;

    [Pure]
    static char? TryGetCharConstant(ICSharpExpression? expression)
        => expression is IConstantValueOwner { ConstantValue: { Kind: ConstantValueKind.Char, CharValue: var value } } ? value : null;

    [Pure]
    static int? TryGetInt32Constant(ICSharpExpression? expression)
        => expression is IConstantValueOwner { ConstantValue: { Kind: ConstantValueKind.Int, IntValue: var value } } ? value : null;

    [Pure]
    static StringComparison? TryGetStringComparisonConstant(ICSharpExpression? expression)
        => expression is IConstantValueOwner { ConstantValue: { Kind: ConstantValueKind.Enum, Type: var enumType } constantValue }
            && enumType.IsClrType(PredefinedType.STRING_COMPARISON_CLASS)
                ? (StringComparison)constantValue.IntValue
                : null;

    [Pure]
    static StringSplitOptions? TryGetStringSplitOptionsConstant(ICSharpExpression? expression)
        => expression is IConstantValueOwner { ConstantValue: { Kind: ConstantValueKind.Enum, Type: var enumType } constantValue }
            && enumType.IsClrType(ClrTypeNames.StringSplitOptions)
                ? (StringSplitOptions)constantValue.IntValue
                : null;

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
    static string CreateStringArray(string[] items, ICSharpExpression context)
    {
        if (context.GetCSharpLanguageLevel() >= CSharpLanguageLevel.CSharp120 && context.TryGetTargetType() is { })
        {
            return $"[{string.Join(", ", items)}]";
        }

        if (items is [])
        {
            if (ArrayEmptyMethodExists(context.GetPsiModule()))
            {
                return $"{nameof(Array)}.{nameof(Array.Empty)}<string>()";
            }

            return "new string[0]";
        }

        return $$"""new[] { {{string.Join(", ", items)}} }""";
    }

    /// <remarks>
    /// <c>text.Contains("")</c> → <c>true</c><para/>
    /// <c>text.Contains(string)</c> → <c>text.Contains(char)</c> (.NET Core 2.1)
    /// </remarks>
    static void AnalyzeContains_String(IHighlightingConsumer consumer, IInvocationExpression invocationExpression, ICSharpArgument valueArgument)
    {
        switch (TryGetStringConstant(valueArgument.Value))
        {
            case "" when !invocationExpression.IsUsedAsStatement():
                consumer.AddHighlighting(new UseExpressionResultSuggestion("The expression is always true.", invocationExpression, "true"));
                break;

            case [var character] when TryGetParameterNamesIfMethodExists(
                nameof(string.Contains),
                [PredefinedType.CHAR_FQN],
                invocationExpression.PsiModule) is [var valueParameterName]:

                consumer.AddHighlighting(
                    new PassSingleCharacterSuggestion(
                        "Pass the single character.",
                        valueArgument,
                        valueArgument.NameIdentifier is { } ? valueParameterName : null,
                        character));
                break;
        }
    }

    /// <remarks>
    /// <c>text.Contains("", StringComparison)</c> → <c>true</c><para/>
    /// <c>text.Contains(string, StringComparison)</c> → <c>text.Contains(char, StringComparison)</c> (.NET Core 2.1)
    /// </remarks>
    static void AnalyzeContains_String_StringComparison(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument valueArgument)
    {
        switch (TryGetStringConstant(valueArgument.Value))
        {
            case "" when !invocationExpression.IsUsedAsStatement():
                consumer.AddHighlighting(new UseExpressionResultSuggestion("The expression is always true.", invocationExpression, "true"));
                break;

            case [var character] when TryGetParameterNamesIfMethodExists(
                nameof(string.Contains),
                [PredefinedType.CHAR_FQN, PredefinedType.STRING_COMPARISON_CLASS],
                invocationExpression.PsiModule) is [var valueParameterName, _]:

                consumer.AddHighlighting(
                    new PassSingleCharacterSuggestion(
                        "Pass the single character.",
                        valueArgument,
                        valueArgument.NameIdentifier is { } ? valueParameterName : null,
                        character));
                break;
        }
    }

    /// <remarks>
    /// <c>text.EndsWith(char)</c> → <c>text is [.., var lastCharacter] &amp;&amp; lastCharacter == value</c> (C# 11)<para/>
    /// <c>text.EndsWith(char)</c> → <c>text is [.., value]</c> (C# 11)
    /// </remarks>
    static void AnalyzeEndsWith_Char(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument valueArgument)
    {
        if (invocationExpression.GetCSharpLanguageLevel() >= CSharpLanguageLevel.CSharp110
            && !invocationExpression.IsUsedAsStatement()
            && valueArgument.Value is { })
        {
            if (TryGetCharConstant(valueArgument.Value) is { } character)
            {
                consumer.AddHighlighting(
                    new UseStringListPatternSuggestion(
                        "Use list pattern.",
                        invocationExpression,
                        invokedExpression,
                        ListPatternSuggestionKind.LastCharacter,
                        [character]));
            }
            else
            {
                consumer.AddHighlighting(
                    new UseStringListPatternSuggestion(
                        "Use list pattern.",
                        invocationExpression,
                        invokedExpression,
                        ListPatternSuggestionKind.LastCharacter,
                        valueArgument.Value.GetText()));
            }
        }
    }

    /// <remarks>
    /// <c>text.EndsWith("")</c> → <c>true</c>
    /// </remarks>
    static void AnalyzeEndsWith_String(IHighlightingConsumer consumer, IInvocationExpression invocationExpression, ICSharpArgument valueArgument)
    {
        if (!invocationExpression.IsUsedAsStatement() && TryGetStringConstant(valueArgument.Value) == "")
        {
            consumer.AddHighlighting(new UseExpressionResultSuggestion("The expression is always true.", invocationExpression, "true"));
        }
    }

    /// <remarks>
    /// <c>text.EndsWith("", StringComparison)</c> → <c>true</c><para/>
    /// <c>text.EndsWith(string, Ordinal)</c> → <c>text is [.., c]</c> (C# 11)<para/>
    /// <c>text.EndsWith(string, OrdinalIgnoresCase)</c> → <c>text is [.., l or u]</c> (C# 11)
    /// </remarks>
    static void AnalyzeEndsWith_String_StringComparison(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument valueArgument,
        ICSharpArgument comparisonTypeArgument)
    {
        if (!invocationExpression.IsUsedAsStatement() && TryGetStringConstant(valueArgument.Value) == "")
        {
            consumer.AddHighlighting(new UseExpressionResultSuggestion("The expression is always true.", invocationExpression, "true"));
        }

        if (invocationExpression.GetCSharpLanguageLevel() >= CSharpLanguageLevel.CSharp110
            && !invocationExpression.IsUsedAsStatement()
            && TryGetStringConstant(valueArgument.Value) is [var character])
        {
            switch (TryGetStringComparisonConstant(comparisonTypeArgument.Value))
            {
                case StringComparison.Ordinal:
                    consumer.AddHighlighting(
                        new UseStringListPatternSuggestion(
                            "Use list pattern.",
                            invocationExpression,
                            invokedExpression,
                            ListPatternSuggestionKind.LastCharacter,
                            [character]));
                    break;

                case StringComparison.OrdinalIgnoreCase:
                    var lowerCaseCharacter = char.ToLowerInvariant(character);
                    var upperCaseCharacter = char.ToUpperInvariant(character);

                    if (lowerCaseCharacter == upperCaseCharacter)
                    {
                        consumer.AddHighlighting(
                            new UseStringListPatternSuggestion(
                                "Use list pattern.",
                                invocationExpression,
                                invokedExpression,
                                ListPatternSuggestionKind.LastCharacter,
                                [lowerCaseCharacter]));
                    }
                    else
                    {
                        consumer.AddHighlighting(
                            new UseStringListPatternSuggestion(
                                "Use list pattern.",
                                invocationExpression,
                                invokedExpression,
                                ListPatternSuggestionKind.LastCharacter,
                                [lowerCaseCharacter, upperCaseCharacter]));
                    }
                    break;
            }
        }
    }

    /// <remarks>
    /// <c>text.IndexOf(char) == 0</c> → <c>text is [var firstCharacter, ..] &amp;&amp; firstCharacter == value</c> (C# 11)<para/>
    /// <c>text.IndexOf(char) == 0</c> → <c>text is [value, ..]</c> (C# 11)<para/>
    /// <c>text.IndexOf(char) != 0</c> → <c>text is not [var firstCharacter, ..] || firstCharacter != value</c> (C# 11)<para/>
    /// <c>text.IndexOf(char) != 0</c> → <c>text is not [value, ..]</c> (C# 11)<para/>
    /// <c>text.IndexOf(char) > -1</c> → <c>text.Contains(char)</c> (.NET Core 2.1)<para/>
    /// <c>text.IndexOf(char) != -1</c> → <c>text.Contains(char) (.NET Core 2.1)</c><para/>
    /// <c>text.IndexOf(char) >= 0</c> → <c>text.Contains(char) (.NET Core 2.1)</c><para/>
    /// <c>text.IndexOf(char) == -1</c> → <c>!text.Contains(char) (.NET Core 2.1)</c><para/>
    /// <c>text.IndexOf(char) &lt; 0</c> → <c>!text.Contains(char) (.NET Core 2.1)</c>
    /// </remarks>
    static void AnalyzeIndexOf_Char(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument valueArgument)
    {
        switch (invocationExpression.Parent)
        {
            case IEqualityExpression equalityExpression when equalityExpression.LeftOperand == invocationExpression && valueArgument.Value is { }:
                switch (equalityExpression.EqualityType, TryGetInt32Constant(equalityExpression.RightOperand))
                {
                    case (EqualityExpressionType.EQEQ, 0):
                        if (invocationExpression.GetCSharpLanguageLevel() >= CSharpLanguageLevel.CSharp110)
                        {
                            if (TryGetCharConstant(valueArgument.Value) is { } character)
                            {
                                consumer.AddHighlighting(
                                    new UseStringListPatternSuggestion(
                                        "Use list pattern.",
                                        invocationExpression,
                                        invokedExpression,
                                        ListPatternSuggestionKind.FirstCharacter,
                                        [character],
                                        equalityExpression));
                            }
                            else
                            {
                                consumer.AddHighlighting(
                                    new UseStringListPatternSuggestion(
                                        "Use list pattern.",
                                        invocationExpression,
                                        invokedExpression,
                                        ListPatternSuggestionKind.FirstCharacter,
                                        valueArgument.Value.GetText(),
                                        equalityExpression));
                            }
                        }
                        break;

                    case (EqualityExpressionType.NE, 0):
                        if (invocationExpression.GetCSharpLanguageLevel() >= CSharpLanguageLevel.CSharp110)
                        {
                            if (TryGetCharConstant(valueArgument.Value) is { } character)
                            {
                                consumer.AddHighlighting(
                                    new UseStringListPatternSuggestion(
                                        "Use list pattern.",
                                        invocationExpression,
                                        invokedExpression,
                                        ListPatternSuggestionKind.NotFirstCharacter,
                                        [character],
                                        equalityExpression));
                            }
                            else
                            {
                                consumer.AddHighlighting(
                                    new UseStringListPatternSuggestion(
                                        "Use list pattern.",
                                        invocationExpression,
                                        invokedExpression,
                                        ListPatternSuggestionKind.NotFirstCharacter,
                                        valueArgument.Value.GetText(),
                                        equalityExpression));
                            }
                        }
                        break;

                    case (EqualityExpressionType.NE, -1):
                        if (TryGetParameterNamesIfMethodExists(
                                nameof(string.Contains),
                                [PredefinedType.CHAR_FQN],
                                invocationExpression.PsiModule) is [_])
                        {
                            consumer.AddHighlighting(
                                new UseOtherMethodSuggestion(
                                    $"Use the '{nameof(string.Contains)}' method.",
                                    invocationExpression,
                                    invokedExpression,
                                    nameof(string.Contains),
                                    false,
                                    [valueArgument.Value.GetText()],
                                    equalityExpression));
                        }
                        break;

                    case (EqualityExpressionType.EQEQ, -1):
                        if (TryGetParameterNamesIfMethodExists(
                                nameof(string.Contains),
                                [PredefinedType.CHAR_FQN],
                                invocationExpression.PsiModule) is [_])
                        {
                            consumer.AddHighlighting(
                                new UseOtherMethodSuggestion(
                                    $"Use the '{nameof(string.Contains)}' method.",
                                    invocationExpression,
                                    invokedExpression,
                                    nameof(string.Contains),
                                    true,
                                    [valueArgument.Value.GetText()],
                                    equalityExpression));
                        }
                        break;
                }
                break;

            case IRelationalExpression relationalExpression when relationalExpression.LeftOperand == invocationExpression
                && TryGetInt32Constant(relationalExpression.RightOperand) is { } value
                && valueArgument.Value is { }:

                var tokenType = relationalExpression.OperatorSign.GetTokenType();

                if ((tokenType == CSharpTokenType.GT && value == -1 || tokenType == CSharpTokenType.GE && value == 0)
                    && TryGetParameterNamesIfMethodExists(nameof(string.Contains), [PredefinedType.CHAR_FQN], invocationExpression.PsiModule) is [_])
                {
                    consumer.AddHighlighting(
                        new UseOtherMethodSuggestion(
                            $"Use the '{nameof(string.Contains)}' method.",
                            invocationExpression,
                            invokedExpression,
                            nameof(string.Contains),
                            false,
                            [valueArgument.Value.GetText()],
                            relationalExpression));
                }

                if (tokenType == CSharpTokenType.LT
                    && value == 0
                    && TryGetParameterNamesIfMethodExists(nameof(string.Contains), [PredefinedType.CHAR_FQN], invocationExpression.PsiModule) is [_])
                {
                    consumer.AddHighlighting(
                        new UseOtherMethodSuggestion(
                            $"Use the '{nameof(string.Contains)}' method.",
                            invocationExpression,
                            invokedExpression,
                            nameof(string.Contains),
                            true,
                            [valueArgument.Value.GetText()],
                            relationalExpression));
                }
                break;
        }
    }

    /// <remarks>
    /// <c>text.IndexOf(char, 0)</c> → <c>text.IndexOf(char)</c>
    /// </remarks>
    static void AnalyzeIndexOf_Char_Int32(IHighlightingConsumer consumer, ICSharpArgument startIndexArgument)
    {
        if (TryGetInt32Constant(startIndexArgument.Value) == 0)
        {
            consumer.AddHighlighting(new RedundantArgumentHint("Passing 0 is redundant.", startIndexArgument));
        }
    }

    /// <remarks>
    /// <c>text.IndexOf(char, StringComparison) > -1</c> → <c>text.Contains(char, StringComparison)</c> (.NET Core 2.1)<para/>
    /// <c>text.IndexOf(char, StringComparison) != -1</c> → <c>text.Contains(char, StringComparison)</c> (.NET Core 2.1)<para/>
    /// <c>text.IndexOf(char, StringComparison) >= 0</c> → <c>text.Contains(char, StringComparison)</c> (.NET Core 2.1)<para/>
    /// <c>text.IndexOf(char, StringComparison) == -1</c> → <c>!text.Contains(char, StringComparison)</c> (.NET Core 2.1)<para/>
    /// <c>text.IndexOf(char, StringComparison) &lt; 0</c> → <c>!text.Contains(char, StringComparison)</c> (.NET Core 2.1)
    /// </remarks>
    static void AnalyzeIndexOf_Char_StringComparison(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument valueArgument,
        ICSharpArgument comparisonTypeArgument)
    {
        switch (invocationExpression.Parent)
        {
            case IEqualityExpression equalityExpression when equalityExpression.LeftOperand == invocationExpression
                && valueArgument.Value is { }
                && comparisonTypeArgument.Value is { }:

                switch (equalityExpression.EqualityType, TryGetInt32Constant(equalityExpression.RightOperand))
                {
                    case (EqualityExpressionType.NE, -1):
                        if (TryGetParameterNamesIfMethodExists(
                                nameof(string.Contains),
                                [PredefinedType.CHAR_FQN, PredefinedType.STRING_COMPARISON_CLASS],
                                invocationExpression.PsiModule) is [_, _])
                        {
                            consumer.AddHighlighting(
                                new UseOtherMethodSuggestion(
                                    $"Use the '{nameof(string.Contains)}' method.",
                                    invocationExpression,
                                    invokedExpression,
                                    nameof(string.Contains),
                                    false,
                                    [valueArgument.Value.GetText(), comparisonTypeArgument.Value.GetText()],
                                    equalityExpression));
                        }
                        break;

                    case (EqualityExpressionType.EQEQ, -1):
                        if (TryGetParameterNamesIfMethodExists(
                                nameof(string.Contains),
                                [PredefinedType.CHAR_FQN, PredefinedType.STRING_COMPARISON_CLASS],
                                invocationExpression.PsiModule) is [_, _])
                        {
                            consumer.AddHighlighting(
                                new UseOtherMethodSuggestion(
                                    $"Use the '{nameof(string.Contains)}' method.",
                                    invocationExpression,
                                    invokedExpression,
                                    nameof(string.Contains),
                                    true,
                                    [valueArgument.Value.GetText(), comparisonTypeArgument.Value.GetText()],
                                    equalityExpression));
                        }
                        break;
                }
                break;

            case IRelationalExpression relationalExpression when relationalExpression.LeftOperand == invocationExpression
                && TryGetInt32Constant(relationalExpression.RightOperand) is { } value
                && valueArgument.Value is { }
                && comparisonTypeArgument.Value is { }:

                var tokenType = relationalExpression.OperatorSign.GetTokenType();

                if ((tokenType == CSharpTokenType.GT && value == -1 || tokenType == CSharpTokenType.GE && value == 0)
                    && TryGetParameterNamesIfMethodExists(
                        nameof(string.Contains),
                        [PredefinedType.CHAR_FQN, PredefinedType.STRING_COMPARISON_CLASS],
                        invocationExpression.PsiModule) is [_, _])
                {
                    consumer.AddHighlighting(
                        new UseOtherMethodSuggestion(
                            $"Use the '{nameof(string.Contains)}' method.",
                            invocationExpression,
                            invokedExpression,
                            nameof(string.Contains),
                            false,
                            [valueArgument.Value.GetText(), comparisonTypeArgument.Value.GetText()],
                            relationalExpression));
                }

                if (tokenType == CSharpTokenType.LT
                    && value == 0
                    && TryGetParameterNamesIfMethodExists(
                        nameof(string.Contains),
                        [PredefinedType.CHAR_FQN, PredefinedType.STRING_COMPARISON_CLASS],
                        invocationExpression.PsiModule) is [_, _])
                {
                    consumer.AddHighlighting(
                        new UseOtherMethodSuggestion(
                            $"Use the '{nameof(string.Contains)}' method.",
                            invocationExpression,
                            invokedExpression,
                            nameof(string.Contains),
                            true,
                            [valueArgument.Value.GetText(), comparisonTypeArgument.Value.GetText()],
                            relationalExpression));
                }

                break;
        }
    }

    /// <remarks>
    /// <c>text.IndexOf("")</c> → 0<para/>
    /// <c>text.IndexOf(string)</c> → <c>text.IndexOf(char, CurrentCulture)</c> (.NET Core 2.1)<para/>
    /// <c>text.IndexOf(string) == 0</c> → <c>text.StartsWith(string)</c><para/>
    /// <c>text.IndexOf(string) != 0</c> → <c>!text.StartsWith(string)</c><para/>
    /// <c>text.IndexOf(string) > -1</c> → <c>text.Contains(string, CurrentCulture)</c> (.NET Core 2.1)<para/>
    /// <c>text.IndexOf(string) != -1</c> → <c>text.Contains(string, CurrentCulture)</c> (.NET Core 2.1)<para/>
    /// <c>text.IndexOf(string) >= 0</c> → <c>text.Contains(string, CurrentCulture)</c> (.NET Core 2.1)<para/>
    /// <c>text.IndexOf(string) == -1</c> → <c>!text.Contains(string, CurrentCulture)</c> (.NET Core 2.1)<para/>
    /// <c>text.IndexOf(string) &lt; 0</c> → <c>!text.Contains(string, CurrentCulture)</c> (.NET Core 2.1)
    /// </remarks>
    static void AnalyzeIndexOf_String(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument valueArgument)
    {
        switch (TryGetStringConstant(valueArgument.Value))
        {
            case "" when !invocationExpression.IsUsedAsStatement():
                consumer.AddHighlighting(new UseExpressionResultSuggestion("The expression is always 0.", invocationExpression, "0"));
                break;

            case [var character] when TryGetParameterNamesIfMethodExists(
                nameof(string.IndexOf),
                [PredefinedType.CHAR_FQN, PredefinedType.STRING_COMPARISON_CLASS],
                invocationExpression.PsiModule) is [var valueParameterName, _]:

                consumer.AddHighlighting(
                    new PassSingleCharacterSuggestion(
                        "Pass the single character.",
                        valueArgument,
                        valueArgument.NameIdentifier is { } ? valueParameterName : null,
                        character,
                        $"{nameof(StringComparison)}.{nameof(StringComparison.CurrentCulture)}"));
                break;

            default:
                switch (invocationExpression.Parent)
                {
                    case IEqualityExpression equalityExpression
                        when equalityExpression.LeftOperand == invocationExpression && valueArgument.Value is { }:

                        switch (equalityExpression.EqualityType, TryGetInt32Constant(equalityExpression.RightOperand))
                        {
                            case (EqualityExpressionType.EQEQ, 0):
                                consumer.AddHighlighting(
                                    new UseOtherMethodSuggestion(
                                        $"Use the '{nameof(string.StartsWith)}' method.",
                                        invocationExpression,
                                        invokedExpression,
                                        nameof(string.StartsWith),
                                        false,
                                        [valueArgument.Value.GetText()],
                                        equalityExpression));
                                break;

                            case (EqualityExpressionType.NE, 0):
                                consumer.AddHighlighting(
                                    new UseOtherMethodSuggestion(
                                        $"Use the '{nameof(string.StartsWith)}' method.",
                                        invocationExpression,
                                        invokedExpression,
                                        nameof(string.StartsWith),
                                        true,
                                        [valueArgument.Value.GetText()],
                                        equalityExpression));
                                break;

                            case (EqualityExpressionType.NE, -1):
                                if (TryGetParameterNamesIfMethodExists(
                                        nameof(string.Contains),
                                        [PredefinedType.STRING_FQN, PredefinedType.STRING_COMPARISON_CLASS],
                                        invocationExpression.PsiModule) is [_, _])
                                {
                                    consumer.AddHighlighting(
                                        new UseOtherMethodSuggestion(
                                            $"Use the '{nameof(string.Contains)}' method.",
                                            invocationExpression,
                                            invokedExpression,
                                            nameof(string.Contains),
                                            false,
                                            [valueArgument.Value.GetText(), $"{nameof(StringComparison)}.{nameof(StringComparison.CurrentCulture)}"],
                                            equalityExpression));
                                }
                                break;

                            case (EqualityExpressionType.EQEQ, -1):
                                if (TryGetParameterNamesIfMethodExists(
                                        nameof(string.Contains),
                                        [PredefinedType.STRING_FQN, PredefinedType.STRING_COMPARISON_CLASS],
                                        invocationExpression.PsiModule) is [_, _])
                                {
                                    consumer.AddHighlighting(
                                        new UseOtherMethodSuggestion(
                                            $"Use the '{nameof(string.Contains)}' method.",
                                            invocationExpression,
                                            invokedExpression,
                                            nameof(string.Contains),
                                            true,
                                            [valueArgument.Value.GetText(), $"{nameof(StringComparison)}.{nameof(StringComparison.CurrentCulture)}"],
                                            equalityExpression));
                                }
                                break;
                        }
                        break;

                    case IRelationalExpression relationalExpression when relationalExpression.LeftOperand == invocationExpression
                        && TryGetInt32Constant(relationalExpression.RightOperand) is { } value
                        && valueArgument.Value is { }:

                        var tokenType = relationalExpression.OperatorSign.GetTokenType();

                        if ((tokenType == CSharpTokenType.GT && value == -1 || tokenType == CSharpTokenType.GE && value == 0)
                            && TryGetParameterNamesIfMethodExists(
                                nameof(string.Contains),
                                [PredefinedType.STRING_FQN, PredefinedType.STRING_COMPARISON_CLASS],
                                invocationExpression.PsiModule) is [_, _])
                        {
                            consumer.AddHighlighting(
                                new UseOtherMethodSuggestion(
                                    $"Use the '{nameof(string.Contains)}' method.",
                                    invocationExpression,
                                    invokedExpression,
                                    nameof(string.Contains),
                                    false,
                                    [valueArgument.Value.GetText(), $"{nameof(StringComparison)}.{nameof(StringComparison.CurrentCulture)}"],
                                    relationalExpression));
                        }

                        if (tokenType == CSharpTokenType.LT
                            && value == 0
                            && TryGetParameterNamesIfMethodExists(
                                nameof(string.Contains),
                                [PredefinedType.STRING_FQN, PredefinedType.STRING_COMPARISON_CLASS],
                                invocationExpression.PsiModule) is [_, _])
                        {
                            consumer.AddHighlighting(
                                new UseOtherMethodSuggestion(
                                    $"Use the '{nameof(string.Contains)}' method.",
                                    invocationExpression,
                                    invokedExpression,
                                    nameof(string.Contains),
                                    true,
                                    [valueArgument.Value.GetText(), $"{nameof(StringComparison)}.{nameof(StringComparison.CurrentCulture)}"],
                                    relationalExpression));
                        }

                        break;
                }
                break;
        }
    }

    /// <remarks>
    /// <c>text.IndexOf(string, 0)</c> → <c>text.IndexOf(string)</c>
    /// </remarks>
    static void AnalyzeIndexOf_String_Int32(IHighlightingConsumer consumer, ICSharpArgument startIndexArgument)
    {
        if (TryGetInt32Constant(startIndexArgument.Value) == 0)
        {
            consumer.AddHighlighting(new RedundantArgumentHint("Passing 0 is redundant.", startIndexArgument));
        }
    }

    /// <remarks>
    /// <c>text.IndexOf("", StringComparison)</c> → <c>0</c><para/>
    /// <c>text.IndexOf(string, StringComparison)</c> → <c>text.IndexOf(char, StringComparison)</c> (.NET Core 2.1)<para/>
    /// <c>text.IndexOf(string, StringComparison) == 0</c> → <c>text.StartsWith(string, StringComparison)</c><para/>
    /// <c>text.IndexOf(string, StringComparison) != 0</c> → <c>!text.StartsWith(string, StringComparison)</c><para/>
    /// <c>text.IndexOf(string, StringComparison) > -1</c> → <c>text.Contains(string, StringComparison)</c> (.NET Core 2.1)<para/>
    /// <c>text.IndexOf(string, StringComparison) != -1</c> → <c>text.Contains(string, StringComparison)</c> (.NET Core 2.1)<para/>
    /// <c>text.IndexOf(string, StringComparison) >= 0</c> → <c>text.Contains(string, StringComparison)</c> (.NET Core 2.1)<para/>
    /// <c>text.IndexOf(string, StringComparison) == -1</c> → <c>!text.Contains(string, StringComparison)</c> (.NET Core 2.1)<para/>
    /// <c>text.IndexOf(string, StringComparison) &lt; 0</c> → <c>!text.Contains(string, StringComparison)</c> (.NET Core 2.1)
    /// </remarks>
    static void AnalyzeIndexOf_String_StringComparison(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument valueArgument,
        ICSharpArgument comparisonTypeArgument)
    {
        switch (TryGetStringConstant(valueArgument.Value))
        {
            case "" when !invocationExpression.IsUsedAsStatement():
                consumer.AddHighlighting(new UseExpressionResultSuggestion("The expression is always 0.", invocationExpression, "0"));
                break;

            case [var character] when TryGetParameterNamesIfMethodExists(
                nameof(string.IndexOf),
                [PredefinedType.CHAR_FQN, PredefinedType.STRING_COMPARISON_CLASS],
                invocationExpression.PsiModule) is [var valueParameterName, _]:

                consumer.AddHighlighting(
                    new PassSingleCharacterSuggestion(
                        "Pass the single character.",
                        valueArgument,
                        valueArgument.NameIdentifier is { } ? valueParameterName : null,
                        character));
                break;

            default:
                switch (invocationExpression.Parent)
                {
                    case IEqualityExpression equalityExpression when equalityExpression.LeftOperand == invocationExpression
                        && valueArgument.Value is { }
                        && comparisonTypeArgument.Value is { }:

                        switch (equalityExpression.EqualityType, TryGetInt32Constant(equalityExpression.RightOperand))
                        {
                            case (EqualityExpressionType.EQEQ, 0):
                                if (TryGetParameterNamesIfMethodExists(
                                        nameof(string.StartsWith),
                                        [PredefinedType.STRING_FQN, PredefinedType.STRING_COMPARISON_CLASS],
                                        invocationExpression.PsiModule) is [_, _])
                                {
                                    consumer.AddHighlighting(
                                        new UseOtherMethodSuggestion(
                                            $"Use the '{nameof(string.StartsWith)}' method.",
                                            invocationExpression,
                                            invokedExpression,
                                            nameof(string.StartsWith),
                                            false,
                                            [valueArgument.Value.GetText(), comparisonTypeArgument.Value.GetText()],
                                            equalityExpression));
                                }
                                break;

                            case (EqualityExpressionType.NE, 0):
                                if (TryGetParameterNamesIfMethodExists(
                                        nameof(string.StartsWith),
                                        [PredefinedType.STRING_FQN, PredefinedType.STRING_COMPARISON_CLASS],
                                        invocationExpression.PsiModule) is [_, _])
                                {
                                    consumer.AddHighlighting(
                                        new UseOtherMethodSuggestion(
                                            $"Use the '{nameof(string.StartsWith)}' method.",
                                            invocationExpression,
                                            invokedExpression,
                                            nameof(string.StartsWith),
                                            true,
                                            [valueArgument.Value.GetText(), comparisonTypeArgument.Value.GetText()],
                                            equalityExpression));
                                }
                                break;

                            case (EqualityExpressionType.NE, -1):
                                if (TryGetParameterNamesIfMethodExists(
                                        nameof(string.Contains),
                                        [PredefinedType.STRING_FQN, PredefinedType.STRING_COMPARISON_CLASS],
                                        invocationExpression.PsiModule) is [_, _])
                                {
                                    consumer.AddHighlighting(
                                        new UseOtherMethodSuggestion(
                                            $"Use the '{nameof(string.Contains)}' method.",
                                            invocationExpression,
                                            invokedExpression,
                                            nameof(string.Contains),
                                            false,
                                            [valueArgument.Value.GetText(), comparisonTypeArgument.Value.GetText()],
                                            equalityExpression));
                                }
                                break;

                            case (EqualityExpressionType.EQEQ, -1):
                                if (TryGetParameterNamesIfMethodExists(
                                        nameof(string.Contains),
                                        [PredefinedType.STRING_FQN, PredefinedType.STRING_COMPARISON_CLASS],
                                        invocationExpression.PsiModule) is [_, _])
                                {
                                    consumer.AddHighlighting(
                                        new UseOtherMethodSuggestion(
                                            $"Use the '{nameof(string.Contains)}' method.",
                                            invocationExpression,
                                            invokedExpression,
                                            nameof(string.Contains),
                                            true,
                                            [valueArgument.Value.GetText(), comparisonTypeArgument.Value.GetText()],
                                            equalityExpression));
                                }
                                break;
                        }
                        break;

                    case IRelationalExpression relationalExpression when relationalExpression.LeftOperand == invocationExpression
                        && TryGetInt32Constant(relationalExpression.RightOperand) is { } value
                        && valueArgument.Value is { }
                        && comparisonTypeArgument.Value is { }:

                        var tokenType = relationalExpression.OperatorSign.GetTokenType();

                        if ((tokenType == CSharpTokenType.GT && value == -1 || tokenType == CSharpTokenType.GE && value == 0)
                            && TryGetParameterNamesIfMethodExists(
                                nameof(string.Contains),
                                [PredefinedType.STRING_FQN, PredefinedType.STRING_COMPARISON_CLASS],
                                invocationExpression.PsiModule) is [_, _])
                        {
                            consumer.AddHighlighting(
                                new UseOtherMethodSuggestion(
                                    $"Use the '{nameof(string.Contains)}' method.",
                                    invocationExpression,
                                    invokedExpression,
                                    nameof(string.Contains),
                                    false,
                                    [valueArgument.Value.GetText(), comparisonTypeArgument.Value.GetText()],
                                    relationalExpression));
                        }

                        if (tokenType == CSharpTokenType.LT
                            && value == 0
                            && TryGetParameterNamesIfMethodExists(
                                nameof(string.Contains),
                                [PredefinedType.STRING_FQN, PredefinedType.STRING_COMPARISON_CLASS],
                                invocationExpression.PsiModule) is [_, _])
                        {
                            consumer.AddHighlighting(
                                new UseOtherMethodSuggestion(
                                    $"Use the '{nameof(string.Contains)}' method.",
                                    invocationExpression,
                                    invokedExpression,
                                    nameof(string.Contains),
                                    true,
                                    [valueArgument.Value.GetText(), comparisonTypeArgument.Value.GetText()],
                                    relationalExpression));
                        }

                        break;
                }
                break;
        }
    }

    /// <remarks>
    /// <c>text.IndexOf(string, 0, StringComparison)</c> → <c>text.IndexOf(string, StringComparison)</c>
    /// </remarks>
    static void AnalyzeIndexOf_String_Int32_StringComparison(IHighlightingConsumer consumer, ICSharpArgument startIndexArgument)
    {
        if (TryGetInt32Constant(startIndexArgument.Value) == 0)
        {
            consumer.AddHighlighting(new RedundantArgumentHint("Passing 0 is redundant.", startIndexArgument));
        }
    }

    /// <remarks>
    /// <c>text.IndexOfAny(char[], 0)</c> → <c>text.IndexOfAny(char[])</c>
    /// </remarks>
    static void AnalyzeIndexOfAny(IHighlightingConsumer consumer, ICSharpArgument startIndexArgument)
    {
        if (TryGetInt32Constant(startIndexArgument.Value) == 0)
        {
            consumer.AddHighlighting(new RedundantArgumentHint("Passing 0 is redundant.", startIndexArgument));
        }
    }

    /// <remarks>
    /// <c>text.LastIndexOf(char, 0)</c> → <c>-1</c>
    /// </remarks>
    static void AnalyzeLastIndexOf_Char_Int32(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument startIndexArgument)
    {
        if (!invocationExpression.IsUsedAsStatement() && TryGetInt32Constant(startIndexArgument.Value) == 0)
        {
            consumer.AddHighlighting(new UseExpressionResultSuggestion("The expression is always -1.", invocationExpression, "-1"));
        }
    }

    /// <remarks>
    /// <c>text.LastIndexOf("")</c> → <c>text.Length</c>
    /// </remarks>
    static void AnalyzeLastIndexOf_String(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument valueArgument)
    {
        if (!invocationExpression.IsUsedAsStatement() && TryGetStringConstant(valueArgument.Value) == "")
        {
            consumer.AddHighlighting(
                new UseStringPropertySuggestion(
                    $"Use the '{nameof(string.Length)}' property.",
                    invocationExpression,
                    invokedExpression,
                    nameof(string.Length)));
        }
    }

    /// <remarks>
    /// <c>text.LastIndexOf("", StringComparison)</c> → <c>text.Length</c><para/>
    /// <c>text.LastIndexOf(string, Ordinal)</c> → <c>text.LastIndexOf(char)</c>
    /// </remarks>
    static void AnalyzeLastIndexOf_String_StringComparison(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument valueArgument,
        ICSharpArgument comparisonTypeArgument)
    {
        switch (TryGetStringConstant(valueArgument.Value))
        {
            case "" when !invocationExpression.IsUsedAsStatement():
                consumer.AddHighlighting(
                    new UseStringPropertySuggestion(
                        $"Use the '{nameof(string.Length)}' property.",
                        invocationExpression,
                        invokedExpression,
                        nameof(string.Length)));
                break;

            case [var character] when TryGetStringComparisonConstant(comparisonTypeArgument.Value) == StringComparison.Ordinal
                && TryGetParameterNamesIfMethodExists(nameof(string.LastIndexOf), [PredefinedType.CHAR_FQN], invocationExpression.PsiModule) is
                [
                    var valueParameterName,
                ]:

                consumer.AddHighlighting(
                    new PassSingleCharacterSuggestion(
                        "Pass the single character.",
                        valueArgument,
                        valueArgument.NameIdentifier is { } ? valueParameterName : null,
                        character,
                        redundantArgument: comparisonTypeArgument));
                break;
        }
    }

    /// <remarks>
    /// <c>text.PadLeft(0)</c> → <c>text</c>
    /// </remarks>
    static void AnalyzePadLeft_Int32(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument totalWidthArgument)
    {
        if (!invocationExpression.IsUsedAsStatement() && TryGetInt32Constant(totalWidthArgument.Value) == 0)
        {
            consumer.AddHighlighting(
                new RedundantMethodInvocationHint(
                    $"Invoking '{nameof(string.PadLeft)}' with 0 is redundant.",
                    invocationExpression,
                    invokedExpression));
        }
    }

    /// <remarks>
    /// <c>text.PadLeft(0, char)</c> → <c>text</c><para/>
    /// <c>text.PadLeft(int, ' ')</c> → <c>text.PadLeft(int)</c>
    /// </remarks>
    static void AnalyzePadLeft_Int32_Char(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument totalWidthArgument,
        ICSharpArgument paddingCharArgument)
    {
        if (!invocationExpression.IsUsedAsStatement())
        {
            if (TryGetInt32Constant(totalWidthArgument.Value) == 0)
            {
                consumer.AddHighlighting(
                    new RedundantMethodInvocationHint(
                        $"Invoking '{nameof(string.PadLeft)}' with 0 is redundant.",
                        invocationExpression,
                        invokedExpression));
                return;
            }

            if (TryGetCharConstant(paddingCharArgument.Value) == ' ')
            {
                consumer.AddHighlighting(new RedundantArgumentHint("Passing ' ' is redundant.", paddingCharArgument));
            }
        }
    }

    /// <remarks>
    /// <c>text.PadRight(0)</c> → <c>text</c>
    /// </remarks>
    static void AnalyzePadRight_Int32(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument totalWidthArgument)
    {
        if (!invocationExpression.IsUsedAsStatement() && TryGetInt32Constant(totalWidthArgument.Value) == 0)
        {
            consumer.AddHighlighting(
                new RedundantMethodInvocationHint(
                    $"Invoking '{nameof(string.PadRight)}' with 0 is redundant.",
                    invocationExpression,
                    invokedExpression));
        }
    }

    /// <remarks>
    /// <c>text.PadRight(0, char)</c> → <c>text</c><para/>
    /// <c>text.PadRight(int, ' ')</c> → <c>text.PadRight(int)</c>
    /// </remarks>
    static void AnalyzePadRight_Int32_Char(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument totalWidthArgument,
        ICSharpArgument paddingCharArgument)
    {
        if (!invocationExpression.IsUsedAsStatement())
        {
            if (TryGetInt32Constant(totalWidthArgument.Value) == 0)
            {
                consumer.AddHighlighting(
                    new RedundantMethodInvocationHint(
                        $"Invoking '{nameof(string.PadRight)}' with 0 is redundant.",
                        invocationExpression,
                        invokedExpression));
                return;
            }

            if (TryGetCharConstant(paddingCharArgument.Value) == ' ')
            {
                consumer.AddHighlighting(new RedundantArgumentHint("Passing ' ' is redundant.", paddingCharArgument));
            }
        }
    }

    /// <remarks>
    /// <c>text.Remove(0)</c> → <c>""</c><para/>
    /// <c>text.Remove(int)</c> → <c>text[..startIndex]</c> (C# 8)
    /// </remarks>
    static void AnalyzeRemove_Int32(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument startIndexArgument)
    {
        if (!invocationExpression.IsUsedAsStatement())
        {
            if (TryGetInt32Constant(startIndexArgument.Value) == 0)
            {
                consumer.AddHighlighting(
                    new UseExpressionResultSuggestion("The expression is always an empty string.", invocationExpression, "\"\""));
                return;
            }

            if (invocationExpression.GetCSharpLanguageLevel() >= CSharpLanguageLevel.CSharp80 && startIndexArgument.Value is { })
            {
                consumer.AddHighlighting(
                    new UseRangeIndexerSuggestion(
                        "Use the range indexer.",
                        invocationExpression,
                        invokedExpression,
                        "",
                        startIndexArgument.Value.GetText()));
            }
        }
    }

    /// <remarks>
    /// <c>text.Remove(int, 0)</c> → <c>text</c><para/>
    /// <c>text.Remove(0, int)</c> → <c>text[count..]</c> (C# 8)
    /// </remarks>
    static void AnalyzeRemove_Int32_Int32(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument startIndexArgument,
        ICSharpArgument countArgument)
    {
        if (!invocationExpression.IsUsedAsStatement())
        {
            if (TryGetInt32Constant(countArgument.Value) == 0)
            {
                consumer.AddHighlighting(
                    new RedundantMethodInvocationHint(
                        $"Invoking '{nameof(string.Remove)}' with the count 0 is redundant.",
                        invocationExpression,
                        invokedExpression));
                return;
            }

            if (invocationExpression.GetCSharpLanguageLevel() >= CSharpLanguageLevel.CSharp80
                && TryGetInt32Constant(startIndexArgument.Value) == 0
                && countArgument.Value is { })
            {
                consumer.AddHighlighting(
                    new UseRangeIndexerSuggestion(
                        "Use the range indexer.",
                        invocationExpression,
                        invokedExpression,
                        countArgument.Value.GetText(),
                        ""));
            }
        }
    }

    /// <remarks>
    /// <c>text.Replace(string, string, Ordinal)</c> → <c>text</c><para/>
    /// <c>text.Replace(string, string, Ordinal)</c> → <c>text.Replace(char, char)</c>
    /// </remarks>
    static void AnalyzeReplace_String_String_StringComparison(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument oldValueArgument,
        ICSharpArgument newValueArgument,
        ICSharpArgument comparisonTypeArgument)
    {
        if (TryGetStringConstant(oldValueArgument.Value) is { } oldValue and not ""
            && TryGetStringConstant(newValueArgument.Value) is { } newValue
            && TryGetStringComparisonConstant(comparisonTypeArgument.Value) == StringComparison.Ordinal)
        {
            if (!invocationExpression.IsUsedAsStatement() && oldValue == newValue)
            {
                consumer.AddHighlighting(
                    new RedundantMethodInvocationHint(
                        $"Invoking '{nameof(string.Replace)}' with identical values is redundant.",
                        invocationExpression,
                        invokedExpression));
                return;
            }

            if (oldValue is [var oldCharacter]
                && newValue is [var newCharacter]
                && TryGetParameterNamesIfMethodExists(
                    nameof(string.Replace),
                    [PredefinedType.CHAR_FQN, PredefinedType.CHAR_FQN],
                    invocationExpression.PsiModule) is [var oldCharParameterName, var newCharParameterName])
            {
                var highlighting = new PassSingleCharactersSuggestion(
                    "Pass the single character.",
                    [oldValueArgument, newValueArgument],
                    [
                        oldValueArgument.NameIdentifier is { } ? oldCharParameterName : null,
                        newValueArgument.NameIdentifier is { } ? newCharParameterName : null,
                    ],
                    [oldCharacter, newCharacter],
                    comparisonTypeArgument);

                consumer.AddHighlighting(highlighting, oldValueArgument.Value.GetDocumentRange());
                consumer.AddHighlighting(highlighting, newValueArgument.Value.GetDocumentRange());
            }
        }
    }

    /// <remarks>
    /// <c>text.Replace(char, char)</c> → <c>text</c>
    /// </remarks>
    static void AnalyzeReplace_Char_Char(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument oldCharArgument,
        ICSharpArgument newCharArgument)
    {
        if (!invocationExpression.IsUsedAsStatement()
            && TryGetCharConstant(oldCharArgument.Value) is { } oldCharacter
            && TryGetCharConstant(newCharArgument.Value) is { } newCharacter
            && oldCharacter == newCharacter)
        {
            consumer.AddHighlighting(
                new RedundantMethodInvocationHint(
                    $"Invoking '{nameof(string.Replace)}' with identical characters is redundant.",
                    invocationExpression,
                    invokedExpression));
        }
    }

    /// <remarks>
    /// <c>text.Replace(string, string)</c> → <c>text</c><para/>
    /// <c>text.Replace(string, string)</c> → <c>text.Replace(char, char)</c>
    /// </remarks>
    static void AnalyzeReplace_String_String(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument oldValueArgument,
        ICSharpArgument newValueArgument)
    {
        if (TryGetStringConstant(oldValueArgument.Value) is { } oldValue and not "" && TryGetStringConstant(newValueArgument.Value) is { } newValue)
        {
            if (!invocationExpression.IsUsedAsStatement() && oldValue == newValue)
            {
                consumer.AddHighlighting(
                    new RedundantMethodInvocationHint(
                        $"Invoking '{nameof(string.Replace)}' with identical values is redundant.",
                        invocationExpression,
                        invokedExpression));
                return;
            }

            if (oldValue is [var oldCharacter]
                && newValue is [var newCharacter]
                && TryGetParameterNamesIfMethodExists(
                    nameof(string.Replace),
                    [PredefinedType.CHAR_FQN, PredefinedType.CHAR_FQN],
                    invocationExpression.PsiModule) is [var oldCharParameterName, var newCharParameterName])
            {
                var highlighting = new PassSingleCharactersSuggestion(
                    "Pass the single character.",
                    [oldValueArgument, newValueArgument],
                    [
                        oldValueArgument.NameIdentifier is { } ? oldCharParameterName : null,
                        newValueArgument.NameIdentifier is { } ? newCharParameterName : null,
                    ],
                    [oldCharacter, newCharacter]);

                consumer.AddHighlighting(highlighting, oldValueArgument.Value.GetDocumentRange());
                consumer.AddHighlighting(highlighting, newValueArgument.Value.GetDocumentRange());
            }
        }
    }

    /// <remarks>
    /// <c>text.Split(char, 0, StringSplitOptions)</c> → <c>Array.Empty&lt;string&gt;()</c> or <c>[]</c> (C# 12)<para/>
    /// <c>text.Split(char, 1, None)</c> → <c>new[] { text }</c> or <c>[text]</c> (C# 12)<para/>
    /// <c>text.Split(char, 1, TrimEntries)</c> → <c>new[] { text.Trim() }</c> or <c>[text.Trim()]</c> (C# 12)
    /// </remarks>
    static void AnalyzeSplit_Char_Int32_StringSplitOptions(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument countArgument,
        ICSharpArgument? optionsArgument)
    {
        if (!invocationExpression.IsUsedAsStatement())
        {
            switch (TryGetInt32Constant(countArgument.Value))
            {
                case 0:
                    consumer.AddHighlighting(
                        new UseExpressionResultSuggestion(
                            "The expression is always an empty array.",
                            invocationExpression,
                            CreateStringArray([], invocationExpression)));
                    break;

                case 1:
                    Debug.Assert(invokedExpression.QualifierExpression is { });

                    switch (optionsArgument is { } ? TryGetStringSplitOptionsConstant(optionsArgument.Value) : StringSplitOptions.None)
                    {
                        case StringSplitOptions.None:
                            consumer.AddHighlighting(
                                new UseExpressionResultSuggestion(
                                    "The expression is always an array with a single element.",
                                    invocationExpression,
                                    CreateStringArray([invokedExpression.QualifierExpression.GetText()], invocationExpression)));
                            break;

                        case (StringSplitOptions)2: // todo: use StringSplitOptions.TrimEntries
                            consumer.AddHighlighting(
                                new UseExpressionResultSuggestion(
                                    "The expression is always an array with a single trimmed element.",
                                    invocationExpression,
                                    CreateStringArray([$"{invokedExpression.QualifierExpression.GetText()}.Trim()"], invocationExpression)));
                            break;
                    }
                    break;
            }
        }
    }

    /// <remarks>
    /// <c>text.Split(char[])</c> → <c>text.Split(char[])</c>
    /// </remarks>
    static void AnalyzeSplit_CharArray(IHighlightingConsumer consumer, TreeNodeCollection<ICSharpArgument> arguments)
    {
        switch (arguments)
        {
            case [_, _, ..]:
            {
                var set = new HashSet<char>(arguments.Count);

                foreach (var argument in arguments)
                {
                    if (TryGetCharConstant(argument.Value) is { } character && !set.Add(character))
                    {
                        consumer.AddHighlighting(new RedundantArgumentHint("The character is already passed.", argument));
                    }
                }

                break;
            }

            case [{ Value: var argumentExpression }] when Collection.TryFrom(argumentExpression) is { } collection:
            {
                var set = new HashSet<char>(collection.Count);

                foreach (var (element, character) in collection.ElementsWithCharConstants)
                {
                    if (!set.Add(character))
                    {
                        consumer.AddHighlighting(new RedundantElementHint("The character is already passed.", element));
                    }
                }

                break;
            }
        }
    }

    /// <remarks>
    /// <c>text.Split(char[], 0)</c> → <c>Array.Empty&lt;string&gt;()</c> or <c>[]</c> (C# 12)<para/>
    /// <c>text.Split(char[], 1)</c> → <c>new[] { text }</c> or <c>[text]</c> (C# 12)<para/>
    /// <c>text.Split(char[], int)</c> → <c>text.Split(char[], int)</c><para/>
    /// </remarks>
    static void AnalyzeSplit_CharArray_Int32(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument separatorArgument,
        ICSharpArgument countArgument)
    {
        switch (separatorArgument.Value, TryGetInt32Constant(countArgument.Value))
        {
            case (_, 0) when !invocationExpression.IsUsedAsStatement():
                consumer.AddHighlighting(
                    new UseExpressionResultSuggestion(
                        "The expression is always an empty array.",
                        invocationExpression,
                        CreateStringArray([], invocationExpression)));
                break;

            case (_, 1) when !invocationExpression.IsUsedAsStatement():
                Debug.Assert(invokedExpression.QualifierExpression is { });

                consumer.AddHighlighting(
                    new UseExpressionResultSuggestion(
                        "The expression is always an array with a single element.",
                        invocationExpression,
                        CreateStringArray([invokedExpression.QualifierExpression.GetText()], invocationExpression)));
                break;

            case (_, _) when Collection.TryFrom(separatorArgument.Value) is { } collection:
            {
                var set = new HashSet<char>(collection.Count);

                foreach (var (element, character) in collection.ElementsWithCharConstants)
                {
                    if (!set.Add(character))
                    {
                        consumer.AddHighlighting(new RedundantElementHint("The character is already passed.", element));
                    }
                }

                break;
            }
        }
    }

    /// <remarks>
    /// <c>text.Split(char[], StringSplitOptions)</c> → <c>text.Split(char[], StringSplitOptions)</c><para/>
    /// </remarks>
    static void AnalyzeSplit_CharArray_StringSplitOptions(IHighlightingConsumer consumer, ICSharpArgument separatorArgument)
    {
        if (Collection.TryFrom(separatorArgument.Value) is { } collection)
        {
            var set = new HashSet<char>(collection.Count);

            foreach (var (element, character) in collection.ElementsWithCharConstants)
            {
                if (!set.Add(character))
                {
                    consumer.AddHighlighting(new RedundantElementHint("The character is already passed.", element));
                }
            }
        }
    }

    /// <remarks>
    /// <c>text.Split(char[], 0, StringSplitOptions)</c> → <c>Array.Empty&lt;string&gt;()</c> or <c>[]</c> (C# 12)<para/>
    /// <c>text.Split(char[], 1, None)</c> → <c>new[] { text }</c> or <c>[text]</c> (C# 12)<para/>
    /// <c>text.Split(char[], 1, TrimEntries)</c> → <c>new[] { text.Trim() }</c> or <c>[text.Trim()]</c> (C# 12)<para/>
    /// <c>text.Split(char[], int, StringSplitOptions)</c> → <c>text.Split(char[], int, StringSplitOptions)</c><para/>
    /// </remarks>
    static void AnalyzeSplit_CharArray_Int32_StringSplitOptions(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument separatorArgument,
        ICSharpArgument countArgument,
        ICSharpArgument optionsArgument)
    {
        switch (separatorArgument.Value, TryGetInt32Constant(countArgument.Value))
        {
            case (_, 0) when !invocationExpression.IsUsedAsStatement():
                consumer.AddHighlighting(
                    new UseExpressionResultSuggestion(
                        "The expression is always an empty array.",
                        invocationExpression,
                        CreateStringArray([], invocationExpression)));
                break;

            case (_, 1) when !invocationExpression.IsUsedAsStatement():
                Debug.Assert(invokedExpression.QualifierExpression is { });

                switch (TryGetStringSplitOptionsConstant(optionsArgument.Value))
                {
                    case StringSplitOptions.None:
                        consumer.AddHighlighting(
                            new UseExpressionResultSuggestion(
                                "The expression is always an array with a single element.",
                                invocationExpression,
                                CreateStringArray([invokedExpression.QualifierExpression.GetText()], invocationExpression)));
                        break;

                    case (StringSplitOptions)2: // todo: use StringSplitOptions.TrimEntries
                        consumer.AddHighlighting(
                            new UseExpressionResultSuggestion(
                                "The expression is always an array with a single trimmed element.",
                                invocationExpression,
                                CreateStringArray([$"{invokedExpression.QualifierExpression.GetText()}.Trim()"], invocationExpression)));
                        break;
                }
                break;

            case (_, _) when Collection.TryFrom(separatorArgument.Value) is { } collection:
            {
                var set = new HashSet<char>(collection.Count);

                foreach (var (element, character) in collection.ElementsWithCharConstants)
                {
                    if (!set.Add(character))
                    {
                        consumer.AddHighlighting(new RedundantElementHint("The character is already passed.", element));
                    }
                }

                break;
            }
        }
    }

    /// <remarks>
    /// <c>text.Split(null, None)</c> → <c>new[] { text }</c> or <c>[text]</c> (C# 12)<para/>
    /// <c>text.Split("", None)</c> → <c>new[] { text }</c> or <c>[text]</c> (C# 12)<para/>
    /// <c>text.Split(null, TrimEntries)</c> → <c>new[] { text.Trim() }</c> or <c>[text.Trim()]</c> (C# 12)<para/>
    /// <c>text.Split("", TrimEntries)</c> → <c>new[] { text.Trim() }</c> or <c>[text.Trim()]</c> (C# 12)<para/>
    /// <c>text.Split(string, StringSplitOptions)</c> → <c>text.Split(char, StringSplitOptions)</c>
    /// </remarks>
    static void AnalyzeSplit_String_StringSplitOptions(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument separatorArgument,
        ICSharpArgument? optionsArgument)
    {
        var separator = IsDefaultValue(separatorArgument.Value) ? "" : TryGetStringConstant(separatorArgument.Value);

        switch (separator)
        {
            case "":
                if (!invocationExpression.IsUsedAsStatement())
                {
                    Debug.Assert(invokedExpression.QualifierExpression is { });

                    switch (optionsArgument is { } ? TryGetStringSplitOptionsConstant(optionsArgument.Value) : StringSplitOptions.None)
                    {
                        case StringSplitOptions.None:
                            consumer.AddHighlighting(
                                new UseExpressionResultSuggestion(
                                    "The expression is always an array with a single element.",
                                    invocationExpression,
                                    CreateStringArray([invokedExpression.QualifierExpression.GetText()], invocationExpression)));
                            break;

                        case (StringSplitOptions)2: // todo: use StringSplitOptions.TrimEntries
                            consumer.AddHighlighting(
                                new UseExpressionResultSuggestion(
                                    "The expression is always an array with a single trimmed element.",
                                    invocationExpression,
                                    CreateStringArray([$"{invokedExpression.QualifierExpression.GetText()}.Trim()"], invocationExpression)));
                            break;
                    }
                }
                break;

            case [var character]:
                if (TryGetParameterNamesIfMethodExists(
                        nameof(string.Split),
                        [PredefinedType.CHAR_FQN, ClrTypeNames.StringSplitOptions],
                        invocationExpression.PsiModule) is [var separatorParameterName, _])
                {
                    consumer.AddHighlighting(
                        new PassSingleCharacterSuggestion(
                            "Pass the single character",
                            separatorArgument,
                            separatorArgument.NameIdentifier is { } ? separatorParameterName : null,
                            character));
                }
                break;
        }
    }

    /// <remarks>
    /// <c>text.Split(string, 0, StringSplitOptions)</c> → <c>Array.Empty&lt;string&gt;()</c> or <c>[]</c> (C# 12)<para/>
    /// <c>text.Split(string, 1, None)</c> → <c>new[] { text }</c> or <c>[text]</c> (C# 12)<para/>
    /// <c>text.Split(string, 1, TrimEntries)</c> → <c>new[] { text.Trim() }</c> or <c>[text.Trim()]</c> (C# 12)<para/>
    /// <c>text.Split(null, int, None)</c> → <c>new[] { text }</c> or <c>[text]</c> (C# 12)<para/>
    /// <c>text.Split("", int, None)</c> → <c>new[] { text }</c> or <c>[text]</c> (C# 12)<para/>
    /// <c>text.Split(null, int, TrimEntries)</c> → <c>new[] { text.Trim() }</c> or <c>[text.Trim()]</c> (C# 12)<para/>
    /// <c>text.Split("", int, TrimEntries)</c> → <c>new[] { text.Trim() }</c> or <c>[text.Trim()]</c> (C# 12)<para/>
    /// <c>text.Split(string, int, StringSplitOptions)</c> → <c>text.Split(char, int, StringSplitOptions)</c>
    /// </remarks>
    static void AnalyzeSplit_String_Int32_StringSplitOptions(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument separatorArgument,
        ICSharpArgument countArgument,
        ICSharpArgument? optionsArgument)
    {
        var separator = IsDefaultValue(separatorArgument.Value) ? "" : TryGetStringConstant(separatorArgument.Value);

        switch (separator, TryGetInt32Constant(countArgument.Value))
        {
            case (_, 0) when !invocationExpression.IsUsedAsStatement():
                consumer.AddHighlighting(
                    new UseExpressionResultSuggestion(
                        "The expression is always an empty array.",
                        invocationExpression,
                        CreateStringArray([], invocationExpression)));
                break;

            case (_, 1) or ("", _) when !invocationExpression.IsUsedAsStatement():
                Debug.Assert(invokedExpression.QualifierExpression is { });

                switch (optionsArgument is { } ? TryGetStringSplitOptionsConstant(optionsArgument.Value) : StringSplitOptions.None)
                {
                    case StringSplitOptions.None:
                        consumer.AddHighlighting(
                            new UseExpressionResultSuggestion(
                                "The expression is always an array with a single element.",
                                invocationExpression,
                                CreateStringArray([invokedExpression.QualifierExpression.GetText()], invocationExpression)));
                        break;

                    case (StringSplitOptions)2: // todo: use StringSplitOptions.TrimEntries
                        consumer.AddHighlighting(
                            new UseExpressionResultSuggestion(
                                "The expression is always an array with a single trimmed element.",
                                invocationExpression,
                                CreateStringArray([$"{invokedExpression.QualifierExpression.GetText()}.Trim()"], invocationExpression)));
                        break;
                }
                break;

            case ([var character], _):
                if (TryGetParameterNamesIfMethodExists(
                        nameof(string.Split),
                        [PredefinedType.CHAR_FQN, PredefinedType.INT_FQN, ClrTypeNames.StringSplitOptions],
                        invocationExpression.PsiModule) is [var separatorParameterName, _, _])
                {
                    consumer.AddHighlighting(
                        new PassSingleCharacterSuggestion(
                            "Pass the single character",
                            separatorArgument,
                            separatorArgument.NameIdentifier is { } ? separatorParameterName : null,
                            character));
                }
                break;
        }
    }

    /// <remarks>
    /// <c>text.Split([""], None)</c> → <c>new[] { text }</c> or <c>[text]</c> (C# 12)<para/>
    /// <c>text.Split([""], TrimEntries)</c> → <c>new[] { text.Trim() }</c> or <c>[text.Trim()]</c> (C# 12)<para/>
    /// <c>text.Split(string[], StringSplitOptions)</c> → <c>text.Split(char[], StringSplitOptions)</c><para/>
    /// <c>text.Split(string[], StringSplitOptions)</c> → <c>text.Split(string[], StringSplitOptions)</c>
    /// </remarks>
    static void AnalyzeSplit_StringArray_StringSplitOptions(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument separatorArgument,
        ICSharpArgument optionsArgument)
    {
        if (Collection.TryFrom(separatorArgument.Value) is { } collection)
        {
            if (collection.StringConstants is [""])
            {
                if (!invocationExpression.IsUsedAsStatement())
                {
                    Debug.Assert(invokedExpression.QualifierExpression is { });

                    switch (TryGetStringSplitOptionsConstant(optionsArgument.Value))
                    {
                        case StringSplitOptions.None:
                            consumer.AddHighlighting(
                                new UseExpressionResultSuggestion(
                                    "The expression is always an array with a single element.",
                                    invocationExpression,
                                    CreateStringArray([invokedExpression.QualifierExpression.GetText()], invocationExpression)));
                            break;

                        case (StringSplitOptions)2: // todo: use StringSplitOptions.TrimEntries
                            consumer.AddHighlighting(
                                new UseExpressionResultSuggestion(
                                    "The expression is always an array with a single trimmed element.",
                                    invocationExpression,
                                    CreateStringArray([$"{invokedExpression.QualifierExpression.GetText()}.Trim()"], invocationExpression)));
                            break;
                    }
                }
            }
            else
            {
                if (collection.StringConstants.All(s => s is [_]))
                {
                    var highlighting = collection.Expression switch
                    {
                        ICollectionExpression collectionExpression => new PassSingleCharactersSuggestion(
                            "Pass the single Character",
                            [..collectionExpression.CollectionElements],
                            [..from s in collection.StringConstants select s[0]]), // todo: check if a string consists of a unicode character

                        IArrayCreationExpression arrayCreationExpression => new PassSingleCharactersSuggestion(
                            "Pass the single Character",
                            arrayCreationExpression,
                            [..from s in collection.StringConstants select s[0]]),

                        _ => throw new NotSupportedException(),
                    };

                    foreach (var element in collection.Elements)
                    {
                        consumer.AddHighlighting(highlighting, element.GetDocumentRange());
                    }
                }
                else
                {
                    var set = new HashSet<string>(collection.Count, StringComparer.Ordinal);

                    foreach (var (element, s) in collection.ElementsWithStringConstants)
                    {
                        if (!set.Add(s))
                        {
                            consumer.AddHighlighting(new RedundantElementHint("The string is already passed.", element));
                        }
                    }
                }
            }
        }
    }

    /// <remarks>
    /// <c>text.Split(string[], 0, StringSplitOptions)</c> → <c>Array.Empty&lt;string&gt;()</c> or <c>[]</c> (C# 12)<para/>
    /// <c>text.Split(string[], 1, None)</c> → <c>new[] { text }</c> or <c>[text]</c> (C# 12)<para/>
    /// <c>text.Split(string[], 1, TrimEntries)</c> → <c>new[] { text.Trim() }</c> or <c>[text.Trim()]</c> (C# 12)<para/>
    /// <c>text.Split([""], int, None)</c> → <c>new[] { text }</c> or <c>[text]</c> (C# 12)<para/>
    /// <c>text.Split([""], int, TrimEntries)</c> → <c>new[] { text.Trim() }</c> or <c>[text.Trim()]</c> (C# 12)<para/>
    /// <c>text.Split(string[], int, StringSplitOptions)</c> → <c>text.Split(char[], int, StringSplitOptions)</c><para/>
    /// <c>text.Split(string[], int, StringSplitOptions)</c> → <c>text.Split(string[], int, StringSplitOptions)</c>
    /// </remarks>
    static void AnalyzeSplit_StringArray_Int32_StringSplitOptions(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument separatorArgument,
        ICSharpArgument countArgument,
        ICSharpArgument optionsArgument)
    {
        switch (Collection.TryFrom(separatorArgument.Value), TryGetInt32Constant(countArgument.Value))
        {
            case (_, 0) when !invocationExpression.IsUsedAsStatement():
                consumer.AddHighlighting(
                    new UseExpressionResultSuggestion(
                        "The expression is always an empty array.",
                        invocationExpression,
                        CreateStringArray([], invocationExpression)));
                break;

            case (_, 1) or ({ StringConstants: [""] }, _) when !invocationExpression.IsUsedAsStatement():
                Debug.Assert(invokedExpression.QualifierExpression is { });

                switch (TryGetStringSplitOptionsConstant(optionsArgument.Value))
                {
                    case StringSplitOptions.None:
                        consumer.AddHighlighting(
                            new UseExpressionResultSuggestion(
                                "The expression is always an array with a single element.",
                                invocationExpression,
                                CreateStringArray([invokedExpression.QualifierExpression.GetText()], invocationExpression)));
                        break;

                    case (StringSplitOptions)2: // todo: use StringSplitOptions.TrimEntries
                        consumer.AddHighlighting(
                            new UseExpressionResultSuggestion(
                                "The expression is always an array with a single trimmed element.",
                                invocationExpression,
                                CreateStringArray([$"{invokedExpression.QualifierExpression.GetText()}.Trim()"], invocationExpression)));
                        break;
                }
                break;

            case ({ } collection, _):
                if (collection.StringConstants.All(s => s is [_]))
                {
                    var highlighting = collection.Expression switch
                    {
                        ICollectionExpression collectionExpression => new PassSingleCharactersSuggestion(
                            "Pass the single Character",
                            [..collectionExpression.CollectionElements],
                            [..from s in collection.StringConstants select s[0]]),

                        IArrayCreationExpression arrayCreationExpression => new PassSingleCharactersSuggestion(
                            "Pass the single Character",
                            arrayCreationExpression,
                            [.. from s in collection.StringConstants select s[0]]),

                        _ => throw new NotSupportedException(),
                    };

                    foreach (var element in collection.Elements)
                    {
                        consumer.AddHighlighting(highlighting, element.GetDocumentRange());
                    }
                }
                else
                {
                    var set = new HashSet<string>(collection.Count, StringComparer.Ordinal);

                    foreach (var (element, s) in collection.ElementsWithStringConstants)
                    {
                        if (!set.Add(s))
                        {
                            consumer.AddHighlighting(new RedundantElementHint("The string is already passed.", element));
                        }
                    }
                }
                break;
        }
    }

    protected override void Run(IInvocationExpression element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
    {
        if (element is { InvokedExpression: IReferenceExpression { QualifierExpression: { }, Reference: var reference } invokedExpression }
            && reference.Resolve().DeclaredElement is IMethod method
            && method.ContainingType.IsSystemString()
            && method is { IsStatic: false, TypeParameters: [], AccessibilityDomain.DomainType: AccessibilityDomain.AccessibilityDomainType.PUBLIC })
        {
            switch (method.ShortName)
            {
                case nameof(string.Contains):
                    switch (method.Parameters, element.Arguments)
                    {
                        case ([{ Type: var valueType }], [var valueArgument]) when valueType.IsString():
                            AnalyzeContains_String(consumer, element, valueArgument);
                            break;

                        case ([{ Type: var valueType }, { Type: var stringComparisonType }], [var valueArgument, _])
                            when valueType.IsString() && IsStringComparison(stringComparisonType):
                            AnalyzeContains_String_StringComparison(consumer, element, valueArgument);
                            break;
                    }
                    break;

                case nameof(string.EndsWith):
                    switch (method.Parameters, element.Arguments)
                    {
                        case ([{ Type: var valueType }], [var valueArgument]) when valueType.IsChar():
                            AnalyzeEndsWith_Char(consumer, element, invokedExpression, valueArgument);
                            break;

                        case ([{ Type: var valueType }], [var valueArgument]) when valueType.IsString():
                            AnalyzeEndsWith_String(consumer, element, valueArgument);
                            break;

                        case ([{ Type: var valueType }, { Type: var stringComparisonType }], [var valueArgument, var comparisonTypeArgument])
                            when valueType.IsString() && IsStringComparison(stringComparisonType):
                            AnalyzeEndsWith_String_StringComparison(consumer, element, invokedExpression, valueArgument, comparisonTypeArgument);
                            break;
                    }
                    break;

                case nameof(string.IndexOf):
                    switch (method.Parameters, element.Arguments)
                    {
                        case ([{ Type: var valueType }], [var valueArgument]) when valueType.IsChar():
                            AnalyzeIndexOf_Char(consumer, element, invokedExpression, valueArgument);
                            break;

                        case ([{ Type: var valueType }, { Type: var startIndexType }], [_, var startIndexArgument])
                            when valueType.IsChar() && startIndexType.IsInt():
                            AnalyzeIndexOf_Char_Int32(consumer, startIndexArgument);
                            break;

                        case ([{ Type: var valueType }, { Type: var stringComparisonType }], [var valueArgument, var comparisonTypeArgument])
                            when valueType.IsChar() && IsStringComparison(stringComparisonType):
                            AnalyzeIndexOf_Char_StringComparison(consumer, element, invokedExpression, valueArgument, comparisonTypeArgument);
                            break;

                        case ([{ Type: var valueType }], [var valueArgument]) when valueType.IsString():
                            AnalyzeIndexOf_String(consumer, element, invokedExpression, valueArgument);
                            break;

                        case ([{ Type: var valueType }, { Type: var startIndexType }], [_, var startIndexArgument])
                            when valueType.IsString() && startIndexType.IsInt():
                            AnalyzeIndexOf_String_Int32(consumer, startIndexArgument);
                            break;

                        case ([{ Type: var valueType }, { Type: var stringComparisonType }], [var valueArgument, var comparisonTypeArgument])
                            when valueType.IsString() && IsStringComparison(stringComparisonType):
                            AnalyzeIndexOf_String_StringComparison(consumer, element, invokedExpression, valueArgument, comparisonTypeArgument);
                            break;

                        case ([{ Type: var valueType }, { Type: var startIndexType }, { Type: var stringComparisonType }], [
                            _, var startIndexArgument, _,
                        ]) when valueType.IsString() && startIndexType.IsInt() && IsStringComparison(stringComparisonType):
                            AnalyzeIndexOf_String_Int32_StringComparison(consumer, startIndexArgument);
                            break;
                    }
                    break;

                case nameof(string.IndexOfAny):
                    switch (method.Parameters, element.Arguments)
                    {
                        case ([_, { Type: var startIndexType }], [_, var startIndexArgument]) when startIndexType.IsInt():
                            AnalyzeIndexOfAny(consumer, startIndexArgument);
                            break;
                    }
                    break;

                case nameof(string.LastIndexOf):
                    switch (method.Parameters, element.Arguments)
                    {
                        case ([{ Type: var valueType }, { Type: var startIndexType }], [_, var startIndexArgument])
                            when valueType.IsChar() && startIndexType.IsInt():
                            AnalyzeLastIndexOf_Char_Int32(consumer, element, startIndexArgument);
                            break;

                        case ([{ Type: var valueType }], [var valueArgument]) when valueType.IsString():
                            AnalyzeLastIndexOf_String(consumer, element, invokedExpression, valueArgument);
                            break;

                        case ([{ Type: var valueType }, { Type: var stringComparisonType }], [var valueArgument, var comparisonTypeArgument])
                            when valueType.IsString() && IsStringComparison(stringComparisonType):
                            AnalyzeLastIndexOf_String_StringComparison(consumer, element, invokedExpression, valueArgument, comparisonTypeArgument);
                            break;
                    }
                    break;

                case nameof(string.PadLeft):
                    switch (method.Parameters, element.Arguments)
                    {
                        case ([{ Type: var totalWidthType }], [var totalWidthArgument]) when totalWidthType.IsInt():
                            AnalyzePadLeft_Int32(consumer, element, invokedExpression, totalWidthArgument);
                            break;

                        case ([{ Type: var totalWidthType }, { Type: var paddingCharType }], [var totalWidthArgument, var paddingCharArgument])
                            when totalWidthType.IsInt() && paddingCharType.IsChar():
                            AnalyzePadLeft_Int32_Char(consumer, element, invokedExpression, totalWidthArgument, paddingCharArgument);
                            break;
                    }
                    break;

                case nameof(string.PadRight):
                    switch (method.Parameters, element.Arguments)
                    {
                        case ([{ Type: var totalWidthType }], [var totalWidthArgument]) when totalWidthType.IsInt():
                            AnalyzePadRight_Int32(consumer, element, invokedExpression, totalWidthArgument);
                            break;

                        case ([{ Type: var totalWidthType }, { Type: var paddingCharType }], [var totalWidthArgument, var paddingCharArgument])
                            when totalWidthType.IsInt() && paddingCharType.IsChar():
                            AnalyzePadRight_Int32_Char(consumer, element, invokedExpression, totalWidthArgument, paddingCharArgument);
                            break;
                    }
                    break;

                case nameof(string.Remove):
                    switch (method.Parameters, element.Arguments)
                    {
                        case ([{ Type: var startIndexType }], [var startIndexArgument]) when startIndexType.IsInt():
                            AnalyzeRemove_Int32(consumer, element, invokedExpression, startIndexArgument);
                            break;

                        case ([{ Type: var startIndexType }, { Type: var countType }], [var startIndexArgument, var countArgument])
                            when startIndexType.IsInt() && countType.IsInt():
                            AnalyzeRemove_Int32_Int32(consumer, element, invokedExpression, startIndexArgument, countArgument);
                            break;
                    }
                    break;

                case nameof(string.Replace):
                    switch (method.Parameters, element.Arguments)
                    {
                        case ([{ Type: var oldValueType }, { Type: var newValueType }, { Type: var stringComparisonType }], [
                            var oldValueArgument, var newValueArgument, var comparisonTypeArgument,
                        ]) when oldValueType.IsString() && newValueType.IsString() && IsStringComparison(stringComparisonType):
                            AnalyzeReplace_String_String_StringComparison(
                                consumer,
                                element,
                                invokedExpression,
                                oldValueArgument,
                                newValueArgument,
                                comparisonTypeArgument);
                            break;

                        case ([{ Type: var oldCharType }, { Type: var newCharType }], [var oldCharArgument, var newCharArgument])
                            when oldCharType.IsChar() && newCharType.IsChar():
                            AnalyzeReplace_Char_Char(consumer, element, invokedExpression, oldCharArgument, newCharArgument);
                            break;

                        case ([{ Type: var oldValueType }, { Type: var newValueType }], [var oldValueArgument, var newValueArgument])
                            when oldValueType.IsString() && newValueType.IsString():
                            AnalyzeReplace_String_String(consumer, element, invokedExpression, oldValueArgument, newValueArgument);
                            break;
                    }
                    break;

                case nameof(string.Split):
                    switch (method.Parameters, element.Arguments)
                    {
                        case ([{ Type: var separatorType }, { Type: var countType }, { Type: var optionsType }], { Count: 2 or 3 } arguments)
                            when separatorType.IsChar() && countType.IsInt() && IsStringSplitOptions(optionsType):
                            AnalyzeSplit_Char_Int32_StringSplitOptions(
                                consumer,
                                element,
                                invokedExpression,
                                arguments[1],
                                arguments.Count == 3 ? arguments[^1] : null);
                            break;

                        case ([{ Type: var separatorType }], var arguments) when separatorType.IsGenericArrayOf(PredefinedType.CHAR_FQN, element):
                            AnalyzeSplit_CharArray(consumer, arguments);
                            break;

                        case ([{ Type: var separatorType }, { Type: var countType }], [var separatorArgument, var countArgument])
                            when separatorType.IsGenericArrayOf(PredefinedType.CHAR_FQN, element) && countType.IsInt():
                            AnalyzeSplit_CharArray_Int32(consumer, element, invokedExpression, separatorArgument, countArgument);
                            break;

                        case ([{ Type: var separatorType }, { Type: var optionsType }], [var separatorArgument, _])
                            when separatorType.IsGenericArrayOf(PredefinedType.CHAR_FQN, element) && IsStringSplitOptions(optionsType):
                            AnalyzeSplit_CharArray_StringSplitOptions(consumer, separatorArgument);
                            break;

                        case ([{ Type: var separatorType }, { Type: var countType }, { Type: var optionsType }], [
                                var separatorArgument, var countArgument, var optionsArgument,
                            ]) when separatorType.IsGenericArrayOf(PredefinedType.CHAR_FQN, element)
                            && countType.IsInt()
                            && IsStringSplitOptions(optionsType):
                            AnalyzeSplit_CharArray_Int32_StringSplitOptions(
                                consumer,
                                element,
                                invokedExpression,
                                separatorArgument,
                                countArgument,
                                optionsArgument);
                            break;

                        case ([{ Type: var separatorType }, { Type: var optionsType }], { Count: 1 or 2 } arguments)
                            when separatorType.IsString() && IsStringSplitOptions(optionsType):
                            AnalyzeSplit_String_StringSplitOptions(
                                consumer,
                                element,
                                invokedExpression,
                                arguments[0],
                                arguments.Count == 2 ? arguments[^1] : null);
                            break;

                        case ([{ Type: var separatorType }, { Type: var countType }, { Type: var optionsType }], { Count: 2 or 3 } arguments)
                            when separatorType.IsString() && countType.IsInt() && IsStringSplitOptions(optionsType):
                            AnalyzeSplit_String_Int32_StringSplitOptions(
                                consumer,
                                element,
                                invokedExpression,
                                arguments[0],
                                arguments[1],
                                arguments.Count == 3 ? arguments[^1] : null);
                            break;

                        case ([{ Type: var separatorType }, { Type: var optionsType }], [var separatorArgument, var optionsArgument])
                            when separatorType.IsGenericArrayOf(PredefinedType.STRING_FQN, element) && IsStringSplitOptions(optionsType):
                            AnalyzeSplit_StringArray_StringSplitOptions(consumer, element, invokedExpression, separatorArgument, optionsArgument);
                            break;

                        case ([{ Type: var separatorType }, { Type: var countType }, { Type: var optionsType }], [
                                var separatorArgument, var countArgument, var optionsArgument,
                            ]) when separatorType.IsGenericArrayOf(PredefinedType.STRING_FQN, element)
                            && countType.IsInt()
                            && IsStringSplitOptions(optionsType):
                            AnalyzeSplit_StringArray_Int32_StringSplitOptions(
                                consumer,
                                element,
                                invokedExpression,
                                separatorArgument,
                                countArgument,
                                optionsArgument);
                            break;
                    }
                    break;
            }
        }
    }
}