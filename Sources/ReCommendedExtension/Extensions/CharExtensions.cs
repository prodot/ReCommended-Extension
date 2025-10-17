using System.Text;
using JetBrains.ReSharper.Psi.CSharp;

namespace ReCommendedExtension.Extensions;

internal static class CharExtensions
{
    [Pure]
    public static string ToLiteralString(this char c, CSharpLanguageLevel languageLevel)
    {
        var builder = new StringBuilder(3);

        builder.Append('\'');

        switch (c)
        {
            case '\'': builder.Append(@"\'"); break;
            case '\\': builder.Append(@"\\"); break;
            case '\0': builder.Append(@"\0"); break;
            case '\a': builder.Append(@"\a"); break;
            case '\b': builder.Append(@"\b"); break;
            case '\f': builder.Append(@"\f"); break;
            case '\n': builder.Append(@"\n"); break;
            case '\r': builder.Append(@"\r"); break;
            case '\t': builder.Append(@"\t"); break;
            case '\v': builder.Append(@"\v"); break;
            case '\e' when languageLevel >= CSharpLanguageLevel.CSharp130: builder.Append(@"\e"); break;
        }

        if (char.IsLetterOrDigit(c) || char.IsWhiteSpace(c) || char.IsSymbol(c) || char.IsPunctuation(c))
        {
            builder.Append(c);
        }
        else
        {
            builder.Append($@"\u{(ushort)c:X4}");
        }

        builder.Append('\'');

        return builder.ToString();
    }
}