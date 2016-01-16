using JetBrains.Annotations;

namespace Test
{
    internal class NotifyPropertyChangedInvocatorFromConstructor
    {
        public NotifyPropertyChangedInvocatorFromConstructor()
        {
            Notify();
        }

        [NotifyPropertyChangedInvocator]
        void Notify() { }
    }
}