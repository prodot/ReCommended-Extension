namespace Test
{
    public class InterpolatedStringItems
    {
        public void Int32(int number)
        {
            var result = $"{number:G{caret}11}";
        }
    }
}