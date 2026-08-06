using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.Collection;

namespace ReCommendedExtension.Tests.Analyzers.Collection;

[TestFixture]
[CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
[TestNet80]
public sealed class ReplaceWithCollectionExpressionQuickFixTests : QuickFixTestBase<UseTargetTypedCollectionExpressionSuggestion.Fix>
{
    protected override string RelativeTestDataPath => @"Analyzers\Collection\QuickFixes";

    [Test]
    public void CollectionInitialization_Array_Target_Array_Field() => DoNamedTest();

    [Test]
    public void CollectionInitialization_Array_Target_Array_MethodArgument() => DoNamedTest();

    [Test]
    public void CollectionInitialization_Array_Target_Array_GenericMethodArgument_InferredTypeArguments_1() => DoNamedTest();

    [Test]
    public void CollectionInitialization_Array_Target_Array_GenericMethodArgument_InferredTypeArguments_2() => DoNamedTest();

    [Test]
    public void CollectionInitialization_Array_Target_Array_GenericMethodArgument_InferredTypeArguments_3() => DoNamedTest();

    [Test]
    public void CollectionInitialization_Array_Target_IEnumerable_Field() => DoNamedTest();

    [Test]
    public void CollectionInitialization_Array_Target_IEnumerable_GenericMethodArgument() => DoNamedTest();

    [Test]
    public void CollectionInitialization_List_Target_IEnumerable_Field() => DoNamedTest();

    [Test]
    public void CollectionInitialization_List_Target_IEnumerable_Field_Empty_1() => DoNamedTest();

    [Test]
    public void CollectionInitialization_List_Target_IEnumerable_Field_Empty_2() => DoNamedTest();

    [Test]
    public void CollectionInitialization_List_Target_IEnumerable_Variable_1() => DoNamedTest();

    [Test]
    public void CollectionInitialization_List_Target_IEnumerable_Variable_2() => DoNamedTest();

    [Test]
    public void CollectionInitialization_List_Target_IEnumerable_GenericMethodArgument_InferredTypeArguments() => DoNamedTest();

    [Test]
    public void CollectionInitialization_List_Target_IEnumerable_GenericMethodArgument_NoInferredTypeArguments() => DoNamedTest();

    [Test]
    public void CollectionInitialization_List_Target_IEnumerable_GenericMethodArgument() => DoNamedTest();

    [Test]
    public void CollectionInitialization_Dictionary_Target_Dictionary_Field() => DoNamedTest();

    [Test]
    public void CollectionInitialization_Dictionary_Target_Dictionary_Property() => DoNamedTest();

    [Test]
    public void CollectionInitialization_Dictionary_Target_Dictionary_MethodArgument() => DoNamedTest();

    [Test]
    public void CollectionInitialization_Dictionary_Target_Dictionary_GenericMethodArgument_InferredTypeArguments() => DoNamedTest();
}