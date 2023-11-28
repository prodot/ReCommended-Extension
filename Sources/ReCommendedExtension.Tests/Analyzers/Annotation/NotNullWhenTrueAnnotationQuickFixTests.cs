using JetBrains.ProjectModel.Properties.CSharp;
using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.Annotation;

namespace ReCommendedExtension.Tests.Analyzers.Annotation;

[TestFixture]
public sealed class NotNullWhenTrueAnnotationQuickFixTests : QuickFixTestBase<AnnotateWithNotNullWhenTrueFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\AnnotationQuickFixes";

    [Test]
    [TestNetCore30]
    [NullableContext(NullableContextKind.Enable)]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp100)]
    public void TestAnnotateWithNotNullWhenTrue() => DoNamedTest2();
}