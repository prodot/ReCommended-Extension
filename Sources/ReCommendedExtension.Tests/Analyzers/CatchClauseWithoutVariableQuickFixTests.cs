using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.CatchClauseWithoutVariable;

namespace ReCommendedExtension.Tests.Analyzers;

[TestFixture]
public sealed class CatchClauseWithoutVariableQuickFixTests : QuickFixTestBase<RemoveExceptionTypeDeclarationFromCatchClauseFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\CatchClauseWithoutVariableQuickFixes";

    [Test]
    public void TestCatchClauseWithoutVariable() => DoNamedTest2();

    [Test]
    public void TestCatchClauseWithoutVariable2() => DoNamedTest2();
}