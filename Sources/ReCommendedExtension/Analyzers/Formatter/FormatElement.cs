using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Util;

namespace ReCommendedExtension.Analyzers.Formatter;

public record struct FormatElement
{
    FormatElement(string? format, IType expressionType, bool canBeRemoved)
    {
        Debug.Assert(!expressionType.IsNullable());

        Format = format;
        ExpressionType = expressionType;
        CanBeRemoved = canBeRemoved;
    }

    internal FormatElement(string? format, IType expressionType, IInterpolatedStringInsert insert) : this(format, expressionType, true)
    {
        Debug.Assert(format is null or [not ':', ..]);

        Insert = insert;
    }

    internal FormatElement(
        string format,
        IType expressionType,
        ICSharpLiteralExpression formatStringExpression,
        FormatStringParser.FormatItem formatItem) : this(format, expressionType, true)
    {
        Debug.Assert(format is [_, ..]);

        FormatStringExpression = formatStringExpression;
        FormatItem = formatItem;
    }

    internal FormatElement(string? format, IType expressionType, ICSharpArgument argument, bool canBeRemoved) : this(
        format,
        expressionType,
        canBeRemoved)
        => Argument = argument;

    internal string? Format { get; }

    internal IType ExpressionType { get; }

    internal bool CanBeRemoved { get; }

    internal IInterpolatedStringInsert? Insert { get; }

    internal ICSharpLiteralExpression? FormatStringExpression { get; }

    internal FormatStringParser.FormatItem? FormatItem { get; }

    internal ICSharpArgument? Argument { get; }
}