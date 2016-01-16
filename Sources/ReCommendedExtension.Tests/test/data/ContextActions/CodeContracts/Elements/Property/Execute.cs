using System;

namespace Test
{
    internal class Class
    {
        string Property{caret}
        {
            get
            {
                return "";
            }
            set
            {
                Console.WriteLine(value);
            }
        }
    }
}