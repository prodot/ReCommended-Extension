using System.Threading.Tasks;

namespace Test
{
    internal class Execute
    {
        async Task AsyncDisposable()
        {
            aw{caret}ait using (new MemoryStream()) { }
        }
    }
}