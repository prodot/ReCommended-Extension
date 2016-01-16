using System.Diagnostics.Contracts;

namespace Test
{
    internal class Execute
    {
        string field{caret};

        [ContractInvariantMethod]
        void ObjectInvariant()
        {
            Contract.Invariant(true);
        }
    }
}