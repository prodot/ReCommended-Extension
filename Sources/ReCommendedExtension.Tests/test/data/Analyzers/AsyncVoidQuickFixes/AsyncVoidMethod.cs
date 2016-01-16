using System;
using System.Threading.Tasks;

namespace Test
{
    public class Class
    {
        void Method()
        {
            AsyncHandler(null, null);
        }

        async vo{caret}id AsyncHandler(object sender, EventArgs e)
        {
            await Task.Yield();
        }
    }
}