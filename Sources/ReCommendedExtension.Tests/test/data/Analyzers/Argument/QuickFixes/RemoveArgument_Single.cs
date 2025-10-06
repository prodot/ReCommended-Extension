namespace Test
{
    public class Arguments
    {
        public void RedundantArgument(string s)
        {
            var result = s.Trim(nu{caret}ll);
        }
    }
}