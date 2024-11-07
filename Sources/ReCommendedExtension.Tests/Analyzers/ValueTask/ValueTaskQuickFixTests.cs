using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.ValueTask;

namespace ReCommendedExtension.Tests.Analyzers.ValueTask;

[TestFixture]
[TestNetCore30]
public sealed class ValueTaskQuickFixTests : QuickFixTestBase<InsertAsTaskFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\ValueTaskQuickFixes";

    [Test]
    public void TestInsertAsTask() => DoNamedTest2();
}