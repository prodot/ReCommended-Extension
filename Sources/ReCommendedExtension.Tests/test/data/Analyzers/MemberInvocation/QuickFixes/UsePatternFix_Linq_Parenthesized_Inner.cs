using System.Linq;

namespace Test
{
    public class Methods
    {
        public void Pattern(int[] array, int? fallback)
        {
            var result = array.FirstOrDefault{caret}(fallback ?? -1);
        }
    }
}