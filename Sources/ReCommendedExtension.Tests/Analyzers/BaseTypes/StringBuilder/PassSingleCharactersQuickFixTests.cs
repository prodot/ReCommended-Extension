using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.StringBuilder;

[TestFixture]
public sealed class PassSingleCharactersQuickFixTests : QuickFixTestBase<PassSingleCharactersFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\StringBuilderQuickFixes";

    [Test]
    public void TestReplace_String_String_SingleChar() => DoNamedTest2();

    [Test]
    public void TestReplace_String_String_Int32_Int32_SingleChar() => DoNamedTest2();
}