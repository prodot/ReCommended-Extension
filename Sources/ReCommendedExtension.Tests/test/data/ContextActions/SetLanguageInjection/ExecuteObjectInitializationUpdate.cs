using System;
using System.Collections.Generic;

namespace Test
{
    public class Execute
    {
        void Method()
        {
            Console.WriteLine();

            var obj = new Execute
            {
                Property =
                    // language=XML
                    "<html {caret:HTML}/>"
            };

            Console.WriteLine();
        }

        public string Property { get; set; }
    }
}