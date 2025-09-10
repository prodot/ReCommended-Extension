using System;

namespace Test
{
    public class DateTimes
    {
        public void _Constructors()
        {
            var result1 = new DateTime(0);

            DateTime result2 = new(0);
        }

        public void Equals(DateTime dateTime)
        {
            var result = dateTime.Equals(null);
        }

        public void GetTypeCode(DateTime dateTime)
        {
            var result = dateTime.GetTypeCode();
        }
    }
}