using System;

namespace Test
{
    public class SomeClass { }

    public class BaseAttribute : Attribute { }

    public class DerivedAttribute : BaseAttribute { }

    [AttributeUsage(AttributeTargets.Class)]
    public class BaseAttribute2 : Attribute { }

    public class DerivedAttribute2 : BaseAttribute2 { }
}