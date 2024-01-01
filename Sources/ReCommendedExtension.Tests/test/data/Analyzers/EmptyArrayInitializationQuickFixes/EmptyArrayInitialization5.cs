namespace Test
{
    public class NonGenericClass
    {
        void Method()
        {
            Method2(new int{caret}[0]);
        }

        void Method2(int[] array) { }
    }
}