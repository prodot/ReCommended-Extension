using System;

namespace Test
{
    public class ExpressionResults
    {
        public void ExpressionResult(Random random, int[] array)
        {
            int[] result = random.GetItems(array, 0);
        }

        public void ExpressionResult<T>(Random random, T[] array)
        {
            T[] result = random.GetItems(array, 0);
        }
    }
}