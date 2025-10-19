using System.Text;
using JetBrains.Application.Settings;
using JetBrains.ProjectModel.Properties.CSharp;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.Method;
using ReCommendedExtension.Tests.Missing;

namespace ReCommendedExtension.Tests.Analyzers.Method;

[TestFixture]
public sealed class MethodAnalyzerTests : CSharpHighlightingTestBase
{
    protected override string RelativeTestDataPath => @"Analyzers\Method";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is RedundantMethodInvocationHint || highlighting.IsError();

    static void Test<T, R>(Func<T, R> expected, Func<T, R> actual, T[] args)
    {
        foreach (var a in args)
        {
            Assert.AreEqual(expected(a), actual(a), $"with value: {a}");
        }
    }

    [Test]
    [SuppressMessage("ReSharper", "RedundantMethodInvocation")]
    public void TestBoolean()
    {
        var values = new[] { true, false };

        // redundant method invocation

        Test(flag => flag.Equals(true), flag => flag, values);

        DoNamedTest2();
    }

    [Test]
    [SuppressMessage("ReSharper", "RedundantMethodInvocation")]
    public void TestDateTime()
    {
        var values = new[]
        {
            DateTime.MinValue,
            DateTime.MaxValue,
            new(2025, 7, 15, 21, 33, 0, 123),
            new(2025, 7, 15, 21, 33, 0, 123, DateTimeKind.Local),
            new(2025, 7, 15, 21, 33, 0, 123, DateTimeKind.Utc),
        };

        // redundant method invocation

        Test(dateTime => dateTime.AddTicks(0), dateTime => dateTime, values);

        DoNamedTest2();
    }

    [Test]
    [SuppressMessage("ReSharper", "RedundantMethodInvocation")]
    public void TestDateTimeOffset()
    {
        var values = new[]
        {
            DateTimeOffset.MinValue,
            DateTimeOffset.MaxValue,
            new(2025, 7, 15, 21, 33, 0, 123, TimeSpan.Zero),
            new(2025, 7, 15, 21, 33, 0, 123, TimeSpan.FromHours(2)),
            new(2025, 7, 15, 21, 33, 0, 123, TimeSpan.FromHours(-6)),
        };

        // redundant method invocation

        Test(dateTimeOffset => dateTimeOffset.AddTicks(0), dateTimeOffset => dateTimeOffset, values);

        DoNamedTest2();
    }

    [Test]
    [TestNet60]
    public void TestDateOnly()
    {
        var values = new[] { DateOnly.MinValue, DateOnly.MaxValue, new(2025, 7, 15) };

        // redundant method invocation

        Test(dateOnly => dateOnly.AddDays(0), dateOnly => dateOnly, values);

        DoNamedTest2();
    }

    [Test]
    [TestNetCore21]
    [SuppressMessage("ReSharper", "RedundantMethodInvocation")]
    public void TestString()
    {
        var values = new[] { null, "", "abcde", "  abcde  ", "ab;cd;e", "ab;cd:e", "..abcde.." };

        // redundant method invocation

        Test(text => text?.PadLeft(0), text => text, values);
        Test(text => text?.PadLeft(0, 'x'), text => text, values);

        Test(text => text?.PadRight(0), text => text, values);
        Test(text => text?.PadRight(0, 'x'), text => text, values);

        Test(text => text?.Replace('c', 'c'), text => text, values);
        Test(text => text?.Replace("cd", "cd"), text => text, values);
        Test(text => text?.Replace("cd", "cd", StringComparison.Ordinal), text => text, values);

        Test(text => text?.Substring(0), text => text, values);

        DoNamedTest2();
    }

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [NullableContext(NullableContextKind.Enable)]
    [TestNet90]
    [SuppressMessage("ReSharper", "RedundantMethodInvocation")]
    public void TestStringBuilder()
    {
        var values = new[] { "", "abcde" };

        // redundant method invocation

        Test(value => new StringBuilder(value).Append(null as string).ToString(), value => new StringBuilder(value).ToString(), values);
        Test(value => new StringBuilder(value).Append("").ToString(), value => new StringBuilder(value).ToString(), values);
        Test(value => new StringBuilder(value).Append(null as char[]).ToString(), value => new StringBuilder(value).ToString(), values);
        Test(value => new StringBuilder(value).Append([]).ToString(), value => new StringBuilder(value).ToString(), values);
        Test(value => new StringBuilder(value).Append(null as object).ToString(), value => new StringBuilder(value).ToString(), values);
        Test(value => new StringBuilder(value).Append(null as StringBuilder).ToString(), value => new StringBuilder(value).ToString(), values);
        Test(value => new StringBuilder(value).Append('x', 0).ToString(), value => new StringBuilder(value).ToString(), values);
        Test(value => new StringBuilder(value).Append(null as string, 0, 0).ToString(), value => new StringBuilder(value).ToString(), values);
        Test(value => new StringBuilder(value).Append("abcde", 1, 0).ToString(), value => new StringBuilder(value).ToString(), values);
        Test(value => new StringBuilder(value).Append(null as StringBuilder, 0, 0).ToString(), value => new StringBuilder(value).ToString(), values);
        Test(
            value => new StringBuilder(value).Append(new StringBuilder("abcde"), 1, 0).ToString(),
            value => new StringBuilder(value).ToString(),
            values);
        Test(value => new StringBuilder(value).Append(null as char[], 0, 0).ToString(), value => new StringBuilder(value).ToString(), values);
        Test(value => new StringBuilder(value).Append([], 0, 0).ToString(), value => new StringBuilder(value).ToString(), values);

        Test(
            value => new StringBuilder(value).AppendJoin(',', (IEnumerable<int>)[]).ToString(),
            value => new StringBuilder(value).ToString(),
            values);
        Test(
            value => new StringBuilder(value).AppendJoin(',', (string[])[]).ToString(),
            value => new StringBuilder(value).ToString(),
            values);
        Test(
            value => new StringBuilder(value).AppendJoin(',', (object[])[]).ToString(),
            value => new StringBuilder(value).ToString(),
            values);
        Test(
            value => new StringBuilder(value).AppendJoin(',', (ReadOnlySpan<string?>)[]).ToString(),
            value => new StringBuilder(value).ToString(),
            values);
        Test(
            value => new StringBuilder(value).AppendJoin(',', (ReadOnlySpan<object?>)[]).ToString(),
            value => new StringBuilder(value).ToString(),
            values);
        Test(
            value => new StringBuilder(value).AppendJoin(", ", (IEnumerable<int>)[]).ToString(),
            value => new StringBuilder(value).ToString(),
            values);
        Test(
            value => new StringBuilder(value).AppendJoin(", ", (string[])[]).ToString(),
            value => new StringBuilder(value).ToString(),
            values);
        Test(
            value => new StringBuilder(value).AppendJoin(", ", (object[])[]).ToString(),
            value => new StringBuilder(value).ToString(),
            values);
        Test(
            value => new StringBuilder(value).AppendJoin(", ", (ReadOnlySpan<string?>)[]).ToString(),
            value => new StringBuilder(value).ToString(),
            values);
        Test(
            value => new StringBuilder(value).AppendJoin(", ", (ReadOnlySpan<object?>)[]).ToString(),
            value => new StringBuilder(value).ToString(),
            values);

        Test(value => new StringBuilder(value).Insert(0, null as object).ToString(), value => new StringBuilder(value).ToString(), values);

        Test(value => new StringBuilder(value).Replace('c', 'c').ToString(), value => new StringBuilder(value).ToString(), values);
        Test(value => new StringBuilder(value).Replace("cd", "cd").ToString(), value => new StringBuilder(value).ToString(), values);

        DoNamedTest2();
    }
}