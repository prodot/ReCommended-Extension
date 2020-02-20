using System;
using System.Xml.Linq;

namespace ReCommendedExtension.Tests.test.data
{
    public class Simple
    {
        void NullCheck()
        {
            if (EventArgs.Empty != null) { }
        }

        void PassingNullToNullable()
        {
            int.TryParse(null, out _);
        }

        void PassingNullToNotBuiltWithNullableAnnotationContext()
        {
            XDocument.Load(null as string);
        }

        void Dereferencing(Exception exception)
        {
            Console.WriteLine(exception.HelpLink.Length);
        }
    }
}