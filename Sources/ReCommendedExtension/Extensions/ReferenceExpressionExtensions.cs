using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace ReCommendedExtension.Extensions;

internal static class ReferenceExpressionExtensions
{
    extension(IReferenceExpression referenceExpression)
    {
        public bool IsPropertyAssignment
            => referenceExpression.Parent switch
            {
                // direct property assignment
                IAssignmentExpression assignmentExpression when assignmentExpression.Dest == referenceExpression => true,

                // tuple component assignment
                ITupleComponent
                {
                    Parent: ITupleComponentList
                    {
                        Parent: ITupleExpression { Parent: IAssignmentExpression assignmentExpression } tupleExpression,
                    },
                } when assignmentExpression.Dest == tupleExpression => true,

                _ => false,
            };

        public bool IsWithinNameofExpression
            => referenceExpression.Parent is ICSharpArgument
                {
                    Invocation: IInvocationExpression
                    {
                        InvokedExpression: IReferenceExpression { Reference: var reference }, TypeArguments: [], Arguments: [_],
                    },
                }
                && reference.GetName() == "nameof";
    }
}