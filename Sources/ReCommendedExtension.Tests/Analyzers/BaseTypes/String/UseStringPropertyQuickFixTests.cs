using JetBrains.ProjectModel.Properties.CSharp;
using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.String;

[TestFixture]
[CSharpLanguageLevel(CSharpLanguageLevel.CSharp90)]
[TestNet50]
public sealed class UseStringPropertyQuickFixTests : QuickFixTestBase<UseStringPropertyFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\StringsQuickFixes";

    [Test]
    public void TestLastIndexOf_Empty() => DoNamedTest2();

    [Test]
    [NullableContext(NullableContextKind.Enable)]
    public void TestLastIndexOf_Empty_Nullable() => DoNamedTest2();

    [Test]
    public void TestLastIndexOf_Empty_StringComparison() => DoNamedTest2();

    [Test]
    [NullableContext(NullableContextKind.Enable)]
    public void TestLastIndexOf_Empty_StringComparison_Nullable() => DoNamedTest2();
}