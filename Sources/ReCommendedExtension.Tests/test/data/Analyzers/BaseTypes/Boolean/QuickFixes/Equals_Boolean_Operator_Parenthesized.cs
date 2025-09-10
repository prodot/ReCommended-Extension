namespace Test
{
    public class Booleans
    {
        public void Equals(int x, int y)
        {
            var result = (x != 0).Equal{caret}s(y != 0);
        }
    }
}