using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;
using ReCommendedExtension.Tests.Missing;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.TimeSpan;

[TestFixture]
public sealed class TimeSpanAnalyzerTests : BaseTypeAnalyzerTests<System.TimeSpan>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\TimeSpan";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is UseExpressionResultSuggestion or RedundantArgumentHint or UseBinaryOperatorSuggestion || highlighting.IsError();

    protected override System.TimeSpan[] TestValues { get; } =
    [
        System.TimeSpan.Zero,
        System.TimeSpan.MinValue,
        System.TimeSpan.MaxValue,
        new(0, 0, 1),
        new(0, 1, 0),
        new(1, 0, 0),
        new(1, 0, 0, 0, 0),
        new(0, 0, 0, 0, 1),
        new(1, 2, 3, 4),
        new(-1, 2, 3, 4),
    ];

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp90)]
    [TestNet70]
    public void Test_Constructors()
    {
        Test(() => new System.TimeSpan(0), () => System.TimeSpan.Zero);
        Test(() => new System.TimeSpan(0, 0, 0), () => System.TimeSpan.Zero);
        Test(() => new System.TimeSpan(0, 0, 0, 0), () => System.TimeSpan.Zero);
        Test(() => new System.TimeSpan(0, 0, 0, 0, 0), () => System.TimeSpan.Zero);
        Test(() => MissingTimeSpanMembers._Ctor(0, 0, 0, 0, 0, 0), () => System.TimeSpan.Zero);
        Test(() => new System.TimeSpan(0, 1, 2, 3), () => new System.TimeSpan(1, 2, 3));
        Test(() => new System.TimeSpan(1, 2, 3, 4, 0), () => new System.TimeSpan(1, 2, 3, 4));
        Test(() => MissingTimeSpanMembers._Ctor(1, 2, 3, 4, 5, 0), () => new System.TimeSpan(1, 2, 3, 4, 5));
        Test(() => new System.TimeSpan(long.MinValue), () => System.TimeSpan.MinValue);
        Test(() => new System.TimeSpan(long.MaxValue), () => System.TimeSpan.MaxValue);

        DoNamedTest2();
    }

    [Test]
    public void TestAdd()
    {
        var values = TestValues.Except([System.TimeSpan.MinValue, System.TimeSpan.MaxValue]).ToArray();

        Test((timeSpan, ts) => timeSpan.Add(ts), (timeSpan, ts) => timeSpan + ts, values, values);

        DoNamedTest2();
    }

    [Test]
    [TestNetCore20]
    public void TestDivide()
    {
        Test(
            (timeSpan, divisor) => timeSpan.Divide(divisor),
            (timeSpan, divisor) => MissingTimeSpanMembers.op_Divide(timeSpan, divisor),
            [..TestValues.Except([System.TimeSpan.MinValue, System.TimeSpan.MaxValue])],
            [1d, 2d, -1d, double.MaxValue, double.MinValue, double.PositiveInfinity, double.NegativeInfinity]);
        Test((timeSpan, ts) => timeSpan.Divide(ts), (timeSpan, ts) => MissingTimeSpanMembers.op_Divide(timeSpan, ts), TestValues, TestValues);

        DoNamedTest2();
    }

    [Test]
    [SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
    public void TestEquals()
    {
        Test((timeSpan, obj) => timeSpan.Equals(obj), (timeSpan, obj) => timeSpan == obj, TestValues, TestValues);
        Test(timeSpan => timeSpan.Equals(null), _ => false);
        Test((t1, t2) => System.TimeSpan.Equals(t1, t2), (t1, t2) => t1 == t2, TestValues, TestValues);

        DoNamedTest2();
    }

    [Test]
    [TestNet90]
    public void TestFromDays()
    {
        Test(_ => MissingTimeSpanMembers.FromDays(0), _ => System.TimeSpan.Zero);
        Test(_ => MissingTimeSpanMembers.FromDays(0, 0), _ => System.TimeSpan.Zero);

        DoNamedTest2();
    }

    [Test]
    [TestNet90]
    public void TestFromHours()
    {
        Test(_ => MissingTimeSpanMembers.FromHours(0), _ => System.TimeSpan.Zero);
        Test(_ => MissingTimeSpanMembers.FromHours(0, 0), _ => System.TimeSpan.Zero);

        DoNamedTest2();
    }

    [Test]
    [TestNet90]
    public void TestFromMicroseconds()
    {
        Test(_ => MissingTimeSpanMembers.FromMicroseconds(0), _ => System.TimeSpan.Zero);

        DoNamedTest2();
    }

    [Test]
    [TestNet90]
    public void TestFromMilliseconds()
    {
        Test(_ => MissingTimeSpanMembers.FromMilliseconds(0), _ => System.TimeSpan.Zero);

        DoNamedTest2();
    }

    [Test]
    [TestNet90]
    public void TestFromMinutes()
    {
        Test(_ => MissingTimeSpanMembers.FromMinutes(0), _ => System.TimeSpan.Zero);
        Test(_ => MissingTimeSpanMembers.FromMinutes(0, 0), _ => System.TimeSpan.Zero);

        DoNamedTest2();
    }

    [Test]
    [TestNet90]
    public void TestFromSeconds()
    {
        Test(_ => MissingTimeSpanMembers.FromSeconds(0), _ => System.TimeSpan.Zero);
        Test(_ => MissingTimeSpanMembers.FromSeconds(0, 0), _ => System.TimeSpan.Zero);

        DoNamedTest2();
    }

    [Test]
    public void TestFromTicks()
    {
        Test(_ => System.TimeSpan.FromTicks(0), _ => System.TimeSpan.Zero);
        Test(_ => System.TimeSpan.FromTicks(long.MinValue), _ => System.TimeSpan.MinValue);
        Test(_ => System.TimeSpan.FromTicks(long.MaxValue), _ => System.TimeSpan.MaxValue);

        DoNamedTest2();
    }
}