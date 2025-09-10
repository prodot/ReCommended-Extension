using System;

namespace Test
{
    public class DatesOnly
    {
        public void AddDays(DateOnly dateOnly)
        {
            var result = dateOnly.Add{caret}Days(0);
        }
    }
}