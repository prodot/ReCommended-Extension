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
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Underscore character used intentionally as a separator.")]
    static new class ParameterTypes
    {
        public static IReadOnlyList<ParameterType> String { get; } = [new() { ClrTypeName = PredefinedType.STRING_FQN }];

        public static IReadOnlyList<ParameterType> IFormatProvider { get; } = [new() { ClrTypeName = PredefinedType.IFORMATPROVIDER_FQN }];
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
            }
        }
    }
}