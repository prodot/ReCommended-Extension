using System.Globalization;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReCommendedExtension.Extensions;
using ReCommendedExtension.Extensions.MethodFinding;

namespace ReCommendedExtension.Analyzers.BaseTypes.Analyzers;

/// <remarks>
/// C# language version checks are only done when a quick fix would require it.
/// </remarks>
[ElementProblemAnalyzer(
    typeof(IInvocationExpression),
    HighlightingTypes = [typeof(UseExpressionResultSuggestion), typeof(UseBinaryOperationSuggestion), typeof(RedundantArgumentHint)])]
public sealed class ByteAnalyzer() : IntegerAnalyzer<byte>(PredefinedType.BYTE_FQN)
{
    [Pure]
    static bool IsNumberStyles(IType type) => type.IsClrType(ClrTypeNames.NumberStyles);

    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Underscore character used intentionally as a separator.")]
    static new class ParameterTypes
    {
        public static IReadOnlyList<ParameterType> String { get; } = [new() { ClrTypeName = PredefinedType.STRING_FQN }];

        public static IReadOnlyList<ParameterType> IFormatProvider { get; } = [new() { ClrTypeName = PredefinedType.IFORMATPROVIDER_FQN }];

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

        public static IReadOnlyList<ParameterType> ReadOnlySpanOfT_IFormatProvider_Byte { get; } =
        [
            new GenericParameterType { ClrTypeName = PredefinedType.SYSTEM_READ_ONLY_SPAN_FQN },
            new() { ClrTypeName = PredefinedType.IFORMATPROVIDER_FQN },
            new() { ClrTypeName = PredefinedType.BYTE_FQN },
        ];
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

    private protected override TypeCode? TryGetTypeCode() => TypeCode.Byte;

    private protected override byte? TryGetConstant(ICSharpExpression? expression, out bool implicitlyConverted)
    {
        if (expression is IConstantValueOwner constantValueOwner)
        {
            switch (constantValueOwner.ConstantValue)
            {
                case { Kind: ConstantValueKind.Byte, ByteValue: var value }:
                    implicitlyConverted = false;
                    return value;

                case { Kind: ConstantValueKind.Int, IntValue: >= byte.MinValue and <= byte.MaxValue and var value }:
                    implicitlyConverted = true;
                    return unchecked((byte)value);
            }
        }

        implicitlyConverted = false;
        return null;
    }

    private protected override string CastConstant(ICSharpExpression constant, bool implicitlyConverted)
    {
        if (implicitlyConverted)
        {
            return constant.Cast("byte").GetText();
        }

        return constant.GetText();
    }

    private protected override string Cast(ICSharpExpression expression) => expression.Cast("byte").GetText();

    private protected override string CastZero(CSharpLanguageLevel languageLevel) => "(byte)0";

    private protected override bool AreEqual(byte x, byte y) => x == y;

    private protected override bool IsZero(byte value) => value == 0;

    private protected override bool AreMinMaxValues(byte min, byte max) => (min, max) == (byte.MinValue, byte.MaxValue);

    private protected override void Analyze(IInvocationExpression element, IReferenceExpression invokedExpression, IMethod method, IHighlightingConsumer consumer)
    {
        base.Analyze(element, invokedExpression, method, consumer);

        if (method.ContainingType.IsClrType(PredefinedType.BYTE_FQN))
        {
            switch (invokedExpression, method)
            {
                case ({ QualifierExpression: { } }, { IsStatic: false }):
                    switch (method.ShortName)
                    {
                        case nameof(byte.ToString):
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