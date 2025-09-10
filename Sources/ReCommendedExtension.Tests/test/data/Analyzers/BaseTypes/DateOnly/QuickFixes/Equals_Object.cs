using System;

namespace Test
{
    public class DatesOnly
    {
        public void Equals(DateOnly dateOnly)
        {
            var result = dateOnly.{caret}Equals(null);
        }
    }
}