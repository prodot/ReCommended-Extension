using System;
using System.IO;
using System.Text;
using System.Runtime.CompilerServices;

namespace Test
{
    public class Formatters
    {
        public void FallbackFormatters(int number)
        {
            var result1 = $"{number:G}";

            FormattableString result2 = $"{number:G}";
        }

        public void StringFormatters(int number, StringBuilder stringBuilder, TextWriter textWriter)
        {
            var result1 = string.Format("{0:G}", number);
            var result2 = FormattableStringFactory.Create("{0:G}", number);

            stringBuilder.AppendFormat("{0:G}", number);
            textWriter.Write("{0:G}", number);
            textWriter.WriteLine("{0:G}", number);
            Console.Write("{0:G}", number);
            Console.WriteLine("{0:G}", number);
        }

        public static void Custom(string format, params object[] args) { }

        public void NoDetection(int number)
        {
            Custom("{0:G0}", number);
        }
    }
}