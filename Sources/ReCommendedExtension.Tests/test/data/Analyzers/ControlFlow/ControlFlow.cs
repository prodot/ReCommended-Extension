using System;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;

namespace Test
{
    internal static class Class
    {
        [NotNull]
        static string NotNullMethod() => "one";

        [NotNull]
        static string field = NotNullMethod().AssertNotNull();

        [NotNull]
        static string Property => NotNullMethod().AssertNotNull();

        [NotNull]
        static Lazy<string> PropertyLazy => new Lazy<string>(() => NotNullMethod().AssertNotNull());

        static string PropertyNullable => null;

        [NotNull]
        static string Property2 { get; } = NotNullMethod().AssertNotNull();

        [NotNull]
        static string Property3 { get; set; } = NotNullMethod().AssertNotNull();

        [NotNull]
        static string Method() => NotNullMethod().AssertNotNull();

        [DebuggerStepThrough]
        [NotNull]
        static T AssertNotNull<T>(this T value) where T : class
        {
            AssertThatTrue(value != null);

            return value;
        }

        class Nested
        {
            string field = NotNullMethod().AssertNotNull();

            string Property => NotNullMethod().AssertNotNull();

            string AutoProperty { get; } = NotNullMethod().AssertNotNull();
        }

        [NotNull]
        [ItemNotNull]
        static readonly string[] Words = { "one", "two", "three" };

        static void Foo(bool b, object s, string x)
        {
            var query0 = from word in Words where word.AssertNotNull().Length > 2 select word; // "AssertNotNull" must be redundant
            var query1 = from word in Words where word != null select word;
            var query2 = from word in Words select word.AssertNotNull(); // "AssertNotNull" must be redundant

            Action action = () =>
            {
                var text = "";
                AssertThatTrue(text != null);
                var text2 = text.AssertNotNull().Replace("a", "b");
                AssertThatTrue(text2 != null);
            };

            var length = Property.     AssertNotNull()         .     AssertNotNull()      .Length;
            var qqq = Property.AssertNotNull().ToList().All(char.IsDigit);

            if (b)
            {
                if (b) { }

                AssertThatTrue(b); // must be redundant

                Console.WriteLine(b.ToString());
            }
            else
            {
                AssertThatFalse(b); // must be redundant
            }

            if (s is string)
            {
                if (s as string != null) { }
                AssertThatTrue(s as string != null);
                AssertThatFalse(s as string == null);
                AssertThatNotNull(s as string);
            }
            else
            {
                AssertThatNull(s as string);
            }

            if (PropertyNullable != null)
            {
                AssertThatTrue(PropertyNullable != null);
            }
            AssertThatFalse(PropertyNullable == null);
            AssertThatNotNull(PropertyNullable);

            AssertThatTrue(true);
            AssertThatFalse(false);
            AssertThatNull<string>(null);

            AssertThatNotNull(new object());
            new object().AssertNotNull();

            if (x != null)
            {
                if (x != null)
                {
                    Foo(true, "", null);
                }

                AssertThatTrue(condition: x != null);
                AssertThatTrue(x != null);
                AssertThatTrue(null != x);

                AssertThatFalse(null == x);
                AssertThatFalse(x == null);

                Console.WriteLine(x.ToString());
            }
            else
            {
                AssertThatTrue(condition: x == null);
                AssertThatTrue(x == null);
                AssertThatTrue(null == x);

                AssertThatFalse(null != x);
                AssertThatFalse(x != null);

                AssertThatNull(x);
            }
        }

        static void Foo([NotNull] string x)
        {
            AssertThatNotNull(x);
            x.AssertNotNull();
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