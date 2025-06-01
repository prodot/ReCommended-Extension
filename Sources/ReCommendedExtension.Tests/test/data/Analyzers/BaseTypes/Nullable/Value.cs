namespace Test
{
    public class Nullables
    {
        public void ReplaceNullableValueWithCast(int? nullable)
        {
            var result = nullable.Value;
        }

        public void ReplaceNullableValueWithCast<T>(T? nullable) where T : struct
        {
            var result = nullable.Value;
        }

        public void NoDetection(int? nullable, (int a, bool b)? tuple)
        {
            var result = tuple.Value;

            nullable.Value = true;            

            (nullable.Value, var x) = (1, true);
        }
    }
}