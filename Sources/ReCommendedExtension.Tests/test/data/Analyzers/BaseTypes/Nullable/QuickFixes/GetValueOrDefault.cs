namespace Test
{
    public class Nullables
    {
        public void GetValueOrDefault(int? nullable)
        {
            var result = nullable.GetValue{caret}OrDefault().ToString();
        }
    }
}