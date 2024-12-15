using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Test
{
    public class Class
    {
        public void Method(IList<int> list, int fallback)
        {
            var single1 = list.SingleOrDefault();
            var single2 = list.SingleOrDefault(fallback);
        }

        public void DefaultValues(
            IList<bool> booleans,
            IList<int> integers,
            IList<long> longs,
            IList<byte> bytes,
            IList<double> doubles,
            IList<decimal> decimals,
            IList<char> chars,
            IList<CancellationToken> structs,
            IList<DayOfWeek> enums,
            IList<string> strings,
            IList<(int first, bool second)> tuples,
            IList<int?> nullableStructs)
        {
            var bool_ = booleans.SingleOrDefault();
            var int_ = integers.SingleOrDefault();
            var long_ = longs.SingleOrDefault();
            var byte_ = bytes.SingleOrDefault();
            var double_ = doubles.SingleOrDefault();
            var decimal_ = decimals.SingleOrDefault();
            var char_ = chars.SingleOrDefault();
            var struct_ = structs.SingleOrDefault();
            var enum_ = enums.SingleOrDefault();
            var string_ = strings.SingleOrDefault();
            var tuple_ = tuples.SingleOrDefault();
            var nullableStruct_ = nullableStructs.SingleOrDefault();
        }

        public void DefaultValues_Unconstrained<T>(IList<T> items)
        {
            var first = items.SingleOrDefault();
        }

        public void DefaultValues_ReferenceType<T>(IList<T> items) where T : class
        {
            var first = items.SingleOrDefault();
        }

        class SomeClass { }

        public void DefaultValues_KnownReferenceType<T>(IList<T> items) where T : SomeClass
        {
            var first = items.SingleOrDefault();
        }

        public void DefaultValues_ValueType<T>(IList<T> items) where T : struct
        {
            var first = items.SingleOrDefault();
        }
    }
}