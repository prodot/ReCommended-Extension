using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.NullCoalescingAssignment;

namespace ReCommendedExtension.Tests.Analyzers
{
    [TestFixture]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp80)]
    public sealed class NullCoalescingAssignmentQuickFixTests : QuickFixTestBase<ReplaceIfStatementWithNullCoalescingAssignmentFix>
    {
        protected override string RelativeTestDataPath => @"Analyzers\NullCoalescingAssignmentQuickFixes";

        [Test]
        public void TestEqualityComparison() => DoNamedTest2();

        [Test]
        public void TestEqualityComparisonBlock() => DoNamedTest2();

        [Test]
        public void TestReverseEqualityComparison() => DoNamedTest2();

        [Test]
        public void TestReverseEqualityComparisonBlock() => DoNamedTest2();

        [Test]
        public void TestIsComparison() => DoNamedTest2();

        [Test]
        public void TestIsComparisonBlock() => DoNamedTest2();
    }
}