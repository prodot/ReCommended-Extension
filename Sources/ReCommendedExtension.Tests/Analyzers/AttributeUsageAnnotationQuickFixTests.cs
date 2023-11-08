using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.Annotation;

namespace ReCommendedExtension.Tests.Analyzers;

[TestFixture]
public sealed class AttributeUsageAnnotationQuickFixTests : QuickFixTestBase<AnnotateWithAttributeUsageFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\AnnotationQuickFixes";

    [Test]
    public void TestAnnotateWithAttributeUsage() => DoNamedTest2();
}