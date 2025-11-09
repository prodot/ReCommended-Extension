namespace Test
{
    public class Arguments
    {
        public void RedundantArgument(string s)
        {
            var result = int.Parse(provider: nu{caret}ll, s: s);
        }
    }
}