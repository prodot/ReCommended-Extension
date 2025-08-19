using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.Util;

namespace ReCommendedExtension.Extensions;

internal static class CSharpArgumentsOwnerExtensions
{
    /// <summary>
    /// Attempts to retrieve the arguments of a method or function call in the order they are declared in the corresponding parameter list.
    /// </summary>
    /// <param name="argumentsOwner">The object that owns the arguments, typically representing a method or function invocation.</param>
    /// <returns>
    ///     A <see cref="TreeNodeCollection{T}"/> containing the arguments in declaration order, or <c>null</c> if the arguments cannot be resolved or
    ///     if the invocation is invalid.
    /// </returns>
    /// <remarks>
    /// This method handles cases where arguments may be unnamed, named, or include additional arguments for a "params" parameter. If the invocation
    /// is invalid (e.g., mismatched arguments and parameters), the method returns <c>null</c>. Skipped optional arguments are represented by
    /// <c>null</c> items.
    /// </remarks>
    public static TreeNodeCollection<ICSharpArgument?>? TryGetArgumentsInDeclarationOrder(this ICSharpArgumentsOwner argumentsOwner)
    {
        if (argumentsOwner.Reference?.Resolve().DeclaredElement is IParametersOwner parametersOwner)
        {
            Debug.Assert(parametersOwner.GetExtensionMemberKind() != ExtensionMemberKind.CLASSIC_METHOD || parametersOwner.Parameters is [_, ..]);

            var isClassicExtensionMethodInvocation = parametersOwner.GetExtensionMemberKind() == ExtensionMemberKind.CLASSIC_METHOD
                && Equals(argumentsOwner.ExtensionQualifier?.MatchingParameter?.Element, parametersOwner.Parameters[0]);

            var parameterCount = isClassicExtensionMethodInvocation ? parametersOwner.Parameters.Count - 1 : parametersOwner.Parameters.Count;

            if (parameterCount == 0)
            {
                if (argumentsOwner.Arguments is [])
                {
                    return argumentsOwner.Arguments; // no parameters and no arguments
                }

                return null; // no parameters, but has arguments (invalid case)
            }

            if (argumentsOwner.Arguments.All(a => a is null or { NameIdentifier: null }))
            {
                // if all arguments are unnamed: the arguments already ordered

                if (argumentsOwner.Arguments.Count >= parameterCount)
                {
                    // same number of arguments and parameters, or the last parameter is "params" thus accepting more arguments

                    return argumentsOwner.Arguments;
                }

                // fewer arguments than parameters (optional parameters used)

                var arguments = new ICSharpArgument?[parameterCount];
                argumentsOwner.Arguments.CopyTo(arguments, 0);

                return new TreeNodeCollection<ICSharpArgument?>(arguments);
            }

            // at least one named parameter used: restore declaration order of arguments

            var parameterArguments = new Dictionary<IParameter, ICSharpArgument>(parameterCount);
            var additionalParamsArguments = null as List<ICSharpArgument>;

            foreach (var argument in argumentsOwner.Arguments)
            {
                if (argument is { })
                {
                    if (argument.MatchingParameter?.Element is { } parameter)
                    {
                        if (parameterArguments.TryAdd(parameter, argument))
                        {
                            continue;
                        }

                        if (parameter.IsParams)
                        {
                            additionalParamsArguments ??= [];
                            additionalParamsArguments.Add(argument);
                            continue;
                        }

                        return null; // non-params parameter already has an argument (invalid case)
                    }

                    return null; // argument without matching parameter (invalid case)
                }
            }

            var result = new ICSharpArgument?[parameterCount + (additionalParamsArguments?.Count ?? 0)];

            for (var i = 0; i < parametersOwner.Parameters.Count; i++)
            {
                int resultIndex;

                if (isClassicExtensionMethodInvocation)
                {
                    if (i == 0)
                    {
                        continue;
                    }

                    resultIndex = i - 1;
                }
                else
                {
                    resultIndex = i;
                }

                var parameter = parametersOwner.Parameters[i];

                if (parameterArguments.TryGetValue(parameter, out var argument))
                {
                    result[resultIndex] = argument;

                    if (parameter.IsParams && additionalParamsArguments is { })
                    {
                        Debug.Assert(i == parametersOwner.Parameters.Count - 1);

                        for (var j = 0; j < additionalParamsArguments.Count; j++)
                        {
                            result[resultIndex + 1 + j] = additionalParamsArguments[j];
                        }

                        break; // no more parameters to process
                    }
                }
            }

            return new TreeNodeCollection<ICSharpArgument?>(result);
        }

        return null;
    }
}