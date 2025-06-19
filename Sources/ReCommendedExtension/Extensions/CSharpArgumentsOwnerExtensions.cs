using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.Extensions;

internal static class CSharpArgumentsOwnerExtensions
{
    public static TreeNodeCollection<ICSharpArgument>? TryGetArgumentsInDeclarationOrder(this ICSharpArgumentsOwner argumentsOwner)
    {
        if (argumentsOwner.Arguments is [] or [{ }] || argumentsOwner.Arguments.All(a => a is { NameIdentifier: null }))
        {
            return argumentsOwner.Arguments;
        }

        var result = new ICSharpArgument[argumentsOwner.Arguments.Count];

        var parameters = argumentsOwner.Arguments[0].MatchingParameter?.Element.ContainingParametersOwner?.Parameters;

        for (var i = 0; i < argumentsOwner.Arguments.Count; i++)
        {
            var argument = argumentsOwner.Arguments[i];

            if (argument is { })
            {
                if (argument.NameIdentifier is { })
                {
                    if (parameters is { })
                    {
                        var index = parameters.IndexOf(argument.MatchingParameter?.Element);
                        if (index >= 0 && index < result.Length)
                        {
                            result[index] = argument;
                            continue;
                        }
                    }
                }
                else
                {
                    result[i] = argument;
                    continue;
                }
            }

            return null;
        }

        if (result.Any(a => a == null))
        {
            return null;
        }

        return new TreeNodeCollection<ICSharpArgument>(result);
    }
}