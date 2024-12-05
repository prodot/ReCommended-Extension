using JetBrains.ReSharper.Feature.Services.LinqTools;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.ControlFlow;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Impl.ControlFlow.NullableAnalysis;
using JetBrains.ReSharper.Psi.CSharp.Impl.ControlFlow.NullableAnalysis.Runner;
using JetBrains.ReSharper.Psi.CSharp.Parsing;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Util;

namespace ReCommendedExtension.Extensions;

internal static class CSharpTreeNodeExtensions
{
    [Pure]
    public static bool IsDefaultValueOf([NotNullWhen(true)] this ICSharpTreeNode? element, IType type)
    {
        switch (element)
        {
            case ICSharpLiteralExpression literalExpression when literalExpression.Literal.GetTokenType() == CSharpTokenType.DEFAULT_KEYWORD:
            case IDefaultExpression defaultExpression when Equals(defaultExpression.Type(), type):
                return true;

            case IParenthesizedExpression { Expression: { } } parenthesizedExpression:
                return parenthesizedExpression.Expression.IsDefaultValueOf(type);
        }

        if (type.IsUnconstrainedGenericType())
        {
            // unconstrained generic type

            return false;
        }

        if (type.IsValueType())
        {
            // value type (non-nullable and nullable)

            switch (element)
            {
                case IConstantValueOwner constantValueOwner when type.IsNullable()
                    ? constantValueOwner.ConstantValue.IsNull()
                    : constantValueOwner.ConstantValue.IsDefaultValue(type, element):
                    return true;

                case IObjectCreationExpression objectCreationExpression:
                    var structType = type.GetStructType(); // null if type is a generic type

                    return Equals(objectCreationExpression.Type(), type)
                        && objectCreationExpression is { Arguments: [], Initializer: not { } or { InitializerElements: [] } }
                        && structType is { HasCustomParameterlessConstructor: false };

                case IAsExpression asExpression:
                    return asExpression is { Operand: { }, TypeOperand: { } }
                        && asExpression.Operand.ConstantValue.IsNull()
                        && Equals(CSharpTypeFactory.CreateType(asExpression.TypeOperand), type)
                        && type.IsNullable();

                default: return false;
            }
        }

        // reference type

        return element switch
        {
            IConstantValueOwner constantValueOwner when constantValueOwner.ConstantValue.IsNull()
                || constantValueOwner.ConstantValue.IsDefaultValue(type, element) => true,

            IAsExpression asExpression => asExpression.Operand is { }
                && asExpression.Operand.ConstantValue.IsNull()
                && asExpression.TypeOperand is { }
                && Equals(CSharpTypeFactory.CreateType(asExpression.TypeOperand), type),

            _ => false,
        };
    }

    [Pure]
    public static CSharpCompilerNullableInspector? TryGetNullableInspector(
        this ICSharpTreeNode treeNode,
        NullableReferenceTypesDataFlowAnalysisRunSynchronizer nullableReferenceTypesDataFlowAnalysisRunSynchronizer)
        => (CSharpCompilerNullableInspector?)nullableReferenceTypesDataFlowAnalysisRunSynchronizer.RunNullableAnalysisAndGetResults(
            treeNode,
            null!, // wrong [NotNull] annotation in R# code
            ValueAnalysisMode.OFF,
            false);
}