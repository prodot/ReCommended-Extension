using JetBrains.ReSharper.Feature.Services.Daemon;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.LockOnObjectWithWeakIdentity;

namespace ReCommendedExtension.Tests.Analyzers.LockOnObjectWithWeakIdentity;

[TestFixture]
public sealed class LockOnObjectWithWeakIdentityAnalyzerTests : CSharpAnalyzerTests
{
    protected override string RelativeTestDataPath => @"Analyzers\LockOnObjectWithWeakIdentity";

    protected override bool UseHighlighting(IHighlighting highlighting) => highlighting is LockOnObjectWithWeakIdentityWarning;

    [Test]
    public void TestLockOnObjectWithWeakIdentity() => DoNamedTest2();
}