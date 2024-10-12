using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.Strings;

namespace ReCommendedExtension.Tests.Analyzers.Strings;

[TestFixture]
public sealed class StringAnalyzerTests : CSharpHighlightingTestBase
{
    protected override string RelativeTestDataPath => @"Analyzers\Strings";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is UseExpressionResultSuggestion
            or UseAsCharacterSuggestion
            or UseStringListPatternSuggestion
            or UseOtherMethodSuggestion
            or RedundantArgumentSuggestion;

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp73)]
    [TestNetCore21]
    public void TestContains() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
    [TestNet70]
    public void TestEndsWith() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
    [TestNet70]
    public void TestIndexOf() => DoNamedTest2();
}

[TestFixture]
public sealed class UseStringQuickFixAvailabilityTests : QuickFixAvailabilityTestBase // todo: move to a separate file
{
    protected override string RelativeTestDataPath => @"Analyzers\StringsQuickFixes";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is UseExpressionResultSuggestion
            or UseAsCharacterSuggestion
            or UseStringListPatternSuggestion
            or UseOtherMethodSuggestion
            or RedundantArgumentSuggestion;

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp73)]
    [TestNetCore21]
    public void TestUseExpressionResultFixAvailability() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp73)]
    [TestNetCore21]
    public void TestUseAsCharacterFixAvailability() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
    [TestNet70]
    public void TestUseListPatternFixAvailability() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
    [TestNet70]
    public void TestUseOtherMethodFixAvailability() => DoNamedTest2();

    [Test]
    public void TestRemoveArgumentFixAvailability() => DoNamedTest2();
}

[TestFixture]
public sealed class UseExpressionResultQuickFixTests : QuickFixTestBase<UseExpressionResultFix> // todo: move to a separate file
{
    protected override string RelativeTestDataPath => @"Analyzers\StringsQuickFixes";

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp73)]
    [TestNetCore21]
    public void TestContains_Empty() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp73)]
    [TestNetCore21]
    public void TestContains_Empty_StringComparison() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp73)]
    [TestNetCore21]
    public void TestContains_Empty_Expression() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp73)]
    [TestNetCore21]
    public void TestEndsWith_Empty() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp73)]
    [TestNetCore21]
    public void TestEndsWith_Empty_StringComparison() => DoNamedTest2();

    [Test]
    public void TestIndexOf_Empty() => DoNamedTest2();
}

[TestFixture]
public sealed class UseAsCharacterQuickFixTests : QuickFixTestBase<UseAsCharacterFix> // todo: move to a separate file
{
    protected override string RelativeTestDataPath => @"Analyzers\StringsQuickFixes";

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp73)]
    [TestNetCore21]
    public void TestContains() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp73)]
    [TestNetCore21]
    public void TestContains_ParameterName() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp73)]
    [TestNetCore21]
    public void TestContains_StringComparison() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp73)]
    [TestNetCore21]
    public void TestContains_StringComparison_ParameterName() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp73)]
    [TestNetCore21]
    public void TestIndexOf_StringAsChar() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp73)]
    [TestNetCore21]
    public void TestIndexOf_StringAsChar_ParameterName() => DoNamedTest2();
}

[TestFixture]
public sealed class UseListPatternQuickFixTests : QuickFixTestBase<UseStringListPatternFix> // todo: move to a separate file
{
    protected override string RelativeTestDataPath => @"Analyzers\StringsQuickFixes";

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
    [TestNet70]
    public void TestEndsWith_Char() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
    [TestNet70]
    public void TestEndsWith_Char_ParameterName() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
    [TestNet70]
    public void TestEndsWith_Argument() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
    [TestNet70]
    public void TestEndsWith_Argument_ParameterName() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
    [TestNet70]
    public void TestEndsWith_String_Ordinal() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
    [TestNet70]
    public void TestEndsWith_String_OrdinalIgnoreCase() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
    [TestNet70]
    public void TestIndexOf_Char_eq_0() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
    [TestNet70]
    public void TestIndexOf_CharConst_eq_0() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
    [TestNet70]
    public void TestIndexOf_Char_ne_0() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
    [TestNet70]
    public void TestIndexOf_CharConst_ne_0() => DoNamedTest2();
}

[TestFixture]
public sealed class UseOtherMethodQuickFixTests : QuickFixTestBase<UseOtherMethodFix> // todo: move to a separate file
{
    protected override string RelativeTestDataPath => @"Analyzers\StringsQuickFixes";

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp73)]
    [TestNetCore21]
    public void TestIndexOf_Char_gt_m1() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp73)]
    [TestNetCore21]
    public void TestIndexOf_Char_ne_m1() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp73)]
    [TestNetCore21]
    public void TestIndexOf_Char_ge_0() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp73)]
    [TestNetCore21]
    public void TestIndexOf_Char_eq_m1() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp73)]
    [TestNetCore21]
    public void TestIndexOf_Char_lt_0() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp73)]
    [TestNetCore21]
    public void TestIndexOf_Char_StringComparison_gt_m1() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp73)]
    [TestNetCore21]
    public void TestIndexOf_Char_StringComparison_ne_m1() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp73)]
    [TestNetCore21]
    public void TestIndexOf_Char_StringComparison_ge_0() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp73)]
    [TestNetCore21]
    public void TestIndexOf_Char_StringComparison_eq_m1() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp73)]
    [TestNetCore21]
    public void TestIndexOf_Char_StringComparison_lt_0() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp73)]
    [TestNetCore21]
    public void TestIndexOf_String_eq_0() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp73)]
    [TestNetCore21]
    public void TestIndexOf_String_ne_0() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp73)]
    [TestNetCore21]
    public void TestIndexOf_String_gt_m1() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp73)]
    [TestNetCore21]
    public void TestIndexOf_String_ne_m1() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp73)]
    [TestNetCore21]
    public void TestIndexOf_String_ge_0() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp73)]
    [TestNetCore21]
    public void TestIndexOf_String_eq_m1() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp73)]
    [TestNetCore21]
    public void TestIndexOf_String_lt_0() => DoNamedTest2();
}

[TestFixture]
public sealed class RemoveArgumentQuickFixTests : QuickFixTestBase<RemoveArgumentFix> // todo: move to a separate file
{
    protected override string RelativeTestDataPath => @"Analyzers\StringsQuickFixes";

    [Test]
    public void TestIndexOf_Char_Int32() => DoNamedTest2();

    [Test]
    public void TestIndexOf_Char_Int32_ParameterName() => DoNamedTest2();
}