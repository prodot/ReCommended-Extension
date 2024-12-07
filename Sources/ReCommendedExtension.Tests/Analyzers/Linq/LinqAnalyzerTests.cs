using JetBrains.Application.Settings;
using JetBrains.ProjectModel.Properties.CSharp;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.Linq;

namespace ReCommendedExtension.Tests.Analyzers.Linq;

[TestFixture]
[CSharpLanguageLevel(CSharpLanguageLevel.CSharp90)]
[TestNet50]
public sealed class LinqAnalyzerTests : CSharpHighlightingTestBase
{
    protected override string RelativeTestDataPath => @"Analyzers\Linq";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is UseIndexerSuggestion
            or UseLinqListPatternSuggestion
            or UseSwitchExpressionSuggestion
            or UseCollectionPropertySuggestion
            or SuspiciousElementAccessWarning
            or UseCollectionCountPropertyWarning; // to figure out which cases are supported by R#

    [SuppressMessage("ReSharper", "UseTargetTypedCollectionExpression")]
    static void Test<T, R>(T[] array, Func<IList<T>, R> expected, Func<IList<T>, R> actual, bool emptyThrows = true)
    {
        var list = array.ToList();

        // empty
        if (emptyThrows)
        {
            Assert.Catch(() => expected([]));
            Assert.Catch(() => actual(new List<T>()));
            Assert.Catch(() => actual(Array.Empty<T>()));
        }
        else
        {
            Assert.AreEqual(expected([]), actual(new List<T>()));
            Assert.AreEqual(expected([]), actual(Array.Empty<T>()));
        }

        // not empty
        Assert.AreEqual(expected(list), actual(list));
        Assert.AreEqual(expected(array), actual(array));
    }

    [SuppressMessage("ReSharper", "UseTargetTypedCollectionExpression")]
    static void TestNullable<T, R>(T[] array, Func<IList<T>?, R> expected, Func<IList<T>?, R> actual, bool emptyThrows = true)
    {
        var list = array.ToList();

        // null
        Assert.AreEqual(expected(null), actual(null));

        // empty
        if (emptyThrows)
        {
            Assert.Catch(() => expected([]));
            Assert.Catch(() => actual(new List<T>()));
            Assert.Catch(() => actual(Array.Empty<T>()));
        }
        else
        {
            Assert.AreEqual(expected([]), actual(new List<T>()));
            Assert.AreEqual(expected([]), actual(Array.Empty<T>()));
        }

        // not empty
        Assert.AreEqual(expected(list), actual(list));
        Assert.AreEqual(expected(array), actual(array));
    }

    static void Test<R>(string text, Func<string, R> expected, Func<string, R> actual, bool emptyThrows = true)
    {
        // empty
        if (emptyThrows)
        {
            Assert.Catch(() => expected(""));
            Assert.Catch(() => actual(""));
        }
        else
        {
            Assert.AreEqual(expected(""), actual(""));
        }

        // not empty
        Assert.AreEqual(expected(text), actual(text));
    }

    static void TestNullable<R>(string text, Func<string?, R> expected, Func<string?, R> actual, bool emptyThrows = true)
    {
        // null
        Assert.AreEqual(expected(null), actual(null));

        // empty
        if (emptyThrows)
        {
            Assert.Catch(() => expected(""));
            Assert.Catch(() => actual(""));
        }
        else
        {
            Assert.AreEqual(expected(""), actual(""));
        }

        // not empty
        Assert.AreEqual(expected(text), actual(text));
    }

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp130)]
    [NullableContext(NullableContextKind.Enable)]
    [TestNet90]
    [SuppressMessage("ReSharper", "UseIndexer")]
    public void TestElementAt()
    {
        Test([1, 2, 3], source => source.ElementAt(1), source => source[1]);
        TestNullable([1, 2, 3], source => source?.ElementAt(1), source => source?[1]);

        Test(["one", "two", "three"], source => source.ElementAt(1), source => source[1]);
        TestNullable(["one", "two", "three"], source => source?.ElementAt(1), source => source?[1]);

        Test("abcde", source => source.ElementAt(1), source => source[1]);
        TestNullable("abcde", source => source?.ElementAt(1), source => source?[1]);

        DoNamedTest2();
    }

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp100)]
    [TestNet60]
    public void TestElementAtOrDefault() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp130)]
    [TestNet90]
    [SuppressMessage("ReSharper", "UseIndexer")]
    public void TestFirst()
    {
        Test([1, 2, 3], source => source.First(), source => source[0]);
        TestNullable([1, 2, 3], source => source?.First(), source => source?[0]);

        Test(["one", "two", "three"], source => source.First(), source => source[0]);
        TestNullable(["one", "two", "three"], source => source?.First(), source => source?[0]);

        Test("abcde", source => source.First(), source => source[0]);
        TestNullable("abcde", source => source?.First(), source => source?[0]);

        DoNamedTest2();
    }

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp130)]
    [NullableContext(NullableContextKind.Enable)]
    [TestNet90]
    public void TestFirstOrDefault()
    {
        Test([1, 2, 3], source => source.FirstOrDefault(), source => source is [var first, ..] ? first : 0, false);
        Test(["one", "two", "three"], source => source.FirstOrDefault(), source => source is [var first, ..] ? first : null, false);
        Test("abcde", source => source.FirstOrDefault(), source => source is [var first, ..] ? first : default, false);

        Test([1, 2, 3], source => source.FirstOrDefault(-1), source => source is [var first, ..] ? first : -1, false);
        Test(["one", "two", "three"], source => source.FirstOrDefault("zero"), source => source is [var first, ..] ? first : "zero", false);
        Test("abcde", source => source.FirstOrDefault('x'), source => source is [var first, ..] ? first : 'x', false);

        DoNamedTest2();
    }

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp130)]
    [TestNet90]
    [SuppressMessage("ReSharper", "UseIndexer")]
    public void TestLast()
    {
        Test([1, 2, 3], source => source.Last(), source => source[^1]);
        TestNullable([1, 2, 3], source => source?.Last(), source => source?[^1]);

        Test(["one", "two", "three"], source => source.Last(), source => source[^1]);
        TestNullable(["one", "two", "three"], source => source?.Last(), source => source?[^1]);

        Test("abcde", source => source.Last(), source => source[^1]);
        TestNullable("abcde", source => source?.Last(), source => source?[^1]);

        DoNamedTest2();
    }

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp130)]
    [NullableContext(NullableContextKind.Enable)]
    [TestNet90]
    public void TestLastOrDefault()
    {
        Test([1, 2, 3], source => source.LastOrDefault(), source => source is [.., var last] ? last : 0, false);
        Test(["one", "two", "three"], source => source.LastOrDefault(), source => source is [.., var last] ? last : null, false);
        Test("abcde", source => source.LastOrDefault(), source => source is [.., var last] ? last : default, false);

        Test([1, 2, 3], source => source.LastOrDefault(-1), source => source is [.., var last] ? last : -1, false);
        Test(["one", "two", "three"], source => source.LastOrDefault("zero"), source => source is [.., var last] ? last : "zero", false);
        Test("abcde", source => source.LastOrDefault('x'), source => source is [.., var last] ? last : 'x', false);

        DoNamedTest2();
    }

    [Test]
    [SuppressMessage("ReSharper", "UseCollectionProperty")]
    public void TestLongCount()
    {
        Test([1, 2, 3], source => source.LongCount(), source => source.Count, false);
        TestNullable([1, 2, 3], source => source?.LongCount(), source => source?.Count, false);

        Test(["one", "two", "three"], source => source.LongCount(), source => source.Count, false);
        TestNullable(["one", "two", "three"], source => source?.LongCount(), source => source?.Count, false);

        Test("abcde", source => source.LongCount(), source => source.Length, false);
        TestNullable("abcde", source => source?.LongCount(), source => source?.Length, false);

        DoNamedTest2();
    }

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp130)]
    [NullableContext(NullableContextKind.Enable)]
    [TestNet90]
    public void TestSingle()
    {
        Test([1], source => source.Single(), source => source is [var item] ? item : throw new InvalidOperationException());
        Test(["one"], source => source.Single(), source => source is [var item] ? item : throw new InvalidOperationException());
        Test("a", source => source.Single(), source => source is [var item] ? item : throw new InvalidOperationException());

        DoNamedTest2();
    }

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp130)]
    [NullableContext(NullableContextKind.Enable)]
    [TestNet90]
    public void TestSingleOrDefault()
    {
        Test(
            [1],
            source => source.SingleOrDefault(),
            source => source switch
            {
                [] => 0,
                [var item] => item,
                _ => throw new InvalidOperationException(),
            },
            false);

        Test(
            ["one"],
            source => source.SingleOrDefault(),
            source => source switch
            {
                [] => null,
                [var item] => item,
                _ => throw new InvalidOperationException(),
            },
            false);

        Test(
            "a",
            source => source.SingleOrDefault(),
            source => source switch
            {
                [] => default,
                [var item] => item,
                _ => throw new InvalidOperationException(),
            },
            false);

        DoNamedTest2();
    }
}