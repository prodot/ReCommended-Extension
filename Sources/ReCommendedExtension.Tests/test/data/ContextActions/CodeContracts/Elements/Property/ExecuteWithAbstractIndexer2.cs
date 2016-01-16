using System.Diagnostics.Contracts;

namespace Test
{
    [ContractClass(typeof(AbstractClassContract))]
    internal abstract class AbstractClass
    {
        internal abstract string this{caret}[int one] { get; set; }
    }

    [ContractClassFor(typeof(AbstractClass))]
    internal abstract class AbstractClassContract : AbstractClass
    {
        internal override string this[int one] {
            get { }
            set { }
        }
    }
}