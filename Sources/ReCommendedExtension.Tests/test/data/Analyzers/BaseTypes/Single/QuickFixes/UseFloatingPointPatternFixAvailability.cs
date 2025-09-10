namespace Test
{
    public class Singles
    {
        public void IsNaN(float value)
        {
            var result = float.IsNaN(value > 1 ? 1f : -1f);
        }
    }
}