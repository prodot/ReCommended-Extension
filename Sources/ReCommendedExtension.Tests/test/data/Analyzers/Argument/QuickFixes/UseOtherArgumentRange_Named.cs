namespace Test
{
    public class Arguments
    {
        public void OtherArgument(string text)
        {
            var result = text.Replace(oldValue: "c", newValue: {caret}"x");
        }
    }
}