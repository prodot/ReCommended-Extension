using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using ReCommendedExtension.Extensions;
using ReCommendedExtension.Extensions.Collections;

namespace ReCommendedExtension.Analyzers.MemberInvocation.Inspections;

internal sealed record RedundantMethodInvocation : Inspection
{
    public static RedundantMethodInvocation NonBooleanConstantWithTrue { get; } = new()
    {
        Condition = (qualifier, args) => qualifier.TryGetBooleanConstant() == null && args[0]?.Value.TryGetBooleanConstant() == true,
        Message = methodName => $"Calling '{methodName}' with true is redundant.",
    };

    public static RedundantMethodInvocation WithNull { get; } = new()
    {
        Condition = (_, args) => args[0] is { } arg && arg.Value.IsDefaultValue(),
        Message = methodName => $"Calling '{methodName}' with null is redundant.",
    };

    public static RedundantMethodInvocation WithInt32Zero { get; } = new()
    {
        Condition = (_, args) => args[0]?.Value.TryGetInt32Constant() == 0,
        Message = methodName => $"Calling '{methodName}' with 0 is redundant.",
    };

    public static RedundantMethodInvocation WithInt64Zero { get; } = new()
    {
        Condition = (_, args) => args[0]?.Value.TryGetInt64Constant() == 0,
        Message = methodName => $"Calling '{methodName}' with 0 is redundant.",
    };

    public static RedundantMethodInvocation WithEmptyString { get; } = new()
    {
        Condition = (_, args) => args[0]?.Value.TryGetStringConstant() == "",
        Message = methodName => $"Calling '{methodName}' with an empty string is redundant.",
    };

    public static RedundantMethodInvocation WithEmptyCollection { get; } = new()
    {
        Condition = (_, args) => CollectionCreation.TryFrom(args[0]?.Value) is { Count: 0 },
        Message = methodName => $"Calling '{methodName}' with an empty collection is redundant.",
    };

    public static RedundantMethodInvocation WithNullInArg1 { get; } = new()
    {
        Condition = (_, args) => args[1] is { } arg && arg.Value.IsDefaultValue(),
        Message = methodName => $"Calling '{methodName}' with null is redundant.",
    };

    public static RedundantMethodInvocation WithEmptyCollectionInArg1 { get; } = new()
    {
        Condition = (_, args) => CollectionCreation.TryFrom(args[1]?.Value) is { Count: 0 },
        Message = methodName => $"Calling '{methodName}' with an empty collection is redundant.",
    };

    public static RedundantMethodInvocation WithEmptyCollectionInParamsArg1 { get; } = new()
    {
        Condition = (_, args) => args is [_] || args is [_, { Value: { } arg }] && CollectionCreation.TryFrom(arg) is { Count: 0 },
        Message = methodName => $"Calling '{methodName}' with an empty collection is redundant.",
    };

    public static RedundantMethodInvocation WithRepeatCountZeroInArg1 { get; } = new()
    {
        Condition = (_, args) => args[1]?.Value.TryGetInt32Constant() == 0,
        Message = methodName => $"Calling '{methodName}' with the repeat count 0 is redundant.",
    };

    public static RedundantMethodInvocation WithIdenticalChars { get; } = new()
    {
        Condition = (_, args) => args[0]?.Value.TryGetCharConstant() is { } c0 && args[1]?.Value.TryGetCharConstant() is { } c1 && c0 == c1,
        Message = methodName => $"Calling '{methodName}' with identical characters is redundant.",
    };

    public static RedundantMethodInvocation WithIdenticalNonEmptyStrings { get; } = new()
    {
        Condition = (_, args)
            => args[0]?.Value.TryGetStringConstant() is { } s0 and not "" && args[1]?.Value.TryGetStringConstant() is { } s1 && s0 == s1,
        Message = methodName => $"Calling '{methodName}' with identical values is redundant.",
    };

    public static RedundantMethodInvocation WithIdenticalNonEmptyStringsOrdinal { get; } = new()
    {
        Condition = (_, args) => args[0]?.Value.TryGetStringConstant() is { } s0 and not ""
            && args[1]?.Value.TryGetStringConstant() is { } s1
            && s0 == s1
            && args[2]?.Value.TryGetStringComparisonConstant() == StringComparison.Ordinal,
        Message = methodName => $"Calling '{methodName}' with identical values is redundant.",
    };

    public static RedundantMethodInvocation WithNullZeroZero { get; } = new()
    {
        Condition = (_, args)
            => args[0] is { } arg
            && arg.Value.IsDefaultValue()
            && args[1]?.Value.TryGetInt32Constant() == 0
            && args[2]?.Value.TryGetInt32Constant() == 0,
        Message = methodName => $"Calling '{methodName}' with 'null, 0, 0' is redundant.",
    };

    public static RedundantMethodInvocation WithEmptyCollectionZeroZero { get; } = new()
    {
        Condition = (_, args)
            => CollectionCreation.TryFrom(args[0]?.Value) is { Count: 0 }
            && args[1]?.Value.TryGetInt32Constant() == 0
            && args[2]?.Value.TryGetInt32Constant() == 0,
        Message = methodName => $"Calling '{methodName}' with an empty collection and '0, 0' is redundant.",
    };

    public static RedundantMethodInvocation WithNonNullCountZero { get; } = new()
    {
        Condition = (_, args) => args[2]?.Value.TryGetInt32Constant() == 0,
        Message = methodName => $"Calling '{methodName}' with count 0 is redundant.",
    };

    public bool IsPureMethod { get; init; } = true;

    public bool EnsureFirstArgumentNotNull { get; init; }

    public required Func<ICSharpExpression, TreeNodeCollection<ICSharpArgument?>, bool> Condition { get; init; }
}