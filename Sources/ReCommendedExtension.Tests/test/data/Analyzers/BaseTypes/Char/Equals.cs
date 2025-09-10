﻿namespace Test
{
    public class Characters
    {
        public void ExpressionResult(char character)
        {
            var result = character.Equals(null);
        }

        public void Operator(char character, char obj)
        {
            var result = character.Equals(obj);
        }

        public void NoDetection(char character, char obj, char? otherObj)
        {
            var result = character.Equals(otherObj);

            character.Equals(null);

            character.Equals(obj);
        }
    }
}