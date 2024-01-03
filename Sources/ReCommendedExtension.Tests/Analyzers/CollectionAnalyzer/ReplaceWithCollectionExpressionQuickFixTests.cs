using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.Collection;

namespace ReCommendedExtension.Tests.Analyzers.CollectionAnalyzer;

[CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
[TestNet80]
[TestFixture]
public sealed class ReplaceWithCollectionExpressionQuickFixTests : QuickFixTestBase<ReplaceWithCollectionExpressionFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\CollectionQuickFixes";

    [Test]
    public void TestCollectionInitialization_TargetArray_Field() => DoNamedTest2();

    [Test]
    public void TestCollectionInitialization_TargetArray_MethodArgument() => DoNamedTest2();

    [Test]
    public void TestCollectionInitialization_TargetArray_GenericMethodArgument_InferredTypeArguments_1() => DoNamedTest2();

    [Test]
    public void TestCollectionInitialization_TargetArray_GenericMethodArgument_InferredTypeArguments_2() => DoNamedTest2();

    [Test]
    public void TestCollectionInitialization_TargetEnumerable_Field_Empty_1() => DoNamedTest2();

    [Test]
    public void TestCollectionInitialization_TargetEnumerable_Field_Empty_2() => DoNamedTest2();

    [Test]
    public void TestCollectionInitialization_TargetEnumerable_Field() => DoNamedTest2();

    [Test]
    public void TestCollectionInitialization_TargetEnumerable_MethodArgument() => DoNamedTest2();

    [Test]
    public void TestCollectionInitialization_TargetEnumerable_GenericMethodArgument_NoInferredTypeArguments() => DoNamedTest2();

    [Test]
    public void TestCollectionInitialization_TargetEnumerable_GenericMethodArgument_InferredTypeArguments() => DoNamedTest2();

    [Test]
    public void TestCollectionInitialization_TargetEnumerable_GenericMethodArgument() => DoNamedTest2();

    [Test]
    public void TestCollectionInitialization_TargetEnumerable_Variable() => DoNamedTest2();

    [Test]
    public void TestCollectionInitialization_TargetEnumerable_MethodArgument_Empty_GenericType() => DoNamedTest2();

    [Test]
    public void TestCollectionInitialization_TargetEnumerable_MethodArgument_GenericType() => DoNamedTest2();

    [Test]
    public void TestCollectionInitialization_TargetEnumerable_Property_GenericType() => DoNamedTest2();
}