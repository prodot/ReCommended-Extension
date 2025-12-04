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
using ReCommendedExtension.Extensions.Collections;
using ReCommendedExtension.Extensions.NumberInfos;

namespace ReCommendedExtension.Extensions;

internal static class CSharpExpressionExtensions
{
    extension(ICSharpExpression expression)
    {
        [Pure]
        public IType? TryGetTargetType(bool forCollectionExpressions)
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
                    case IReferenceExpression referenceExpression when referenceExpression.GetExtensionInvocationKind() is var kind
                        && (kind == ExtensionMemberKind.CLASSIC_METHOD
                            || kind == ExtensionMemberKind.INSTANCE_METHOD
                            || kind == ExtensionMemberKind.INSTANCE_PROPERTY):
                    case IQueryFirstFrom or IQueryParameterPlatform:
                        return null;
                }
            }

            return targetType;
        }
    }

    extension([NotNullWhen(true)] ICSharpExpression? expression)
    {
        public bool IsDefaultValueOrNull => expression is { } && expression.IsDefaultValueOf(expression.Type());
    }

    extension(ICSharpExpression? expression)
    {
        public string? AsStringConstant
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
                    } @field
                    && @field.ContainingType.IsSystemString() => "",

                _ => null,
            };

        public char? AsCharConstant
            => expression is IConstantValueOwner { ConstantValue: { Kind: ConstantValueKind.Char, CharValue: var value } } ? value : null;

        public int? AsInt32Constant => NumberInfo.Int32.TryGetConstant(expression, out _);

        public long? AsInt64Constant => NumberInfo.Int64.TryGetConstant(expression, out _);

        public ulong? AsUInt64Constant => NumberInfo.UInt64.TryGetConstant(expression, out _);

        public bool? AsBooleanConstant
            => expression is IConstantValueOwner { ConstantValue: { Kind: ConstantValueKind.Bool, BoolValue: var value } } ? value : null;

        public StringComparison? AsStringComparisonConstant
            => expression is IConstantValueOwner { ConstantValue: { Kind: ConstantValueKind.Enum, Type: var enumType } constantValue }
                && enumType.IsStringComparison()
                    ? (StringComparison)constantValue.IntValue
                    : null;

        public StringSplitOptions? AsStringSplitOptionsConstant
            => expression is IConstantValueOwner { ConstantValue: { Kind: ConstantValueKind.Enum, Type: var enumType } constantValue }
                && enumType.IsStringSplitOptions()
                    ? (StringSplitOptions)constantValue.IntValue
                    : null;

        public NumberStyles? AsNumberStylesConstant
            => expression is IConstantValueOwner { ConstantValue: { Kind: ConstantValueKind.Enum, Type: var enumType } constantValue }
                && enumType.IsNumberStyles()
                    ? (NumberStyles)constantValue.IntValue
                    : null;

        public MidpointRounding? AsMidpointRoundingConstant
            => expression is IConstantValueOwner { ConstantValue: { Kind: ConstantValueKind.Enum, Type: var enumType } constantValue }
                && enumType.IsMidpointRounding()
                    ? (MidpointRounding)constantValue.IntValue
                    : null;

        public TimeSpanStyles? AsTimeSpanStylesConstant
            => expression is IConstantValueOwner { ConstantValue: { Kind: ConstantValueKind.Enum, Type: var enumType } constantValue }
                && enumType.IsTimeSpanStyles()
                    ? (TimeSpanStyles)constantValue.IntValue
                    : null;

        public DateTimeKind? AsDateTimeKindConstant
            => expression is IConstantValueOwner { ConstantValue: { Kind: ConstantValueKind.Enum, Type: var enumType } constantValue }
                && enumType.IsDateTimeKind()
                    ? (DateTimeKind)constantValue.IntValue
                    : null;

        public DateTimeStyles? AsDateTimeStylesConstant
            => expression is IConstantValueOwner { ConstantValue: { Kind: ConstantValueKind.Enum, Type: var enumType } constantValue }
                && enumType.IsDateTimeStyles()
                    ? (DateTimeStyles)constantValue.IntValue
                    : null;

        public CollectionCreation? AsCollectionCreation => CollectionCreation.TryFrom(expression);
    }

    extension(IParenthesizedExpression parenthesizedExpression)
    {
        [Pure]
        public bool AreParenthesesRedundant()
            => CodeStyleUtil.SuggestStyle<IRedundantParenthesesCodeStyleSuggestion>(parenthesizedExpression, LanguageManager.Instance, null) is
            {
                NeedsToRemove: true,
            };
    }

    extension(ICSharpExpression expression)
    {
        /// <returns>The inner expression (regardless if parentheses have been removed).</returns>
        public ICSharpExpression TryRemoveParentheses(CSharpElementFactory factory)
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
        public ICSharpExpression Cast(string typeName)
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
        public CSharpControlFlowNullReferenceState GetNullReferenceStateByNullableContext(CSharpCompilerNullableInspector? nullabilityInspector)
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
        public bool IsNotNullHere(NullableReferenceTypesDataFlowAnalysisRunSynchronizer nullableReferenceTypesDataFlowAnalysisRunSynchronizer)
            => expression.IsNullableWarningsContextEnabled()
                && expression.TryGetNullableInspector(nullableReferenceTypesDataFlowAnalysisRunSynchronizer) is { } inspector
                && expression.GetNullReferenceStateByNullableContext(inspector) == CSharpControlFlowNullReferenceState.NOT_NULL;
    }
}