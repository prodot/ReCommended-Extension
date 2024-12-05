using System;

namespace Test
{
    public class Strings
    {
        public void Contains(string text, object someObject)
        {
            var result1 = text.Contains("");
            var result2 = text.Contains("", StringComparison.OrdinalIgnoreCase);

            var result3 = $"{someObject}".Contains(value: "", StringComparison.OrdinalIgnoreCase);
        }

        public void EndsWith(string text, object someObject)
        {
            var result1 = text.EndsWith("");
            var result2 = text.EndsWith("", StringComparison.OrdinalIgnoreCase);

            var result3 = $"{someObject}".EndsWith(value: "", StringComparison.OrdinalIgnoreCase);
        }

        public void IndexOf(string text, object someObject)
        {
            var result1 = text.IndexOf("");
            var result2 = text.IndexOf("", StringComparison.OrdinalIgnoreCase);

            var result3 = $"{someObject}".IndexOf("", StringComparison.OrdinalIgnoreCase);
        }

        public void IndexOfAny(string text)
        {
            var result = text.IndexOfAny([]);
        }

        public void LastIndexOf(string text, char c)
        {
            var result = text.LastIndexOf(c, 0);
        }

        public void LastIndexOfAny(string text, char[] c)
        {
            var result1 = text.LastIndexOfAny([]);
            var result2 = text.LastIndexOfAny(c, 0);
            var result3 = text.LastIndexOfAny(c, 0, 0);
            var result4 = text.LastIndexOfAny(c, 0, 1);
        }

        public void Remove(string text)
        {
            var result = text.Remove(0);
        }

        public void Split(string text, string s, int count, StringSplitOptions options)
        {
            var result11 = text.Split('a', 0);
            var result12 = text.Split('a', 0, options);
            var result13 = text.Split('a', 1);
            var result14 = text.Split('a', 1, StringSplitOptions.None);
            var result15 = text.Split('a', 1, StringSplitOptions.TrimEntries);

            var result21 = text.Split(['a'], 0);
            var result22 = text.Split(['a'], 1);

            var result31 = text.Split(['a'], 0, options);
            var result32 = text.Split(['a'], 1, StringSplitOptions.None);
            var result33 = text.Split(['a'], 1, StringSplitOptions.TrimEntries);

            var result41 = text.Split(null as string);
            var result42 = text.Split((string?)null);
            var result43 = text.Split("");
            var result44 = text.Split(null as string, StringSplitOptions.None);
            var result45 = text.Split((string?)null, StringSplitOptions.None);
            var result46 = text.Split("", StringSplitOptions.None);
            var result47 = text.Split(null as string, StringSplitOptions.TrimEntries);
            var result48 = text.Split((string?)null, StringSplitOptions.TrimEntries);
            var result49 = text.Split("", StringSplitOptions.TrimEntries);

            var result51 = text.Split(s, 0);
            var result52 = text.Split(s, 0, options);
            var result53 = text.Split(s, 1);
            var result54 = text.Split(s, 1, StringSplitOptions.None);
            var result55 = text.Split(s, 1, StringSplitOptions.TrimEntries);
            var result56 = text.Split(null as string, count);
            var result57 = text.Split(null as string, count, StringSplitOptions.None);
            var result58 = text.Split(null as string, count, StringSplitOptions.TrimEntries);
            var result59 = text.Split("", count);
            var result50 = text.Split("", count, StringSplitOptions.None);
            var result5A = text.Split("", count, StringSplitOptions.TrimEntries);

            var result61 = text.Split([""], StringSplitOptions.None);
            var result62 = text.Split([""], StringSplitOptions.TrimEntries);
            var result63 = text.Split(new[] { "" }, StringSplitOptions.None);
            var result64 = text.Split(new[] { "" }, StringSplitOptions.TrimEntries);

            var result71 = text.Split([s], 0, options);
            var result72 = text.Split([s], 1, StringSplitOptions.None);
            var result73 = text.Split([s], 1, StringSplitOptions.TrimEntries);
            var result74 = text.Split([""], count, StringSplitOptions.None);
            var result75 = text.Split([""], count, StringSplitOptions.TrimEntries);
            var result76 = text.Split(new[] { "" }, count, StringSplitOptions.None);
            var result77 = text.Split(new[] { "" }, count, StringSplitOptions.TrimEntries);
        }

        public void StartsWith(string text, object someObject)
        {
            var result1 = text.StartsWith("");
            var result2 = text.StartsWith("", StringComparison.OrdinalIgnoreCase);

            var result3 = $"{someObject}".StartsWith(value: "", StringComparison.OrdinalIgnoreCase);
        }

        public void Join_Empty(string s, char c, string?[] stringItems, string stringItem)
        {
            var result11 = string.Join(s, (object?[])[]);
            var result12 = string.Join(s, new object?[0]);
            var result13 = string.Join(s, new object?[] { });
            var result14 = string.Join(s, Array.Empty<object?>());

            var result21 = string.Join(s, (int[])[]);
            var result22 = string.Join(s, new int[0]);
            var result23 = string.Join(s, new int[] { });
            var result24 = string.Join(s, Array.Empty<int>());

            var result31 = string.Join(s, (string?[])[]);
            var result32 = string.Join(s, new string?[0]);
            var result33 = string.Join(s, new string?[] { });
            var result34 = string.Join(s, Array.Empty<string?>());

            var result41 = string.Join(c, (object?[])[]);
            var result42 = string.Join(c, new object?[0]);
            var result43 = string.Join(c, new object?[] { });
            var result44 = string.Join(c, Array.Empty<object?>());

            var result51 = string.Join(c, (int[])[]);
            var result52 = string.Join(c, new int[0]);
            var result53 = string.Join(c, new int[] { });
            var result54 = string.Join(c, Array.Empty<int>());

            var result61 = string.Join(c, (string?[])[]);
            var result62 = string.Join(c, new string?[0]);
            var result63 = string.Join(c, new string?[] { });
            var result64 = string.Join(c, Array.Empty<string?>());

            var result71 = string.Join(s, default(ReadOnlySpan<object?>));
            var result72 = string.Join(s, new ReadOnlySpan<object?>());
            var result73 = string.Join(s, (ReadOnlySpan<object?>)[]);

            var result81 = string.Join(s, default(ReadOnlySpan<string?>));
            var result82 = string.Join(s, new ReadOnlySpan<string?>());
            var result83 = string.Join(s, (ReadOnlySpan<string?>)[]);

            var result91 = string.Join(c, default(ReadOnlySpan<object?>));
            var result92 = string.Join(c, new ReadOnlySpan<object?>());
            var result93 = string.Join(c, (ReadOnlySpan<object?>)[]);

            var resultA1 = string.Join(c, default(ReadOnlySpan<string?>));
            var resultA2 = string.Join(c, new ReadOnlySpan<string?>());
            var resultA3 = string.Join(c, (ReadOnlySpan<string?>)[]);

            var resultB1 = string.Join(s, stringItems, 0, 0);
            var resultB2 = string.Join(s, [stringItem], 1, 0);

            var resultC1 = string.Join(c, stringItems, 0, 0);
            var resultC2 = string.Join(c, [stringItem], 1, 0);
        }

        public void Join_Item(string s, char c, object objectItem, int intItem, string stringItem, string?[] stringItems)
        {
            var result11 = string.Join(s, (object?[])[objectItem]);
            var result12 = string.Join(s, new[] { objectItem });

            var result21 = string.Join(s, (int[])[intItem]);
            var result22 = string.Join(s, new[] { intItem });

            var result31 = string.Join(s, (string?[])[stringItem]);
            var result32 = string.Join(s, new[] { stringItem });

            var result41 = string.Join(c, (object?[])[objectItem]);
            var result43 = string.Join(c, new[] { objectItem });

            var result51 = string.Join(c, (int[])[intItem]);
            var result52 = string.Join(c, new[] { intItem });

            var result61 = string.Join(c, (string?[])[stringItem]);
            var result62 = string.Join(c, new[] { stringItem });

            var result71 = string.Join(s, [objectItem]);
            var result72 = string.Join(s, objectItem);

            var result81 = string.Join(s, (ReadOnlySpan<string?>)[stringItem]);
            var result82 = string.Join(s, stringItem);

            var result91 = string.Join(c, [objectItem]);
            var result92 = string.Join(c, objectItem);

            var resultA1 = string.Join(c, (ReadOnlySpan<string?>)[stringItem]);
            var resultA2 = string.Join(c, stringItem);

            var resultB1 = string.Join(s, [stringItem], 0, 1);

            var resultC1 = string.Join(c, [stringItem], 0, 1);
        }
    }
}