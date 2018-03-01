using System;

namespace Test
{
    public class DelegateInvoke
    {
        void Method(Action action, Action<int> action2, Func<int> func, Func<int, int> func2)
        {
            action.Invoke();

            var methodName = nameof(Action.Invoke);

            action?.Invoke();
            action();

            action2.Invoke(1);
            action2?.Invoke(1);
            action2(1);

            func.Invoke();
            func?.Invoke();
            func();

            func2.Invoke(1);
            func2?.Invoke(1);
            func2(1);

            action . Invoke   ();
        }
    }
}