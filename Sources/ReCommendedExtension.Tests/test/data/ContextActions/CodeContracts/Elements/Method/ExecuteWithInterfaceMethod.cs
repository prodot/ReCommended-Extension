using System.Diagnostics.Contracts;

namespace Test
{
    internal class Class
    {
        [ContractClass(typeof(InterfaceContract))]
        internal interface IInterface
        {
            string InterfaceMethod{caret}();
        }

        [ContractClassFor(typeof(IInterface))]
        private abstract class InterfaceContract : IInterface { }
    }
}