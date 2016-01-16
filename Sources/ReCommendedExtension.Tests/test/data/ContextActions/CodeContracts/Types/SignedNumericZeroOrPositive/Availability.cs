namespace Test
{
    internal class Availability
    {
        void Available(int one{on}, long two{on}, sbyte three{on}, short four{on}, decimal five{on}, double six{on}, float seven{on}) { }

        void NotAvailable(uint one{off}, string two{off}) { }
    }
}