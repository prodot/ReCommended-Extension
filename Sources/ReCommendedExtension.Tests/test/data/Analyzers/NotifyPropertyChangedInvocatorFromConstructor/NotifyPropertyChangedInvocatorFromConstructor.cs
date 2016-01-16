using System;
using JetBrains.Annotations;

namespace Test
{
    internal class NotifyPropertyChangedInvocatorFromConstructor
    {
        public NotifyPropertyChangedInvocatorFromConstructor()
        {
            Notify();

            Action lambda = () => Notify();
            Action lambda2 = Notify;
            Action anonymousDelegate = delegate { Notify(); };
            Action anonymousDelegate2 = Notify;
        }

        void Method() => Notify();

        int Property
        {
            set
            {
                Notify();
            }
        }

        [NotifyPropertyChangedInvocator]
        void Notify() { }
    }
}