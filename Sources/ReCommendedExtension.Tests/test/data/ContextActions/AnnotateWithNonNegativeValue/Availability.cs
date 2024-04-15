namespace Test
{
    internal class Members
    {
        int Me{on}thod() => 3;

        int Prope{on}rty { get; set; } = 3;
        int Prop{on}erty2 { get; } = 3;
        int Prope{on}rty3 => 3;

        int f{on}ield = 3;

        void Method(int x{on}) { }

        delegate int Cal{on}lback();

        const int cons{off}tant = 3;

        void MethodWithLocalFunction()
        {
            int Local{on}Function() => 3;
        }
    }

    internal class Types
    {
        void Method(int {on}a, long {on}b, sbyte {on}c, short {on}d) { }

        void Method(uint {off}a, ulong {off}b, byte {off}c, ushort {off}d) { }

        void Method(decimal a{off}, float b{off}, double c{off}) { }
    }
}