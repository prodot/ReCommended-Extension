using System;
using JetBrains.Annotations;

namespace Test
{
    internal class AnnotatedWithNotNull
    {
        [NotNull]
        string Method1([NotNull] string p1)
        {
            return "";
        }

        [NotNull]
        string Method2([NotNull] string p1) => "";

        [NotNull]
        string Property1
        {
            get
            {
                return "";
            }
            set { }
        }

        [NotNull]
        string Property2 { get; set; }

        [NotNull]
        string Property3
        {
            get
            {
                return "";
            }
        }

        [NotNull]
        string Property4 => "";

        [NotNull]
        string Property5 { get; set; } = "";

        [NotNull]
        string Property6 { get; } = "";

        [NotNull]
        string this[[NotNull] string index]
        {
            get
            {
                return "";
            }
            set { }
        }

        [NotNull]
        string this[[NotNull] string index1, [NotNull] string index2]
        {
            get
            {
                return "";
            }
        }

        [NotNull]
        string this[[NotNull] string index1, [NotNull] string index2, [NotNull] string index3] => "";

        [NotNull]
        string field1;
    }

    internal class AnnotatedWithCanBeNull
    {
        [CanBeNull]
        string Method1([CanBeNull] string p1)
        {
            return null;
        }

        [CanBeNull]
        string Method2([CanBeNull] string p1) => null;

        [CanBeNull]
        string Property1
        {
            get
            {
                return null;
            }
            set { }
        }

        [CanBeNull]
        string Property2 { get; set; }

        [CanBeNull]
        string Property3
        {
            get
            {
                return null;
            }
        }

        [CanBeNull]
        string Property4 => null;

        [CanBeNull]
        string Property5 { get; set; } = null;

        [CanBeNull]
        string Property6 { get; } = null;

        [CanBeNull]
        string this[[CanBeNull] string index]
        {
            get
            {
                return null;
            }
            set { }
        }

        [CanBeNull]
        string this[[CanBeNull] string index1, [CanBeNull] string index2]
        {
            get
            {
                return null;
            }
        }

        [CanBeNull]
        string this[[CanBeNull] string index1, [CanBeNull] string index2, [CanBeNull] string index3] => null;

        [CanBeNull]
        string field1;
    }

    internal class NotAnnotated
    {
        string Method1(string p1)
        {
            return null;
        }

        string Method2(string p1) => null;

        string Property1
        {
            get
            {
                return null;
            }
            set { }
        }

        string Property2 { get; set; }

        string Property3
        {
            get
            {
                return null;
            }
        }

        string Property4 => null;

        string Property5 { get; set; } = null;

        string Property6 { get; } = null;

        string this[string index]
        {
            get
            {
                return null;
            }
            set { }
        }

        string this[string index1, string index2]
        {
            get
            {
                return null;
            }
        }

        string this[string index1, string index2, string index3] => null;

        string field1;
    }
}