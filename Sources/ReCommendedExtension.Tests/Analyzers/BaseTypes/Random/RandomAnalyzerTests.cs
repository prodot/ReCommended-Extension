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

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.Random;

[TestFixture]
[NullableContext(NullableContextKind.Enable)]
public sealed class RandomAnalyzerTests : CSharpHighlightingTestBase
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\Random";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is UseExpressionResultSuggestion or RedundantArgumentHint || highlighting.IsError();

    static void Test<R>(Func<System.Random, R> expected, Func<System.Random, R> actual, int? seed = null)
    {
        if (seed is { } s)
        {
            Assert.AreEqual(expected(new System.Random(s)), actual(new System.Random(s)));
        }
        else
        {
            var random = new System.Random();

            Assert.AreEqual(expected(random), actual(random));
        }
    }

    static void TestNullable<R>(Func<System.Random?, R> expected, Func<System.Random?, R> actual, int? seed = null)
    {
        Assert.AreEqual(expected(null), actual(null));

        if (seed is { } s)
        {
            Assert.AreEqual(expected(new System.Random(s)), actual(new System.Random(s)));
        }
        else
        {
            var random = new System.Random();

            Assert.AreEqual(expected(random), actual(random));
        }
    }

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNet80]
    public void TestGetItems()
    {
        Test(random => random.GetItems((int[])[], 0), _ => []);
        Test(random => random.GetItems(new[] { 1, 2, 3 }, 0), _ => []);

        Test(random => random.GetItems(default(ReadOnlySpan<int>), 0), _ => []);
        Test(random => random.GetItems(new ReadOnlySpan<int>(), 0), _ => []);
        Test(random => random.GetItems((ReadOnlySpan<int>)[], 0), _ => []);
        Test(random => random.GetItems([1, 2, 3], 0), _ => []);

        DoNamedTest2();
    }

    [Test]
    [SuppressMessage("ReSharper", "RedundantArgument")]
    public void TestNext()
    {
        Test(random => random.Next(int.MaxValue), random => random.Next(), 1);
        Test(random => random.Next(0, 10), random => random.Next(10), 1);

        TestNullable(random => random?.Next(int.MaxValue), random => random?.Next(), 1);
        TestNullable(random => random?.Next(0, 10), random => random?.Next(10), 1);

        Test(random => random.Next(0), _ => 0);
        Test(random => random.Next(1), _ => 0);
        Test(random => random.Next(10, 10), _ => 10);
        Test(random => random.Next(10, 11), _ => 10);

        DoNamedTest2();
    }

    [Test]
    [TestNet60]
    public void TestNextInt64()
    {
        Test(random => random.NextInt64(long.MaxValue), random => random.NextInt64(), 1);
        Test(random => random.NextInt64(0, 10), random => random.NextInt64(10), 1);

        TestNullable(random => random?.NextInt64(long.MaxValue), random => random?.NextInt64(), 1);
        TestNullable(random => random?.NextInt64(0, 10), random => random?.NextInt64(10), 1);

        Test(random => random.NextInt64(0), _ => 0);
        Test(random => random.NextInt64(1), _ => 0);
        Test(random => random.NextInt64(10, 10), _ => 10);
        Test(random => random.NextInt64(10, 11), _ => 10);

        DoNamedTest2();
    }
}