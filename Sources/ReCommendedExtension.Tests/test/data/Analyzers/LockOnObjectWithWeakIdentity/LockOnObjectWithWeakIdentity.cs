using System;
using System.IO;
using System.Threading;

namespace Test
{
    public class LockOnObjectWithWeakIdentity
    {
        public void Method(int parameter)
        {
            lock ("string") { }

            lock (new char[0]) { }
            lock (new char[0][]) { }
            lock (new char[0, 0]) { }

            lock (new sbyte[0]) { }
            lock (new byte[0]) { }
            lock (new short[0]) { }
            lock (new ushort[0]) { }
            lock (new int[0]) { }
            lock (new uint[0]) { }
            lock (new long[0]) { }
            lock (new ulong[0]) { }

            lock (new float[0]) { }
            lock (new double[0]) { }

            lock (new bool[0]) { }

            using (var marshalByRefObject = new MemoryStream())
            {
                lock (marshalByRefObject) { }
            }

            lock (typeof(LockOnObjectWithWeakIdentity)) { }

            var methodInfo = typeof(LockOnObjectWithWeakIdentity).GetMethod(nameof(Method));
            lock (methodInfo) { }
            lock (methodInfo.GetParameters()[0]) { }

            lock (new OutOfMemoryException()) { }

            lock (new StackOverflowException()) { }

            lock (new ExecutionEngineException()) { }

            lock (Thread.CurrentThread) { }
        }
    }
}