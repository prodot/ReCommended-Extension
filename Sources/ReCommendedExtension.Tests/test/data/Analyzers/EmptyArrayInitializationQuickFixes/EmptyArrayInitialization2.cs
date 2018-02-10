namespace Test
{
    public class GenericClass<T>
    {
        void Method()
        {
            var array = new T[] {caret}{ };
        }
    }
}