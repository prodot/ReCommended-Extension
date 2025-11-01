using static System.DateTime;

namespace Test
{
    public class Properties
    {
        public void StaticProperty()
        {
            var result = Now.{caret}Date;
        }
    }
}