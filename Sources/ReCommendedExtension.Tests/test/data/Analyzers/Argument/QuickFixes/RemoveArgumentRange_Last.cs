using System;

namespace Test
{
    public class Arguments
    {
        public void RedundantArgumentRange(int year, int month, int day)
        {
            var result = new DateTime(year, month, day, 0, {caret}0, 0);
        }
    }
}