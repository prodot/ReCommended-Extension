using System;

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
            var result62 = text.Trim([]);
            var result63 = text.Trim('.', '.', '.');

            var result71 = text.TrimEnd(null);
            var result72 = text.TrimEnd([]);
            var result73 = text.TrimEnd('.', '.', '.');

            var result81 = text.TrimStart(null);
            var result82 = text.TrimStart([]);
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

            var result31 = text.Split([';', ';']);
            var result32 = text.Split([';', ';'], count);
            var result33 = text.Split([';', ';'], options);
            var result34 = text.Split([';', ';'], count, options);
            var result35 = text.Split([";", ";"], options);
            var result36 = text.Split([";", ";"], count, options);

            var result41 = text.Trim(['.', '.', '.']);

            var result51 = text.TrimEnd(['.', '.', '.']);

            var result61 = text.TrimStart(['.', '.', '.']);
        }

        public void NoDetection(string text, char c, char[] charArray, string s, int startIndex, int count, StringComparison comparison, int totalWidth, StringSplitOptions options)
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
        }
    }
}