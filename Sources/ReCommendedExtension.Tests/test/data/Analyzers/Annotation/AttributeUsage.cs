using System;

namespace Test
{
    public class SomeClass { }

    public class BaseAttribute : Attribute { }

    public class DerivedAttribute : BaseAttribute { }

    [AttributeUsage(AttributeTargets.Class)]
    public class BaseAttribute2 : Attribute { }

    public class DerivedAttribute2 : BaseAttribute2 { }

    public class DerivedAttribute21 : DerivedAttribute2 { }

    public abstract class BaseAttribute3 : Attribute { }

    public class DerivedAttribute3 : BaseAttribute3 { }
}

namespace PartialClasses
{
    public partial class PartialClassAttribute { }

    [AttributeUsage(AttributeTargets.Class)]
    public partial class PartialClassAttribute : Attribute { }
}