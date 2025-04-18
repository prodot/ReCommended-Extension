﻿using JetBrains.ProjectModel.Properties.CSharp;
using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.Strings;

namespace ReCommendedExtension.Tests.Analyzers.StringBuilder;

[TestFixture]
[CSharpLanguageLevel(CSharpLanguageLevel.CSharp73)]
[NullableContext(NullableContextKind.Enable)]
[TestNetCore21]
public sealed class UseOtherMethodQuickFixTests : QuickFixTestBase<UseOtherMethodFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\StringBuilderQuickFixes";

    [Test]
    public void TestAppendJoin_OneItemArray() => DoNamedTest2();

    [Test]
    public void TestAppendJoin_OneItemArray_Nullable() => DoNamedTest2();
}