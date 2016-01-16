using System.Diagnostics.Contracts;

namespace Test
{
    [ContractClass(typeof(AbstractClassContract))]
    internal abstract class AbstractClass
    {
        internal abstract string AbstractProperty{caret} { get; set; }
    }

    [ContractClassFor(typeof(AbstractClass))]
    internal abstract class AbstractClassContract : AbstractClass
    {
        internal override string AbstractProperty
        {
            get { }
            set { }
        }
    }
}