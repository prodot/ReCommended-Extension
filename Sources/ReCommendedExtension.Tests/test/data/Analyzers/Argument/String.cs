using System;
using System.Collections.Generic;

namespace Test
{
    public class Arguments
    {
        public void RedundantArgument(string text, char c, char[] charArray, string s, StringComparison comparison, int totalWidth)
        {
            var result11 = text.IndexOf(c, 0);
            var result12 = text.IndexOf(s, 0);
            var result13 = text.IndexOf(s, 0, comparison);

            var result21 = text.IndexOfAny(charArray, 0);

            var result31 = text.PadLeft(totalWidth, ' ');

            var result41 = text.PadRight(totalWidth, ' ');

            var result51 = text.Split(';', ';', ';');

            var result61 = text.Trim(null);
            var result62 = text.Trim(Array.Empty<char>());
            var result63 = text.Trim('.', '.', '.');

            var result71 = text.TrimEnd(null);
            var result72 = text.TrimEnd(Array.Empty<char>());
            var result73 = text.TrimEnd('.', '.', '.');

            var result81 = text.TrimStart(null);
            var result82 = text.TrimStart(Array.Empty<char>());
            var result82 = text.TrimStart('.', '.', '.');
        }

        public void RedundantCollectionElement(string text, int startIndex, int count, StringSplitOptions options)
        {
            var result11 = text.IndexOfAny(['a', 'a']);
            var result12 = text.IndexOfAny(['a', 'a'], startIndex);
            var result13 = text.IndexOfAny(['a', 'a'], startIndex, count);

            var result21 = text.LastIndexOfAny(['a', 'a']);
            var result22 = text.LastIndexOfAny(['a', 'a'], startIndex);
            var result23 = text.LastIndexOfAny(['a', 'a'], startIndex, count);

            var result31 = text.Split(new[] { ';', ';' });
            var result32 = text.Split([';', ';'], count);
            var result33 = text.Split([';', ';'], options);
            var result34 = text.Split([';', ';'], count, options);
            var result35 = text.Split([";;", ";;"], options);
            var result36 = text.Split([";;", ";;"], count, options);

            var result41 = text.Trim(new [] { '.', '.', '.' });

            var result51 = text.TrimEnd(new[] { '.', '.', '.' });

            var result61 = text.TrimStart(new[] { '.', '.', '.' });
        }

        public void OtherArgument(string text, int startIndex, int count, IEnumerable<int> valuesGenericEnumerable, string[] valuesStringArray, object[] valuesObjectArray, ReadOnlySpan<string> valuesStrings, ReadOnlySpan<object> valuesObjects, StringComparison comparison, StringSplitOptions options)
        {
            var result11 = text.Contains("c");
            var result12 = text.Contains("c", comparison);

            var result21 = text.IndexOf("c");
            var result22 = text.IndexOf("c", comparison);

            var result31 = string.Join(";", valuesGenericEnumerable);
            var result32 = string.Join(";", valuesStringArray);
            var result33 = string.Join(";", valuesStringArray, startIndex, count);
            var result34 = string.Join(";", valuesObjectArray);
            var result35 = string.Join(";", valuesStrings);
            var result36 = string.Join(";", valuesObjects);

            var result41 = text.LastIndexOf("c", StringComparison.Ordinal);

            var result51 = text.Split(";", options);
            var result52 = text.Split(";", count, options);
            var result53 = text.Split([";", ":"], options);
            var result54 = text.Split(new[] { ";", ":" }, options);
            var result55 = text.Split([";", ":"], count, options);
            var result56 = text.Split(new[] { ";", ":" }, count, options);
        }

        public void OtherArgumentRange(string text)
        {
            var result1 = text.Replace("c", "x");
            var result2 = text.Replace("c", "x", StringComparison.Ordinal);
        }

        public void NoDetection(string text, char c, char[] charArray, string s, int startIndex, int count, StringComparison comparison, int totalWidth, StringSplitOptions options, IEnumerable<int> valuesGenericEnumerable, string[] valuesStringArray, object[] valuesObjectArray, ReadOnlySpan<string> valuesStrings, ReadOnlySpan<object> valuesObjects)
        {
            var result11 = text.IndexOf(c, startIndex);
            var result12 = text.IndexOf(s, startIndex);
            var result13 = text.IndexOf(s, startIndex, comparison);

            var result21 = text.IndexOfAny(charArray, startIndex);

            var result31 = text.PadLeft(totalWidth, c);

            var result41 = text.PadRight(totalWidth, c);

            var result51 = text.Split(';', '.');

            var result61 = text.Trim(charArray);
            var result62 = text.Trim(';', '.');

            var result71 = text.TrimEnd(charArray);
            var result72 = text.TrimEnd(';', '.');

            var result81 = text.TrimStart(charArray);
            var result82 = text.TrimStart(';', '.');

            var result91 = text.IndexOfAny([c, c]);
            var result92 = text.IndexOfAny([c, c], startIndex);
            var result93 = text.IndexOfAny([c, c], startIndex, count);

            var resultA1 = text.LastIndexOfAny([c, c]);
            var resultA2 = text.LastIndexOfAny([c, c], startIndex);
            var resultA3 = text.LastIndexOfAny([c, c], startIndex, count);

            var resultB1 = text.Split([c, c]);
            var resultB2 = text.Split([c, c], count);
            var resultB3 = text.Split([c, c], options);
            var resultB4 = text.Split([c, c], count, options);
            var resultB5 = text.Split([s, s], options);
            var resultB6 = text.Split([s, s], count, options);

            var resultC1 = text.Trim([';', '.']);

            var resultD1 = text.TrimEnd([';', '.']);

            var resultE1 = text.TrimStart([';', '.']);

            var resultF1 = text.Contains("cc");
            var resultF2 = text.Contains(s);
            var resultF3 = text.Contains("cc", comparison);
            var resultF4 = text.Contains(s, comparison);

            var resultG1 = text.IndexOf("cc");
            var resultG2 = text.IndexOf(s);
            var resultG3 = text.IndexOf("cc", comparison);
            var resultG4 = text.IndexOf(s, comparison);

            var resultH1 = string.Join(";;", valuesGenericEnumerable);
            var resultH2 = string.Join(s, valuesGenericEnumerable);
            var resultH3 = string.Join(";;", valuesStringArray);
            var resultH4 = string.Join(s, valuesStringArray);
            var resultH5 = string.Join(";;", valuesStringArray, startIndex, count);
            var resultH6 = string.Join(s, valuesStringArray, startIndex, count);
            var resultH7 = string.Join(";;", valuesObjectArray);
            var resultH8 = string.Join(s, valuesObjectArray);
            var resultH9 = string.Join(";;", valuesStrings);
            var resultHA = string.Join(s, valuesStrings);
            var resultHB = string.Join("dd", valuesObjects);
            var resultHC = string.Join(s, valuesObjects);

            var resultI1 = text.LastIndexOf("cc", StringComparison.Ordinal);
            var resultI2 = text.LastIndexOf(s, StringComparison.Ordinal);
            var resultI3 = text.LastIndexOf("c", StringComparison.OrdinalIgnoreCase);

            var resultJ1 = text.Split(";;", options);
            var resultJ2 = text.Split(s, options);
            var resultJ3 = text.Split(";;", count, options);
            var resultJ4 = text.Split(s, count, options);
            var resultJ5 = text.Split([";;", ":"], options);
            var resultJ6 = text.Split([s, ":"], options);
            var resultJ7 = text.Split(new[] { ";;", ":" }, options);
            var resultJ8 = text.Split(new[] { s, ":" }, options);
            var resultJ9 = text.Split([";;", ":"], count, options);
            var resultJA = text.Split([s, ":"], count, options);
            var resultJB = text.Split(new[] { ";;", ":" }, count, options);
            var resultJC = text.Split(new[] { s, ":" }, count, options);

            var resultK1 = text.Replace("cc", "x");
            var resultK2 = text.Replace("c", "xx");
            var resultK3 = text.Replace(s, "x");
            var resultK4 = text.Replace("c", s);
            var resultK5 = text.Replace("cc", "x", StringComparison.Ordinal);
            var resultK6 = text.Replace("c", "xx", StringComparison.Ordinal);
            var resultK7 = text.Replace(s, "x", StringComparison.Ordinal);
            var resultK8 = text.Replace("c", s, StringComparison.Ordinal);
            var resultK9 = text.Replace("c", "x", StringComparison.OrdinalIgnoreCase);
        }
    }
}