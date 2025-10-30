namespace Test
{
    public class Methods
    {
        public void Pattern(float n)
        {
            var result = float.IsNaN(n);
        }

        public void NoDetection(float n)
        {
            float.IsNaN(n);
        }
    }
}