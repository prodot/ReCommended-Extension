using System.Globalization;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using ReCommendedExtension.Extensions;

namespace ReCommendedExtension.Analyzers.Argument.Inspections;

internal abstract record RedundantArgument : Inspection
{
    public static RedundantArgumentByPosition FormatProvider { get; } =
        new() { Condition = _ => true, Message = "Passing a format provider is redundant." };

    public static RedundantArgumentByPosition Discard { get; } = new() { Condition = arg => arg.IsDiscard(), Message = "Discarding is redundant." };

    public static RedundantArgumentByPosition Null { get; } =
        new() { Condition = arg => arg.Value.IsDefaultValueOrNull, Message = "Passing null is redundant." };

    public static RedundantArgumentByPosition False { get; } = new()
    {
        Condition = arg => arg.Value.AsBooleanConstant == false, Message = "Passing false is redundant.",
    };

    public static RedundantArgumentByPosition EmptyArray { get; } = new()
    {
        Condition = arg => arg.Value.AsCollectionCreation is { Count: 0 }, Message = "Passing an empty array is redundant.",
    };

    public static RedundantArgumentByPosition ZeroInt32 { get; } = new()
    {
        Condition = arg => arg.Value.AsInt32Constant == 0, Message = "Passing 0 is redundant.",
    };

    public static RedundantArgumentByPosition OneInt32 { get; } = new()
    {
        Condition = arg => arg.Value.AsInt32Constant == 1, Message = "Passing 1 is redundant.",
    };

    public static RedundantArgumentByPosition ZeroInt64 { get; } = new()
    {
        Condition = arg => arg.Value.AsInt64Constant == 0, Message = "Passing 0 is redundant.",
    };

    public static RedundantArgumentByPosition SpaceChar { get; } = new()
    {
        Condition = arg => arg.Value.AsCharConstant == ' ', Message = "Passing ' ' is redundant.",
    };

    public static RedundantArgumentByPosition MaxValueInt32 { get; } = new()
    {
        Condition = arg => arg.Value.AsInt32Constant == int.MaxValue, Message = $"Passing the int.{nameof(int.MaxValue)} is redundant.",
    };

    public static RedundantArgumentByPosition MaxValueInt64 { get; } = new()
    {
        Condition = arg => arg.Value.AsInt64Constant == long.MaxValue, Message = $"Passing the long.{nameof(long.MaxValue)} is redundant.",
    };

    public static RedundantArgumentByPosition NumberStylesInteger { get; } = new()
    {
        Condition = arg => arg.Value.AsNumberStylesConstant == NumberStyles.Integer,
        Message = $"Passing {nameof(NumberStyles)}.{nameof(NumberStyles.Integer)} is redundant.",
    };

    public static RedundantArgumentByPosition NumberStylesNumber { get; } = new()
    {
        Condition = arg => arg.Value.AsNumberStylesConstant == NumberStyles.Number,
        Message = $"Passing {nameof(NumberStyles)}.{nameof(NumberStyles.Number)} is redundant.",
    };

    public static RedundantArgumentByPosition NumberStylesFloat { get; } = new()
    {
        Condition = arg => arg.Value.AsNumberStylesConstant == (NumberStyles.Float | NumberStyles.AllowThousands),
        Message = $"Passing {nameof(NumberStyles)}.{nameof(NumberStyles.Float)} | {nameof(NumberStyles)}.{nameof(NumberStyles.AllowThousands)} is redundant.",
    };

    public static RedundantArgumentByPosition MidpointRoundingToEven { get; } = new()
    {
        Condition = arg => arg.Value.AsMidpointRoundingConstant == MidpointRounding.ToEven,
        Message = $"Passing {nameof(MidpointRounding)}.{nameof(MidpointRounding.ToEven)} is redundant.",
    };

    public static RedundantArgumentByPosition DateTimeKindUnspecified { get; } = new()
    {
        Condition = arg => arg.Value.AsDateTimeKindConstant == DateTimeKind.Unspecified,
        Message = $"Passing {nameof(DateTimeKind)}.{nameof(DateTimeKind.Unspecified)} is redundant.",
    };

    public static RedundantArgumentByPosition DateTimeStylesNone { get; } = new()
    {
        Condition = arg => arg.Value.AsDateTimeStylesConstant == DateTimeStyles.None,
        Message = $"Passing {nameof(DateTimeStyles)}.{nameof(DateTimeStyles.None)} is redundant.",
    };

    public static RedundantArgumentByPosition TimeSpanStylesNone { get; } = new()
    {
        Condition = arg => arg.Value.AsTimeSpanStylesConstant == TimeSpanStyles.None,
        Message = $"Passing {nameof(TimeSpanStyles)}.{nameof(TimeSpanStyles.None)} is redundant.",
    };

    public static DuplicateArgument CharDuplicateArgument { get; } = new()
    {
        Selector = args =>
        {
            [Pure]
            static IEnumerable<ICSharpArgument> Iterate(TreeNodeCollection<ICSharpArgument?> args)
            {
                var set = new HashSet<char>(args.Count);

                foreach (var argument in args)
                {
                    if (argument is { Value.AsCharConstant: { } character } && !set.Add(character))
                    {
                        yield return argument;
                    }
                }
            }

            return args is [_, _, ..] ? Iterate(args) : [];
        },
        Message = "The character is already passed.",
    };
}