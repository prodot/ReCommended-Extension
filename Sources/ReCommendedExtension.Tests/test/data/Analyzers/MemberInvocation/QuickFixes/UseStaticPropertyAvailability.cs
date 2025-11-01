using System;
using static System.DateTime;

namespace Test
{
    public class Properties
    {
        public void StaticProperty()
        {
            var result1 = DateTime.Now.Date;
            var result2 = Now.Date;
        }
    }
}