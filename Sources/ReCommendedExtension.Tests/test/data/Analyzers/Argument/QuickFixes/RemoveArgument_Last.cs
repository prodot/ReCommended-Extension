namespace Test
{
    public class Arguments
    {
        public void RedundantArgument(string s)
        {
            var result = int.Parse(s, nu{caret}ll);
        }
    }
}