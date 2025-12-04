using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.Util;

namespace ReCommendedExtension.Extensions;

internal static class CSharpArgumentsOwnerExtensions
{
    /// <param name="argumentsOwner">The object that owns the arguments, typically representing a method or function invocation.</param>
    extension(ICSharpArgumentsOwner argumentsOwner)
    {
        [Pure]
        void DeriveExtensionArgumentAndExpectedArgumentCount(
            IParametersOwner parametersOwner,
            out ICSharpArgument? extensionArgument,
            [NonNegativeValue] out int expectedArgumentCount)
        {
            if (parametersOwner.GetExtensionMemberKind() == ExtensionMemberKind.CLASSIC_METHOD
                && argumentsOwner is { ExtensionQualifier: IExpressionArgumentInfo qualifier }
                && parametersOwner.Parameters is [var firstParameter, ..]
                && Equals(qualifier.MatchingParameter?.Element, firstParameter))
            {
                // classic extension invoked as an extension

                var factory = CSharpElementFactory.GetInstance(argumentsOwner);

                extensionArgument = factory.CreateArgument(qualifier.Kind, qualifier.Expression);

                expectedArgumentCount = parametersOwner.Parameters.Count - 1;
            }
            else
            {
                // not a classic extension or classic extension not invoked as an extension

                extensionArgument = null;
                expectedArgumentCount = parametersOwner.Parameters.Count;
            }
        }

        [Pure]
        TreeNodeCollection<ICSharpArgument?>? TryGetArgumentsForNoParameters(ICSharpArgument? extensionArgument)
        {
            if (argumentsOwner.Arguments is [])
            {
                return extensionArgument is { }
                    ? new TreeNodeCollection<ICSharpArgument?>([extensionArgument]) // no further parameters and no further arguments
                    : argumentsOwner.Arguments; // no parameters and no arguments
            }

            return null; // no (further) parameters, but has (further) arguments (invalid case)
        }

        [Pure]
        TreeNodeCollection<ICSharpArgument?>? TryGetArgumentsForUnnamedArguments(
            IParametersOwner parametersOwner,
            ICSharpArgument? extensionArgument,
            [NonNegativeValue] int expectedArgumentCount)
        {
            if (argumentsOwner.Arguments.Count >= expectedArgumentCount)
            {
                // same number of arguments and parameters, or the last parameter is "params" thus accepting more arguments

                return extensionArgument is { }
                    ? new TreeNodeCollection<ICSharpArgument?>([extensionArgument, ..argumentsOwner.Arguments])
                    : argumentsOwner.Arguments;
            }

            // fewer arguments than parameters (optional parameters used)

            var arguments = new ICSharpArgument?[parametersOwner.Parameters.Count];

            if (extensionArgument is { })
            {
                arguments[0] = extensionArgument;

                argumentsOwner.Arguments.CopyTo(arguments, 1);
            }
            else
            {
                argumentsOwner.Arguments.CopyTo(arguments, 0);
            }

            return new TreeNodeCollection<ICSharpArgument?>(arguments);
        }

        [Pure]
        TreeNodeCollection<ICSharpArgument?>? TryGetArgumentForNamedArguments(
            IParametersOwner parametersOwner,
            ICSharpArgument? extensionArgument,
            [NonNegativeValue] int expectedArgumentCount)
        {
            var parameterArguments = new Dictionary<IParameter, ICSharpArgument>(expectedArgumentCount);
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

            var result = new ICSharpArgument?[parametersOwner.Parameters.Count + (additionalParamsArguments?.Count ?? 0)];

            if (extensionArgument is { })
            {
                result[0] = extensionArgument;
            }

            for (var i = extensionArgument is { } ? 1 : 0; i < parametersOwner.Parameters.Count; i++)
            {
                var parameter = parametersOwner.Parameters[i];

                if (parameterArguments.TryGetValue(parameter, out var argument))
                {
                    result[i] = argument;

                    if (parameter.IsParams && additionalParamsArguments is { })
                    {
                        Debug.Assert(i == parametersOwner.Parameters.Count - 1);

                        for (var j = 0; j < additionalParamsArguments.Count; j++)
                        {
                            result[i + 1 + j] = additionalParamsArguments[j];
                        }

                        break; // no more parameters to process
                    }
                }
            }

            return new TreeNodeCollection<ICSharpArgument?>(result);
        }

        /// <summary>
        /// Attempts to retrieve the arguments of a method or function call in the order they are declared in the corresponding parameter list.
        /// </summary>
        /// <returns>
        ///     A <see cref="TreeNodeCollection{T}"/> containing the arguments in declaration order, or <c>null</c> if the arguments cannot be
        ///     resolved or if the invocation is invalid.
        /// </returns>
        /// <remarks>
        /// This method handles cases where arguments may be unnamed, named, or include additional arguments for a "params" parameter. If the
        /// invocation is invalid (e.g., mismatched arguments and parameters), the method returns <c>null</c>. Skipped optional arguments are
        /// represented by <c>null</c> items.<para/>
        /// For extension invocations, the qualifier is returned as the first argument. Note that the first argument is not a part of the syntax tree
        /// in this case and therefore cannot be used for highlighting.
        /// </remarks>
        [Pure]
        public TreeNodeCollection<ICSharpArgument?>? TryGetArgumentsInDeclarationOrder()
        {
            // todo: C# 14: add support for non-classic extension invocations (if ExtensionMemberKind.INSTANCE_METHOD or ExtensionMemberKind.INSTANCE_PROPERTY (is an ICSharpArgumentsOwner?) is detected: argumentsOwner.ExtensionQualifier?.MatchingParameter might not be directly in the IParametersOwner)

            if (argumentsOwner.Reference?.Resolve().DeclaredElement is IParametersOwner parametersOwner)
            {
                argumentsOwner.DeriveExtensionArgumentAndExpectedArgumentCount(parametersOwner, out var extensionArgument, out var expectedArgumentCount);

                if (expectedArgumentCount == 0)
                {
                    return argumentsOwner.TryGetArgumentsForNoParameters(extensionArgument);
                }

                if (argumentsOwner.Arguments.All(static a => a is null or { NameIdentifier: null }))
                {
                    // if all arguments are unnamed: the arguments already ordered

                    return argumentsOwner.TryGetArgumentsForUnnamedArguments(parametersOwner, extensionArgument, expectedArgumentCount);
                }

                // at least one named parameter used: restore declaration order of arguments

                return argumentsOwner.TryGetArgumentForNamedArguments(parametersOwner, extensionArgument, expectedArgumentCount);
            }

            return null;
        }

        /// <summary>
        /// Determines whether the method is invoked as an extension method.
        /// </summary>
        /// <remarks>
        /// The method return <c>false</c> when an extension method is invoked as a "regular" method.
        /// </remarks>
        [Pure]
        public bool IsInvokedAsExtension()
        {
            if (argumentsOwner.Reference?.Resolve().DeclaredElement is IParametersOwner parametersOwner)
            {
                argumentsOwner.DeriveExtensionArgumentAndExpectedArgumentCount(parametersOwner, out var extensionArgument, out _);

                return extensionArgument is { };
            }

            return false;
        }
    }
}