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
using JetBrains.ReSharper.Psi.CSharp.Util.Literals;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Util;
using ReCommendedExtension.Analyzers.BaseTypes.NumberInfos;

namespace ReCommendedExtension.Extensions;

internal static class CSharpExpressionExtensions
{
    [Pure]
    public static IType? TryGetTargetType(this ICSharpExpression expression, bool forCollectionExpressions)
    {
        var targetType = expression.GetImplicitlyConvertedTo();

        if (targetType.IsUnknown)
        {
            return null;
        }

        if (forCollectionExpressions)
        {
            switch (expression.Parent)
            {
                case IReferenceExpression referenceExpression when referenceExpression.IsExtensionMethodInvocation():
                case IQueryFirstFrom or IQueryParameterPlatform:
                    return null;
            }
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
            IConstantValueOwner { ConstantValue: { Kind: ConstantValueKind.String, StringValue: var value } } when
                expression is not ICSharpLiteralExpression literalExpression || !literalExpression.IsUtf8StringLiteral() => value,

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
    public static int? TryGetInt32Constant(this ICSharpExpression? expression) => NumberInfo.Int32.TryGetConstant(expression, out _);

    [Pure]
    public static long? TryGetInt64Constant(this ICSharpExpression? expression) => NumberInfo.Int64.TryGetConstant(expression, out _);

    [Pure]
    public static bool? TryGetBooleanConstant(this ICSharpExpression? expression)
        => expression is IConstantValueOwner { ConstantValue: { Kind: ConstantValueKind.Bool, BoolValue: var value } } ? value : null;

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

    [Pure]
    public static MidpointRounding? TryGetMidpointRoundingConstant(this ICSharpExpression? expression)
        => expression is IConstantValueOwner { ConstantValue: { Kind: ConstantValueKind.Enum, Type: var enumType } constantValue }
            && enumType.IsClrType(ClrTypeNames.MidpointRounding)
                ? (MidpointRounding)constantValue.IntValue
                : null;

    [Pure]
    public static TimeSpanStyles? TryGetTimeSpanStylesConstant(this ICSharpExpression? expression)
        => expression is IConstantValueOwner { ConstantValue: { Kind: ConstantValueKind.Enum, Type: var enumType } constantValue }
            && enumType.IsClrType(ClrTypeNames.TimeSpanStyles)
                ? (TimeSpanStyles)constantValue.IntValue
                : null;

    [Pure]
    public static DateTimeKind? TryGetDateTimeKindConstant(this ICSharpExpression? expression)
        => expression is IConstantValueOwner { ConstantValue: { Kind: ConstantValueKind.Enum, Type: var enumType } constantValue }
            && enumType.IsClrType(ClrTypeNames.DateTimeKind)
                ? (DateTimeKind)constantValue.IntValue
                : null;

    [Pure]
    public static DateTimeStyles? TryGetDateTimeStylesConstant(this ICSharpExpression? expression)
        => expression is IConstantValueOwner { ConstantValue: { Kind: ConstantValueKind.Enum, Type: var enumType } constantValue }
            && enumType.IsClrType(ClrTypeNames.DateTimeStyles)
                ? (DateTimeStyles)constantValue.IntValue
                : null;

    [Pure]
    public static bool AreParenthesesRedundant(this IParenthesizedExpression parenthesizedExpression)
        => CodeStyleUtil.SuggestStyle<IRedundantParenthesesCodeStyleSuggestion>(parenthesizedExpression, LanguageManager.Instance, null) is
        {
            NeedsToRemove: true,
        };

    /// <returns>The inner expression (regardless if parentheses have been removed).</returns>
    public static ICSharpExpression TryRemoveParentheses(this ICSharpExpression expression, CSharpElementFactory factory)
    {
        if (expression is IParenthesizedExpression parenthesizedExpression)
        {
            if (parenthesizedExpression.AreParenthesesRedundant())
            {
                return ModificationUtil.ReplaceChild(expression, factory.CreateExpression("$0", parenthesizedExpression.Expression));
            }

            return parenthesizedExpression.Expression;
        }

        return expression;
    }

    [Pure]
    public static ICSharpExpression Cast(this ICSharpExpression expression, string typeName)
    {
        var factory = CSharpElementFactory.GetInstance(expression);

        var newExpression = factory.CreateExpression($"({typeName})($0)", expression);

        if (newExpression is ICastExpression { Op: IParenthesizedExpression parenthesizedExpression } castExpression
            && parenthesizedExpression.AreParenthesesRedundant())
        {
            castExpression.SetOp(factory.CreateExpression("$0", expression));
        }

        return newExpression;
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

                // the nullability detection doesn't work well for extension method invocations
                NullableAnnotation.RuntimeNotNullable when expression.Parent is not IReferenceExpression referenceExpression
                    || referenceExpression.ConditionalAccessSign == null => CSharpControlFlowNullReferenceState.NOT_NULL,

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