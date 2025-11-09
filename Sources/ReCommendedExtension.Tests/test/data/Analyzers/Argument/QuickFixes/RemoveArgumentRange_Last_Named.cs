using System;

namespace Test
{
    public class Arguments
    {
        public void RedundantArgumentRange(int year, int month, int day)
        {
            var result = new DateTime(year, month, day, hour: 0, minute: {caret}0, second: 0);
        }
    }
}