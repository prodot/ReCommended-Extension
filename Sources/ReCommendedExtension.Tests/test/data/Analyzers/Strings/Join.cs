using System;

namespace Test
{
    public class StringBuilders
    {
        public void ExpressionResult_Empty(string s, char c, string?[] stringItems, string stringItem)
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

        public void ExpressionResult_Item(string s, char c, object objectItem, int intItem, string stringItem, string?[] stringItems)
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

        public void SingleCharacter(object?[] objectItems, int[] intItems, string?[] stringItems, ReadOnlySpan<object?> spanOfObjects, ReadOnlySpan<string?> spanOfStrings, int startIndex, int count)
        {
            var result1 = string.Join(",", objectItems);
            var result2 = string.Join(",", intItems);
            var result3 = string.Join(",", stringItems);
            var result4 = string.Join(",", spanOfObjects);
            var result5 = string.Join(",", spanOfStrings);
            var result6 = string.Join(",", stringItems, startIndex, count);
        }

        public void NoDetection(StringBuilder builder, string s, char c, object objectItem, object?[] objectItems, int intItem, int[] intItems, string stringItem, string?[] stringItems, ReadOnlySpan<object?> spanOfObjects, ReadOnlySpan<string?> spanOfStrings)
        {
            var result11 = string.Join(s, (object?[])[objectItem, objectItem]);
            var result12 = string.Join(s, new[] { objectItem, objectItem });
            var result13 = string.Join(s, objectItems);

            var result21 = string.Join(s, (int[])[intItem, intItem]);
            var result22 = string.Join(s, new[] { intItem, intItem });
            var result23 = string.Join(s, intItems);

            var result31 = builder.AppendJoin(s, (string?[])[stringItem, stringItem]);
            var result32 = builder.AppendJoin(s, new[] { stringItem, stringItem });
            var result33 = builder.AppendJoin(s, stringItems);

            var result41 = builder.AppendJoin(c, (object?[])[objectItem, objectItem]);
            var result42 = builder.AppendJoin(c, new[] { objectItem, objectItem });
            var result43 = builder.AppendJoin(c, objectItems);

            var result51 = builder.AppendJoin(c, (int[])[intItem, intItem]);
            var result52 = builder.AppendJoin(c, new[] { intItem, intItem });
            var result53 = builder.AppendJoin(c, intItems);

            var result61 = builder.AppendJoin(c, (string?[])[stringItem, stringItem]);
            var result62 = builder.AppendJoin(c, new[] { stringItem, stringItem });
            var result63 = builder.AppendJoin(c, stringItems);

            var result71 = string.Join(s, objectItem, objectItem);
            var result72 = string.Join(s, [objectItem, objectItem]);
            var result73 = string.Join(s, spanOfObjects);

            var result81 = builder.AppendJoin(s, stringItem, stringItem);
            var result82 = builder.AppendJoin(s, (ReadOnlySpan<string?>)[stringItem, stringItem]);
            var result83 = builder.AppendJoin(s, spanOfStrings);

            var result91 = builder.AppendJoin(c, objectItem, objectItem);
            var result92 = builder.AppendJoin(c, [objectItem, objectItem]);
            var result93 = builder.AppendJoin(c, spanOfObjects);

            var resultA1 = builder.AppendJoin(c, stringItem, stringItem);
            var resultA2 = builder.AppendJoin(c, (ReadOnlySpan<string?>)[stringItem, stringItem]);
            var resultA3 = builder.AppendJoin(c, spanOfStrings);

            string.Join(s, (object?[])[]);
            string.Join(s, new object?[0]);
            string.Join(s, new object?[] { });
            string.Join(s, Array.Empty<object?>());
            string.Join(s, (object?[])[objectItem]);
            string.Join(s, new[] { objectItem });

            string.Join(s, default(ReadOnlySpan<object?>));
            string.Join(s, new ReadOnlySpan<object?>());
            string.Join(s, (ReadOnlySpan<object?>)[]);
            string.Join(s, [objectItem]);
            string.Join(s, objectItem);

            string.Join(s, (int[])[]);
            string.Join(s, new int[0]);
            string.Join(s, new int[] { });
            string.Join(s, Array.Empty<int>());
            string.Join(s, (int[])[intItem]);
            string.Join(s, new[] { intItem });

            string.Join(s, (string?[])[]);
            string.Join(s, new string?[0]);
            string.Join(s, new string?[] { });
            string.Join(s, Array.Empty<string?>());
            string.Join(s, (string?[])[stringItem]);
            string.Join(s, new[] { stringItem });

            string.Join(s, stringItems, 0, 0);
            string.Join(s, [stringItem], 1, 0);
            string.Join(s, [stringItem], 0, 1);

            string.Join(s, default(ReadOnlySpan<string?>));
            string.Join(s, new ReadOnlySpan<string?>());
            string.Join(s, (ReadOnlySpan<string?>)[]);
            string.Join(s, (ReadOnlySpan<string?>)[stringItem]);
            string.Join(s, stringItem);

            string.Join(c, (object?[])[]);
            string.Join(c, new object?[0]);
            string.Join(c, new object?[] { });
            string.Join(c, Array.Empty<object?>());
            string.Join(c, (object?[])[objectItem]);
            string.Join(c, new[] { objectItem });

            string.Join(c, default(ReadOnlySpan<object?>));
            string.Join(c, new ReadOnlySpan<object?>());
            string.Join(c, (ReadOnlySpan<object?>)[]);
            string.Join(c, [objectItem]);
            string.Join(c, objectItem);

            string.Join(c, (int[])[]);
            string.Join(c, new int[0]);
            string.Join(c, new int[] { });
            string.Join(c, Array.Empty<int>());
            string.Join(c, (int[])[intItem]);
            string.Join(c, new[] { intItem });

            string.Join(c, (string?[])[]);
            string.Join(c, new string?[0]);
            string.Join(c, new string?[] { });
            string.Join(c, Array.Empty<string?>());
            string.Join(c, (string?[])[stringItem]);
            string.Join(c, new[] { stringItem });

            string.Join(c, stringItems, 0, 0);
            string.Join(c, [stringItem], 1, 0);
            string.Join(c, [stringItem], 0, 1);

            string.Join(c, default(ReadOnlySpan<string?>));
            string.Join(c, new ReadOnlySpan<string?>());
            string.Join(c, (ReadOnlySpan<string?>)[]);
            string.Join(c, (ReadOnlySpan<string?>)[stringItem]);
            string.Join(c, stringItem);
        }
    }
}