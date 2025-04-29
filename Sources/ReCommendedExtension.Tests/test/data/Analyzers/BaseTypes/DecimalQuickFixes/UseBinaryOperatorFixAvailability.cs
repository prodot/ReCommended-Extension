namespace Test
{
    public class Decimals
    {
        public void Equals(decimal number, decimal obj)
        {
            var result = number.Equals(obj);
        }

        public void Add(decimal d1, decimal d2, int a, int b)
        {
            var result11 = decimal.Add(d1, d2);
            var result12 = decimal.Add(d1, b);
            var result13 = decimal.Add(a, d2);

            var result21 = decimal.Add(1m, 2m);
            var result22 = decimal.Add(1, b);
            var result23 = decimal.Add(a, 1);
            var result24 = decimal.Add(a, b);
        }

        public void Divide(decimal d1, decimal d2, int a, int b)
        {
            var result11 = decimal.Divide(d1, d2);
            var result12 = decimal.Divide(d1, b);
            var result13 = decimal.Divide(a, d2);

            var result21 = decimal.Divide(1m, 2m);
            var result22 = decimal.Divide(1, b);
            var result23 = decimal.Divide(a, 1);
            var result24 = decimal.Divide(a, b);
        }

        public void Multiply(decimal d1, decimal d2, int a, int b)
        {
            var result11 = decimal.Multiply(d1, d2);
            var result12 = decimal.Multiply(d1, b);
            var result13 = decimal.Multiply(a, d2);

            var result21 = decimal.Multiply(1m, 2m);
            var result22 = decimal.Multiply(1, b);
            var result23 = decimal.Multiply(a, 1);
            var result24 = decimal.Multiply(a, b);
        }

        public void Remainder(decimal d1, decimal d2, int a, int b)
        {
            var result11 = decimal.Remainder(d1, d2);
            var result12 = decimal.Remainder(d1, b);
            var result13 = decimal.Remainder(a, d2);

            var result21 = decimal.Remainder(1m, 2m);
            var result22 = decimal.Remainder(1, b);
            var result23 = decimal.Remainder(a, 1);
            var result24 = decimal.Remainder(a, b);
        }

        public void Subtract(decimal d1, decimal d2, int a, int b)
        {
            var result11 = decimal.Subtract(d1, d2);
            var result12 = decimal.Subtract(d1, b);
            var result13 = decimal.Subtract(a, d2);

            var result21 = decimal.Subtract(1m, 2m);
            var result22 = decimal.Subtract(1, b);
            var result23 = decimal.Subtract(a, 1);
            var result24 = decimal.Subtract(a, b);
        }
    }
}