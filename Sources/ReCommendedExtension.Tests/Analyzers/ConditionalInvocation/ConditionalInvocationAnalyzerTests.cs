using JetBrains.ReSharper.Feature.Services.Daemon;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.ConditionalInvocation;

namespace ReCommendedExtension.Tests.Analyzers.ConditionalInvocation;

[TestFixture]
public sealed class ConditionalInvocationAnalyzerTests : CSharpAnalyzerTests
{
    protected override string RelativeTestDataPath => @"Analyzers\ConditionalInvocation";

    protected override bool UseHighlighting(IHighlighting highlighting) => highlighting is ConditionalInvocationHint;

    [Test]
    public void TestConditional() => DoNamedTest2();
}