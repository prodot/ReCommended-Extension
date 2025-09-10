using System;
using System.Text;

namespace Test
{
    public class StringBuilders
    {
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
    }
}