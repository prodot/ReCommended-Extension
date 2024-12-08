using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using AnyException = System.InvalidTimeZoneException;

namespace Test
{
    public class ThrowExceptionInUnexpectedLocation
    {
        public void Method(string s)
        {
            try
            {
                if (s == null)
                {
                    throw new ArgumentNullException("s");
                }
            }
            catch (ArgumentNullException e) when (e.ParamName == "s" ? throw new AnyException() : true) // not allowed
            {

            }
            finally
            {
                void LocalFunction()
                {
                    throw new AnyException(); // allowed
                }

                throw new AnyException(); // not allowed
            }
        }

        public void Method2()
        {
            try { }
            finally
            {
                void LocalFunction()
                {
                    try { }
                    catch (ArgumentNullException e) when (e.ParamName == "s" ? throw new AnyException() : true) // not allowed
                    { }
                    finally
                    {
                        throw new AnyException(); // not allowed
                    }
                }
            }
        }
    }
}