using System;

namespace Test
{
    public class Methods
    {
        public void BinaryOperator(DateTime dateTime, TimeSpan timeSpan)
        {
            var result = dateTime.Add{caret}(timeSpan).ToString();
        }
    }
}