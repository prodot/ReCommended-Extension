using JetBrains.ProjectModel.Properties.CSharp;
using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.MemberInvocation;

namespace ReCommendedExtension.Tests.Analyzers.MemberInvocation;

[TestFixture]
[TestNet50]
public sealed class UsePropertyQuickFixTests : QuickFixTestBase<UsePropertySuggestion.Fix>
{
    protected override string RelativeTestDataPath => @"Analyzers\MemberInvocation\QuickFixes";

    [Test]
    public void UsePropertyFix() => DoNamedTest();

    [Test]
    [NullableContext(NullableContextKind.Enable)]
    public void UsePropertyFix_Nullable() => DoNamedTest();

    [Test]
    [NullableContext(NullableContextKind.Enable)]
    public void UsePropertyFix_Extension_NoCast() => DoNamedTest();

    [Test]
    [NullableContext(NullableContextKind.Enable)]
    public void UsePropertyFix_Extension_NoCast_Nullable() => DoNamedTest();

    [Test]
    [NullableContext(NullableContextKind.Enable)]
    public void UsePropertyFix_Extension_NoCast_NonNullable() => DoNamedTest();

    [Test]
    [NullableContext(NullableContextKind.Enable)]
    public void UsePropertyFix_Extension_Cast() => DoNamedTest();

    [Test]
    [NullableContext(NullableContextKind.Enable)]
    public void UsePropertyFix_Extension_Cast_Nullable() => DoNamedTest();
}