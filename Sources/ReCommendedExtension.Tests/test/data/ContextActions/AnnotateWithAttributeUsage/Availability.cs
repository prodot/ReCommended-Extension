using System;

namespace Test
{
    internal class Non{off}Attribute { }

    [AttributeUsage(AttributeTargets.Class)]
    internal class Annotated{off}Attribute : Attribute { }

    internal class DerivedFromAnnotated{on}Attribute: AnnotatedAttribute { }

    internal class Some{on}Attribute : Attribute { }

    internal abstract class Abstract{on}Attribute : Attribute { }

    internal class Derived{on}Attribute : AbstractAttribute { }

    internal class Final{on}Attribute : DerivedAttribute { }
}