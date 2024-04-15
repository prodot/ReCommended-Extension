using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.Collection;

namespace ReCommendedExtension.Tests.Analyzers.Collection;

[CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
[TestNet80]
[TestFixture]
public sealed class ReplaceWithCollectionExpressionQuickFixTests : QuickFixTestBase<ReplaceWithCollectionExpressionFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\CollectionQuickFixes";

    [Test]
    public void TestCollectionInitialization_Array_Target_Array_Field() => DoNamedTest2();

    [Test]
    public void TestCollectionInitialization_Array_Target_Array_MethodArgument() => DoNamedTest2();

    [Test]
    public void TestCollectionInitialization_Array_Target_Array_GenericMethodArgument_InferredTypeArguments_1() => DoNamedTest2();

    [Test]
    public void TestCollectionInitialization_Array_Target_Array_GenericMethodArgument_InferredTypeArguments_2() => DoNamedTest2();

    [Test]
    public void TestCollectionInitialization_Array_Target_Array_GenericMethodArgument_InferredTypeArguments_3() => DoNamedTest2();

    [Test]
    public void TestCollectionInitialization_Array_Target_IEnumerable_Field_Empty_1() => DoNamedTest2();

    [Test]
    public void TestCollectionInitialization_Array_Target_IEnumerable_Field_Empty_2() => DoNamedTest2();

    [Test]
    public void TestCollectionInitialization_Array_Target_IEnumerable_Field() => DoNamedTest2();

    [Test]
    public void TestCollectionInitialization_Array_Target_IEnumerable_MethodArgument() => DoNamedTest2();

    [Test]
    public void TestCollectionInitialization_Array_Target_IEnumerable_GenericMethodArgument_NoInferredTypeArguments() => DoNamedTest2();

    [Test]
    public void TestCollectionInitialization_Array_Target_IEnumerable_GenericMethodArgument_InferredTypeArguments() => DoNamedTest2();

    [Test]
    public void TestCollectionInitialization_Array_Target_IEnumerable_GenericMethodArgument() => DoNamedTest2();

    [Test]
    public void TestCollectionInitialization_Array_Target_IEnumerable_Variable() => DoNamedTest2();

    [Test]
    public void TestCollectionInitialization_Array_Target_IEnumerable_MethodArgument_Empty_GenericType() => DoNamedTest2();

    [Test]
    public void TestCollectionInitialization_Array_Target_IEnumerable_MethodArgument_GenericType() => DoNamedTest2();

    [Test]
    public void TestCollectionInitialization_Array_Target_IEnumerable_Property_GenericType() => DoNamedTest2();

    [Test]
    public void TestCollectionInitialization_List_Target_IEnumerable_Field() => DoNamedTest2();

    [Test]
    public void TestCollectionInitialization_List_Target_IEnumerable_Field_Empty_1() => DoNamedTest2();

    [Test]
    public void TestCollectionInitialization_List_Target_IEnumerable_Field_Empty_2() => DoNamedTest2();

    [Test]
    public void TestCollectionInitialization_List_Target_IEnumerable_Variable_1() => DoNamedTest2();

    [Test]
    public void TestCollectionInitialization_List_Target_IEnumerable_Variable_2() => DoNamedTest2();

    [Test]
    public void TestCollectionInitialization_List_Target_IEnumerable_GenericMethodArgument_InferredTypeArguments() => DoNamedTest2();

    [Test]
    public void TestCollectionInitialization_List_Target_IEnumerable_GenericMethodArgument_NoInferredTypeArguments() => DoNamedTest2();

    [Test]
    public void TestCollectionInitialization_List_Target_IEnumerable_GenericMethodArgument() => DoNamedTest2();

    [Test]
    public void TestCollectionInitialization_Dictionary_Target_Dictionary_Field() => DoNamedTest2();

    [Test]
    public void TestCollectionInitialization_Dictionary_Target_Dictionary_Property() => DoNamedTest2();

    [Test]
    public void TestCollectionInitialization_Dictionary_Target_Dictionary_MethodArgument() => DoNamedTest2();

    [Test]
    public void TestCollectionInitialization_Dictionary_Target_Dictionary_GenericMethodArgument_InferredTypeArguments() => DoNamedTest2();
}