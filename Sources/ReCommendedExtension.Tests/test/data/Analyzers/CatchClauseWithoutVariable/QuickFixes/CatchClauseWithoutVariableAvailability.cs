using System;

namespace Test
{
    internal class CatchClauseWithoutVariable
    {
        void Method()
        {
            try { }
            catch (Exception) { }

            try { }
            catch (Exception) when (true) { }

            try { }
            catch (Exception e) { }

            try { }
            catch (Exception e) when (e.Message != null) { }

            try { }
            catch (InvalidOperationException) { }
        }
    }
}