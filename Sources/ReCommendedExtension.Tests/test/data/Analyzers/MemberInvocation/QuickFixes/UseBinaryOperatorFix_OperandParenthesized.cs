using System;

namespace Test
{
    public class Methods
    {
        public void BinaryOperator(DateTime dateTime, TimeSpan timeSpan, TimeSpan ts)
        {
            var result = dateTime.Add{caret}(timeSpan + ts);
        }
    }
}