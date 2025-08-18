using System;

namespace Test
{
    public class DatesOnly
    {
        public void Operator(DateOnly dateOnly, DateOnly value)
        {
            var result = dateOnly.Equals{caret}(value);
        }
    }
}