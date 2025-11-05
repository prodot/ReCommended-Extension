using System.Linq;

namespace Test
{
    public class Methods
    {
        public void SingleOrDefault(int[] array)
        {
            var result = array.SingleOrDefault({caret}-1).ToString();
        }
    }
}