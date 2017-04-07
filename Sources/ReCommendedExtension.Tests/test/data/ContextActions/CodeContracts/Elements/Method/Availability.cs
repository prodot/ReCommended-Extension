namespace Test
{
    internal abstract class AbstractClass
    {
        internal abstract string AbstractMethod{on}();
    }

    internal interface IInterface
    {
        string InterfaceMethod{on}();
    }

    internal class Class : AbstractClass, IInterface
    {
        internal override string AbstractMethod{off}()
        {
            throw new System.NotImplementedException();
        }

        public string InterfaceMethod{off}()
        {
            throw new System.NotImplementedException();
        }

        internal string Method{on}()
        {
            string Local{off}Function() => null;

            throw new System.NotImplementedException();
        }

        internal void VoidMethod{off}();

        static extern string ExternMethod{off}();

        internal string ExpressionBodiedMethod{off}() => "";
    }
}