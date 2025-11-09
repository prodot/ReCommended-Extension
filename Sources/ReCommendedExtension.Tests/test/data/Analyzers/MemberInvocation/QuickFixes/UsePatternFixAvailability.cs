using System;
using System.Collections.Generic;
using System.Linq;

namespace Test
{
    public class Methods
    {
        public void Pattern(char c, double d, float f, string text, string? textNullable, int[] array, List<int> list)
        {
            var result11 = char.IsAsciiDigit(c);
            var result12 = char.IsAsciiHexDigit(c);
            var result13 = char.IsAsciiHexDigitLower(c);
            var result14 = char.IsAsciiHexDigitUpper(c);
            var result15 = char.IsAsciiLetter(c);
            var result16 = char.IsAsciiLetterLower(c);
            var result17 = char.IsAsciiLetterOrDigit(c);
            var result18 = char.IsAsciiLetterUpper(c);
            var result19 = char.IsBetween(c, 'a', 'c');

            var result21 = double.IsNaN(d);
            var result22 = float.IsNaN(f);

            var result31 = text.EndsWith('c');
            var result32 = text.EndsWith(c);
            var result33 = text.EndsWith("c", StringComparison.Ordinal);
            var result34 = text.EndsWith("\a", StringComparison.Ordinal);
            var result35 = text.EndsWith("\u200b", StringComparison.Ordinal);
            var result36 = text.EndsWith("c", StringComparison.OrdinalIgnoreCase);
            var result37 = text.EndsWith("ß", StringComparison.OrdinalIgnoreCase);

            var result41 = textNullable?.IndexOf('c') == 0;
            var result42 = 0 == textNullable?.IndexOf('c');
            var result43 = textNullable?.IndexOf('c') != 0;
            var result44 = 0 != textNullable?.IndexOf('c');
            var result45 = textNullable?.IndexOf(c) == 0;
            var result46 = 0 == textNullable?.IndexOf(c);
            var result47 = textNullable?.IndexOf(c) != 0;
            var result48 = 0 != textNullable?.IndexOf(c);

            var result51 = text.StartsWith('c');
            var result52 = text.StartsWith(c);
            var result53 = text.StartsWith("c", StringComparison.Ordinal);
            var result54 = text.StartsWith("\r", StringComparison.Ordinal);
            var result55 = text.StartsWith("\u200b", StringComparison.Ordinal);
            var result56 = text.StartsWith("c", StringComparison.OrdinalIgnoreCase);
            var result57 = text.StartsWith("€", StringComparison.OrdinalIgnoreCase);

            var result61 = array.FirstOrDefault();
            var result62 = array.FirstOrDefault(-1);

            var result71 = array.LastOrDefault();
            var result72 = array.LastOrDefault(-1);

            var result81 = text.Single();
            var result82 = array.Single();

            var result91 = text.SingleOrDefault();
            var result92 = text.SingleOrDefault('c');
            var result93 = list.SingleOrDefault();
            var result94 = array.SingleOrDefault(-1);
        }
    }
}