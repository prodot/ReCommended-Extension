using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Test
{
    public class Class
    {
        public void Method(IList<int> list, int fallback)
        {
            var first1 = list.FirstOrDefault();
            var first2 = list.FirstOrDefault(fallback);

            var last1 = list.LastOrDefault();
            var last2 = list.LastOrDefault(1);

            var single = list.Single();
        }

        public enum NonZeroBased
        {
            One = 1,
            Two = 2,
            Three = 3,
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
            IList<NonZeroBased> nonZeroBasedEnums,
            IList<string> strings,
            IList<(int first, bool second)> tuples,
            IList<int?> nullableStructs)
        {
            var bool_ = booleans.FirstOrDefault();
            var int_ = integers.FirstOrDefault();
            var long_ = longs.FirstOrDefault();
            var byte_ = bytes.FirstOrDefault();
            var double_ = doubles.FirstOrDefault();
            var decimal_ = decimals.FirstOrDefault();
            var char_ = chars.LastOrDefault();
            var struct_ = structs.LastOrDefault();
            var enum_ = enums.LastOrDefault();
            var nonZeroBasedEnums_ = nonZeroBasedEnums.LastOrDefault();
            var string_ = strings.LastOrDefault();
            var tuple_ = tuples.LastOrDefault();
            var nullableStruct_ = nullableStructs.LastOrDefault();
        }

        public void DefaultValues_Unconstrained<T>(IList<T> items)
        {
            var first = items.FirstOrDefault();
        }

        public void DefaultValues_ReferenceType<T>(IList<T> items) where T : class
        {
            var first = items.FirstOrDefault();
        }

        public class SomeClass { }

        public void DefaultValues_KnownReferenceType<T>(IList<T> items) where T : SomeClass
        {
            var first = items.FirstOrDefault();
        }

        public void DefaultValues_ValueType<T>(IList<T> items) where T : struct
        {
            var first = items.FirstOrDefault();
        }
    }
}