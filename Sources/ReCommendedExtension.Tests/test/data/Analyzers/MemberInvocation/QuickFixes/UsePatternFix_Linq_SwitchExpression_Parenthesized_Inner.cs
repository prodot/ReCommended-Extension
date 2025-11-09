using System.Linq;

namespace Test
{
    public class Methods
    {
        public void SingleOrDefault(int[] array, int fallback)
        {
            var result = array.SingleOrDefault({caret}fallback + 1);
        }
    }
}