using System;

namespace Test
{
    public class ExpressionResults
    {
        public void ExpressionResult(int number)
        {
            var result = int.Clamp{caret}(number, 2, 0x01 + 1).ToString();
        }
    }
}