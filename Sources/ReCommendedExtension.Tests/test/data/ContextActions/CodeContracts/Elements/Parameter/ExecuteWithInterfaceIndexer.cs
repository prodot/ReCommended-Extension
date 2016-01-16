namespace Test
{
    internal interface IInterface
    {
        int this[string one{caret}] { get; set; }
    }
}