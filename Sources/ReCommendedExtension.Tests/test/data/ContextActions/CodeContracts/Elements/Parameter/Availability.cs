namespace Test
{
    internal abstract class AbstractClass
    {
        internal abstract void AbstractMethod(string one{on}, ref string two{on}, out string three{on});

        internal abstract int this[string one{on}, int two] { get; set; }
    }

    internal interface IInterface
    {
        void InterfaceMethod(string one{on}, ref string two{on}, out string three{on});

        int this[string one{on}, bool two] { get; set; }
    }

    internal class Class : AbstractClass, IInterface
    {
        internal override void AbstractMethod(string one{off}, ref string two{off}, out string three{off})
        {
            three = "";
        }

        public void InterfaceMethod(string one{off}, ref string two{off}, out string three{off})
        {
            three = "";
        }

        internal void Method(string one{on}, ref string two{on}, out string three{on})
        {
            three = "";
        }

        internal int ExpressionBodiedMethod(string one{off}, ref string two {off}, out string three {off}) => 0;

        internal override int this[string one{off}, int two]
        { 
            get
            {
                return 0;
            }
            set { }
        }

        public int this[string one{off}, bool two]
        { 
            get
            {
                return 0;
            }
            set { }
        }

        int this[string one{on}]
        { 
            get
            {
                return 0;
            }
            set { }
        }

        int this[string one{off}, int two, bool three] => 0;

        static extern string ExternMethod(string one{off}, ref string two{off}, out string three{off});
    }
}