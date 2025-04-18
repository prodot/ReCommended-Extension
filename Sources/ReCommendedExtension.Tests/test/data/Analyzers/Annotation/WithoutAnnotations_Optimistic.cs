using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Test
{
    internal class AsyncMethod
    {
        IEnumerable<string> Iterator(IEnumerable<string> p)
        {
            yield break;
        }

        async Task<string> AsyncMethod() => await AsyncProperty1;

        Task<string> AsyncProperty1 { get; set; }

        Task<string> AsyncProperty2 => null;

        Task<string> this[Task<string> index] => null;

        Task<string> this[Task<string> index, Task<string> index2] => null;

        Lazy<string> LazyMethod() => null;

        Lazy<string> LazyProperty1 { get; set; }

        Lazy<string> LazyProperty2 => null;

        Lazy<string> this[Lazy<string> index] => null;

        Lazy<string> this[Lazy<string> index, Lazy<string> index2] => null;

        string Method1(string p1)
        {
            return null;
        }

        string Method2(string p1) => null;

        string Property1
        {
            get
            {
                return null;
            }
            set { }
        }

        string Property2 { get; set; }

        string Property3
        {
            get
            {
                return null;
            }
        }

        string Property4 => null;

        string Property5 { get; set; } = null;

        string Property6 { get; } = null;

        string this[string index]
        {
            get
            {
                return null;
            }
            set { }
        }

        string this[string index1, string index2]
        {
            get
            {
                return null;
            }
        }

        string this[string index1, string index2, string index3] => null;

        string field1;

        void Functions()
        {
            string LocalFunction(string x) => null;

            Func<string, string> lambda = x => null;
        }
    }
}