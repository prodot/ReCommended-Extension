using System;
using System.Text;

namespace Test
{
    public class StringBuilders
    {
        public void RedundantInvocation(StringBuilder builder, string s, char c)
        {
            var result11 = builder.AppendJoin(s, (object?[])[]);
            var result12 = builder.AppendJoin(s, new object?[0]);
            var result13 = builder.AppendJoin(s, new object?[] { });
            var result14 = builder.AppendJoin(s, Array.Empty<object?>());

            var result21 = builder.AppendJoin(s, (int[])[]);
            var result22 = builder.AppendJoin(s, new int[0]);
            var result23 = builder.AppendJoin(s, new int[] { });
            var result24 = builder.AppendJoin(s, Array.Empty<int>());

            var result31 = builder.AppendJoin(s, (string?[])[]);
            var result32 = builder.AppendJoin(s, new string?[0]);
            var result33 = builder.AppendJoin(s, new string?[] { });
            var result34 = builder.AppendJoin(s, Array.Empty<string?>());

            var result41 = builder.AppendJoin(c, (object?[])[]);
            var result42 = builder.AppendJoin(c, new object?[0]);
            var result43 = builder.AppendJoin(c, new object?[] { });
            var result44 = builder.AppendJoin(c, Array.Empty<object?>());

            var result51 = builder.AppendJoin(c, (int[])[]);
            var result52 = builder.AppendJoin(c, new int[0]);
            var result53 = builder.AppendJoin(c, new int[] { });
            var result54 = builder.AppendJoin(c, Array.Empty<int>());

            var result61 = builder.AppendJoin(c, (string?[])[]);
            var result62 = builder.AppendJoin(c, new string?[0]);
            var result63 = builder.AppendJoin(c, new string?[] { });
            var result64 = builder.AppendJoin(c, Array.Empty<string?>());

            var result71 = builder.AppendJoin(s, default(ReadOnlySpan<object?>));
            var result72 = builder.AppendJoin(s, new ReadOnlySpan<object?>());

            var result81 = builder.AppendJoin(s, default(ReadOnlySpan<string?>));
            var result82 = builder.AppendJoin(s, new ReadOnlySpan<string?>());

            var result91 = builder.AppendJoin(c, default(ReadOnlySpan<object?>));
            var result92 = builder.AppendJoin(c, new ReadOnlySpan<object?>());

            var resultA1 = builder.AppendJoin(c, default(ReadOnlySpan<string?>));
            var resultA2 = builder.AppendJoin(c, new ReadOnlySpan<string?>());

            builder.AppendJoin(s, (object?[])[]);
            builder.AppendJoin(s, new object?[0]);
            builder.AppendJoin(s, new object?[] { });
            builder.AppendJoin(s, Array.Empty<object?>());

            builder.AppendJoin(s, (int[])[]);
            builder.AppendJoin(s, new int[0]);
            builder.AppendJoin(s, new int[] { });
            builder.AppendJoin(s, Array.Empty<int>());

            builder.AppendJoin(s, (string?[])[]);
            builder.AppendJoin(s, new string?[0]);
            builder.AppendJoin(s, new string?[] { });
            builder.AppendJoin(s, Array.Empty<string?>());

            builder.AppendJoin(c, (object?[])[]);
            builder.AppendJoin(c, new object?[0]);
            builder.AppendJoin(c, new object?[] { });
            builder.AppendJoin(c, Array.Empty<object?>());

            builder.AppendJoin(c, (int[])[]);
            builder.AppendJoin(c, new int[0]);
            builder.AppendJoin(c, new int[] { });
            builder.AppendJoin(c, Array.Empty<int>());

            builder.AppendJoin(c, (string?[])[]);
            builder.AppendJoin(c, new string?[0]);
            builder.AppendJoin(c, new string?[] { });
            builder.AppendJoin(c, Array.Empty<string?>());

            builder.AppendJoin(s, default(ReadOnlySpan<object?>));
            builder.AppendJoin(s, new ReadOnlySpan<object?>());

            builder.AppendJoin(s, default(ReadOnlySpan<string?>));
            builder.AppendJoin(s, new ReadOnlySpan<string?>());

            builder.AppendJoin(c, default(ReadOnlySpan<object?>));
            builder.AppendJoin(c, new ReadOnlySpan<object?>());

            builder.AppendJoin(c, default(ReadOnlySpan<string?>));
            builder.AppendJoin(c, new ReadOnlySpan<string?>());
        }

        public void RedundantInvocation_Nullable(StringBuilder? builder, string s, char c)
        {
            var result11 = builder?.AppendJoin(s, (object?[])[]);
            var result12 = builder?.AppendJoin(s, new object?[0]);
            var result13 = builder?.AppendJoin(s, new object?[] { });
            var result14 = builder?.AppendJoin(s, Array.Empty<object?>());

            var result21 = builder?.AppendJoin(s, (int[])[]);
            var result22 = builder?.AppendJoin(s, new int[0]);
            var result23 = builder?.AppendJoin(s, new int[] { });
            var result24 = builder?.AppendJoin(s, Array.Empty<int>());

            var result31 = builder?.AppendJoin(s, (string?[])[]);
            var result32 = builder?.AppendJoin(s, new string?[0]);
            var result33 = builder?.AppendJoin(s, new string?[] { });
            var result34 = builder?.AppendJoin(s, Array.Empty<string?>());

            var result41 = builder?.AppendJoin(c, (object?[])[]);
            var result42 = builder?.AppendJoin(c, new object?[0]);
            var result43 = builder?.AppendJoin(c, new object?[] { });
            var result44 = builder?.AppendJoin(c, Array.Empty<object?>());

            var result51 = builder?.AppendJoin(c, (int[])[]);
            var result52 = builder?.AppendJoin(c, new int[0]);
            var result53 = builder?.AppendJoin(c, new int[] { });
            var result54 = builder?.AppendJoin(c, Array.Empty<int>());

            var result61 = builder?.AppendJoin(c, (string?[])[]);
            var result62 = builder?.AppendJoin(c, new string?[0]);
            var result63 = builder?.AppendJoin(c, new string?[] { });
            var result64 = builder?.AppendJoin(c, Array.Empty<string?>());

            var result71 = builder?.AppendJoin(s, default(ReadOnlySpan<object?>));
            var result72 = builder?.AppendJoin(s, new ReadOnlySpan<object?>());

            var result81 = builder?.AppendJoin(s, default(ReadOnlySpan<string?>));
            var result82 = builder?.AppendJoin(s, new ReadOnlySpan<string?>());

            var result91 = builder?.AppendJoin(c, default(ReadOnlySpan<object?>));
            var result92 = builder?.AppendJoin(c, new ReadOnlySpan<object?>());

            var resultA1 = builder?.AppendJoin(c, default(ReadOnlySpan<string?>));
            var resultA2 = builder?.AppendJoin(c, new ReadOnlySpan<string?>());

            builder?.AppendJoin(s, (object?[])[]);
            builder?.AppendJoin(s, new object?[0]);
            builder?.AppendJoin(s, new object?[] { });
            builder?.AppendJoin(s, Array.Empty<object?>());

            builder?.AppendJoin(s, (int[])[]);
            builder?.AppendJoin(s, new int[0]);
            builder?.AppendJoin(s, new int[] { });
            builder?.AppendJoin(s, Array.Empty<int>());

            builder?.AppendJoin(s, (string?[])[]);
            builder?.AppendJoin(s, new string?[0]);
            builder?.AppendJoin(s, new string?[] { });
            builder?.AppendJoin(s, Array.Empty<string?>());

            builder?.AppendJoin(c, (object?[])[]);
            builder?.AppendJoin(c, new object?[0]);
            builder?.AppendJoin(c, new object?[] { });
            builder?.AppendJoin(c, Array.Empty<object?>());

            builder?.AppendJoin(c, (int[])[]);
            builder?.AppendJoin(c, new int[0]);
            builder?.AppendJoin(c, new int[] { });
            builder?.AppendJoin(c, Array.Empty<int>());

            builder?.AppendJoin(c, (string?[])[]);
            builder?.AppendJoin(c, new string?[0]);
            builder?.AppendJoin(c, new string?[] { });
            builder?.AppendJoin(c, Array.Empty<string?>());

            builder?.AppendJoin(s, default(ReadOnlySpan<object?>));
            builder?.AppendJoin(s, new ReadOnlySpan<object?>());

            builder?.AppendJoin(s, default(ReadOnlySpan<string?>));
            builder?.AppendJoin(s, new ReadOnlySpan<string?>());

            builder?.AppendJoin(c, default(ReadOnlySpan<object?>));
            builder?.AppendJoin(c, new ReadOnlySpan<object?>());

            builder?.AppendJoin(c, default(ReadOnlySpan<string?>));
            builder?.AppendJoin(c, new ReadOnlySpan<string?>());
        }

        public void OtherMethod(StringBuilder builder, string s, char c, object objectItem, int intItem, string stringItem)
        {
            var result11 = builder.AppendJoin(s, (object?[])[objectItem]);
            var result12 = builder.AppendJoin(s, new[] { objectItem });

            var result21 = builder.AppendJoin(s, (int[])[intItem]);
            var result22 = builder.AppendJoin(s, new[] { intItem });

            var result31 = builder.AppendJoin(s, (string?[])[stringItem]);
            var result32 = builder.AppendJoin(s, new[] { stringItem });

            var result41 = builder.AppendJoin(c, (object?[])[objectItem]);
            var result43 = builder.AppendJoin(c, new[] { objectItem });

            var result51 = builder.AppendJoin(c, (int[])[intItem]);
            var result52 = builder.AppendJoin(c, new[] { intItem });

            var result61 = builder.AppendJoin(c, (string?[])[stringItem]);
            var result62 = builder.AppendJoin(c, new[] { stringItem });

            var result71 = builder.AppendJoin(s, [(object)"item"]);
            var result72 = builder.AppendJoin(s, (object)"item");

            var result81 = builder.AppendJoin(s, (ReadOnlySpan<string?>)["item"]);
            var result82 = builder.AppendJoin(s, "item");

            var result91 = builder.AppendJoin(c, [(object)"item"]);
            var result92 = builder.AppendJoin(c, (object)"item");

            var resultA1 = builder.AppendJoin(c, (ReadOnlySpan<string?>)["item"]);
            var resultA2 = builder.AppendJoin(c, "item");

            builder.AppendJoin(s, (object?[])[objectItem]);
            builder.AppendJoin(s, new[] { objectItem });

            builder.AppendJoin(s, (int[])[intItem]);
            builder.AppendJoin(s, new[] { intItem });

            builder.AppendJoin(s, (string?[])[stringItem]);
            builder.AppendJoin(s, new[] { stringItem });

            builder.AppendJoin(c, (object?[])[objectItem]);
            builder.AppendJoin(c, new[] { objectItem });

            builder.AppendJoin(c, (int[])[intItem]);
            builder.AppendJoin(c, new[] { intItem });

            builder.AppendJoin(c, (string?[])[stringItem]);
            builder.AppendJoin(c, new[] { stringItem });

            builder.AppendJoin(s, [(object)"item"]);
            builder.AppendJoin(s, (object)"item");

            builder.AppendJoin(s, (ReadOnlySpan<string?>)["item"]);
            builder.AppendJoin(s, "item");

            builder.AppendJoin(c, [(object)"item"]);
            builder.AppendJoin(c, (object)"item");

            builder.AppendJoin(c, (ReadOnlySpan<string?>)["item"]);
            builder.AppendJoin(c, "item");
        }

        public void OtherMethod_Nullable(StringBuilder? builder, string s, char c, object objectItem, int intItem, string stringItem)
        {
            var result11 = builder?.AppendJoin(s, (object?[])[objectItem]);
            var result12 = builder?.AppendJoin(s, new[] { objectItem });

            var result21 = builder?.AppendJoin(s, (int[])[intItem]);
            var result22 = builder?.AppendJoin(s, new[] { intItem });

            var result31 = builder?.AppendJoin(s, (string?[])[stringItem]);
            var result32 = builder?.AppendJoin(s, new[] { stringItem });

            var result41 = builder?.AppendJoin(c, (object?[])[objectItem]);
            var result43 = builder?.AppendJoin(c, new[] { objectItem });

            var result51 = builder?.AppendJoin(c, (int[])[intItem]);
            var result52 = builder?.AppendJoin(c, new[] { intItem });

            var result61 = builder?.AppendJoin(c, (string?[])[stringItem]);
            var result62 = builder?.AppendJoin(c, new[] { stringItem });

            var result71 = builder?.AppendJoin(s, [(object)"item"]);
            var result72 = builder?.AppendJoin(s, (object)"item");

            var result81 = builder?.AppendJoin(s, (ReadOnlySpan<string?>)["item"]);
            var result82 = builder?.AppendJoin(s, "item");

            var result91 = builder?.AppendJoin(c, [(object)"item"]);
            var result92 = builder?.AppendJoin(c, (object)"item");

            var resultA1 = builder?.AppendJoin(c, (ReadOnlySpan<string?>)["item"]);
            var resultA2 = builder?.AppendJoin(c, "item");

            builder?.AppendJoin(s, (object?[])[objectItem]);
            builder?.AppendJoin(s, new[] { objectItem });

            builder?.AppendJoin(s, (int[])[intItem]);
            builder?.AppendJoin(s, new[] { intItem });

            builder?.AppendJoin(s, (string?[])[stringItem]);
            builder?.AppendJoin(s, new[] { stringItem });

            builder?.AppendJoin(c, (object?[])[objectItem]);
            builder?.AppendJoin(c, new[] { objectItem });

            builder?.AppendJoin(c, (int[])[intItem]);
            builder?.AppendJoin(c, new[] { intItem });

            builder?.AppendJoin(c, (string?[])[stringItem]);
            builder?.AppendJoin(c, new[] { stringItem });

            builder?.AppendJoin(s, [(object)"item"]);
            builder?.AppendJoin(s, (object)"item");

            builder?.AppendJoin(s, (ReadOnlySpan<string?>)["item"]);
            builder?.AppendJoin(s, "item");

            builder?.AppendJoin(c, [(object)"item"]);
            builder?.AppendJoin(c, (object)"item");

            builder?.AppendJoin(c, (ReadOnlySpan<string?>)["item"]);
            builder?.AppendJoin(c, "item");
        }

        public void SingleCharacter(StringBuilder builder, object?[] objectItems, int[] intItems, string?[] stringItems, ReadOnlySpan<object?> spanOfObjects, ReadOnlySpan<string?> spanOfStrings)
        {
            var result1 = builder.AppendJoin(",", objectItems);
            var result2 = builder.AppendJoin(",", intItems);
            var result3 = builder.AppendJoin(",", stringItems);
            var result4 = builder.AppendJoin(",", spanOfObjects);
            var result5 = builder.AppendJoin(",", spanOfStrings);

            builder.AppendJoin(",", objectItems);
            builder.AppendJoin(",", intItems);
            builder.AppendJoin(",", stringItems);
            builder.AppendJoin(",", spanOfObjects);
            builder.AppendJoin(",", spanOfStrings);
        }

        public void SingleCharacter_Nullable(StringBuilder? builder, object?[] objectItems, int[] intItems, string?[] stringItems, ReadOnlySpan<object?> spanOfObjects, ReadOnlySpan<string?> spanOfStrings)
        {
            var result1 = builder?.AppendJoin(",", objectItems);
            var result2 = builder?.AppendJoin(",", intItems);
            var result3 = builder?.AppendJoin(",", stringItems);
            var result4 = builder?.AppendJoin(",", spanOfObjects);
            var result5 = builder?.AppendJoin(",", spanOfStrings);

            builder?.AppendJoin(",", objectItems);
            builder?.AppendJoin(",", intItems);
            builder?.AppendJoin(",", stringItems);
            builder?.AppendJoin(",", spanOfObjects);
            builder?.AppendJoin(",", spanOfStrings);
        }

        public void NoDetection(StringBuilder builder, string s, char c, object objectItem, object?[] objectItems, int intItem, int[] intItems, string stringItem, string?[] stringItems, ReadOnlySpan<object?> spanOfObjects, ReadOnlySpan<string?> spanOfStrings)
        {
            var result11 = builder.AppendJoin(s, (object?[])[objectItem, objectItem]);
            var result12 = builder.AppendJoin(s, new[] { objectItem, objectItem });
            var result13 = builder.AppendJoin(s, objectItems);

            var result21 = builder.AppendJoin(s, (int[])[intItem, intItem]);
            var result22 = builder.AppendJoin(s, new[] { intItem, intItem });
            var result23 = builder.AppendJoin(s, intItems);

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

            var result71 = builder.AppendJoin(s, objectItem, objectItem);
            var result72 = builder.AppendJoin(s, [objectItem, objectItem]);
            var result73 = builder.AppendJoin(s, spanOfObjects);

            var result81 = builder.AppendJoin(s, stringItem, stringItem);
            var result82 = builder.AppendJoin(s, (ReadOnlySpan<string?>)[stringItem, stringItem]);
            var result83 = builder.AppendJoin(s, spanOfStrings);

            var result91 = builder.AppendJoin(c, objectItem, objectItem);
            var result92 = builder.AppendJoin(c, [objectItem, objectItem]);
            var result93 = builder.AppendJoin(c, spanOfObjects);

            var resultA1 = builder.AppendJoin(c, stringItem, stringItem);
            var resultA2 = builder.AppendJoin(c, (ReadOnlySpan<string?>)[stringItem, stringItem]);
            var resultA3 = builder.AppendJoin(c, spanOfStrings);
        }
    }
}