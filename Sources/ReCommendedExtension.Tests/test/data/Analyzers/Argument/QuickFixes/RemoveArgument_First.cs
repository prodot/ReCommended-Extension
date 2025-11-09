using System;

namespace Test
{
    public class Arguments
    {
        public void RedundantArgument()
        {
            var result = new TimeSpan(0{caret}, 1, 2, 3);
        }
    }
}