namespace Test
{
    public class GenericClass<T>
    {
        void Method()
        {
            var variable = new[] { default, default(T) {caret}};
        }
    }
}