using System.Threading.Tasks;

namespace Test
{
    internal class Execute
    {
        async Task AsyncDisposable_Multiple()
        {
            aw{caret}ait using (Stream m1 = new MemoryStream(), m2 = new MemoryStream()) { }
        }
    }
}