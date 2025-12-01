using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.ValueTask;

namespace ReCommendedExtension.Tests.Analyzers.ValueTask;

[TestFixture]
[TestNetCore30]
public sealed class ValueTaskAnalyzerTests : CSharpAnalyzerTests
{
    protected override string RelativeTestDataPath => @"Analyzers\ValueTask";

    protected override bool UseHighlighting(IHighlighting highlighting)
        => highlighting is PossibleMultipleConsumptionWarning or IntentionalBlockingAttemptWarning;

    [Test]
    public void TestCodePaths() => DoNamedTest2();

    [Test]
    public void TestSingleConsumption() => DoNamedTest2();

    [Test]
    public void TestMultipleConsumption() => DoNamedTest2();

    [Test]
    public void TestIntentionalBlockingAttempts() => DoNamedTest2();
}