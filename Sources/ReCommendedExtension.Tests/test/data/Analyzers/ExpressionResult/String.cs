using System;
using System.Collections.Generic;

namespace Test
{
    public class ExpressionResults
    {
        public void ExpressionResult(string text, StringComparison comparisonType, char c, string s, string[] stringArray, char[] charArray, StringSplitOptions options, int count)
        {
            var result11 = text.Contains("");
            var result12 = text.Contains("", comparisonType);

            var result21 = text.EndsWith("");
            var result22 = text.EndsWith("", comparisonType);

            var result31 = text.GetTypeCode();

            var result41 = text.IndexOf("");
            var result42 = text.IndexOf("", comparisonType);

            var result51 = text.IndexOfAny([]);

            var result61 = string.Join(c, (IEnumerable<int>)[]);
            var result62 = string.Join(c, (IEnumerable<int>)[100]);
            var result63 = string.Join(c, (string[])[]);
            var result64 = string.Join(c, (string[])["item"]);
            var result65 = string.Join(c, (object[])[]);
            var result66 = string.Join(c, (object[])[true]);
            var result67 = string.Join(c, (ReadOnlySpan<string?>)[]);
            var result68 = string.Join(c, (ReadOnlySpan<string?>)["item"]);
            var result69 = string.Join(c, (ReadOnlySpan<object?>)[]);
            var result6A = string.Join(c, (ReadOnlySpan<object?>)[true]);
            var result6B = string.Join(c, 100);
            var result6C = string.Join(c, true);
            var result6D = string.Join(c, "item");
            var result6E = string.Join(s, (IEnumerable<int>)[]);
            var result6F = string.Join(s, (IEnumerable<int>)[100]);
            var result6G = string.Join(s, (string[])[]);
            var result6H = string.Join(s, (string[])["item"]);
            var result6I = string.Join(s, (object[])[]);
            var result6J = string.Join(s, (object[])[true]);
            var result6K = string.Join(s, (ReadOnlySpan<string?>)[]);
            var result6L = string.Join(s, (ReadOnlySpan<string?>)["item"]);
            var result6M = string.Join(s, (ReadOnlySpan<object?>)[]);
            var result6N = string.Join(s, (ReadOnlySpan<object?>)[true]);
            var result6O = string.Join(s, 100);
            var result6P = string.Join(s, true);
            var result6Q = string.Join(s, "item");
            var result6R = string.Join(c, stringArray, 0, 0);
            var result6S = string.Join(c, ["item"], 1, 0);
            var result6T = string.Join(c, ["item"], 0, 1);
            var result6U = string.Join(s, stringArray, 0, 0);
            var result6V = string.Join(s, ["item"], 1, 0);
            var result6W = string.Join(s, ["item"], 0, 1);

            var result71 = text.LastIndexOf(c, 0);

            var result81 = text.LastIndexOfAny([]);
            var result82 = text.LastIndexOfAny(charArray, 0);
            var result83 = text.LastIndexOfAny(charArray, 0, 0);
            var result84 = text.LastIndexOfAny(charArray, 0, 1);

            var result91 = text.Remove(0);

            var resultA1 = text.Split(null as string);
            var resultA2 = text.Split(null as string, StringSplitOptions.None);
            var resultA3 = text.Split("");
            var resultA4 = text.Split("", StringSplitOptions.None);
            var resultA5 = text.Split(null as string, StringSplitOptions.TrimEntries);
            var resultA6 = text.Split("", StringSplitOptions.TrimEntries);
            var resultA7 = text.Split([';'], 0);
            var resultA8 = text.Split([';'], 1);
            var resultA9 = text.Split([""], StringSplitOptions.None);
            var resultAA = text.Split([""], StringSplitOptions.TrimEntries);
            var resultAB = text.Split(';', 0);
            var resultAC = text.Split(';', 0, options);
            var resultAD = text.Split(';', 1);
            var resultAE = text.Split(';', 1, StringSplitOptions.None);
            var resultAF = text.Split(';', 1, StringSplitOptions.TrimEntries);
            var resultAG = text.Split("; ", 0);
            var resultAH = text.Split("; ", 0, options);
            var resultAI = text.Split("; ", 1);
            var resultAJ = text.Split("; ", 1, StringSplitOptions.None);
            var resultAK = text.Split(null as string, count);
            var resultAL = text.Split(null as string, count, StringSplitOptions.None);
            var resultAM = text.Split("", count);
            var resultAN = text.Split("", count, StringSplitOptions.None);
            var resultAO = text.Split("; ", 1, StringSplitOptions.TrimEntries);
            var resultAP = text.Split(null as string, count, StringSplitOptions.TrimEntries);
            var resultAQ = text.Split("", count, StringSplitOptions.TrimEntries);
            var resultAR = text.Split([';'], 0, options);
            var resultAS = text.Split([';'], 1, StringSplitOptions.None);
            var resultAT = text.Split([';'], 1, StringSplitOptions.TrimEntries);
            var resultAU = text.Split(["; "], 0, options);
            var resultAV = text.Split(["; "], 1, StringSplitOptions.None);
            var resultAW = text.Split([""], count, StringSplitOptions.None);
            var resultAX = text.Split(["; "], 1, StringSplitOptions.TrimEntries);
            var resultAY = text.Split([""], count, StringSplitOptions.TrimEntries);

            var resultB1 = text.StartsWith("");
            var resultB2 = text.StartsWith("", comparisonType);
        }

        public void NoDetection(string text, string? textNullable, string value, StringComparison comparisonType, char[] charArray, char c, string s, string[] stringArray, int startIndex, StringSplitOptions options, int count)
        {
            var result11 = text.Contains(value);
            var result12 = text.Contains(value, comparisonType);
            var result13 = textNullable?.Contains("");
            var result14 = textNullable?.Contains("", comparisonType);

            var result21 = text.EndsWith(value);
            var result22 = text.EndsWith(value, comparisonType);
            var result23 = textNullable?.EndsWith("");
            var result24 = textNullable?.EndsWith("", comparisonType);

            var result31 = textNullable?.GetTypeCode();

            var result41 = text.IndexOf(value);
            var result42 = text.IndexOf(value, comparisonType);
            var result43 = textNullable?.IndexOf("");
            var result44 = textNullable?.IndexOf("", comparisonType);

            var result51 = text.IndexOfAny(charArray);
            var result52 = textNullable?.IndexOfAny([]);

            var result61 = string.Join(c, (IEnumerable<int>)[100, 100]);
            var result62 = string.Join(c, (string[])["item", "item"]);
            var result63 = string.Join(c, (object[])[true, true]);
            var result64 = string.Join(c, (ReadOnlySpan<string?>)["item", "item"]);
            var result65 = string.Join(c, (ReadOnlySpan<object?>)[true, true]);
            var result66 = string.Join(c, 100, 100);
            var result67 = string.Join(c, true, true);
            var result68 = string.Join(c, "item", "item");
            var result69 = string.Join(s, (IEnumerable<int>)[100, 100]);
            var result6A = string.Join(s, (string[])["item", "item"]);
            var result6B = string.Join(s, (object[])[true, true]);
            var result6C = string.Join(s, (ReadOnlySpan<string?>)["item", "item"]);
            var result6D = string.Join(s, (ReadOnlySpan<object?>)[true, true]);
            var result6E = string.Join(s, 100, 100);
            var result6F = string.Join(s, true, true);
            var result6G = string.Join(s, "item", "item");
            var result6H = string.Join(c, stringArray, 1, 2);
            var result6I = string.Join(s, stringArray, 1, 2);

            var result71 = text.LastIndexOf(c, startIndex);
            var result72 = textNullable?.LastIndexOf(c, 0);

            var result81 = text.LastIndexOfAny(charArray);
            var result82 = text.LastIndexOfAny(charArray, 1);
            var result83 = text.LastIndexOfAny(charArray, 1, 2);
            var result84 = text.LastIndexOfAny(charArray, 1, 2);
            var result85 = textNullable?.LastIndexOfAny([]);
            var result86 = textNullable?.LastIndexOfAny(charArray, 0);
            var result87 = textNullable?.LastIndexOfAny(charArray, 0, 0);
            var result88 = textNullable?.LastIndexOfAny(charArray, 0, 1);

            var result91 = text.Remove(startIndex);
            var result92 = textNullable?.Remove(0);

            var resultA1 = textNullable?.Split(null as string);
            var resultA2 = textNullable?.Split(null as string, StringSplitOptions.None);
            var resultA3 = textNullable?.Split("");
            var resultA4 = textNullable?.Split("", StringSplitOptions.None);
            var resultA5 = textNullable?.Split(null as string, StringSplitOptions.TrimEntries);
            var resultA6 = textNullable?.Split("", StringSplitOptions.TrimEntries);
            var resultA7 = textNullable?.Split([';'], 0);
            var resultA8 = textNullable?.Split([';'], 1);
            var resultA9 = textNullable?.Split([""], StringSplitOptions.None);
            var resultAA = textNullable?.Split([""], StringSplitOptions.TrimEntries);
            var resultAB = textNullable?.Split(';', 0);
            var resultAC = textNullable?.Split(';', 0, options);
            var resultAD = textNullable?.Split(';', 1);
            var resultAE = textNullable?.Split(';', 1, StringSplitOptions.None);
            var resultAF = textNullable?.Split(';', 1, StringSplitOptions.TrimEntries);
            var resultAG = textNullable?.Split("; ", 0);
            var resultAH = textNullable?.Split("; ", 0, options);
            var resultAI = textNullable?.Split("; ", 1);
            var resultAJ = textNullable?.Split("; ", 1, StringSplitOptions.None);
            var resultAK = textNullable?.Split(null as string, count);
            var resultAL = textNullable?.Split(null as string, count, StringSplitOptions.None);
            var resultAM = textNullable?.Split("", count);
            var resultAN = textNullable?.Split("", count, StringSplitOptions.None);
            var resultAO = textNullable?.Split("; ", 1, StringSplitOptions.TrimEntries);
            var resultAP = textNullable?.Split(null as string, count, StringSplitOptions.TrimEntries);
            var resultAQ = textNullable?.Split("", count, StringSplitOptions.TrimEntries);
            var resultAR = textNullable?.Split([';'], 0, options);
            var resultAS = textNullable?.Split([';'], 1, StringSplitOptions.None);
            var resultAT = textNullable?.Split([';'], 1, StringSplitOptions.TrimEntries);
            var resultAU = textNullable?.Split(["; "], 0, options);
            var resultAV = textNullable?.Split(["; "], 1, StringSplitOptions.None);
            var resultAW = textNullable?.Split([""], count, StringSplitOptions.None);
            var resultAX = textNullable?.Split(["; "], 1, StringSplitOptions.TrimEntries);
            var resultAY = textNullable?.Split([""], count, StringSplitOptions.TrimEntries);

            var resultB1 = text.StartsWith(value);
            var resultB2 = text.StartsWith(value, comparisonType);
            var resultB3 = textNullable?.StartsWith("");
            var resultB4 = textNullable?.StartsWith("", comparisonType);

            text.Contains("");
            text.Contains("", comparisonType);

            text.EndsWith("");
            text.EndsWith("", comparisonType);

            text.GetTypeCode();

            text.IndexOf("");
            text.IndexOf("", comparisonType);

            text.IndexOfAny([]);

            string.Join(c, (IEnumerable<int>)[]);
            string.Join(c, (IEnumerable<int>)[100]);
            string.Join(c, (string[])[]);
            string.Join(c, (string[])["item"]);
            string.Join(c, (object[])[]);
            string.Join(c, (object[])[true]);
            string.Join(c, (ReadOnlySpan<string?>)[]);
            string.Join(c, (ReadOnlySpan<string?>)["item"]);
            string.Join(c, (ReadOnlySpan<object?>)[]);
            string.Join(c, (ReadOnlySpan<object?>)[true]);
            string.Join(c, 100);
            string.Join(c, true);
            string.Join(c, "item");
            string.Join(s, (IEnumerable<int>)[]);
            string.Join(s, (IEnumerable<int>)[100]);
            string.Join(s, (string[])[]);
            string.Join(s, (string[])["item"]);
            string.Join(s, (object[])[]);
            string.Join(s, (object[])[true]);
            string.Join(s, (ReadOnlySpan<string?>)[]);
            string.Join(s, (ReadOnlySpan<string?>)["item"]);
            string.Join(s, (ReadOnlySpan<object?>)[]);
            string.Join(s, (ReadOnlySpan<object?>)[true]);
            string.Join(s, 100);
            string.Join(s, true);
            string.Join(s, "item");
            string.Join(c, stringArray, 0, 0);
            string.Join(c, ["item"], 1, 0);
            string.Join(c, ["item"], 0, 1);
            string.Join(s, stringArray, 0, 0);
            string.Join(s, ["item"], 1, 0);
            string.Join(s, ["item"], 0, 1);

            text.LastIndexOf(c, 0);

            text.LastIndexOfAny([]);
            text.LastIndexOfAny(charArray, 0);
            text.LastIndexOfAny(charArray, 0, 0);
            text.LastIndexOfAny(charArray, 0, 1);

            text.Remove(0);

            text.Split(null as string);
            text.Split(null as string, StringSplitOptions.None);
            text.Split("");
            text.Split("", StringSplitOptions.None);
            text.Split(null as string, StringSplitOptions.TrimEntries);
            text.Split("", StringSplitOptions.TrimEntries);
            text.Split([';'], 0);
            text.Split([';'], 1);
            text.Split([""], StringSplitOptions.None);
            text.Split([""], StringSplitOptions.TrimEntries);
            text.Split(';', 0);
            text.Split(';', 0, options);
            text.Split(';', 1);
            text.Split(';', 1, StringSplitOptions.None);
            text.Split(';', 1, StringSplitOptions.TrimEntries);
            text.Split("; ", 0);
            text.Split("; ", 0, options);
            text.Split("; ", 1);
            text.Split("; ", 1, StringSplitOptions.None);
            text.Split(null as string, count);
            text.Split(null as string, count, StringSplitOptions.None);
            text.Split("", count);
            text.Split("", count, StringSplitOptions.None);
            text.Split("; ", 1, StringSplitOptions.TrimEntries);
            text.Split(null as string, count, StringSplitOptions.TrimEntries);
            text.Split("", count, StringSplitOptions.TrimEntries);
            text.Split([';'], 0, options);
            text.Split([';'], 1, StringSplitOptions.None);
            text.Split([';'], 1, StringSplitOptions.TrimEntries);
            text.Split(["; "], 0, options);
            text.Split(["; "], 1, StringSplitOptions.None);
            text.Split([""], count, StringSplitOptions.None);
            text.Split(["; "], 1, StringSplitOptions.TrimEntries);
            text.Split([""], count, StringSplitOptions.TrimEntries);

            text.StartsWith("");
            text.StartsWith("", comparisonType);
        }
    }
}