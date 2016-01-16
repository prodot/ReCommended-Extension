namespace Test
{
    internal class Class
    {
        internal abstract class AbstractClass<T> where T : class
        {
            internal abstract T AbstractMethod{caret}();
        }
    }
}