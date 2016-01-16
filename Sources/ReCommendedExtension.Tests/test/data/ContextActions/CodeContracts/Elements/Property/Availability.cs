namespace Test
{
    internal abstract class AbstractClass
    {
        internal abstract string AbstractProperty{on} { get; set; }

        internal abstract string this{on}[int one, int two] { get; set; }
    }

    internal interface IInterface
    {
        string InterfaceProperty{on} { get; set; }

        string this{on}[int one, bool two] { get; set; }
    }

    internal class Class : AbstractClass, IInterface
    {
        internal override string AbstractProperty{off}
        { 
            get
            {
                return "";
            }
            set { }
        }

        public string InterfaceProperty {off}
        {
            get
            {
                return "";
            }
            set { }
        }

        string Property{on}
        {
            get
            {
                return "";
            }
            set { }
        }

        internal override string this{off}[int one, int two]
        {
            get
            {
                return "";
            }
            set { }
        }

        public string this{off}[int one, bool two]
        {
            get
            {
                return "";
            }
            set { }
        }

        string this{on}[int one]
        {
            get
            {
                return "";
            }
            set { }
        }

        string ReadOnlyProperty{on}
        {
            get
            {
                return "";
            }
        }

        string WriteOnlyProperty{on}
        {
            set { }
        }

        string this{on}[int one, int two, int three]
        {
            get
            {
                return "";
            }
        }

        string this{on}[int one, int two, bool three]
        {
            set { }
        }

        string AutoProperty{on} { get; set; }

        static string StaticAutoProperty{off} { get; set; }

        static extern string ExternProperty{off} { get; set; }

        string ExpressionBodiedProperty{off} => "";
    }
}