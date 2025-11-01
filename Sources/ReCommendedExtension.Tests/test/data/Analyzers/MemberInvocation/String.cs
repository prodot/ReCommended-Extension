using System;

namespace Test
{
    public class Methods
    {
        public void RedundantMethodInvocation(string? text, char c)
        {
            var result11 = text?.PadLeft(0);
            var result12 = text?.PadLeft(0, c);

            var result21 = text?.PadRight(0);
            var result22 = text?.PadRight(0, c);

            var result31 = text?.Replace('c', 'c');
            var result32 = text?.Replace("cd", "cd");
            var result33 = text?.Replace("cd", "cd", StringComparison.Ordinal);
        }

        public void OtherMethodInvocation(string text, string? textNullable, char c, string s, StringComparison comparisonType, int startIndex, int count)
        {
            var result11 = text.IndexOf(c) != -1;
            var result12 = text.IndexOf(c) > -1;
            var result13 = text.IndexOf(c) >= 0;
            var result14 = -1 != text.IndexOf(c);
            var result15 = -1 < text.IndexOf(c);
            var result16 = 0 <= text.IndexOf(c);

            var result21 = text.IndexOf(c) == -1;
            var result22 = text.IndexOf(c) < 0;
            var result23 = -1 == text.IndexOf(c);
            var result24 = 0 > text.IndexOf(c);

            var result31 = text.IndexOf(c, comparisonType) != -1;
            var result32 = text.IndexOf(c, comparisonType) > -1;
            var result33 = text.IndexOf(c, comparisonType) >= 0;
            var result34 = -1 != text.IndexOf(c, comparisonType);
            var result35 = -1 < text.IndexOf(c, comparisonType);
            var result36 = 0 <= text.IndexOf(c, comparisonType);

            var result41 = text.IndexOf(c, comparisonType) == -1;
            var result42 = text.IndexOf(c, comparisonType) < 0;
            var result43 = -1 == text.IndexOf(c, comparisonType);
            var result44 = 0 > text.IndexOf(c, comparisonType);

            var result51 = text.IndexOf(s) == 0;
            var result52 = text.IndexOf(s) != 0;
            var result53 = 0 == text.IndexOf(s);
            var result54 = 0 != text.IndexOf(s);

            var result61 = text.IndexOf(s) != -1;
            var result62 = text.IndexOf(s) > -1;
            var result63 = text.IndexOf(s) >= 0;
            var result64 = -1 != text.IndexOf(s);
            var result65 = -1 < text.IndexOf(s);
            var result66 = 0 <= text.IndexOf(s);

            var result71 = text.IndexOf(s) == -1;
            var result72 = text.IndexOf(s) < 0;
            var result73 = -1 == text.IndexOf(s);
            var result74 = 0 > text.IndexOf(s);

            var result81 = text.IndexOf(s, comparisonType) == 0;
            var result82 = text.IndexOf(s, comparisonType) != 0;
            var result83 = 0 == text.IndexOf(s, comparisonType);
            var result84 = 0 != text.IndexOf(s, comparisonType);

            var result91 = text.IndexOf(s, comparisonType) != -1;
            var result92 = text.IndexOf(s, comparisonType) > -1;
            var result93 = text.IndexOf(s, comparisonType) >= 0;
            var result94 = -1 != text.IndexOf(s, comparisonType);
            var result95 = -1 < text.IndexOf(s, comparisonType);
            var result96 = 0 <= text.IndexOf(s, comparisonType);

            var resultA1 = text.IndexOf(s, comparisonType) == -1;
            var resultA2 = text.IndexOf(s, comparisonType) < 0;
            var resultA3 = -1 == text.IndexOf(s, comparisonType);
            var resultA4 = 0 > text.IndexOf(s, comparisonType);

            var resultB1 = textNullable?.IndexOfAny([c]);
            var resultB2 = textNullable?.IndexOfAny([c], startIndex);
            var resultB3 = textNullable?.IndexOfAny([c], startIndex, count);
            var resultB4 = textNullable?.IndexOfAny((char[])[c]);
            var resultB5 = textNullable?.IndexOfAny((char[])[c], startIndex);
            var resultB6 = textNullable?.IndexOfAny((char[])[c], startIndex, count);
            var resultB7 = textNullable?.IndexOfAny(new[]{ c });
            var resultB8 = textNullable?.IndexOfAny(new[]{ c }, startIndex);
            var resultB9 = textNullable?.IndexOfAny(new[]{ c }, startIndex, count);

            var resultC1 = textNullable?.LastIndexOfAny([c]);
            var resultC2 = textNullable?.LastIndexOfAny([c], startIndex);
            var resultC3 = textNullable?.LastIndexOfAny([c], startIndex, count);
            var resultC4 = textNullable?.LastIndexOfAny((char[])[c]);
            var resultC5 = textNullable?.LastIndexOfAny((char[])[c], startIndex);
            var resultC6 = textNullable?.LastIndexOfAny((char[])[c], startIndex, count);
            var resultC7 = textNullable?.LastIndexOfAny(new[] { c });
            var resultC8 = textNullable?.LastIndexOfAny(new[] { c }, startIndex);
            var resultC9 = textNullable?.LastIndexOfAny(new[] { c }, startIndex, count);
        }

        public void Pattern(string text, string? textNullable, char c)
        {
            var result11 = text.EndsWith('c');
            var result12 = text.EndsWith(c);
            var result13 = text.EndsWith("c", StringComparison.Ordinal);
            var result14 = text.EndsWith("c", StringComparison.OrdinalIgnoreCase);

            var result21 = textNullable?.IndexOf('c') == 0;
            var result22 = 0 == textNullable?.IndexOf('c');
            var result23 = textNullable?.IndexOf('c') != 0;
            var result24 = 0 != textNullable?.IndexOf('c');
            var result25 = textNullable?.IndexOf(c) == 0;
            var result26 = 0 == textNullable?.IndexOf(c);
            var result27 = textNullable?.IndexOf(c) != 0;
            var result28 = 0 != textNullable?.IndexOf(c);

            var result31 = text.StartsWith('c');
            var result32 = text.StartsWith(c);
            var result33 = text.StartsWith("c", StringComparison.Ordinal);
            var result34 = text.StartsWith("c", StringComparison.OrdinalIgnoreCase);
        }

        public void RangeIndexer(string text, string? textNullable, int startIndex, int count)
        {
            var result1 = text.Remove(startIndex);
            var result2 = text.Remove(1);
            var result3 = textNullable?.Remove(startIndex);
            var result4 = textNullable?.Remove(1);
            var result5 = text.Remove(0, count);
            var result6 = textNullable?.Remove(0, count);
        }

        public void Property(string text, string? textNullable, StringComparison comparisonType)
        {
            var result1 = text.LastIndexOf("");
            var result2 = text.LastIndexOf("", comparisonType);
            var result3 = textNullable?.LastIndexOf("");
            var result4 = textNullable?.LastIndexOf("", comparisonType);
        }

        public void NoDetection(string text, string? textNullable, int totalWidth, char c, string s, int startIndex, int count, StringComparison comparisonType)
        {
            var result11 = text.PadLeft(totalWidth);
            var result12 = text.PadLeft(totalWidth, c);

            var result21 = text.PadRight(totalWidth);
            var result22 = text.PadRight(totalWidth, c);

            var result31 = text.Replace('c', c);
            var result32 = text.Replace(c, 'c');
            var result33 = text.Replace(c, c);
            var result34 = text.Replace("", "");
            var result35 = text.Replace("cd", s);
            var result36 = text.Replace(s, "cd");
            var result37 = text.Replace(s, s);

            var result41 = textNullable?.IndexOf(c) != -1;
            var result42 = textNullable?.IndexOf(c) > -1;
            var result43 = textNullable?.IndexOf(c) >= 0;
            var result44 = -1 != textNullable?.IndexOf(c);
            var result45 = -1 < textNullable?.IndexOf(c);
            var result46 = 0 <= textNullable?.IndexOf(c);

            var result51 = textNullable?.IndexOf(c) == -1;
            var result52 = textNullable?.IndexOf(c) < 0;
            var result53 = -1 == textNullable?.IndexOf(c);
            var result54 = 0 > textNullable?.IndexOf(c);

            var result61 = textNullable?.IndexOf(c, comparisonType) != -1;
            var result62 = textNullable?.IndexOf(c, comparisonType) > -1;
            var result63 = textNullable?.IndexOf(c, comparisonType) >= 0;
            var result64 = -1 != textNullable?.IndexOf(c, comparisonType);
            var result65 = -1 < textNullable?.IndexOf(c, comparisonType);
            var result66 = 0 <= textNullable?.IndexOf(c, comparisonType);

            var result71 = textNullable?.IndexOf(c, comparisonType) == -1;
            var result72 = textNullable?.IndexOf(c, comparisonType) < 0;
            var result73 = -1 == textNullable?.IndexOf(c, comparisonType);
            var result74 = 0 > textNullable?.IndexOf(c, comparisonType);

            var result81 = textNullable?.IndexOf(s) == 0;
            var result82 = textNullable?.IndexOf(s) != 0;
            var result83 = 0 == textNullable?.IndexOf(s);
            var result84 = 0 != textNullable?.IndexOf(s);

            var result91 = textNullable?.IndexOf(s) != -1;
            var result92 = textNullable?.IndexOf(s) > -1;
            var result93 = textNullable?.IndexOf(s) >= 0;
            var result94 = -1 != textNullable?.IndexOf(s);
            var result95 = -1 < textNullable?.IndexOf(s);
            var result96 = 0 <= textNullable?.IndexOf(s);

            var resultA1 = textNullable?.IndexOf(s) == -1;
            var resultA2 = textNullable?.IndexOf(s) < 0;
            var resultA3 = -1 == textNullable?.IndexOf(s);
            var resultA4 = 0 > textNullable?.IndexOf(s);

            var resultB1 = textNullable?.IndexOf(s, comparisonType) == 0;
            var resultB2 = textNullable?.IndexOf(s, comparisonType) != 0;
            var resultB3 = 0 == textNullable?.IndexOf(s, comparisonType);
            var resultB4 = 0 != textNullable?.IndexOf(s, comparisonType);

            var resultC1 = textNullable?.IndexOf(s, comparisonType) != -1;
            var resultC2 = textNullable?.IndexOf(s, comparisonType) > -1;
            var resultC3 = textNullable?.IndexOf(s, comparisonType) >= 0;
            var resultC4 = -1 != textNullable?.IndexOf(s, comparisonType);
            var resultC5 = -1 < textNullable?.IndexOf(s, comparisonType);
            var resultC6 = 0 <= textNullable?.IndexOf(s, comparisonType);

            var resultD1 = textNullable?.IndexOf(s, comparisonType) == -1;
            var resultD2 = textNullable?.IndexOf(s, comparisonType) < 0;
            var resultD3 = -1 == textNullable?.IndexOf(s, comparisonType);
            var resultD4 = 0 > textNullable?.IndexOf(s, comparisonType);

            var resultE1 = textNullable?.IndexOfAny([c, c]);
            var resultE2 = textNullable?.IndexOfAny([c, c], startIndex);
            var resultE3 = textNullable?.IndexOfAny([c, c], startIndex, count);
            var resultE4 = textNullable?.IndexOfAny((char[])[c, c]);
            var resultE5 = textNullable?.IndexOfAny((char[])[c, c], startIndex);
            var resultE6 = textNullable?.IndexOfAny((char[])[c, c], startIndex, count);
            var resultE7 = textNullable?.IndexOfAny(new[] { c, c });
            var resultE8 = textNullable?.IndexOfAny(new[] { c, c }, startIndex);
            var resultE9 = textNullable?.IndexOfAny(new[] { c, c }, startIndex, count);

            var resultF1 = textNullable?.LastIndexOfAny([c, c]);
            var resultF2 = textNullable?.LastIndexOfAny([c, c], startIndex);
            var resultF3 = textNullable?.LastIndexOfAny([c, c], startIndex, count);
            var resultF4 = textNullable?.LastIndexOfAny((char[])[c, c]);
            var resultF5 = textNullable?.LastIndexOfAny((char[])[c, c], startIndex);
            var resultF6 = textNullable?.LastIndexOfAny((char[])[c, c], startIndex, count);
            var resultF7 = textNullable?.LastIndexOfAny(new[] { c, c });
            var resultF8 = textNullable?.LastIndexOfAny(new[] { c, c }, startIndex);
            var resultF9 = textNullable?.LastIndexOfAny(new[] { c, c }, startIndex, count);

            var resultG1 = text.EndsWith(s, comparisonType);
            var resultG2 = textNullable?.EndsWith('c');
            var resultG3 = textNullable?.EndsWith(c);
            var resultG4 = textNullable?.EndsWith("c", StringComparison.Ordinal);
            var resultG5 = textNullable?.EndsWith("c", StringComparison.OrdinalIgnoreCase);

            var resultH1 = text.StartsWith(s, comparisonType);
            var resultH2 = textNullable?.StartsWith('c');
            var resultH3 = textNullable?.StartsWith(c);
            var resultH4 = textNullable?.StartsWith("c", StringComparison.Ordinal);
            var resultH5 = textNullable?.StartsWith("c", StringComparison.OrdinalIgnoreCase);

            var resultI1 = text.Remove(0);
            var resultI2 = textNullable?.Remove(0);
            var resultI3 = text.Remove(1, count);
            var resultI4 = textNullable?.Remove(1, count);

            var resultJ1 = text.LastIndexOf(s);
            var resultJ2 = text.LastIndexOf(s, comparisonType);
            var resultJ3 = textNullable?.LastIndexOf(s);
            var resultJ4 = textNullable?.LastIndexOf(s, comparisonType);

            text.EndsWith('c');
            text.EndsWith(c);
            text.EndsWith("c", StringComparison.Ordinal);
            text.EndsWith("c", StringComparison.OrdinalIgnoreCase);

            text.StartsWith('c');
            text.StartsWith(c);
            text.StartsWith("c", StringComparison.Ordinal);
            text.StartsWith("c", StringComparison.OrdinalIgnoreCase);

            text.Remove(startIndex);
            text.Remove(1);
            textNullable?.Remove(startIndex);
            textNullable?.Remove(1);
            text.Remove(0, count);
            textNullable?.Remove(0, count);

            text.LastIndexOf("");
            text.LastIndexOf("", comparisonType);
            textNullable?.LastIndexOf("");
            textNullable?.LastIndexOf("", comparisonType);
        }
    }
}