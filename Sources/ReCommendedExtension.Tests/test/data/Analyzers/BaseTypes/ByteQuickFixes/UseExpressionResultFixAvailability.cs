namespace Test
{
    public class Bytes
    {
        public void Clamp(byte number)
        {
            var result1 = byte.Clamp(number, 1, 1);
            var result2 = byte.Clamp(number, 0, 255);
        }

        public void DivRem(byte left)
        {
            var result1 = byte.DivRem(0, 10);
            var result2 = byte.DivRem(left, 1);
        }

        public void Equals(byte number)
        {
            var result = number.Equals(null);
        }

        public void GetTypeCode(byte number)
        {
            var result = number.GetTypeCode();
        }

        public void Max()
        {
            var result = byte.Max(10, 10);
        }

        public void Min()
        {
            var result = byte.Min(10, 10);
        }

        public void RotateLeft(byte n)
        {
            var result = byte.RotateLeft(n, 0);
        }

        public void RotateRight(byte n)
        {
            var result = byte.RotateRight(n, 0);
        }
    }
}