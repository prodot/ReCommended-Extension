using JetBrains.ReSharper.Psi.CSharp;

namespace ReCommendedExtension.Extensions;

internal static class CharExtensions
{
    extension(char c)
    {
        [Pure]
        public string ToLiteralString(CSharpLanguageLevel languageLevel)
            => $"\'{
                c switch
                {
                    '\'' => @"\'",
                    '\\' => @"\\",
                    '\0' => @"\0",
                    '\a' => @"\a",
                    '\b' => @"\b",
                    '\f' => @"\f",
                    '\n' => @"\n",
                    '\r' => @"\r",
                    '\t' => @"\t",
                    '\v' => @"\v",
                    '\e' when languageLevel >= CSharpLanguageLevel.CSharp130 => @"\e",
                    _ => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c) || char.IsSymbol(c) || char.IsPunctuation(c)
                        ? c.ToString()
                        : $@"\u{(ushort)c:X4}",
                }
            }\'";
    }
}