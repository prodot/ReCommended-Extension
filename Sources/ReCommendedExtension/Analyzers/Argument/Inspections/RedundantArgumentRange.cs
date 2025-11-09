using System.Globalization;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReCommendedExtension.Extensions;

namespace ReCommendedExtension.Analyzers.Argument.Inspections;

internal sealed record RedundantArgumentRange : Inspection
{
    public static RedundantArgumentRange TripleZero { get; } = new()
    {
        Condition = args => (args[0].Value.TryGetInt32Constant(), args[1].Value.TryGetInt32Constant(), args[2].Value.TryGetInt32Constant())
            == (0, 0, 0),
        Message = "Passing '0, 0, 0' is redundant.",
    };

    public static RedundantArgumentRange NullDateTimeStylesNone { get; } = new()
    {
        Condition = args => args[0].Value.IsDefaultValue() && args[1].Value.TryGetDateTimeStylesConstant() == DateTimeStyles.None,
        Message = $"Passing 'null, {nameof(DateTimeStyles)}.{nameof(DateTimeStyles.None)}' is redundant.",
    };

    public Range ParameterIndexRange { get; init; } = ..;

    public required Func<IReadOnlyList<ICSharpArgument>, bool> Condition { get; init; }
}