namespace Test
{
    public class NonGenericClass
    {
        int[] field = { 0, default, default(int) };
        int[] field_ = { 0, 1, 2 };

        void Method()
        {
            var variable = new[] { null, default, default(string) };
            var variable2 = new string[] { null, default, default(string) };

            var variable_ = new[] { null, "" };
        }
    }

    public class GenericClass<T>
    {
        T[] field = { default, default(T) };

        void Method()
        {
            var variable = new[] { default, default(T) };
            var variable2 = new T[] { default, default(T) };
        }
    }
}