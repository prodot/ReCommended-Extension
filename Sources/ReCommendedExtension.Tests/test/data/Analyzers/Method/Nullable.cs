namespace Test
{
    public class Methods
    {
        public void BinaryOperator(int? nullable)
        {
            var result1 = nullable.GetValueOrDefault();
            var result2 = nullable.GetValueOrDefault(-1);
        }

        public void NoDetection(int? nullable)
        {
            nullable.GetValueOrDefault();
            nullable.GetValueOrDefault(-1);
        }
    }
}