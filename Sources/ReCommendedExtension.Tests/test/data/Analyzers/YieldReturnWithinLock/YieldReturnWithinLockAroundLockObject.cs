using System.Collections.Generic;
using System.Threading;

namespace Test
{
    public class YieldReturnWithinLock
    {
        readonly Lock sync = new();

        IEnumerable<int> Method()
        {
            yield return 0;

            lock (sync)
            {
                yield return 1;
                yield return 2;
                yield return 3;

                IEnumerable<string> LocalFunction()
                {
                    yield return "one";
                }

                foreach (var s in LocalFunction())
                {
                    yield return s.Length;
                }
            }

            yield return 4;

            lock ((object)sync)
            {
                yield return 5;
            }
        }
    }
}