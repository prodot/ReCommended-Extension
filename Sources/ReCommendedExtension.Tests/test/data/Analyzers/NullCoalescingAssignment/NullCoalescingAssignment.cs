using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tests
{
    public class Sample
    {
        void EqualityComparison(string value, string x)
        {
            if (value == null)
            {
                value = "one";
            }

            if (x == null) x = new Version(1, 2).ToString();
        }

        void EqualityComparison_Reverse(string value, string x)
        {
            if (null == value)
            {
                value = string.Join(", ", new[] { "one", "two", "three" });
            }

            if (null == x) x = "four".Remove(0, 1);
        }

        class Data
        {
            string Property { get; set; }
        }

        Data data;

        string Property { get; set; }

        void IsComparison(string fallbackValue)
        {
            if (data.Property is null)
            {
                data.Property = fallbackValue;
            }

            if (Property is null) Property = fallbackValue.Trim();
        }

        void Unavailable(string x)
        {
            if (value == null)
            {
                value = "one";
            }
            else
            {
                value = "two";
            }

            if (value == null)
            {
                value = "one";
                value = "two";
            }
        }
    }
}