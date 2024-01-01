namespace Test
{
    public class GenericClass<T>
    {
        void Method()
        {
            var variable = new T[] { default, default(T) {caret}};
        }
    }
}