using JetBrains.Application.Settings;
using JetBrains.ProjectModel.Properties.CSharp;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;
using ReCommendedExtension.Tests.Missing;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.StringBuilder;

[TestFixture]
[CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
[NullableContext(NullableContextKind.Enable)]
[TestNet80]
public sealed class StringBuilderAnalyzerTests : CSharpHighlightingTestBase
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\StringBuilder";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is UseOtherMethodSuggestion || highlighting.IsError();

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
        Test("abcde", builder => builder.Append("xyz", 1, 1), builder => builder.Append('y'));
        TestNullable("abcde", builder => builder?.Append("xyz", 1, 1), builder => builder?.Append('y'));

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
        Test("abcde", builder => builder.AppendJoin(", ", (object?[])["item"]), builder => builder.Append((object)"item"));
        TestNullable("abcde", builder => builder?.AppendJoin(", ", (object?[])["item"]), builder => builder?.Append((object)"item"));

        Test("abcde", builder => builder.AppendJoin(", ", (ReadOnlySpan<object?>)[1]), builder => builder.Append((object)1));
        TestNullable("abcde", builder => builder?.AppendJoin(", ", (ReadOnlySpan<object?>)[1]), builder => builder?.Append((object)1));
        Test(
            "abcde",
            builder => builder.AppendJoin(",", new object?[] { 1, 2, 3 }.AsSpan()),
            builder => builder.AppendJoin(',', new object?[] { 1, 2, 3 }.AsSpan()));

        Test("abcde", builder => builder.AppendJoin(", ", (IEnumerable<int>)[3]), builder => builder.Append(3));
        TestNullable("abcde", builder => builder?.AppendJoin(", ", (IEnumerable<int>)[3]), builder => builder?.Append(3));

        Test("abcde", builder => builder.AppendJoin(", ", (string?[])["item"]), builder => builder.Append("item"));
        TestNullable("abcde", builder => builder?.AppendJoin(", ", (string?[])["item"]), builder => builder?.Append("item"));

        Test("abcde", builder => builder.AppendJoin(", ", new[] { (string?)"item" }.AsSpan()), builder => builder.Append("item"));
        TestNullable("abcde", builder => builder?.AppendJoin(", ", new[] { (string?)"item" }.AsSpan()), builder => builder?.Append("item"));
        Test("abcde", builder => builder.AppendJoin(", ", (ReadOnlySpan<string?>)["item"]), builder => builder.Append("item"));
        TestNullable("abcde", builder => builder?.AppendJoin(", ", (ReadOnlySpan<string?>)["item"]), builder => builder?.Append("item"));

        Test("abcde", builder => builder.AppendJoin(',', (object?[])["item"]), builder => builder.Append("item"));
        TestNullable("abcde", builder => builder?.AppendJoin(',', (object?[])["item"]), builder => builder?.Append("item"));

        Test("abcde", builder => builder.AppendJoin(',', new[] { (object?)"item" }.AsSpan()), builder => builder.Append((object)"item"));
        TestNullable("abcde", builder => builder?.AppendJoin(',', new[] { (object?)"item" }.AsSpan()), builder => builder?.Append((object)"item"));
        Test("abcde", builder => builder.AppendJoin(',', (ReadOnlySpan<object?>)[1]), builder => builder.Append((object)1));
        TestNullable("abcde", builder => builder?.AppendJoin(',', (ReadOnlySpan<object?>)[1]), builder => builder?.Append((object)1));

        Test("abcde", builder => builder.AppendJoin(',', (string?[])["item"]), builder => builder.Append("item"));
        TestNullable("abcde", builder => builder?.AppendJoin(',', (string?[])["item"]), builder => builder?.Append("item"));

        Test("abcde", builder => builder.AppendJoin(',', new[] { (string?)"item" }.AsSpan()), builder => builder.Append("item"));
        TestNullable("abcde", builder => builder?.AppendJoin(',', new[] { (string?)"item" }.AsSpan()), builder => builder?.Append("item"));
        Test("abcde", builder => builder.AppendJoin(',', (ReadOnlySpan<string?>)["item"]), builder => builder.Append("item"));
        TestNullable("abcde", builder => builder?.AppendJoin(',', (ReadOnlySpan<string?>)["item"]), builder => builder?.Append("item"));

        DoNamedTest2();
    }
}