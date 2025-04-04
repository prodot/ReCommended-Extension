using System.Globalization;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReCommendedExtension.Extensions;
using ReCommendedExtension.Extensions.MethodFinding;

namespace ReCommendedExtension.Analyzers.BaseTypes.Analyzers;

/// <remarks>
/// C# language version checks are only done when a quick fix would require it.
/// </remarks>
[ElementProblemAnalyzer(
    typeof(IInvocationExpression),
    HighlightingTypes = [typeof(UseBinaryOperationSuggestion), typeof(UseExpressionResultSuggestion), typeof(RedundantArgumentHint)])]
public sealed class ByteAnalyzer : ElementProblemAnalyzer<IInvocationExpression>
{
    [Pure]
    static bool IsNumberStyles(IType type) => type.IsClrType(ClrTypeNames.NumberStyles);

    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Underscore character used intentionally as a separator.")]
    static class ParameterTypes
    {
        public static IReadOnlyList<ParameterType> String { get; } = [new() { ClrTypeName = PredefinedType.STRING_FQN }];

        public static IReadOnlyList<ParameterType> IFormatProvider { get; } = [new() { ClrTypeName = PredefinedType.IFORMATPROVIDER_FQN }];

        public static IReadOnlyList<ParameterType> String_IFormatProvider { get; } =
        [
            new() { ClrTypeName = PredefinedType.STRING_FQN }, new() { ClrTypeName = PredefinedType.IFORMATPROVIDER_FQN },
        ];

        public static IReadOnlyList<ParameterType> String_NumberStyles { get; } =
        [
            new() { ClrTypeName = PredefinedType.STRING_FQN }, new() { ClrTypeName = ClrTypeNames.NumberStyles },
        ];

        public static IReadOnlyList<ParameterType> String_Byte { get; } =
        [
            new() { ClrTypeName = PredefinedType.STRING_FQN }, new() { ClrTypeName = PredefinedType.BYTE_FQN },
        ];

        public static IReadOnlyList<ParameterType> ReadOnlySpanOfT_Byte { get; } =
        [
            new GenericParameterType { ClrTypeName = PredefinedType.SYSTEM_READ_ONLY_SPAN_FQN }, new() { ClrTypeName = PredefinedType.BYTE_FQN },
        ];

        public static IReadOnlyList<ParameterType> String_IFormatProvider_Byte { get; } =
        [
            new() { ClrTypeName = PredefinedType.STRING_FQN },
            new() { ClrTypeName = PredefinedType.IFORMATPROVIDER_FQN },
            new() { ClrTypeName = PredefinedType.BYTE_FQN },
        ];

        public static IReadOnlyList<ParameterType> ReadOnlySpanOfT_NumberStyles_IFormatProvider { get; } =
        [
            new GenericParameterType { ClrTypeName = PredefinedType.SYSTEM_READ_ONLY_SPAN_FQN },
            new() { ClrTypeName = ClrTypeNames.NumberStyles },
            new() { ClrTypeName = PredefinedType.IFORMATPROVIDER_FQN },
        ];

        public static IReadOnlyList<ParameterType> ReadOnlySpanOfT_IFormatProvider_Byte { get; } =
        [
            new GenericParameterType { ClrTypeName = PredefinedType.SYSTEM_READ_ONLY_SPAN_FQN },
            new() { ClrTypeName = PredefinedType.IFORMATPROVIDER_FQN },
            new() { ClrTypeName = PredefinedType.BYTE_FQN },
        ];
    }

    /// <remarks>
    /// <c>byte.Clamp(value, n, n)</c> → <c>n</c> (.NET 7)<para/>
    /// <c>byte.Clamp(value, 0, 255)</c> → <c>value</c> (.NET 7)
    /// </remarks>
    static void AnalyzeClamp(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument valueArgument,
        ICSharpArgument minArgument,
        ICSharpArgument maxArgument)
    {
        if (!invocationExpression.IsUsedAsStatement())
        {
            switch (minArgument.Value.TryGetByteConstant(out var implicitlyConverted), maxArgument.Value.TryGetByteConstant(out _))
            {
                case (0, byte.MaxValue) when valueArgument.Value is { } value:
                {
                    var cast = value.TryGetByteConstant(out var valueImplicitlyConverted) is { }
                        && valueImplicitlyConverted
                        && invocationExpression.TryGetTargetType() == null
                            ? "(byte)"
                            : "";
                    consumer.AddHighlighting(
                        new UseExpressionResultSuggestion(
                            "The expression is always the same as the first argument.",
                            invocationExpression,
                            $"{cast}{value.GetText()}"));
                    break;
                }

                case ({ } min, { } max) when min == max:
                {
                    var cast = implicitlyConverted && invocationExpression.TryGetTargetType() == null ? "(byte)" : "";
                    consumer.AddHighlighting(
                        new UseExpressionResultSuggestion($"The expression is always {min}.", invocationExpression, $"{cast}{min}"));
                    break;
                }
            }
        }
    }

    /// <remarks>
    /// <c>byte.DivRem(0, right)</c> → <c>(0, 0)</c> (.NET 7)<para/>
    /// <c>byte.DivRem(left, 1)</c> → <c>(left, 0)</c> (.NET 7)
    /// </remarks>
    static void AnalyzeDivRem(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument leftArgument,
        ICSharpArgument rightArgument)
    {
        if (!invocationExpression.IsUsedAsStatement())
        {
            switch (leftArgument.Value.TryGetByteConstant(out _), rightArgument.Value.TryGetByteConstant(out _))
            {
                case (0, { } and not 0):
                    consumer.AddHighlighting(
                        new UseExpressionResultSuggestion(
                            "The expression is always (0, 0).",
                            invocationExpression,
                            invocationExpression.TryGetTargetType() == null ? "(Quotient: (byte)0, Remainder: (byte)0)" : "(0, 0)"));
                    break;

                case (_, 1) when leftArgument.Value is { } left:
                    var replacement = invocationExpression.TryGetTargetType() == null
                        ? leftArgument.Value.TryGetByteConstant(out var leftImplicitlyConverted) is { } && leftImplicitlyConverted
                            ? $"(Quotient: (byte){left.GetText()}, Remainder: (byte)0)"
                            : $"(Quotient: {left.GetText()}, Remainder: (byte)0)"
                        : $"({left.GetText()}, 0)";
                    consumer.AddHighlighting(
                        new UseExpressionResultSuggestion(
                            "The expression is always the same as the first argument with no remainder.",
                            invocationExpression,
                            replacement));
                    break;
            }
        }
    }

    /// <remarks>
    /// <c>number.Equals(obj)</c> → <c>flag == obj</c>
    /// </remarks>
    static void AnalyzeEquals_Byte(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument objArgument)
    {
        Debug.Assert(invokedExpression.QualifierExpression is { });

        if (!invocationExpression.IsUsedAsStatement() && objArgument.Value is { })
        {
            consumer.AddHighlighting(
                new UseBinaryOperationSuggestion(
                    "Use the '==' operator.",
                    invocationExpression,
                    "==",
                    invokedExpression.QualifierExpression,
                    objArgument.Value));
        }
    }

    /// <remarks>
    /// <c>flag.Equals(null)</c> → <c>false</c>
    /// </remarks>
    static void AnalyzeEquals_Object(IHighlightingConsumer consumer, IInvocationExpression invocationExpression, ICSharpArgument objArgument)
    {
        if (!invocationExpression.IsUsedAsStatement() && objArgument.Value.IsDefaultValue())
        {
            consumer.AddHighlighting(new UseExpressionResultSuggestion("The expression is always false.", invocationExpression, "false"));
        }
    }

    /// <remarks>
    /// <c>flag.GetTypeCode()</c> → <c>TypeCode.Byte</c>
    /// </remarks>
    static void AnalyzeGetTypeCode(IHighlightingConsumer consumer, IInvocationExpression invocationExpression)
    {
        if (!invocationExpression.IsUsedAsStatement())
        {
            consumer.AddHighlighting(
                new UseExpressionResultSuggestion(
                    $"The expression is always {nameof(TypeCode)}.{nameof(TypeCode.Byte)}.",
                    invocationExpression,
                    $"{nameof(TypeCode)}.{nameof(TypeCode.Byte)}"));
        }
    }

    /// <remarks>
    /// <c>byte.Max(n, n)</c> → <c>n</c> (.NET 7)
    /// </remarks>
    static void AnalyzeMax(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument xArgument,
        ICSharpArgument yArgument)
    {
        if (!invocationExpression.IsUsedAsStatement()
            && xArgument.Value.TryGetByteConstant(out var implicitlyConverted) is { } x
            && yArgument.Value.TryGetByteConstant(out _) is { } y
            && x == y)
        {
            var cast = implicitlyConverted && invocationExpression.TryGetTargetType() == null ? "(byte)" : "";
            consumer.AddHighlighting(new UseExpressionResultSuggestion($"The expression is always {x}.", invocationExpression, $"{cast}{x}"));
        }
    }

    /// <remarks>
    /// <c>byte.Min(n, n)</c> → <c>n</c> (.NET 7)
    /// </remarks>
    static void AnalyzeMin(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument xArgument,
        ICSharpArgument yArgument)
    {
        if (!invocationExpression.IsUsedAsStatement()
            && xArgument.Value.TryGetByteConstant(out var implicitlyConverted) is { } x
            && yArgument.Value.TryGetByteConstant(out _) is { } y
            && x == y)
        {
            var cast = implicitlyConverted && invocationExpression.TryGetTargetType() == null ? "(byte)" : "";
            consumer.AddHighlighting(new UseExpressionResultSuggestion($"The expression is always {x}.", invocationExpression, $"{cast}{x}"));
        }
    }

    /// <remarks>
    /// <c>byte.Parse(s, NumberStyles.Integer)</c> → <c>byte.Parse(s)</c>
    /// </remarks>
    static void AnalyzeParse_String_NumberStyles(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument styleArgument)
    {
        if (styleArgument.Value.TryGetNumberStylesConstant() == NumberStyles.Integer
            && PredefinedType.BYTE_FQN.HasMethod(
                new MethodSignature { Name = nameof(byte.Parse), ParameterTypes = ParameterTypes.String, IsStatic = true },
                invocationExpression.PsiModule))
        {
            consumer.AddHighlighting(
                new RedundantArgumentHint($"Passing {nameof(NumberStyles)}.{nameof(NumberStyles.Integer)} is redundant.", styleArgument));
        }
    }

    /// <remarks>
    /// <c>byte.Parse(s, null)</c> → <c>byte.Parse(s)</c>
    /// </remarks>
    static void AnalyzeParse_String_IFormatProvider(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument providerArgument)
    {
        if (providerArgument.Value.IsDefaultValue()
            && PredefinedType.BYTE_FQN.HasMethod(
                new MethodSignature { Name = nameof(byte.Parse), ParameterTypes = ParameterTypes.String, IsStatic = true },
                invocationExpression.PsiModule))
        {
            consumer.AddHighlighting(new RedundantArgumentHint("Passing null is redundant.", providerArgument));
        }
    }

    /// <remarks>
    /// <c>byte.Parse(s, NumberStyles.Integer, provider)</c> → <c>byte.Parse(s, provider)</c><para/>
    /// <c>byte.Parse(s, style, null)</c> → <c>byte.Parse(s, style)</c>
    /// </remarks>
    static void AnalyzeParse_String_NumberStyles_IFormatProvider(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument styleArgument,
        ICSharpArgument providerArgument)
    {
        if (styleArgument.Value.TryGetNumberStylesConstant() == NumberStyles.Integer
            && PredefinedType.BYTE_FQN.HasMethod(
                new MethodSignature { Name = nameof(byte.Parse), ParameterTypes = ParameterTypes.String_IFormatProvider, IsStatic = true },
                invocationExpression.PsiModule))
        {
            consumer.AddHighlighting(
                new RedundantArgumentHint($"Passing {nameof(NumberStyles)}.{nameof(NumberStyles.Integer)} is redundant.", styleArgument));
        }

        if (providerArgument.Value.IsDefaultValue()
            && PredefinedType.BYTE_FQN.HasMethod(
                new MethodSignature { Name = nameof(byte.Parse), ParameterTypes = ParameterTypes.String_NumberStyles, IsStatic = true },
                invocationExpression.PsiModule))
        {
            consumer.AddHighlighting(new RedundantArgumentHint("Passing null is redundant.", providerArgument));
        }
    }

    /// <remarks>
    /// <c>byte.Parse(s, null)</c> → <c>byte.Parse(s)</c> (.NET Core 2.1)
    /// </remarks>
    static void AnalyzeParse_ReadOnlySpanOfChar_IFormatProvider(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument providerArgument)
    {
        if (providerArgument.Value.IsDefaultValue()
            && PredefinedType.BYTE_FQN.HasMethod(
                new MethodSignature
                {
                    Name = nameof(byte.Parse), ParameterTypes = ParameterTypes.ReadOnlySpanOfT_NumberStyles_IFormatProvider, IsStatic = true,
                },
                invocationExpression.PsiModule))
        {
            consumer.AddHighlighting(new RedundantArgumentHint("Passing null is redundant.", providerArgument));
        }
    }

    /// <remarks>
    /// <c>byte.Parse(utf8Text, null)</c> → <c>byte.Parse(utf8Text)</c> (.NET 8)
    /// </remarks>
    static void AnalyzeParse_ReadOnlySpanOfByte_IFormatProvider(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument providerArgument)
    {
        if (providerArgument.Value.IsDefaultValue()
            && PredefinedType.BYTE_FQN.HasMethod(
                new MethodSignature
                {
                    Name = nameof(byte.Parse),
                    ParameterTypes = ParameterTypes.ReadOnlySpanOfT_NumberStyles_IFormatProvider,
                    IsStatic = true,
                },
                invocationExpression.PsiModule))
        {
            consumer.AddHighlighting(new RedundantArgumentHint("Passing null is redundant.", providerArgument));
        }
    }

    /// <remarks>
    /// <c>byte.RotateLeft(n, 0)</c> → <c>n</c> (.NET 7)
    /// </remarks>
    static void AnalyzeRotateLeft(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument valueArgument,
        ICSharpArgument rotateAmountArgument)
    {
        if (!invocationExpression.IsUsedAsStatement() && rotateAmountArgument.Value.TryGetInt32Constant() is 0 && valueArgument.Value is { } value)
        {
            var cast = value.TryGetByteConstant(out var implicitlyConverted) is { }
                && implicitlyConverted
                && invocationExpression.TryGetTargetType() == null
                    ? "(byte)"
                    : "";
            consumer.AddHighlighting(
                new UseExpressionResultSuggestion(
                    "The expression is always the same as the first argument.",
                    invocationExpression,
                    $"{cast}{value.GetText()}"));
        }
    }

    /// <remarks>
    /// <c>byte.RotateRight(n, 0)</c> → <c>n</c> (.NET 7)
    /// </remarks>
    static void AnalyzeRotateRight(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument valueArgument,
        ICSharpArgument rotateAmountArgument)
    {
        if (!invocationExpression.IsUsedAsStatement() && rotateAmountArgument.Value.TryGetInt32Constant() is 0 && valueArgument.Value is { } value)
        {
            var cast = value.TryGetByteConstant(out var implicitlyConverted) is { }
                && implicitlyConverted
                && invocationExpression.TryGetTargetType() == null
                    ? "(byte)"
                    : "";
            consumer.AddHighlighting(
                new UseExpressionResultSuggestion(
                    "The expression is always the same as the first argument.",
                    invocationExpression,
                    $"{cast}{value.GetText()}"));
        }
    }

    /// <remarks>
    /// <c>number.ToString(null)</c> → <c>number.ToString()</c><para/>
    /// <c>number.ToString("")</c> → <c>number.ToString()</c>
    /// </remarks>
    static void AnalyzeToString_String(IHighlightingConsumer consumer, IInvocationExpression invocationExpression, ICSharpArgument formatArgument)
    {
        if ((formatArgument.Value.IsDefaultValue() || formatArgument.Value.TryGetStringConstant() == "")
            && PredefinedType.BYTE_FQN.HasMethod(
                new MethodSignature { Name = nameof(byte.ToString), ParameterTypes = [] },
                invocationExpression.PsiModule))
        {
            consumer.AddHighlighting(new RedundantArgumentHint("Passing null or an empty string is redundant.", formatArgument));
        }
    }

    /// <remarks>
    /// <c>number.ToString(null)</c> → <c>number.ToString()</c>
    /// </remarks>
    static void AnalyzeToString_IFormatProvider(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument providerArgument)
    {
        if (providerArgument.Value.IsDefaultValue()
            && PredefinedType.BYTE_FQN.HasMethod(
                new MethodSignature { Name = nameof(byte.ToString), ParameterTypes = [] },
                invocationExpression.PsiModule))
        {
            consumer.AddHighlighting(new RedundantArgumentHint("Passing null is redundant.", providerArgument));
        }
    }

    /// <remarks>
    /// <c>number.ToString(null, provider)</c> → <c>number.ToString(provider)</c><para/>
    /// <c>number.ToString("", provider)</c> → <c>number.ToString(provider)</c><para/>
    /// <c>number.ToString(format, null)</c> → <c>number.ToString(format)</c>
    /// </remarks>
    static void AnalyzeToString_String_IFormatProvider(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument formatArgument,
        ICSharpArgument providerArgument)
    {
        if ((formatArgument.Value.IsDefaultValue() || formatArgument.Value.TryGetStringConstant() == "")
            && PredefinedType.BYTE_FQN.HasMethod(
                new MethodSignature { Name = nameof(byte.ToString), ParameterTypes = ParameterTypes.IFormatProvider },
                invocationExpression.PsiModule))
        {
            consumer.AddHighlighting(new RedundantArgumentHint("Passing null or an empty string is redundant.", formatArgument));
        }

        if (providerArgument.Value.IsDefaultValue()
            && PredefinedType.BYTE_FQN.HasMethod(
                new MethodSignature { Name = nameof(byte.ToString), ParameterTypes = ParameterTypes.String },
                invocationExpression.PsiModule))
        {
            consumer.AddHighlighting(new RedundantArgumentHint("Passing null is redundant.", providerArgument));
        }
    }

    /// <remarks>
    /// <c>byte.TryParse(s, NumberStyles.Integer, provider, out result)</c> → <c>byte.TryParse(s, provider, out result)</c> (.NET 7)
    /// </remarks>
    static void AnalyzeTryParse_String_NumberStyles_IFormatProvider_Byte(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument styleArgument)
    {
        if (styleArgument.Value.TryGetNumberStylesConstant() == NumberStyles.Integer
            && PredefinedType.BYTE_FQN.HasMethod(
                new MethodSignature { Name = nameof(byte.TryParse), ParameterTypes = ParameterTypes.String_IFormatProvider_Byte, IsStatic = true },
                invocationExpression.PsiModule))
        {
            consumer.AddHighlighting(
                new RedundantArgumentHint($"Passing {nameof(NumberStyles)}.{nameof(NumberStyles.Integer)} is redundant.", styleArgument));
        }
    }

    /// <remarks>
    /// <c>byte.TryParse(s, null, out result)</c> → <c>byte.TryParse(s, out result)</c>
    /// </remarks>
    static void AnalyzeTryParse_String_IFormatProvider_Byte(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument providerArgument)
    {
        if (providerArgument.Value.IsDefaultValue()
            && PredefinedType.BYTE_FQN.HasMethod(
                new MethodSignature { Name = nameof(byte.TryParse), ParameterTypes = ParameterTypes.String_Byte, IsStatic = true },
                invocationExpression.PsiModule))
        {
            consumer.AddHighlighting(new RedundantArgumentHint("Passing null is redundant.", providerArgument));
        }
    }

    /// <remarks>
    /// <c>byte.TryParse(s, NumberStyles.Integer, provider, out result)</c> → <c>byte.TryParse(s, provider, out result)</c> (.NET 7)
    /// </remarks>
    static void AnalyzeTryParse_ReadOnlySpanOfChar_NumberStyles_IFormatProvider_Byte(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument styleArgument)
    {
        if (styleArgument.Value.TryGetNumberStylesConstant() == NumberStyles.Integer
            && PredefinedType.BYTE_FQN.HasMethod(
                new MethodSignature
                {
                    Name = nameof(byte.TryParse), ParameterTypes = ParameterTypes.ReadOnlySpanOfT_IFormatProvider_Byte, IsStatic = true,
                },
                invocationExpression.PsiModule))
        {
            consumer.AddHighlighting(
                new RedundantArgumentHint($"Passing {nameof(NumberStyles)}.{nameof(NumberStyles.Integer)} is redundant.", styleArgument));
        }
    }

    /// <remarks>
    /// <c>byte.TryParse(s, null, out result)</c> → <c>byte.TryParse(s, out result)</c> (.NET Core 2.1)
    /// </remarks>
    static void AnalyzeTryParse_ReadOnlySpanOfChar_IFormatProvider_Byte(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument providerArgument)
    {
        if (providerArgument.Value.IsDefaultValue()
            && PredefinedType.BYTE_FQN.HasMethod(
                new MethodSignature { Name = nameof(byte.TryParse), ParameterTypes = ParameterTypes.ReadOnlySpanOfT_Byte, IsStatic = true },
                invocationExpression.PsiModule))
        {
            consumer.AddHighlighting(new RedundantArgumentHint("Passing null is redundant.", providerArgument));
        }
    }

    /// <remarks>
    /// <c>byte.TryParse(utf8Text, NumberStyles.Integer, provider, out result)</c> → <c>byte.TryParse(utf8Text, provider, out result)</c> (.NET 8)
    /// </remarks>
    static void AnalyzeTryParse_ReadOnlySpanOfByte_NumberStyles_IFormatProvider_Byte(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument styleArgument)
    {
        if (styleArgument.Value.TryGetNumberStylesConstant() == NumberStyles.Integer
            && PredefinedType.BYTE_FQN.HasMethod(
                new MethodSignature
                {
                    Name = nameof(byte.TryParse),
                    ParameterTypes = ParameterTypes.ReadOnlySpanOfT_IFormatProvider_Byte,
                    IsStatic = true,
                },
                invocationExpression.PsiModule))
        {
            consumer.AddHighlighting(
                new RedundantArgumentHint($"Passing {nameof(NumberStyles)}.{nameof(NumberStyles.Integer)} is redundant.", styleArgument));
        }
    }

    /// <remarks>
    /// <c>byte.TryParse(utf8Text, null, out result)</c> → <c>byte.TryParse(utf8Text, out result)</c> (.NET 8)
    /// </remarks>
    static void AnalyzeTryParse_ReadOnlySpanOfByte_IFormatProvider_Byte(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument providerArgument)
    {
        if (providerArgument.Value.IsDefaultValue()
            && PredefinedType.BYTE_FQN.HasMethod(
                new MethodSignature { Name = nameof(byte.TryParse), ParameterTypes = ParameterTypes.ReadOnlySpanOfT_Byte, IsStatic = true },
                invocationExpression.PsiModule))
        {
            consumer.AddHighlighting(new RedundantArgumentHint("Passing null is redundant.", providerArgument));
        }
    }

    protected override void Run(IInvocationExpression element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
    {
        if (element is { InvokedExpression: IReferenceExpression { Reference: var reference } invokedExpression }
            && reference.Resolve().DeclaredElement is IMethod
            {
                AccessibilityDomain.DomainType: AccessibilityDomain.AccessibilityDomainType.PUBLIC, TypeParameters: [],
            } method
            && method.ContainingType.IsClrType(PredefinedType.BYTE_FQN))
        {
            switch (invokedExpression, method)
            {
                case ({ QualifierExpression: { } }, { IsStatic: false }):
                    switch (method.ShortName)
                    {
                        case nameof(byte.Equals):
                            switch (method.Parameters, element.Arguments)
                            {
                                case ([{ Type: var objType }], [var objArgument]) when objType.IsByte():
                                    AnalyzeEquals_Byte(consumer, element, invokedExpression, objArgument);
                                    break;

                                case ([{ Type: var objType }], [var objArgument]) when objType.IsObject():
                                    AnalyzeEquals_Object(consumer, element, objArgument);
                                    break;
                            }
                            break;

                        case nameof(bool.GetTypeCode):
                            switch (method.Parameters, element.Arguments)
                            {
                                case ([], []): AnalyzeGetTypeCode(consumer, element); break;
                            }
                            break;

                        case nameof(bool.ToString):
                            switch (method.Parameters, element.Arguments)
                            {
                                case ([{ Type: var formatType }], [var formatArgument]) when formatType.IsString():
                                    AnalyzeToString_String(consumer, element, formatArgument);
                                    break;

                                case ([{ Type: var providerType }], [var providerArgument]) when providerType.IsIFormatProvider():
                                    AnalyzeToString_IFormatProvider(consumer, element, providerArgument);
                                    break;

                                case ([{ Type: var formatType }, { Type: var providerType }], [var formatArgument, var providerArgument])
                                    when formatType.IsString() && providerType.IsIFormatProvider():

                                    AnalyzeToString_String_IFormatProvider(consumer, element, formatArgument, providerArgument);
                                    break;
                            }
                            break;
                    }
                    break;

                case (_, { IsStatic: true }):
                    switch (method.ShortName)
                    {
                        case "Clamp": // todo: nameof(byte.Clamp) when available
                            switch (method.Parameters, element.Arguments)
                            {
                                case ([{ Type: var valueType }, { Type: var minType }, { Type: var maxType }], [
                                    var valueArgument, var minArgument, var maxArgument,
                                ]) when valueType.IsByte() && minType.IsByte() && maxType.IsByte():
                                    AnalyzeClamp(consumer, element, valueArgument, minArgument, maxArgument);
                                    break;
                            }
                            break;

                        case "DivRem": // todo: nameof(byte.DivRem) when available
                            switch (method.Parameters, element.Arguments)
                            {
                                case ([{ Type: var leftType }, { Type: var rightType }], [var leftArgument, var rightArgument])
                                    when leftType.IsByte() && rightType.IsByte():

                                    AnalyzeDivRem(consumer, element, leftArgument, rightArgument);
                                    break;
                            }
                            break;

                        case "Max": // todo: nameof(byte.Max) when available
                            switch (method.Parameters, element.Arguments)
                            {
                                case ([{ Type: var xType }, { Type: var yType }], [var xArgument, var yArgument])
                                    when xType.IsByte() && yType.IsByte():

                                    AnalyzeMax(consumer, element, xArgument, yArgument);
                                    break;
                            }
                            break;

                        case "Min": // todo: nameof(byte.Min) when available
                            switch (method.Parameters, element.Arguments)
                            {
                                case ([{ Type: var xType }, { Type: var yType }], [var xArgument, var yArgument])
                                    when xType.IsByte() && yType.IsByte():

                                    AnalyzeMin(consumer, element, xArgument, yArgument);
                                    break;
                            }
                            break;

                        case "RotateLeft": // todo: nameof(byte.RotateLeft) when available
                            switch (method.Parameters, element.Arguments)
                            {
                                case ([{ Type: var valueType }, { Type: var rotateAmountType }], [var valueArgument, var rotateAmountArgument])
                                    when valueType.IsByte() && rotateAmountType.IsInt():

                                    AnalyzeRotateLeft(consumer, element, valueArgument, rotateAmountArgument);
                                    break;
                            }
                            break;

                        case "RotateRight": // todo: nameof(byte.RotateRight) when available
                            switch (method.Parameters, element.Arguments)
                            {
                                case ([{ Type: var valueType }, { Type: var rotateAmountType }], [var valueArgument, var rotateAmountArgument])
                                    when valueType.IsByte() && rotateAmountType.IsInt():

                                    AnalyzeRotateRight(consumer, element, valueArgument, rotateAmountArgument);
                                    break;
                            }
                            break;

                        case nameof(byte.Parse):
                            switch (method.Parameters, element.Arguments)
                            {
                                case ([{ Type: var sType }, { Type: var styleType }], [_, var styleArgument])
                                    when sType.IsString() && IsNumberStyles(styleType):

                                    AnalyzeParse_String_NumberStyles(consumer, element, styleArgument);
                                    break;

                                case ([{ Type: var sType }, { Type: var providerType }], [_, var providerArgument])
                                    when sType.IsString() && providerType.IsIFormatProvider():

                                    AnalyzeParse_String_IFormatProvider(consumer, element, providerArgument);
                                    break;

                                case ([{ Type: var sType }, { Type: var styleType }, { Type: var providerType }], [
                                    _, var styleArgument, var providerArgument,
                                ]) when sType.IsString() && IsNumberStyles(styleType) && providerType.IsIFormatProvider():

                                    AnalyzeParse_String_NumberStyles_IFormatProvider(consumer, element, styleArgument, providerArgument);
                                    break;

                                case ([{ Type: var sType }, { Type: var providerType }], [_, var providerArgument])
                                    when sType.IsReadOnlySpan(out var spanTypeArgument)
                                    && spanTypeArgument.IsChar()
                                    && providerType.IsIFormatProvider():

                                    AnalyzeParse_ReadOnlySpanOfChar_IFormatProvider(consumer, element, providerArgument);
                                    break;

                                case ([{ Type: var utf8TextType }, { Type: var providerType }], [_, var providerArgument])
                                    when utf8TextType.IsReadOnlySpan(out var spanTypeArgument)
                                    && spanTypeArgument.IsByte()
                                    && providerType.IsIFormatProvider():

                                    AnalyzeParse_ReadOnlySpanOfByte_IFormatProvider(consumer, element, providerArgument);
                                    break;
                            }
                            break;

                        case nameof(byte.TryParse):
                            switch (method.Parameters, element.Arguments)
                            {
                                case ([{ Type: var sType }, { Type: var styleType }, { Type: var providerType }, { Type: var resultType }], [
                                    _, var styleArgument, _, _,
                                ]) when sType.IsString() && IsNumberStyles(styleType) && providerType.IsIFormatProvider() && resultType.IsByte():
                                    AnalyzeTryParse_String_NumberStyles_IFormatProvider_Byte(consumer, element, styleArgument);
                                    break;

                                case ([{ Type: var sType }, { Type: var providerType }, { Type: var resultType }], [_, var providerArgument, _])
                                    when sType.IsString() && providerType.IsIFormatProvider() && resultType.IsByte():

                                    AnalyzeTryParse_String_IFormatProvider_Byte(consumer, element, providerArgument);
                                    break;

                                case ([{ Type: var sType }, { Type: var styleType }, { Type: var providerType }, { Type: var resultType }], [
                                        _, var styleArgument, _, _,
                                    ]) when sType.IsReadOnlySpan(out var spanTypeArgument)
                                    && spanTypeArgument.IsChar()
                                    && IsNumberStyles(styleType)
                                    && providerType.IsIFormatProvider()
                                    && resultType.IsByte():

                                    AnalyzeTryParse_ReadOnlySpanOfChar_NumberStyles_IFormatProvider_Byte(consumer, element, styleArgument);
                                    break;

                                case ([{ Type: var sType }, { Type: var providerType }, { Type: var resultType }], [_, var providerArgument, _])
                                    when sType.IsReadOnlySpan(out var spanTypeArgument)
                                    && spanTypeArgument.IsChar()
                                    && providerType.IsIFormatProvider()
                                    && resultType.IsByte():

                                    AnalyzeTryParse_ReadOnlySpanOfChar_IFormatProvider_Byte(consumer, element, providerArgument);
                                    break;

                                case ([{ Type: var utf8TextType }, { Type: var styleType }, { Type: var providerType }, { Type: var resultType }], [
                                        _, var styleArgument, _, _,
                                    ]) when utf8TextType.IsReadOnlySpan(out var spanTypeArgument)
                                    && spanTypeArgument.IsByte()
                                    && IsNumberStyles(styleType)
                                    && providerType.IsIFormatProvider()
                                    && resultType.IsByte():

                                    AnalyzeTryParse_ReadOnlySpanOfByte_NumberStyles_IFormatProvider_Byte(consumer, element, styleArgument);
                                    break;

                                case ([{ Type: var sType }, { Type: var providerType }, { Type: var resultType }], [_, var providerArgument, _])
                                    when sType.IsReadOnlySpan(out var spanTypeArgument)
                                    && spanTypeArgument.IsByte()
                                    && providerType.IsIFormatProvider()
                                    && resultType.IsByte():

                                    AnalyzeTryParse_ReadOnlySpanOfByte_IFormatProvider_Byte(consumer, element, providerArgument);
                                    break;
                            }
                            break;
                    }
                    break;
            }
        }
    }
}