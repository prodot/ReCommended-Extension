using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.Boolean;

[TestFixture]
public sealed class BooleanAnalyzerTests : BaseTypeAnalyzerTests<bool>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\Boolean";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is UseExpressionResultSuggestion or UseBinaryOperatorSuggestion or RedundantMethodInvocationHint or RedundantArgumentHint
            || highlighting.IsError();

    protected override bool[] TestValues { get; } = [true, false];

    [Test]
    [SuppressMessage("ReSharper", "UseExpressionResult")]
    [SuppressMessage("ReSharper", "RedundantMethodInvocation")]
    [SuppressMessage("ReSharper", "UseBinaryOperator")]
    public void TestEquals()
    {
        Test(flag => flag.Equals(true), flag => flag);
        Test(flag => flag.Equals(false), flag => !flag);
        Test(obj => true.Equals(obj), obj => obj);
        Test(obj => false.Equals(obj), obj => !obj);
        Test((flag, obj) => flag.Equals(obj), (flag, obj) => flag == obj, TestValues, TestValues);

        Test(flag => flag.Equals(null), _ => false);

        DoNamedTest2();
    }

    [Test]
    [SuppressMessage("ReSharper", "UseExpressionResult")]
    public void TestGetTypeCode()
    {
        Test(flag => flag.GetTypeCode(), _ => TypeCode.Boolean);

        DoNamedTest2();
    }

    [Test]
    [SuppressMessage("ReSharper", "RedundantArgument")]
    public void TestToString()
    {
        Test(flag => flag.ToString(null), flag => flag.ToString());
        Test((flag, provider) => flag.ToString(provider), (flag, _) => flag.ToString(), TestValues, FormatProviders);

        DoNamedTest2();
    }
}