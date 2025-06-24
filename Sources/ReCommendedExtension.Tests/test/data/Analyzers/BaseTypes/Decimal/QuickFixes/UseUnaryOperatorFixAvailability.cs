namespace Test
{
    public class Decimals
    {
        public void Negate(decimal d, int a)
        {
            var result11 = decimal.Negate(d);
            var result12 = decimal.Negate(a);
            var result13 = decimal.Negate(1m);
            var result14 = decimal.Negate(1);
        }
    }
}