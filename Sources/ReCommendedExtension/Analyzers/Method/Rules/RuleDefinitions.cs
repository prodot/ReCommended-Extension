using System.Text;
using JetBrains.Metadata.Reader.API;
using JetBrains.ReSharper.Psi;
using ReCommendedExtension.Analyzers.Method.Inspections;
using ReCommendedExtension.Extensions.MethodFinding;

namespace ReCommendedExtension.Analyzers.Method.Rules;

internal static class RuleDefinitions
{
    /// <remarks>
    /// type → (method name → method overloads)
    /// </remarks>
    static readonly Dictionary<IClrTypeName, Dictionary<string, IReadOnlyCollection<Method>>> typeMethods = new(new ClrTypeNameEqualityComparer())
    {
        { PredefinedType.BOOLEAN_FQN, CreateBooleanMethods() },
        { PredefinedType.DATETIME_FQN, CreateDateTimeMethods() },
        { PredefinedType.DATETIMEOFFSET_FQN, CreateDateTimeOffsetMethods() },
        { PredefinedType.DATE_ONLY_FQN, CreateDateOnlyMethods() },
        { PredefinedType.STRING_FQN, CreateStringMethods() },
        { PredefinedType.STRING_BUILDER_FQN, CreateStringBuilderMethods() },
    };

    [Pure]
    static Dictionary<string, IReadOnlyCollection<Method>> CreateBooleanMethods()
        => new(StringComparer.Ordinal)
        {
            {
                nameof(bool.Equals),
                [
                    new Method
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Boolean] },
                        Inspections = [RedundantMethodInvocation.NonConstantWithTrue],
                    },
                ]
            },
        };

    [Pure]
    static Dictionary<string, IReadOnlyCollection<Method>> CreateDateTimeMethods()
        => new(StringComparer.Ordinal)
        {
            {
                nameof(DateTime.AddTicks),
                [
                    new Method
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Int64] },
                        Inspections = [RedundantMethodInvocation.WithInt64Zero],
                    },
                ]
            },
        };

    [Pure]
    static Dictionary<string, IReadOnlyCollection<Method>> CreateDateTimeOffsetMethods()
        => new(StringComparer.Ordinal)
        {
            {
                nameof(DateTimeOffset.AddTicks),
                [
                    new Method
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Int64] },
                        Inspections = [RedundantMethodInvocation.WithInt64Zero],
                    },
                ]
            },
        };

    [Pure]
    static Dictionary<string, IReadOnlyCollection<Method>> CreateDateOnlyMethods()
        => new(StringComparer.Ordinal)
        {
            {
                "AddDays", // todo: nameof(DateOnly.AddDays) when available
                [
                    new Method
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Int32] }, Inspections = [RedundantMethodInvocation.WithInt32Zero],
                    },
                ]
            },
        };

    [Pure]
    static Dictionary<string, IReadOnlyCollection<Method>> CreateStringMethods()
        => new(StringComparer.Ordinal)
        {
            {
                nameof(string.PadLeft),
                [
                    new Method
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Int32] },
                        Inspections = [RedundantMethodInvocation.WithInt32Zero],
                    },
                    new Method
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Int32, Parameter.Char] },
                        Inspections = [RedundantMethodInvocation.WithInt32Zero],
                    },
                ]
            },
            {
                nameof(string.PadRight),
                [
                    new Method
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Int32] },
                        Inspections = [RedundantMethodInvocation.WithInt32Zero],
                    },
                    new Method
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Int32, Parameter.Char] },
                        Inspections = [RedundantMethodInvocation.WithInt32Zero],
                    },
                ]
            },
            {
                nameof(string.Replace),
                [
                    new Method
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Char, Parameter.Char] },
                        Inspections = [RedundantMethodInvocation.WithIdenticalChars],
                    },
                    new Method
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.String, Parameter.String] },
                        Inspections = [RedundantMethodInvocation.WithIdenticalNonEmptyStrings],
                    },
                    new Method
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.String, Parameter.String, Parameter.StringComparison] },
                        Inspections = [RedundantMethodInvocation.WithIdenticalNonEmptyStringsOrdinal],
                    },
                ]
            },
            {
                nameof(string.Substring),
                [
                    new Method
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Int32] },
                        Inspections = [RedundantMethodInvocation.WithInt32Zero],
                    },
                ]
            },
        };

    [Pure]
    static Dictionary<string, IReadOnlyCollection<Method>> CreateStringBuilderMethods()
        => new(StringComparer.Ordinal)
        {
            {
                nameof(StringBuilder.Append),
                [
                    new Method
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.String] },
                        Inspections =
                        [
                            RedundantMethodInvocation.WithNull with { IsPureMethod = false },
                            RedundantMethodInvocation.WithEmptyString with { IsPureMethod = false },
                        ],
                    },
                    new Method
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.CharArray] },
                        Inspections =
                        [
                            RedundantMethodInvocation.WithNull with { IsPureMethod = false },
                            RedundantMethodInvocation.WithEmptyCollection with { IsPureMethod = false },
                        ],
                    },
                    new Method
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Object] },
                        Inspections = [RedundantMethodInvocation.WithNull with { IsPureMethod = false }],
                    },
                    new Method
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.StringBuilder] },
                        Inspections = [RedundantMethodInvocation.WithNull with { IsPureMethod = false }],
                    },
                    new Method
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Char, Parameter.Int32] },
                        Inspections = [RedundantMethodInvocation.WithRepeatCountZeroInArg1 with { IsPureMethod = false }],
                    },
                    new Method
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.String, Parameter.Int32, Parameter.Int32] },
                        Inspections =
                        [
                            RedundantMethodInvocation.WithNullZeroZero with { IsPureMethod = false },
                            RedundantMethodInvocation.WithNonNullCountZero with { IsPureMethod = false },
                        ],
                    },
                    new Method
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.StringBuilder, Parameter.Int32, Parameter.Int32] },
                        Inspections =
                        [
                            RedundantMethodInvocation.WithNullZeroZero with { IsPureMethod = false },
                            RedundantMethodInvocation.WithNonNullCountZero with { IsPureMethod = false },
                        ],
                    },
                    new Method
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.CharArray, Parameter.Int32, Parameter.Int32] },
                        Inspections =
                        [
                            RedundantMethodInvocation.WithNullZeroZero with { IsPureMethod = false },
                            RedundantMethodInvocation.WithEmptyCollectionZeroZero with { IsPureMethod = false },
                        ],
                    },
                ]
            },
            {
                "AppendJoin", // todo: nameof(StringBuilder.AppendJoin) when available
                [
                    new Method
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Char, Parameter.IEnumerableOfT], GenericParametersCount = 1 },
                        Inspections = [RedundantMethodInvocation.WithEmptyCollectionInArg1 with { IsPureMethod = false }],
                    },
                    new Method
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Char, Parameter.StringArray] },
                        Inspections = [RedundantMethodInvocation.WithEmptyCollectionInParamsArg1 with { IsPureMethod = false }],
                    },
                    new Method
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Char, Parameter.ObjectArray] },
                        Inspections = [RedundantMethodInvocation.WithEmptyCollectionInParamsArg1 with { IsPureMethod = false }],
                    },
                    new Method
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Char, Parameter.ReadOnlySpanOfString] },
                        Inspections = [RedundantMethodInvocation.WithEmptyCollectionInParamsArg1 with { IsPureMethod = false }],
                    },
                    new Method
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Char, Parameter.ReadOnlySpanOfObject] },
                        Inspections = [RedundantMethodInvocation.WithEmptyCollectionInParamsArg1 with { IsPureMethod = false }],
                    },
                    new Method
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.String, Parameter.IEnumerableOfT], GenericParametersCount = 1 },
                        Inspections = [RedundantMethodInvocation.WithEmptyCollectionInArg1 with { IsPureMethod = false }],
                    },
                    new Method
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.String, Parameter.StringArray] },
                        Inspections = [RedundantMethodInvocation.WithEmptyCollectionInParamsArg1 with { IsPureMethod = false }],
                    },
                    new Method
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.String, Parameter.ObjectArray] },
                        Inspections = [RedundantMethodInvocation.WithEmptyCollectionInParamsArg1 with { IsPureMethod = false }],
                    },
                    new Method
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.String, Parameter.ReadOnlySpanOfString] },
                        Inspections = [RedundantMethodInvocation.WithEmptyCollectionInParamsArg1 with { IsPureMethod = false }],
                    },
                    new Method
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.String, Parameter.ReadOnlySpanOfObject] },
                        Inspections = [RedundantMethodInvocation.WithEmptyCollectionInParamsArg1 with { IsPureMethod = false }],
                    },
                ]
            },
            {
                nameof(StringBuilder.Insert),
                [
                    new Method
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Int32, Parameter.Object] },
                        Inspections = [RedundantMethodInvocation.WithNullInArg1 with { IsPureMethod = false }],
                    },
                ]
            },
            {
                nameof(StringBuilder.Replace),
                [
                    new Method
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.Char, Parameter.Char] },
                        Inspections = [RedundantMethodInvocation.WithIdenticalChars with { IsPureMethod = false }],
                    },
                    new Method
                    {
                        Signature = new MethodSignature { Parameters = [Parameter.String, Parameter.String] },
                        Inspections = [RedundantMethodInvocation.WithIdenticalNonEmptyStrings with { IsPureMethod = false }],
                    },
                ]
            },
        };

    [Pure]
    static Method? TryFindMethod(IReadOnlyCollection<Method> methods, IMethod resolvedMethod)
    {
        [Pure]
        static bool AreParametersMatching(IReadOnlyList<Parameter> parameters, IList<IParameter> resolvedParameters)
        {
            if (parameters.Count != resolvedParameters.Count)
            {
                return false;
            }

            for (var i = 0; i < parameters.Count; i++)
            {
                var parameter = parameters[i];
                var resolvedParameter = resolvedParameters[i];

                if (parameter.Kind != resolvedParameter.Kind || !parameter.IsType(resolvedParameter.Type))
                {
                    return false;
                }
            }

            return true;
        }

        foreach (var method in methods)
        {
            if (method.Signature.IsStatic == resolvedMethod.IsStatic
                && method.Signature.GenericParametersCount == resolvedMethod.TypeParametersCount
                && AreParametersMatching(method.Signature.Parameters, resolvedMethod.Parameters))
            {
                return method;
            }
        }

        return null;
    }

    [Pure]
    public static Method? TryGetMethod(ITypeElement type, IMethod resolvedMethod)
    {
        if (typeMethods.TryGetValue(type.GetClrName(), out var overloads) && overloads.TryGetValue(resolvedMethod.ShortName, out var methods))
        {
            return TryFindMethod(methods, resolvedMethod);
        }

        return null;
    }
}