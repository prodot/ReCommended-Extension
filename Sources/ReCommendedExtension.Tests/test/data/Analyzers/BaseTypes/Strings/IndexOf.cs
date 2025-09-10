using System;

namespace Test
{
    public class Strings
    {
        public void Comparison(string text, char c, string s, StringComparison comparisonType)
        {
            var result11 = text.IndexOf(c) == 0;
            var result12 = text.IndexOf('a') == 0;

            var result21 = text.IndexOf(c) != 0;
            var result22 = text.IndexOf('a') != 0;
            var result23 = text.IndexOf(c) > -1;
            var result24 = text.IndexOf(c) != -1;
            var result25 = text.IndexOf(c) >= 0;

            var result31 = text.IndexOf(c) == -1;
            var result32 = text.IndexOf(c) < 0;

            var result41 = text.IndexOf(c, comparisonType) > -1;
            var result42 = text.IndexOf(c, comparisonType) != -1;
            var result43 = text.IndexOf(c, comparisonType) >= 0;
            var result44 = text.IndexOf(c, comparisonType) == -1;
            var result45 = text.IndexOf(c, comparisonType) < 0;

            var result51 = text.IndexOf(s) == 0;
            var result52 = text.IndexOf(s) != 0;

            var result61 = text.IndexOf(s) > -1;
            var result62 = text.IndexOf(s) != -1;
            var result63 = text.IndexOf(s) >= 0;
            var result64 = text.IndexOf(s) == -1;
            var result65 = text.IndexOf(s) < 0;

            var result71 = text.IndexOf(s, comparisonType) == 0;
            var result72 = text.IndexOf(s, comparisonType) != 0;

            var result81 = text.IndexOf(s, comparisonType) > -1;
            var result82 = text.IndexOf(s, comparisonType) != -1;
            var result83 = text.IndexOf(s, comparisonType) >= 0;
            var result84 = text.IndexOf(s, comparisonType) == -1;
            var result85 = text.IndexOf(s, comparisonType) < 0;
        }

        public void RedundantArguments(string text, char c, string s)
        {
            var result1 = text.IndexOf(c, 0);
            var result2 = text.IndexOf(c, startIndex: 0);

            var result3 = text.IndexOf(s, 0);
            var result4 = text.IndexOf(s, startIndex: 0);

            var result5 = text.IndexOf(s, 0, StringComparison.CurrentCulture);
            var result6 = text.IndexOf(s, startIndex: 0, StringComparison.CurrentCulture);
        }

        public void ExpressionResult(string text)
        {
            var result1 = text.IndexOf("");
            var result2 = text.IndexOf("", StringComparison.CurrentCulture);
        }

        public void AsCharacter(string text)
        {
            var result1 = text.IndexOf("a");
            var result2 = text.IndexOf("a", StringComparison.OrdinalIgnoreCase);
        }

        public void NoDetection(string text, char c, int startIndex)
        {
            var result1 = text.IndexOf(c, 1);
            var result2 = text.IndexOf(c, startIndex);

            text.IndexOf("");
            text.IndexOf("", StringComparison.CurrentCulture);
        }

        public void NoDetection(string? text, StringComparison comparisonType, char c, string s)
        {
            var result21 = text?.IndexOf(c) > -1;
            var result22 = text?.IndexOf(c) != -1;
            var result23 = text?.IndexOf(c) >= 0;
            var result24 = text?.IndexOf(s) == -1;
            var result25 = text?.IndexOf(s) < 0;

            var result41 = text?.IndexOf(c, comparisonType) > -1;
            var result42 = text?.IndexOf(c, comparisonType) != -1;
            var result43 = text?.IndexOf(c, comparisonType) >= 0;
            var result44 = text?.IndexOf(c, comparisonType) == -1;
            var result45 = text?.IndexOf(c, comparisonType) < 0;

            var result11 = text?.IndexOf("");
            var result12 = text?.IndexOf("", comparisonType);

            var result51 = text?.IndexOf(s) == 0;
            var result52 = text?.IndexOf(s) != 0;

            var result61 = text?.IndexOf(s) > -1;
            var result62 = text?.IndexOf(s) != -1;
            var result63 = text?.IndexOf(s) >= 0;
            var result64 = text?.IndexOf(s) == -1;
            var result65 = text?.IndexOf(s) < 0;

            var result71 = text?.IndexOf(s, comparisonType) == 0;
            var result72 = text?.IndexOf(s, comparisonType) != 0;

            var result81 = text?.IndexOf(s, comparisonType) > -1;
            var result82 = text?.IndexOf(s, comparisonType) != -1;
            var result83 = text?.IndexOf(s, comparisonType) >= 0;
            var result84 = text?.IndexOf(s, comparisonType) == -1;
            var result85 = text?.IndexOf(s, comparisonType) < 0;
        }
    }
}