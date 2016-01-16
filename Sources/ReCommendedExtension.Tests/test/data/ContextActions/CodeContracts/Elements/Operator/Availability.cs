using System;

namespace Test
{
    internal class Class
    {
        public static explicit operator{on} string(Class parameter) { }

        public static explicit operator{off} Version(Class parameter) => null;

        public static string operator{on} +(Class x, Class y) { }

        public static extern string operator{off} *(Class x, Class y);

        public static string operator{off} -(Class x, Class y) => "";
    }
}