using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Test
{
    internal static class Class
    {
        class Foo
        {
            public void Method() { }
        }

        static async Task Promises(TaskCompletionSource<DayOfWeek?> promise)
        {
            var dayOfWeek = await promise.Task;

            Debug.Assert(dayOfWeek != null); // not redundant (dayOfWeek is nullable)
        }

        static string NotNullMethod() => "one";

        static string field = NotNullMethod().AssertNotNull(); // redundant

        static string Property => NotNullMethod().AssertNotNull(); // redundant

        static Lazy<string?> PropertyLazy => new Lazy<string?>(() => NotNullMethod().AssertNotNull()); // redundant

        static string? PropertyNullable => null;

        static string Property2 { get; } = NotNullMethod().AssertNotNull(); // redundant

        static string Property3 { get; set; } = NotNullMethod().AssertNotNull(); // redundant

        static string Method() => NotNullMethod().AssertNotNull(); // redundant

        [DebuggerStepThrough]
        [JetBrains.Annotations.NotNull]
        static T AssertNotNull<T>(this T? value) where T : class
        {
            AssertThatTrue(value != null);

            return value;
        }

        class Nested
        {
            string? field = NotNullMethod().AssertNotNull(); // redundant

            string? Property => NotNullMethod().AssertNotNull(); // redundant

            string? AutoProperty { get; } = NotNullMethod().AssertNotNull(); // redundant
        }

        static void ClassConstraint<T>(T one, T? two) where T : class
        {
            var x = one.AssertNotNull(); // redundant

            var y = two.AssertNotNull().AssertNotNull(); // redundant (2nd)
        }

        static void ClassNullableClassConstraint<T>(T one) where T : class?
        {
            var x = one.AssertNotNull().AssertNotNull(); // redundant (2nd)
        }

        static readonly string[] Words = { "one", "two", "three" };

        static readonly Dictionary<int, string> WordMap = new Dictionary<int, string>{ { 1, "one" }, { 2, "two" } };

        static readonly Dictionary<int, string[]> WordMap2 =
            new Dictionary<int, string[]> { { 1, new[] { "one", "two", "three" } }, { 2, new[] { "one", "two", "three" } } };

        static void Iterations()
        {
            var query0 = from word in Words where word.AssertNotNull().Length > 2 select word; // "AssertNotNull" must be redundant
            var query1 = from word in Words where word != null select word; // "word != null" is always true
            var query2 = from word in Words select word.AssertNotNull(); // "AssertNotNull" must be redundant

            AssertThatNotNull(Words); // redundant
            foreach (var word in Words)
            {
                AssertThatNotNull(word); // redundant
            }

            AssertThatNotNull(WordMap); // redundant
            foreach (var (key, value) in WordMap)
            {
                AssertThatNotNull(value); // redundant
            }
            foreach (var value in WordMap.Values)
            {
                AssertThatNotNull(value); // redundant
            }

            AssertThatNotNull(WordMap2); // redundant
            foreach (var (key, values) in WordMap2)
            {
                AssertThatNotNull(values); // redundant
                foreach (var value in values)
                {
                    AssertThatNotNull(value); // redundant
                }
            }
            foreach (var values in WordMap2.Values)
            {
                AssertThatNotNull(values); // redundant
                foreach (var value in values)
                {
                    AssertThatNotNull(value); // redundant
                }
            }
        }

        static void Foo(bool b, object? s, string? x)
        {
            Action action = () =>
            {
                var text = "";
                AssertThatTrue(text != null); // redundant
                var text2 = text.AssertNotNull().Replace("a", "b"); // redundant
                AssertThatTrue(text2 != null); // redundant
            };

            var length = Property.     AssertNotNull()         .     AssertNotNull()      .Length; // redundant (both)
            var qqq = Property.AssertNotNull().ToList().All(char.IsDigit); // redundant

            if (b)
            {
                if (b) { }

                AssertThatTrue(b); // redundant

                Console.WriteLine(b.ToString());
            }
            else
            {
                AssertThatFalse(b); // redundant
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
                AssertThatTrue(PropertyNullable != null); // redundant
            }
            AssertThatFalse(PropertyNullable == null); // redundant
            AssertThatNotNull(PropertyNullable);

            AssertThatTrue(true);
            AssertThatFalse(false);
            AssertThatNull<string>(null);

            AssertThatNotNull(new object());
            new object().AssertNotNull();

            if (x != null)
            {
                AssertThatTrue(condition: x != null); // redundant
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

        static void Foo(string xf)
        {
            AssertThatNotNull(xf); // redundant
            xf.AssertNotNull(); // redundant
            AssertThatTrue(xf != null); // redundant
        }

        static void Foo2(string x2)
        {
            AssertThatFalse(x2 == null); // redundant
        }

        class A
        {
            public string NotNull { get; set; }

            public string? CanBeNull { get; set; }
        }

        static void NullPropagation1(A? canBeNull) => AssertThatTrue(canBeNull?.NotNull != null);

        static void NullPropagation2(A? canBeNull) => AssertThatNotNull(canBeNull?.NotNull);

        static void NullPropagation3(A? canBeNull) => Console.WriteLine(canBeNull?.NotNull.AssertNotNull()); // redundant

        static void NullPropagation4(A notNull) => AssertThatTrue(notNull?.NotNull != null); // redundant

        static void NullPropagation5(A notNull) => AssertThatNotNull(notNull?.NotNull); // redundant

        static void NullPropagation6(A notNull) => Console.WriteLine(notNull?.NotNull.AssertNotNull()); // redundant

        static void NullPropagation7(A notNull) => AssertThatTrue(notNull?.CanBeNull != null);

        static void NullPropagation8(A notNull) => AssertThatNotNull(notNull?.CanBeNull);

        static void NullPropagation9(A notNull) => Console.WriteLine(notNull?.CanBeNull.AssertNotNull());

        [AssertionMethod]
        [ContractAnnotation("false => void")]
        static void AssertThatTrue([AssertionCondition(AssertionConditionType.IS_TRUE)] bool condition) => Debug.Assert(condition);

        [AssertionMethod]
        [ContractAnnotation("true => void")]
        static void AssertThatFalse([AssertionCondition(AssertionConditionType.IS_FALSE)] bool condition) => Debug.Assert(!condition);

        [AssertionMethod]
        [ContractAnnotation("notnull => void")]
        static void AssertThatNull<T>([AssertionCondition(AssertionConditionType.IS_NULL)] T? reference) where T : class
            => Debug.Assert(reference == null);

        [AssertionMethod]
        [ContractAnnotation("null => void")]
        static void AssertThatNotNull<T>([AssertionCondition(AssertionConditionType.IS_NOT_NULL)] T? reference) where T : class
            => Debug.Assert(reference != null);

        [return: NotNullIfNotNull("value")]
        static T? PassThrough<T>(T? value) where T : class => value;

        static void CheckPassedThrough(string value)
        {
            var result = PassThrough(value);

            Debug.Assert(result != null); // redundant
        }
    }
}