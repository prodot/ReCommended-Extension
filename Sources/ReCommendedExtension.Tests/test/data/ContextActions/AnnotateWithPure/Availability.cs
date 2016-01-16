namespace Test
{
    internal class Availability
    {
        string Meth{on}od() => null;

        void Meth{on}od2(out int one) { }

        void Meth{off}od3(ref int one) { }

        void Meth{off}od4(int one) { }

        string Meth{off}od5(ref int one) => null;

        void Meth{off}od6(ref int one, out string two) { }

        string Proper{off}ty => null;
    }
}