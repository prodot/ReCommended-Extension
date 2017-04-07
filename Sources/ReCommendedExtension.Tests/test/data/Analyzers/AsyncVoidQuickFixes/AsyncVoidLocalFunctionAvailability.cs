using System;
using System.Threading.Tasks;

namespace Test
{
    public class Class
    {
        void Method()
        {
            async void AsyncHandler(object sender, EventArgs e)
            {
                await Task.Yield();
            }

            AsyncHandler(null, null);
        }
    }
}