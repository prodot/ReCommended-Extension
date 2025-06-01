namespace Test
{
    public class Nullables
    {
        public void Operator(int? nullable, int defaultValue)
        {
            var result1 = nullable.GetValueOrDefault();
            var result2 = nullable.GetValueOrDefault(defaultValue);
        }

        public void Operator<T>(T? nullable, T defaultValue) where T : struct
        {
            var result1 = nullable.GetValueOrDefault();
            var result2 = nullable.GetValueOrDefault(defaultValue);
        }

        public void NoDetection(int? nullable, int defaultValue)
        {
            nullable.GetValueOrDefault();
            nullable.GetValueOrDefault(defaultValue);
        }
    }
}