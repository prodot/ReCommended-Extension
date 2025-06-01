namespace Test
{
    public class Nullables
    {
        public void HasValueAlternative(int? nullable)
        {
            var result = nullable.HasValue;
        }

        public void HasValueAlternative<T>(T? nullable) where T : struct
        {
            var result = nullable.HasValue;
        }

        public void NoDetection(int? nullable)
        {
            nullable.HasValue = true;

            (nullable.HasValue, var x) = (true, 1);
        }
    }
}