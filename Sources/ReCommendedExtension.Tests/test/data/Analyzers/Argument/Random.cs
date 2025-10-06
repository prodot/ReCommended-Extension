using System;

namespace Test
{
    public class Arguments
    {
        public void RedundantArgument(Random r, int maxValueInt32, long maxValueInt64)
        {
            var result11 = r.Next(2_147_483_647);
            var result12 = r.Next(0, maxValueInt32);

            var result21 = r.NextInt64(long.MaxValue);
            var result22 = r.NextInt64(0, maxValueInt64);
        }

        public void NoDetection(Random r, int minValue, int maxValueInt32, long maxValueInt64)
        {
            var result11 = r.Next(maxValueInt32);
            var result12 = r.Next(minValue, maxValueInt32);

            var result21 = r.NextInt64(maxValueInt64);
            var result22 = r.NextInt64(minValue, maxValueInt64);
        }
    }
}