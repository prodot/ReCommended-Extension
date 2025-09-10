namespace Test
{
    public class Characters
    {
        public void ExpressionResult(char character)
        {
            var result = character.GetTypeCode();
        }

        public void NoDetection(char character)
        {
            character.GetTypeCode();
        }
    }
}