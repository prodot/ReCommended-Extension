using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.AsyncVoid;

namespace ReCommendedExtension.Tests.Analyzers.AsyncVoid;

[TestFixture]
[TestNetFramework45]
public sealed class AsyncVoidQuickFixChangeTypeTests : QuickFixTestBase<ChangeToAsyncTaskFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\AsyncVoid\QuickFixes";

    [Test]
    public void TestAsyncVoidMethod() => DoNamedTest2();

    [Test]
    public void TestAsyncVoidLocalFunction() => DoNamedTest2();
}