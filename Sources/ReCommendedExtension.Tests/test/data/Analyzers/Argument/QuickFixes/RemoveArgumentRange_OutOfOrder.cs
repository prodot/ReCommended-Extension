using System;

namespace Test
{
    public class Arguments
    {
        public void RedundantArgumentRange(int year, int month, int day)
        {
            var result = new DateTime(year, month, second: 0, day: day, minute: {caret}0, hour: 0);
        }
    }
}