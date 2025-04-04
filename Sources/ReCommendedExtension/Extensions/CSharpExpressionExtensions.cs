using System.Globalization;
using JetBrains.Metadata.Reader.API;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeStyle;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.CodeStyle.Suggestions;
using JetBrains.ReSharper.Psi.CSharp.ControlFlow;
using JetBrains.ReSharper.Psi.CSharp.Impl.ControlFlow.NullableAnalysis;
using JetBrains.ReSharper.Psi.CSharp.Impl.ControlFlow.NullableAnalysis.Runner;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Util;

namespace ReCommendedExtension.Extensions;

internal static class CSharpExpressionExtensions
{
    [Pure]
    public static IType? TryGetTargetType(this ICSharpExpression expression)
    {
        var targetType = expression.GetImplicitlyConvertedTo();

        if (targetType.IsUnknown)
        {
            return null;
        }

        switch (expression.Parent)
        {
            case IReferenceExpression referenceExpression when referenceExpression.IsExtensionMethodInvocation():
            case IQueryFirstFrom or IQueryParameterPlatform:
                return null;
        }

        return targetType;
    }

    [Pure]
    public static bool IsDefaultValue([NotNullWhen(true)] this ICSharpExpression? expression)
        => expression is { } && expression.IsDefaultValueOf(expression.Type());

    [Pure]
    public static string? TryGetStringConstant(this ICSharpExpression? expression)
        => expression switch
        {
            IConstantValueOwner { ConstantValue: { Kind: ConstantValueKind.String, StringValue: var value } } => value,

            IReferenceExpression { Reference: var reference } when reference.Resolve().DeclaredElement is IField
                {
                    ShortName: nameof(string.Empty),
                    IsStatic: true,
                    IsReadonly: true,
                    AccessibilityDomain.DomainType: AccessibilityDomain.AccessibilityDomainType.PUBLIC,
                } field
                && field.ContainingType.IsSystemString() => "",

            _ => null,
        };

    [Pure]
    public static char? TryGetCharConstant(this ICSharpExpression? expression)
        => expression is IConstantValueOwner { ConstantValue: { Kind: ConstantValueKind.Char, CharValue: var value } } ? value : null;

    [Pure]
    public static int? TryGetInt32Constant(this ICSharpExpression? expression)
        => expression is IConstantValueOwner { ConstantValue: { Kind: ConstantValueKind.Int, IntValue: var value } } ? value : null;

    [Pure]
    public static bool? TryGetBooleanConstant(this ICSharpExpression? expression)
        => expression is IConstantValueOwner { ConstantValue: { Kind: ConstantValueKind.Bool, BoolValue: var value } } ? value : null;

    [Pure]
    public static byte? TryGetByteConstant(this ICSharpExpression? expression, out bool implicitlyConverted)
    {
        if (expression is IConstantValueOwner constantValueOwner)
        {
            switch (constantValueOwner.ConstantValue)
            {
                case { Kind: ConstantValueKind.Byte, ByteValue: var value }:
                    implicitlyConverted = false;
                    return value;

                case { Kind: ConstantValueKind.Int, IntValue: >= 0 and <= byte.MaxValue and var value }:
                    implicitlyConverted = true;
                    return unchecked((byte)value);
            }
        }

        implicitlyConverted = false;
        return null;
    }

    [Pure]
    public static StringComparison? TryGetStringComparisonConstant(this ICSharpExpression? expression)
        => expression is IConstantValueOwner { ConstantValue: { Kind: ConstantValueKind.Enum, Type: var enumType } constantValue }
            && enumType.IsClrType(PredefinedType.STRING_COMPARISON_CLASS)
                ? (StringComparison)constantValue.IntValue
                : null;

    [Pure]
    public static StringSplitOptions? TryGetStringSplitOptionsConstant(this ICSharpExpression? expression)
        => expression is IConstantValueOwner { ConstantValue: { Kind: ConstantValueKind.Enum, Type: var enumType } constantValue }
            && enumType.IsClrType(ClrTypeNames.StringSplitOptions)
                ? (StringSplitOptions)constantValue.IntValue
                : null;

    [Pure]
    public static NumberStyles? TryGetNumberStylesConstant(this ICSharpExpression? expression)
        => expression is IConstantValueOwner { ConstantValue: { Kind: ConstantValueKind.Enum, Type: var enumType } constantValue }
            && enumType.IsClrType(ClrTypeNames.NumberStyles)
                ? (NumberStyles)constantValue.IntValue
                : null;

    public static ICSharpExpression TryRemoveParentheses(this ICSharpExpression expression, CSharpElementFactory factory)
    {
        if (expression is IParenthesizedExpression parenthesizedExpression
            && CodeStyleUtil.SuggestStyle<IRedundantParenthesesCodeStyleSuggestion>(expression, LanguageManager.Instance, null) is
            {
                NeedsToRemove: true,
            })
        {
            return ModificationUtil.ReplaceChild(expression, factory.CreateExpression("$0", parenthesizedExpression.Expression));
        }

        return expression;
    }

    public static void TryRemoveRangeIndexParentheses(this ICSharpExpression expression, CSharpElementFactory factory)
    {
        if (expression is IElementAccessExpression { Arguments: [{ Value: IRangeExpression rangeExpression }] })
        {
            rangeExpression.LeftOperand?.TryRemoveParentheses(factory);
            rangeExpression.RightOperand?.TryRemoveParentheses(factory);
        }
    }

    public static void TryRemoveUnaryOperatorParentheses(this ICSharpExpression expression, CSharpElementFactory factory)
    {
        if (expression is IUnaryOperatorExpression unaryOperatorExpression)
        {
            unaryOperatorExpression.Operand.TryRemoveParentheses(factory);
        }
    }

    public static void TryRemoveBinaryOperatorParentheses(this ICSharpExpression expression, CSharpElementFactory factory)
    {
        if (expression is IBinaryExpression binaryExpression)
        {
            binaryExpression.LeftOperand.TryRemoveParentheses(factory);
            binaryExpression.RightOperand.TryRemoveParentheses(factory);
        }
    }

    [Pure]
    public static CSharpControlFlowNullReferenceState GetNullReferenceStateByNullableContext(
        this ICSharpExpression expression,
        CSharpCompilerNullableInspector? nullabilityInspector)
    {
        var type = expression.Type();
        if (expression.IsDefaultValueOf(type))
        {
            switch (type.Classify)
            {
                case TypeClassification.VALUE_TYPE:
                    return type.IsNullable() ? CSharpControlFlowNullReferenceState.NULL : CSharpControlFlowNullReferenceState.NOT_NULL;

                case TypeClassification.REFERENCE_TYPE: return CSharpControlFlowNullReferenceState.NULL;

                case TypeClassification.UNKNOWN: return CSharpControlFlowNullReferenceState.UNKNOWN; // unconstrained generic type

                default: goto case TypeClassification.UNKNOWN;
            }
        }

        if (expression.GetContainingNode<ICSharpClosure>() is { } closure)
        {
            nullabilityInspector = nullabilityInspector?.GetClosureAnalysisResult(closure) as CSharpCompilerNullableInspector;
        }

        if (nullabilityInspector?.ControlFlowGraph.GetLeafElementsFor(expression).LastOrDefault()?.Exits.FirstOrDefault() is { } edge)
        {
            var nullableContext = nullabilityInspector.GetContext(edge);

            return nullableContext?.ExpressionAnnotation switch
            {
                NullableAnnotation.NotAnnotated or NullableAnnotation.NotNullable => CSharpControlFlowNullReferenceState.NOT_NULL,

                NullableAnnotation.RuntimeNotNullable when expression is IObjectCreationExpression
                    || expression.Parent is not IReferenceExpression
                    {
                        Reference: var reference,
                    } // the nullability detection doesn't work well for extension method invocations
                    || reference.Resolve().DeclaredElement is not IMethod { IsExtensionMethod: true } => CSharpControlFlowNullReferenceState.NOT_NULL,

                NullableAnnotation.Annotated or NullableAnnotation.Nullable =>
                    CSharpControlFlowNullReferenceState.MAY_BE_NULL, // todo: distinguish if the expression is "null" or just "may be null" here

                _ => CSharpControlFlowNullReferenceState.UNKNOWN,
            };
        }

        return CSharpControlFlowNullReferenceState.UNKNOWN;
    }

    [Pure]
    public static bool IsNotNullHere(
        this ICSharpExpression expression,
        NullableReferenceTypesDataFlowAnalysisRunSynchronizer nullableReferenceTypesDataFlowAnalysisRunSynchronizer)
        => expression.IsNullableWarningsContextEnabled()
            && expression.TryGetNullableInspector(nullableReferenceTypesDataFlowAnalysisRunSynchronizer) is { } inspector
            && expression.GetNullReferenceStateByNullableContext(inspector) == CSharpControlFlowNullReferenceState.NOT_NULL;
}