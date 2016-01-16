using System.Diagnostics;
using JetBrains.Annotations;

namespace Test
{
    internal static class Class
    {
        static void Foo(string x)
        {
            if (x != null)
            {
                Assert{caret}ThatTrue(x != null);
            }
        }

        [AssertionMethod]
        [ContractAnnotation("false => void")]
        static void AssertThatTrue([AssertionCondition(AssertionConditionType.IS_TRUE)] bool condition) => Debug.Assert(condition);

        [AssertionMethod]
        [ContractAnnotation("true => void")]
        static void AssertThatFalse([AssertionCondition(AssertionConditionType.IS_FALSE)] bool condition) => Debug.Assert(!condition);

        [AssertionMethod]
        [ContractAnnotation("notnull => void")]
        static void AssertThatNull<T>([AssertionCondition(AssertionConditionType.IS_NULL)] T reference) where T : class
            => Debug.Assert(reference == null);

        [AssertionMethod]
        [ContractAnnotation("null => void")]
        static void AssertThatNotNull<T>([AssertionCondition(AssertionConditionType.IS_NOT_NULL)] T reference) where T : class
            => Debug.Assert(reference != null);
    }
}