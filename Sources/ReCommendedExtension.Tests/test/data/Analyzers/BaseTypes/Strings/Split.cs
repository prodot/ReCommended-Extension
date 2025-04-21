using System;

namespace Test
{
    public class Strings
    {
        public void ExpressionResult(string text, string s, int count, StringSplitOptions options)
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

        public void DuplicateItems(string text, int count, StringSplitOptions options)
        {
            var result11 = text.Split('a', 'b', 'a');
            var result12 = text.Split(['a', 'b', 'a']);
            var result13 = text.Split(new[] { 'a', 'b', 'a' });

            var result21 = text.Split(['a', 'b', 'a'], count);
            var result22 = text.Split(new[] { 'a', 'b', 'a' }, count);

            var result31 = text.Split(['a', 'b', 'a'], options);
            var result32 = text.Split(new[] { 'a', 'b', 'a' }, options);

            var result41 = text.Split(['a', 'b', 'a'], count, options);
            var result42 = text.Split(new[] { 'a', 'b', 'a' }, count, options);

            var result51 = text.Split(["aa", "bb", "aa"], options);
            var result52 = text.Split(new[] { "aa", "bb", "aa" }, options);

            var result61 = text.Split(["aa", "bb", "aa"], count, options);
            var result62 = text.Split(new[] { "aa", "bb", "aa" }, count, options);
        }

        public void PassSingleCharacter(string text, int count, StringSplitOptions options)
        {
            var result11 = text.Split("a");
            var result12 = text.Split("a", options);

            var result21 = text.Split("a", count);
            var result22 = text.Split("a", count, options);

            var result31 = text.Split(["a", "a"], options);
            var result32 = text.Split(new[] { "a", "a" }, options);

            var result41 = text.Split(["a", "a"], count, options);
            var result42 = text.Split(new[] { "a", "a" }, count, options);
        }

        public void NoDetection(string text, int count, StringSplitOptions options, char c, string s)
        {
            var result11 = text.Split('a', count);
            var result12 = text.Split('a', 2);
            var result13 = text.Split('a', 1, StringSplitOptions.RemoveEmptyEntries);
            var result14 = text.Split('a', 1, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            var result15 = text.Split('a', 1, options);

            var result21 = text.Split(c, 'b', c);
            var result22 = text.Split([c, 'b', c]);
            var result23 = text.Split(new[] { c, 'b', c });

            var result31 = text.Split();
            var result32 = text.Split([]);
            var result33 = text.Split(new char[] { });
            var result34 = text.Split(new char[0]);
            var result35 = text.Split(Array.Empty<char>());

            var result41 = text.Split('a');
            var result42 = text.Split(['a']);
            var result43 = text.Split(new[] { 'a' });

            var result51 = text.Split('a', 'b', 'c');
            var result52 = text.Split(['a', 'b', 'c']);
            var result53 = text.Split(new[] { 'a', 'b', 'c' });

            var result61 = text.Split(['a'], 2);
            var result62 = text.Split(['a'], count);
            var result63 = text.Split(['a', 'b', 'c'], count);
            var result64 = text.Split(new[] { 'a', 'b', 'c' }, count);

            var result71 = text.Split(['a', 'b', 'c'], options);
            var result72 = text.Split(new[] { 'a', 'b', 'c' }, options);

            var result81 = text.Split(['a'], count, options);
            var result82 = text.Split(['a'], 1, StringSplitOptions.RemoveEmptyEntries);
            var result83 = text.Split(['a'], 1, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            var result84 = text.Split(['a'], 1, options);

            var result91 = text.Split(null as string, options);
            var result92 = text.Split((string?)null, options);
            var result93 = text.Split("", options);
            var result94 = text.Split("aa");
            var result95 = text.Split("aa", options);

            var result54 = text.Split(s, 1, options);
            var result55 = text.Split("aa", count, options);

            var resultA1 = text.Split(new string[] { }, options);
            var resultA2 = text.Split(["a", "bb"], options);
            var resultA3 = text.Split(["a", s], options);

            var resultB1 = text.Split([s], 1, options);
            var resultB2 = text.Split([""], count, options);
            var resultB3 = text.Split(["aa", "bb"], count, options);
            var resultB4 = text.Split(["a", "bb"], count, options);

            text.Split('a', 0);
            text.Split('a', 0, StringSplitOptions.RemoveEmptyEntries);
            text.Split('a', 1);
            text.Split('a', 1, StringSplitOptions.None);
            text.Split('a', 1, StringSplitOptions.TrimEntries);

            text.Split(['a'], 0);
            text.Split(['a'], 1);

            text.Split(['a'], 0, options);
            text.Split(['a'], 1, StringSplitOptions.None);
            text.Split(['a'], 1, StringSplitOptions.TrimEntries);

            text.Split(null as string);
            text.Split((string?)null);
            text.Split("");
            text.Split(null as string, StringSplitOptions.None);
            text.Split((string?)null, StringSplitOptions.None);
            text.Split("", StringSplitOptions.None);
            text.Split(null as string, StringSplitOptions.TrimEntries);
            text.Split((string?)null, StringSplitOptions.TrimEntries);
            text.Split("", StringSplitOptions.TrimEntries);

            text.Split(s, 0);
            text.Split(s, 0, options);
            text.Split(s, 1);
            text.Split(s, 1, StringSplitOptions.None);
            text.Split(s, 1, StringSplitOptions.TrimEntries);
            text.Split(null as string, count);
            text.Split(null as string, count, StringSplitOptions.None);
            text.Split(null as string, count, StringSplitOptions.TrimEntries);
            text.Split("", count);
            text.Split("", count, StringSplitOptions.None);
            text.Split("", count, StringSplitOptions.TrimEntries);

            text.Split([""], StringSplitOptions.None);
            text.Split([""], StringSplitOptions.TrimEntries);
            text.Split(new[] { "" }, StringSplitOptions.None);
            text.Split(new[] { "" }, StringSplitOptions.TrimEntries);

            text.Split([s], 0, options);
            text.Split([s], 1, StringSplitOptions.None);
            text.Split([s], 1, StringSplitOptions.TrimEntries);
            text.Split([""], count, StringSplitOptions.None);
            text.Split([""], count, StringSplitOptions.TrimEntries);
            text.Split(new[] { "" }, count, StringSplitOptions.None);
            text.Split(new[] { "" }, count, StringSplitOptions.TrimEntries);
        }

        public void NoDetection(string? text, StringSplitOptions options, string s, char c, int count)
        {
            var result11 = text?.Split(c, 0);
            var result12 = text?.Split(c, 0, options);
            var result13 = text?.Split(c, 1);
            var result14 = text?.Split(c, 1, StringSplitOptions.None);
            var result15 = text?.Split(c, 1, StringSplitOptions.TrimEntries);

            var result21 = text?.Split(['a'], 0);
            var result22 = text?.Split(['a'], 1);

            var result31 = text?.Split(['a'], 0, options);
            var result32 = text?.Split(['a'], 1, StringSplitOptions.None);
            var result33 = text?.Split(['a'], 1, StringSplitOptions.TrimEntries);

            var result41 = text?.Split(null as string);
            var result42 = text?.Split((string?)null);
            var result43 = text?.Split("");
            var result44 = text?.Split(null as string, StringSplitOptions.None);
            var result45 = text?.Split((string?)null, StringSplitOptions.None);
            var result46 = text?.Split("", StringSplitOptions.None);
            var result47 = text?.Split(null as string, StringSplitOptions.TrimEntries);
            var result48 = text?.Split((string?)null, StringSplitOptions.TrimEntries);
            var result49 = text?.Split("", StringSplitOptions.TrimEntries);

            var result51 = text?.Split(s, 0);
            var result52 = text?.Split(s, 0, options);
            var result53 = text?.Split(s, 1);
            var result54 = text?.Split(s, 1, StringSplitOptions.None);
            var result55 = text?.Split(s, 1, StringSplitOptions.TrimEntries);
            var result56 = text?.Split(null as string, count);
            var result57 = text?.Split(null as string, count, StringSplitOptions.None);
            var result58 = text?.Split(null as string, count, StringSplitOptions.TrimEntries);
            var result59 = text?.Split("", count);
            var result50 = text?.Split("", count, StringSplitOptions.None);
            var result5A = text?.Split("", count, StringSplitOptions.TrimEntries);

            var result61 = text?.Split([""], StringSplitOptions.None);
            var result62 = text?.Split([""], StringSplitOptions.TrimEntries);
            var result63 = text?.Split(new[] { "" }, StringSplitOptions.None);
            var result64 = text?.Split(new[] { "" }, StringSplitOptions.TrimEntries);

            var result71 = text?.Split([s], 0, options);
            var result72 = text?.Split([s], 1, StringSplitOptions.None);
            var result73 = text?.Split([s], 1, StringSplitOptions.TrimEntries);
            var result74 = text?.Split([""], count, StringSplitOptions.None);
            var result75 = text?.Split([""], count, StringSplitOptions.TrimEntries);
            var result76 = text?.Split(new[] { "" }, count, StringSplitOptions.None);
            var result77 = text?.Split(new[] { "" }, count, StringSplitOptions.TrimEntries);
        }
    }
}