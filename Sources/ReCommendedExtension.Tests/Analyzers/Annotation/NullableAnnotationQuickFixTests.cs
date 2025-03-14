﻿using JetBrains.ProjectModel.Properties.CSharp;
using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.Annotation;

namespace ReCommendedExtension.Tests.Analyzers.Annotation;

[TestFixture]
[CSharpLanguageLevel(CSharpLanguageLevel.CSharp80)]
[NullableContext(NullableContextKind.Enable)]
[TestNetCore30]
public sealed class NullableAnnotationQuickFixTests : QuickFixTestBase<RemoveNullableAnnotationFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\AnnotationQuickFixes";

    [Test]
    public void TestRedundantNullableAnnotation() => DoNamedTest2();
}