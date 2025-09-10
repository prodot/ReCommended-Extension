namespace Test
{
    public class NonGenericClass
    {
        int[] field = { 0, default, default(int) {caret}};
    }
}