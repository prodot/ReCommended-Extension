using System.Threading.Tasks;

namespace Test
{
    internal class Execute
    {
        async Task Method()
        {
            aw{caret}ait Task.Run(() => { });
        }
    }
}