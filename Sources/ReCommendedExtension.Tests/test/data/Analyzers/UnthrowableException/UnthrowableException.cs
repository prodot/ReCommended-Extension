using System;

namespace Test
{
    internal class UnthrowableException
    {
        void Method()
        {
            throw new Exception();
        }

        void Method(Exception e)
        {
            throw e;
        }

        void Method2() => throw new Exception();

        string x;

        void Method3(string value) => x = value ?? throw new Exception();

        void Method4()
        {
            try
            {
                Method();
            }
            catch (Exception)
            {
                throw;
            }
        }

        void Method5()
        {
            try
            {
                Method();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}