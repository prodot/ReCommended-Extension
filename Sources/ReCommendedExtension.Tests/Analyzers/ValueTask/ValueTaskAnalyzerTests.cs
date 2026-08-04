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
    public void CodePaths() => DoNamedTest();

    [Test]
    public void SingleConsumption() => DoNamedTest();

    [Test]
    public void MultipleConsumption() => DoNamedTest();

    [Test]
    public void IntentionalBlockingAttempts() => DoNamedTest();
}