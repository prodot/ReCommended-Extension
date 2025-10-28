using System;

namespace Test
{
    public class ExpressionResults
    {
        public void ExpressionResult(int number)
        {
            var result = int.Clamp{caret}(number, 1, 0x01);
        }
    }
}