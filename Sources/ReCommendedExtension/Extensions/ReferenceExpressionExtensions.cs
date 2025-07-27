using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace ReCommendedExtension.Extensions;

internal static class ReferenceExpressionExtensions
{
    [Pure]
    public static bool IsPropertyAssignment(this IReferenceExpression referenceExpression)
        => referenceExpression.Parent switch
        {
            // direct property assignment
            IAssignmentExpression assignmentExpression when assignmentExpression.Dest == referenceExpression => true,

            // tuple component assignment
            ITupleComponent
            {
                Parent: ITupleComponentList { Parent: ITupleExpression { Parent: IAssignmentExpression assignmentExpression } tupleExpression },
            } when assignmentExpression.Dest == tupleExpression => true,

            _ => false,
        };
}