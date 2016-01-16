namespace Test
{
    internal class Class
    {
        internal abstract class AbstractClass
        {
            internal abstract string AbstractMethod{caret}<T, U>() where T : class where U : new();
        }
    }
}