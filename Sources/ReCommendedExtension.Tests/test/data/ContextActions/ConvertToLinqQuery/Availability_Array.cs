using System.Collections.Generic;
using System.Linq;

namespace Test
{
    public class ConvertToLinqQuery
    {
        public void Method(int[] source, int[]? sourceNullable)
        {
            int[] result11 = sou{on}rce;
            int[]? result13 = source{off}Nullable;

            List<int> result21 = [..sour{on}ce];
            List<int> result22 = [..source{off}Nullable];

            int[] result31 = sour{on}ce.ToArray();
            int[]? result32 = source{off}Nullable?.ToArray();
        }

        public void Method<T>(T[] source, T[]? sourceNullable)
        {
            T[] result11 = sou{on}rce;
            T[]? result13 = source{off}Nullable;

            List<T> result21 = [..sour{on}ce];
            List<T> result22 = [..source{off}Nullable];

            T[] result31 = sour{on}ce.ToArray();
            T[]? result32 = source{off}Nullable?.ToArray();
        }
    }
}