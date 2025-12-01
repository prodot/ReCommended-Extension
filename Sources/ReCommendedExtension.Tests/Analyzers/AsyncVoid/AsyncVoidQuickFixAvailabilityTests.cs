using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.AsyncVoid;

namespace ReCommendedExtension.Tests.Analyzers.AsyncVoid;

[TestFixture]
[TestNetFramework45]
public sealed class AsyncVoidQuickFixAvailabilityTests : QuickFixAvailabilityTests
{
    protected override string RelativeTestDataPath => @"Analyzers\AsyncVoid\QuickFixes";

    protected override bool UseHighlighting(IHighlighting highlighting)
        => highlighting is AsyncVoidFunctionExpressionWarning or AvoidAsyncVoidWarning;

    [Test]
    public void TestAnonymousMethodAvailability() => DoNamedTest2();

    [Test]
    public void TestLambdaExpressionAvailability() => DoNamedTest2();

    [Test]
    public void TestAsyncVoidMethodAvailability() => DoNamedTest2();

    [Test]
    public void TestAsyncVoidLocalFunctionAvailability() => DoNamedTest2();
}