using JetBrains.Application.Settings;
using JetBrains.ProjectModel.Properties.CSharp;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
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
            or PassSingleCharacterSuggestion
            or PassSingleCharactersSuggestion
            or UseStringListPatternSuggestion
            or UseOtherMethodSuggestion
            or RedundantArgumentHint
            or RedundantElementHint
            or UseStringPropertySuggestion
            or RedundantMethodInvocationHint
            or UseRangeIndexerSuggestion
            or RedundantToStringCallWarning // to figure out which cases are supported by R#
            or ReplaceSubstringWithRangeIndexerWarning; // to figure out which cases are supported by R#

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

    [Test]
    public void TestIndexOfAny() => DoNamedTest2();

    [Test]
    public void TestLastIndexOf() => DoNamedTest2();

    [Test]
    public void TestPadLeft() => DoNamedTest2();

    [Test]
    public void TestPadRight() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp80)]
    [TestNetCore30]
    public void TestRemove() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp73)]
    [TestNetCore20]
    public void TestReplace() => DoNamedTest2();

    [Test]
    [NullableContext(NullableContextKind.Enable)]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNet80]
    public void TestSplit() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
    [TestNet70]
    public void TestStartsWith() => DoNamedTest2();

    [Test]
    public void TestSubstring() => DoNamedTest2();

    [Test]
    public void TestToString() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNet80]
    public void TestTrim() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNet80]
    public void TestTrimEnd() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNet80]
    public void TestTrimStart() => DoNamedTest2();
}

[TestFixture]
public sealed class UseStringQuickFixAvailabilityTests : QuickFixAvailabilityTestBase // todo: move to a separate file
{
    protected override string RelativeTestDataPath => @"Analyzers\StringsQuickFixes";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is UseExpressionResultSuggestion
            or PassSingleCharacterSuggestion
            or PassSingleCharactersSuggestion
            or UseStringListPatternSuggestion
            or UseOtherMethodSuggestion
            or RedundantArgumentHint
            or RedundantElementHint
            or UseStringPropertySuggestion
            or RedundantMethodInvocationHint
            or UseRangeIndexerSuggestion;

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNet80]
    public void TestUseExpressionResultFixAvailability() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp73)]
    [TestNetCore21]
    public void TestPassSingleCharacterFixAvailability() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp73)]
    [TestNetCore21]
    public void TestPassSingleCharactersFixAvailability() => DoNamedTest2();

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

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNet80]
    public void TestRemoveElementFixAvailability() => DoNamedTest2();

    [Test]
    public void TestUseStringPropertyAvailability() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp73)]
    [TestNetCore21]
    public void TestRemoveMethodInvocationAvailability() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp80)]
    [TestNetCore30]
    public void TestUseRangeIndexerAvailability() => DoNamedTest2();
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

    [Test]
    public void TestIndexOf_Empty_StringComparison() => DoNamedTest2();

    [Test]
    public void TestLastIndexOf_Char_0() => DoNamedTest2();

    [Test]
    public void TestRemove_0() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp73)]
    [TestNetCore21]
    public void TestSplit_EmptyArray() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNet80]
    public void TestSplit_EmptyArray_CollectionExpression() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp73)]
    [TestNetCore21]
    public void TestSplit_ArrayWithOneItem() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNet80]
    public void TestSplit_ArrayWithOneItem_CollectionExpression() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNet80]
    public void TestSplit_ArrayWithOneTrimmedItem() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNet80]
    public void TestSplit_ArrayWithOneTrimmedItem_CollectionExpression() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp73)]
    [TestNetCore21]
    public void TestStartsWith_Empty() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp73)]
    [TestNetCore21]
    public void TestStartsWith_Empty_StringComparison() => DoNamedTest2();

    [Test]
    public void TestSubstring_Int32_0() => DoNamedTest2();
}

[TestFixture]
public sealed class PassSingleCharacterQuickFixTests : QuickFixTestBase<PassSingleCharacterFix> // todo: move to a separate file
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

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp73)]
    [TestNetCore21]
    public void TestIndexOf_StringAsChar_StringComparison() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp73)]
    [TestNetCore21]
    public void TestIndexOf_StringAsChar_ParameterName_StringComparison() => DoNamedTest2();

    [Test]
    public void TestLastIndexOf_StringAsChar_Ordinal() => DoNamedTest2();

    [Test]
    public void TestLastIndexOf_StringAsChar_ParameterName_Ordinal() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp73)]
    [TestNetCore21]
    public void TestSplit_SingleCharacter() => DoNamedTest2();
}

[TestFixture]
public sealed class PassSingleCharactersQuickFixTests : QuickFixTestBase<PassSingleCharactersFix> // todo: move to a separate file
{
    protected override string RelativeTestDataPath => @"Analyzers\StringsQuickFixes";

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp73)]
    [TestNetCore21]
    public void TestReplace_String_String_Ordinal() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp73)]
    [TestNetCore21]
    public void TestReplace_String_String_Ordinal_ParameterName1() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp73)]
    [TestNetCore21]
    public void TestReplace_String_String_Ordinal_ParameterName2() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp73)]
    [TestNetCore21]
    public void TestReplace_String_String_Ordinal_ParameterNames() => DoNamedTest2();

    [Test]
    public void TestReplace_String_String() => DoNamedTest2();

    [Test]
    public void TestReplace_String_String_ParameterName1() => DoNamedTest2();

    [Test]
    public void TestReplace_String_String_ParameterName2() => DoNamedTest2();

    [Test]
    public void TestReplace_String_String_ParameterNames() => DoNamedTest2();

    [Test]
    public void TestSplit_SingleCharacters() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNet80]
    public void TestSplit_SingleCharacters_CollectionExpression() => DoNamedTest2();
}

[TestFixture]
[CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
[TestNet70]
public sealed class UseListPatternQuickFixTests : QuickFixTestBase<UseStringListPatternFix> // todo: move to a separate file
{
    protected override string RelativeTestDataPath => @"Analyzers\StringsQuickFixes";

    [Test]
    public void TestEndsWith_Char() => DoNamedTest2();

    [Test]
    public void TestEndsWith_Char_ParameterName() => DoNamedTest2();

    [Test]
    public void TestEndsWith_Argument() => DoNamedTest2();

    [Test]
    public void TestEndsWith_Argument_ParameterName() => DoNamedTest2();

    [Test]
    public void TestEndsWith_String_Ordinal() => DoNamedTest2();

    [Test]
    public void TestEndsWith_String_OrdinalIgnoreCase() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
    [TestNet70]
    public void TestIndexOf_Char_eq_0() => DoNamedTest2();

    [Test]
    public void TestIndexOf_CharConst_eq_0() => DoNamedTest2();

    [Test]
    public void TestIndexOf_Char_ne_0() => DoNamedTest2();

    [Test]
    public void TestIndexOf_CharConst_ne_0() => DoNamedTest2();

    [Test]
    public void TestStartsWith_Char() => DoNamedTest2();

    [Test]
    public void TestStartsWith_Char_ParameterName() => DoNamedTest2();

    [Test]
    public void TestStartsWith_Argument() => DoNamedTest2();

    [Test]
    public void TestStartsWith_Argument_ParameterName() => DoNamedTest2();

    [Test]
    public void TestStartsWith_String_Ordinal() => DoNamedTest2();

    [Test]
    public void TestStartsWith_String_OrdinalIgnoreCase() => DoNamedTest2();
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

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp73)]
    [TestNetCore21]
    public void TestIndexOf_String_StringComparison_eq_0() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp73)]
    [TestNetCore21]
    public void TestIndexOf_String_StringComparison_ne_0() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp73)]
    [TestNetCore21]
    public void TestIndexOf_String_StringComparison_gt_m1() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp73)]
    [TestNetCore21]
    public void TestIndexOf_String_StringComparison_ne_m1() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp73)]
    [TestNetCore21]
    public void TestIndexOf_String_StringComparison_ge_0() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp73)]
    [TestNetCore21]
    public void TestIndexOf_String_StringComparison_eq_m1() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp73)]
    [TestNetCore21]
    public void TestIndexOf_String_StringComparison_lt_0() => DoNamedTest2();
}

[TestFixture]
public sealed class RemoveArgumentQuickFixTests : QuickFixTestBase<RemoveArgumentFix> // todo: move to a separate file
{
    protected override string RelativeTestDataPath => @"Analyzers\StringsQuickFixes";

    [Test]
    public void TestIndexOf_Char_Int32() => DoNamedTest2();

    [Test]
    public void TestIndexOf_Char_Int32_ParameterName() => DoNamedTest2();

    [Test]
    public void TestIndexOf_String_Int32() => DoNamedTest2();

    [Test]
    public void TestIndexOf_String_Int32_ParameterName() => DoNamedTest2();

    [Test]
    public void TestIndexOf_String_Int32_StringComparison() => DoNamedTest2();

    [Test]
    public void TestIndexOf_String_Int32_ParameterName_StringComparison() => DoNamedTest2();

    [Test]
    public void TestIndexOfAny() => DoNamedTest2();

    [Test]
    public void TestIndexOfAny_ParameterName() => DoNamedTest2();

    [Test]
    public void TestPadLeft_Int32_Space() => DoNamedTest2();

    [Test]
    public void TestPadRight_Int32_Space() => DoNamedTest2();

    [Test]
    public void TestSplit_DuplicateArgument() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNet80]
    public void TestTrim_Empty() => DoNamedTest2();

    [Test]
    public void TestTrim_EmptyArray() => DoNamedTest2();

    [Test]
    public void TestTrim_EmptyArray_2() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp73)]
    [TestNetCore21]
    public void TestTrim_EmptyArray_3() => DoNamedTest2();

    [Test]
    public void TestTrim_Null() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNet80]
    public void TestTrimEnd_Empty() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp73)]
    [TestNetCore21]
    public void TestTrimEnd_EmptyArray_3() => DoNamedTest2();

    [Test]
    public void TestTrimEnd_Null() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNet80]
    public void TestTrimStart_Empty() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp73)]
    [TestNetCore21]
    public void TestTrimStart_EmptyArray_3() => DoNamedTest2();

    [Test]
    public void TestTrimStart_Null() => DoNamedTest2();

    [Test]
    public void TestTrim_DuplicateArgument() => DoNamedTest2();

    [Test]
    public void TestTrimEnd_DuplicateArgument() => DoNamedTest2();

    [Test]
    public void TestTrimStart_DuplicateArgument() => DoNamedTest2();
}

[TestFixture]
public sealed class RemoveElementQuickFixTests : QuickFixTestBase<RemoveElementFix> // todo: move to a separate file
{
    protected override string RelativeTestDataPath => @"Analyzers\StringsQuickFixes";

    [Test]
    public void TestSplit_DuplicateElement() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNet80]
    public void TestSplit_DuplicateElement_CollectionExpression() => DoNamedTest2();

    [Test]
    public void TestTrim_DuplicateElement() => DoNamedTest2();

    [Test]
    public void TestTrimEnd_DuplicateElement() => DoNamedTest2();

    [Test]
    public void TestTrimStart_DuplicateElement() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNet80]
    public void TestTrim_DuplicateElement_CollectionExpression() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNet80]
    public void TestTrimEnd_DuplicateElement_CollectionExpression() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNet80]
    public void TestTrimStart_DuplicateElement_CollectionExpression() => DoNamedTest2();
}

[TestFixture]
public sealed class UseStringPropertyQuickFixTests : QuickFixTestBase<UseStringPropertyFix> // todo: move to a separate file
{
    protected override string RelativeTestDataPath => @"Analyzers\StringsQuickFixes";

    [Test]
    public void TestLastIndexOf_Empty() => DoNamedTest2();

    [Test]
    public void TestLastIndexOf_Empty_StringComparison() => DoNamedTest2();
}

[TestFixture]
public sealed class RemoveMethodInvocationFixTests : QuickFixTestBase<RemoveMethodInvocationFix> // todo: move to a separate file
{
    protected override string RelativeTestDataPath => @"Analyzers\StringsQuickFixes";

    [Test]
    public void TestPadLeft_0() => DoNamedTest2();

    [Test]
    public void TestPadLeft_0_Char() => DoNamedTest2();

    [Test]
    public void TestPadRight_0() => DoNamedTest2();

    [Test]
    public void TestPadRight_0_Char() => DoNamedTest2();

    [Test]
    public void TestRemove_Int32_0() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp73)]
    [TestNetCore21]
    public void TestReplace_String_String_Ordinal_Identical() => DoNamedTest2();

    [Test]
    public void TestReplace_String_String_Identical() => DoNamedTest2();

    [Test]
    public void TestReplace_Char_Char_Identical() => DoNamedTest2();

    [Test]
    public void TestSubstring_0() => DoNamedTest2();

    [Test]
    public void TestToString_IFormatProvider() => DoNamedTest2();
}

[TestFixture]
[CSharpLanguageLevel(CSharpLanguageLevel.CSharp80)]
[TestNetCore30]
public sealed class UseRangeIndexerFixTests : QuickFixTestBase<UseRangeIndexerFix> // todo: move to a separate file
{
    protected override string RelativeTestDataPath => @"Analyzers\StringsQuickFixes";

    [Test]
    public void TestRemove_Int32() => DoNamedTest2();

    [Test]
    public void TestRemove_Int32_Parenthesized() => DoNamedTest2();

    [Test]
    public void TestRemove_0_Int32() => DoNamedTest2();

    [Test]
    public void TestRemove_0_Int32_Parenthesized() => DoNamedTest2();
}