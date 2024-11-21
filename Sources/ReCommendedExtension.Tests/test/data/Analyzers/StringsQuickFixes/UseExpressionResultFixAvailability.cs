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

        public void LastIndexOf(string text, char c)
        {
            var result = text.LastIndexOf(c, 0);
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
    }
}