using JetBrains.Annotations;

namespace Test
{
    internal class NotifyPropertyChangedInvocatorFromConstructor
    {
        public NotifyPropertyChangedInvocatorFromConstructor()
        {
            No{caret}tify();
        }

        [NotifyPropertyChangedInvocator]
        void Notify() { }
    }
}