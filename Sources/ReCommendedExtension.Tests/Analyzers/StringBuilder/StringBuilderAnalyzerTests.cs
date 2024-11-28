using JetBrains.Application.Settings;
using JetBrains.ProjectModel.Properties.CSharp;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.Strings;

namespace ReCommendedExtension.Tests.Analyzers.StringBuilder;

[TestFixture]
[CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
[NullableContext(NullableContextKind.Enable)]
[TestNet80]
public sealed class StringBuilderAnalyzerTests : CSharpHighlightingTestBase
{
    protected override string RelativeTestDataPath => @"Analyzers\StringBuilder";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is PassSingleCharacterSuggestion
            or PassSingleCharactersSuggestion
            or UseOtherMethodSuggestion
            or RedundantArgumentHint
            or RedundantMethodInvocationHint;

    static void AreEqual<T>(T? expected, T? actual)
    {
        if (typeof(T) == typeof(System.Text.StringBuilder))
        {
            Assert.AreEqual(expected?.ToString(), actual?.ToString());
        }
        else
        {
            Assert.AreEqual(expected, actual);
        }
    }

    static void Test<R>(string text, Func<System.Text.StringBuilder, R> expected, Func<System.Text.StringBuilder, R> actual, bool emptyThrows = false)
    {
        // empty
        if (emptyThrows)
        {
            Assert.Catch(() => expected(new System.Text.StringBuilder()));
            Assert.Catch(() => actual(new System.Text.StringBuilder()));
        }
        else
        {
            AreEqual(expected(new System.Text.StringBuilder()), actual(new System.Text.StringBuilder()));
        }

        // not empty
        AreEqual(expected(new System.Text.StringBuilder(text)), actual(new System.Text.StringBuilder(text)));
    }

    static void TestNullable<R>(
        string text,
        Func<System.Text.StringBuilder?, R> expected,
        Func<System.Text.StringBuilder?, R> actual,
        bool emptyThrows = false)
    {
        // null
        AreEqual(expected(null), actual(null));

        // empty
        if (emptyThrows)
        {
            Assert.Catch(() => expected(new System.Text.StringBuilder()));
            Assert.Catch(() => actual(new System.Text.StringBuilder()));
        }
        else
        {
            AreEqual(expected(new System.Text.StringBuilder()), actual(new System.Text.StringBuilder()));
        }

        // not empty
        AreEqual(expected(new System.Text.StringBuilder(text)), actual(new System.Text.StringBuilder(text)));
    }

    [Test]
    [SuppressMessage("ReSharper", "RedundantMethodInvocation")]
    [SuppressMessage("ReSharper", "RedundantArgument")]
    [SuppressMessage("ReSharper", "PassSingleCharacter")]
    public void TestAppend()
    {
        Test("abcde", builder => builder.Append('x', 0), builder => builder);
        TestNullable("abcde", builder => builder?.Append('x', 0), builder => builder);
        Test("abcde", builder => builder.Append('x', 1), builder => builder.Append('x'));
        TestNullable("abcde", builder => builder?.Append('x', 1), builder => builder?.Append('x'));

        Test("abcde", builder => builder.Append(null as char[]), builder => builder);
        TestNullable("abcde", builder => builder?.Append(null as char[]), builder => builder);
        Test("abcde", builder => builder.Append([]), builder => builder);
        TestNullable("abcde", builder => builder?.Append([]), builder => builder);

        Test("abcde", builder => builder.Append(null as char[], 0, 0), builder => builder);
        TestNullable("abcde", builder => builder?.Append(null as char[], 0, 0), builder => builder);
        Test("abcde", builder => builder.Append([], 0, 0), builder => builder);
        TestNullable("abcde", builder => builder?.Append([], 0, 0), builder => builder);

        Test("abcde", builder => builder.Append(null as string), builder => builder);
        TestNullable("abcde", builder => builder?.Append(null as string), builder => builder);
        Test("abcde", builder => builder.Append(""), builder => builder);
        TestNullable("abcde", builder => builder?.Append(""), builder => builder);
        Test("abcde", builder => builder.Append("x"), builder => builder.Append('x'));
        TestNullable("abcde", builder => builder?.Append("x"), builder => builder?.Append('x'));

        Test("abcde", builder => builder.Append(null as string, 0, 0), builder => builder);
        TestNullable("abcde", builder => builder?.Append(null as string, 0, 0), builder => builder);
        Test("abcde", builder => builder.Append("", 10, 0), builder => builder);
        TestNullable("abcde", builder => builder?.Append("", 10, 0), builder => builder);
        Test("abcde", builder => builder.Append("xyz", 10, 0), builder => builder);
        TestNullable("abcde", builder => builder?.Append("xyz", 10, 0), builder => builder);
        Test("abcde", builder => builder.Append("xyz", 1, 1), builder => builder.Append('y'));
        TestNullable("abcde", builder => builder?.Append("xyz", 1, 1), builder => builder?.Append('y'));

        Test("abcde", builder => builder.Append(null as System.Text.StringBuilder), builder => builder);
        TestNullable("abcde", builder => builder?.Append(null as System.Text.StringBuilder), builder => builder);

        Test("abcde", builder => builder.Append(null as System.Text.StringBuilder, 0, 0), builder => builder);
        TestNullable("abcde", builder => builder?.Append(null as System.Text.StringBuilder, 0, 0), builder => builder);
        Test("abcde", builder => builder.Append(new System.Text.StringBuilder(), 10, 0), builder => builder);
        TestNullable("abcde", builder => builder?.Append(new System.Text.StringBuilder(), 10, 0), builder => builder);

        Test("abcde", builder => builder.Append(null as object), builder => builder);
        TestNullable("abcde", builder => builder?.Append(null as object), builder => builder);

        DoNamedTest2();
    }

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp130)]
    [NullableContext(NullableContextKind.Enable)]
    [TestNet90]
    [SuppressMessage("ReSharper", "RedundantCast")]
    [SuppressMessage("ReSharper", "RedundantExplicitArrayCreation")]
    public void TestAppendJoin()
    {
        Test("abcde", builder => builder.AppendJoin(", ", (object?[])[]), builder => builder);
        TestNullable("abcde", builder => builder?.AppendJoin("..", (object?[])[]), builder => builder);
        Test("abcde", builder => builder.AppendJoin(", ", (object?[])["item"]), builder => builder.Append((object)"item"));
        TestNullable("abcde", builder => builder?.AppendJoin(", ", (object?[])["item"]), builder => builder?.Append((object)"item"));
        Test(
            "abcde",
            builder => builder.AppendJoin(",", (object?[])["item1", "item2"]),
            builder => builder.AppendJoin(',', (object?[])["item1", "item2"]));
        TestNullable(
            "abcde",
            builder => builder?.AppendJoin(",", (object?[])["item1", "item2"]),
            builder => builder?.AppendJoin(',', (object?[])["item1", "item2"]));

        Test("abcde", builder => builder.AppendJoin(", ", default(ReadOnlySpan<object?>)), builder => builder);
        TestNullable("abcde", builder => builder?.AppendJoin(", ", default(ReadOnlySpan<object?>)), builder => builder);
        Test("abcde", builder => builder.AppendJoin(", ", new ReadOnlySpan<object?>()), builder => builder);
        TestNullable("abcde", builder => builder?.AppendJoin(", ", new ReadOnlySpan<object?>()), builder => builder);
        Test("abcde", builder => builder.AppendJoin(", ", new[] { (object?)"item" }.AsSpan()), builder => builder.Append((object)"item"));
        TestNullable("abcde", builder => builder?.AppendJoin(", ", new[] { (object?)"item" }.AsSpan()), builder => builder?.Append((object)"item"));
        Test("abcde", builder => builder.AppendJoin(", ", (ReadOnlySpan<object?>)[1]), builder => builder.Append((object)1));
        TestNullable("abcde", builder => builder?.AppendJoin(", ", (ReadOnlySpan<object?>)[1]), builder => builder?.Append((object)1));
        Test(
            "abcde",
            builder => builder.AppendJoin(",", new object?[] { 1, 2, 3 }.AsSpan()),
            builder => builder.AppendJoin(',', new object?[] { 1, 2, 3 }.AsSpan()));

        Test("abcde", builder => builder.AppendJoin(", ", (int[])[]), builder => builder);
        TestNullable("abcde", builder => builder?.AppendJoin(", ", (int[])[]), builder => builder);
        Test("abcde", builder => builder.AppendJoin(", ", (int[])[3]), builder => builder.Append(3));
        TestNullable("abcde", builder => builder?.AppendJoin(", ", (int[])[3]), builder => builder?.Append(3));
        Test("abcde", builder => builder.AppendJoin(",", (int[])[1, 2, 3]), builder => builder.AppendJoin(',', (int[])[1, 2, 3]));
        TestNullable("abcde", builder => builder?.AppendJoin(",", (int[])[1, 2, 3]), builder => builder?.AppendJoin(',', (int[])[1, 2, 3]));

        Test("abcde", builder => builder.AppendJoin(", ", (string?[])[]), builder => builder);
        TestNullable("abcde", builder => builder?.AppendJoin(", ", (string?[])[]), builder => builder);
        Test("abcde", builder => builder.AppendJoin(", ", (string?[])["item"]), builder => builder.Append("item"));
        TestNullable("abcde", builder => builder?.AppendJoin(", ", (string?[])["item"]), builder => builder?.Append("item"));
        Test(
            "abcde",
            builder => builder.AppendJoin(",", (string?[])["item", "item2"]),
            builder => builder.AppendJoin(',', (string?[])["item", "item2"]));
        TestNullable(
            "abcde",
            builder => builder?.AppendJoin(",", (string?[])["item", "item2"]),
            builder => builder?.AppendJoin(',', (string?[])["item", "item2"]));

        Test("abcde", builder => builder.AppendJoin(", ", default(ReadOnlySpan<string?>)), builder => builder);
        TestNullable("abcde", builder => builder?.AppendJoin(", ", default(ReadOnlySpan<string?>)), builder => builder);
        Test("abcde", builder => builder.AppendJoin(", ", new ReadOnlySpan<string?>()), builder => builder);
        TestNullable("abcde", builder => builder?.AppendJoin(", ", new ReadOnlySpan<string?>()), builder => builder);
        Test("abcde", builder => builder.AppendJoin(", ", new[] { (string?)"item" }.AsSpan()), builder => builder.Append("item"));
        TestNullable("abcde", builder => builder?.AppendJoin(", ", new[] { (string?)"item" }.AsSpan()), builder => builder?.Append("item"));
        Test("abcde", builder => builder.AppendJoin(", ", (ReadOnlySpan<string?>)["item"]), builder => builder.Append("item"));
        TestNullable("abcde", builder => builder?.AppendJoin(", ", (ReadOnlySpan<string?>)["item"]), builder => builder?.Append("item"));
        Test(
            "abcde",
            builder => builder.AppendJoin(",", new string?[] { "one", "two" }.AsSpan()),
            builder => builder.AppendJoin(',', new string?[] { "one", "two" }.AsSpan()));

        Test("abcde", builder => builder.AppendJoin(',', (object?[])[]), builder => builder);
        TestNullable("abcde", builder => builder?.AppendJoin(',', (object?[])[]), builder => builder);
        Test("abcde", builder => builder.AppendJoin(',', (object?[])["item"]), builder => builder.Append("item"));
        TestNullable("abcde", builder => builder?.AppendJoin(',', (object?[])["item"]), builder => builder?.Append("item"));

        Test("abcde", builder => builder.AppendJoin(',', default(ReadOnlySpan<object?>)), builder => builder);
        TestNullable("abcde", builder => builder?.AppendJoin(',', default(ReadOnlySpan<object?>)), builder => builder);
        Test("abcde", builder => builder.AppendJoin(',', new ReadOnlySpan<object?>()), builder => builder);
        TestNullable("abcde", builder => builder?.AppendJoin(',', new ReadOnlySpan<object?>()), builder => builder);
        Test("abcde", builder => builder.AppendJoin(',', new[] { (object?)"item" }.AsSpan()), builder => builder.Append((object)"item"));
        TestNullable("abcde", builder => builder?.AppendJoin(',', new[] { (object?)"item" }.AsSpan()), builder => builder?.Append((object)"item"));
        Test("abcde", builder => builder.AppendJoin(',', (ReadOnlySpan<object?>)[1]), builder => builder.Append((object)1));
        TestNullable("abcde", builder => builder?.AppendJoin(',', (ReadOnlySpan<object?>)[1]), builder => builder?.Append((object)1));

        Test("abcde", builder => builder.AppendJoin(',', (int[])[]), builder => builder);
        TestNullable("abcde", builder => builder?.AppendJoin(',', (int[])[]), builder => builder);
        Test("abcde", builder => builder.AppendJoin(',', (int[])[3]), builder => builder.Append(3));
        TestNullable("abcde", builder => builder?.AppendJoin(',', (int[])[3]), builder => builder?.Append(3));

        Test("abcde", builder => builder.AppendJoin(',', (string?[])[]), builder => builder);
        TestNullable("abcde", builder => builder?.AppendJoin(',', (string?[])[]), builder => builder);
        Test("abcde", builder => builder.AppendJoin(',', (string?[])["item"]), builder => builder.Append("item"));
        TestNullable("abcde", builder => builder?.AppendJoin(',', (string?[])["item"]), builder => builder?.Append("item"));

        Test("abcde", builder => builder.AppendJoin(',', default(ReadOnlySpan<string?>)), builder => builder);
        TestNullable("abcde", builder => builder?.AppendJoin(',', default(ReadOnlySpan<string?>)), builder => builder);
        Test("abcde", builder => builder.AppendJoin(',', new ReadOnlySpan<string?>()), builder => builder);
        TestNullable("abcde", builder => builder?.AppendJoin(',', new ReadOnlySpan<string?>()), builder => builder);
        Test("abcde", builder => builder.AppendJoin(',', new[] { (string?)"item" }.AsSpan()), builder => builder.Append("item"));
        TestNullable("abcde", builder => builder?.AppendJoin(',', new[] { (string?)"item" }.AsSpan()), builder => builder?.Append("item"));
        Test("abcde", builder => builder.AppendJoin(',', (ReadOnlySpan<string?>)["item"]), builder => builder.Append("item"));
        TestNullable("abcde", builder => builder?.AppendJoin(',', (ReadOnlySpan<string?>)["item"]), builder => builder?.Append("item"));

        DoNamedTest2();
    }

    [Test]
    [SuppressMessage("ReSharper", "PassSingleCharacter")]
    [SuppressMessage("ReSharper", "RedundantMethodInvocation")]
    public void TestInsert()
    {
        Test("abcde", builder => builder.Insert(1, "xyz", 1), builder => builder.Insert(1, "xyz"), true);
        TestNullable("abcde", builder => builder?.Insert(1, "xyz", 1), builder => builder?.Insert(1, "xyz"), true);
        Test("abcde", builder => builder.Insert(1, "x", 1), builder => builder.Insert(1, 'x'), true);
        TestNullable("abcde", builder => builder?.Insert(1, "x", 1), builder => builder?.Insert(1, 'x'), true);

        Test("abcde", builder => builder.Insert(1, "x"), builder => builder.Insert(1, 'x'), true);
        TestNullable("abcde", builder => builder?.Insert(1, "x"), builder => builder?.Insert(1, 'x'), true);

        Test("abcde", builder => builder.Insert(10, null as object), builder => builder);

        DoNamedTest2();
    }

    [Test]
    [SuppressMessage("ReSharper", "RedundantMethodInvocation")]
    [SuppressMessage("ReSharper", "PassSingleCharacters")]
    public void TestReplace()
    {
        Test("abcde", builder => builder.Replace("cd", "cd"), builder => builder);
        TestNullable("abcde", builder => builder?.Replace("cd", "cd"), builder => builder);
        Test("abcde", builder => builder.Replace("c", "x"), builder => builder.Replace('c', 'x'));
        TestNullable("abcde", builder => builder?.Replace("c", "x"), builder => builder?.Replace('c', 'x'));

        Test("abcde", builder => builder.Replace('c', 'c'), builder => builder);
        TestNullable("abcde", builder => builder?.Replace('c', 'c'), builder => builder);

        Test("abcde", builder => builder.Replace("c", "x", 1, 3), builder => builder.Replace('c', 'x', 1, 3), true);
        TestNullable("abcde", builder => builder?.Replace("c", "x", 1, 3), builder => builder?.Replace('c', 'x', 1, 3), true);

        DoNamedTest2();
    }
}