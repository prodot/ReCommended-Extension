using JetBrains.ReSharper.Feature.Services.Daemon;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.DelegateInvoke;

namespace ReCommendedExtension.Tests.Analyzers.DelegateInvoke;

[TestFixture]
public sealed class DelegateInvokeAnalyzerTests : CSharpAnalyzerTests
{
    protected override string RelativeTestDataPath => @"Analyzers\DelegateInvoke";

    protected override bool UseHighlighting(IHighlighting highlighting) => highlighting is RedundantDelegateInvokeHint;

    [Test]
    public void TestDelegateInvoke() => DoNamedTest2();
}