using System.Diagnostics.Contracts;

namespace Test
{
    internal class Class
    {
        [ContractClass(typeof(AbstractClassContract))]
        internal abstract class AbstractClass
        {
            internal abstract string AbstractMethod{caret}();
        }

        [ContractClassFor(typeof(AbstractClass))]
        private abstract class AbstractClassContract : AbstractClass
        {
            internal override string AbstractMethod()
            {
                return default(T);
            }
        }
    }
}