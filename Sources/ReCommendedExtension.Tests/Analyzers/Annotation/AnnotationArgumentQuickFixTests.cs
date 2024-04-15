﻿using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.Annotation;

namespace ReCommendedExtension.Tests.Analyzers.Annotation;

[TestFixture]
public sealed class AnnotationArgumentQuickFixTests : QuickFixTestBase<RemoveAttributeArgumentFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\AnnotationQuickFixes";

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    [TestNet80(ANNOTATIONS_PACKAGE)]
    public void TestRedundantAnnotationArgument() => DoNamedTest2();
}