using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.Strings;

namespace ReCommendedExtension.Tests.Analyzers.StringBuilder;

[TestFixture]
public sealed class PassSingleCharactersQuickFixTests : QuickFixTestBase<PassSingleCharactersFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\StringBuilderQuickFixes";

    [Test]
    public void TestReplace_String_String_SingleChar() => DoNamedTest2();

    [Test]
    public void TestReplace_String_String_Int32_Int32_SingleChar() => DoNamedTest2();
}