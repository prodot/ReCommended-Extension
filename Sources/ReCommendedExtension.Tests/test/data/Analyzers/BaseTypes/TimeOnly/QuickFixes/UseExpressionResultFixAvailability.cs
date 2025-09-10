using System;

namespace Test
{
    public class TimesOnly
    {
        public void _Constructors()
        {
            var result11 = new TimeOnly(0);
            var result12 = new TimeOnly(0, 0);
            var result13 = new TimeOnly(0, 0, 0);
            var result14 = new TimeOnly(0, 0, 0, 0);
            var result15 = new TimeOnly(0, 0, 0, 0, 0);

            TimeOnly result21 = new(0);
            TimeOnly result22 = new(0, 0);
            TimeOnly result23 = new(0, 0, 0);
            TimeOnly result24 = new(0, 0, 0, 0);
            TimeOnly result25 = new(0, 0, 0, 0, 0);
        }

        public void Equals(TimeOnly timeOnly)
        {
            var result = timeOnly.Equals(null);
        }
    }
}