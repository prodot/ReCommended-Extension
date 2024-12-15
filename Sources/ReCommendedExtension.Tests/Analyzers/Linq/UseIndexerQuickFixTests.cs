using JetBrains.ProjectModel.Properties.CSharp;
using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.Linq;

namespace ReCommendedExtension.Tests.Analyzers.Linq;

[TestFixture]
[CSharpLanguageLevel(CSharpLanguageLevel.CSharp90)]
[TestNet50]
public sealed class UseIndexerQuickFixTests : QuickFixTestBase<UseIndexerFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\LinqQuickFixes";

    [Test]
    public void TestUseIndexer_First() => DoNamedTest2();

    [Test]
    [NullableContext(NullableContextKind.Enable)]
    public void TestUseIndexer_First_Nullable() => DoNamedTest2();

    [Test]
    public void TestUseIndexer_Last() => DoNamedTest2();

    [Test]
    [NullableContext(NullableContextKind.Enable)]
    public void TestUseIndexer_Last_Nullable() => DoNamedTest2();

    [Test]
    public void TestUseIndexer_ElementAt_Int32() => DoNamedTest2();

    [Test]
    [NullableContext(NullableContextKind.Enable)]
    public void TestUseIndexer_ElementAt_Int32_Nullable() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp100)]
    [TestNet60]
    public void TestUseIndexer_ElementAt_Index() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp100)]
    [NullableContext(NullableContextKind.Enable)]
    [TestNet60]
    public void TestUseIndexer_ElementAt_Index_Nullable() => DoNamedTest2();
}