namespace Test
{
    public class Methods
    {
        public void Pattern(double n)
        {
            var result = double.IsNaN(n);
        }

        public void NoDetection(double n)
        {
            double.IsNaN(n);
        }
    }
}