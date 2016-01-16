namespace Test
{
    internal class Availability
    {
        void Available(
            int one{on}, 
            uint two{on}, 
            long three{on}, 
            ulong four{on}, 
            byte five{on}, 
            sbyte six{on}, 
            short seven{on}, 
            ushort eight{on}, 
            decimal nine{on}, 
            double ten{on}, 
            float eleven{on}) { }

        void NotAvailable(string one{off}) { }
    }
}