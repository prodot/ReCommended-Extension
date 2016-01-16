using JetBrains.Annotations;

namespace Test
{
    internal interface IBase
    {
        string Method1(string p1);

        string Property1 { get; set; }

        string Property2 { get; }

        string this[string index] { get; set; }

        string this[string index, string index2] { get; }
    }

    internal class BaseInterfaceImplementation : IBase
    {
        [NotNull]
        public string Method1([NotNull] string p1) => "";

        [NotNull]
        public string Property1 { get; set; }

        [NotNull]
        public string Property2 { get; }

        [NotNull]
        public string this[[NotNull] string index]
        {
            get
            {
                return "";
            }
            set { }
        }

        [NotNull]
        public string this[[NotNull] string index, [NotNull] string index2] => "";
    }

    internal abstract class Base
    {
        internal abstract string Method1(string p1);

        internal abstract string Property1 { get; set; }

        internal abstract string Property2 { get; }

        internal abstract string this[string index] { get; set; }

        internal abstract string this[string index, string index2] { get; }
    }

    internal class BaseClassImplementation : Base
    {
        [NotNull]
        internal override string Method1([NotNull] string p1) => "";

        [NotNull]
        internal override string Property1 { get; set; }

        [NotNull]
        internal override string Property2 { get; }

        [NotNull]
        internal override string this[[NotNull] string index]
        {
            get
            {
                return "";
            }
            set { }
        }

        [NotNull]
        internal override string this[[NotNull] string index, [NotNull] string index2] => "";
    }
}