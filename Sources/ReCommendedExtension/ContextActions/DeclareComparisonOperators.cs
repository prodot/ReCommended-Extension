using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Feature.Services.CSharp.ContextActions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.TextControl;
using JetBrains.Util;
using ReCommendedExtension.Analyzers.InterfaceImplementation;

namespace ReCommendedExtension.ContextActions;

[ContextAction(
    Group = "C#",
    Name = "Declare IComparisonOperators<T, T, bool>" + ZoneMarker.Suffix,
    Description = "Declare IComparisonOperators<T, T, bool>.")]
public sealed class DeclareComparisonOperators(ICSharpContextActionDataProvider provider) : ContextActionBase
{
    IClassLikeDeclaration? declaration;
    ITypeElement? comparisonOperatorsInterface;

    [MemberNotNullWhen(true, nameof(declaration))]
    [MemberNotNullWhen(true, nameof(comparisonOperatorsInterface))]
    public override bool IsAvailable(IUserDataHolder cache)
    {
        if (provider.GetSelectedElement<IClassLikeDeclaration>(true, false) is { } declaration
            && declaration.GetCSharpLanguageLevel() >= CSharpLanguageLevel.CSharp110
            && InterfaceImplementationAnalyzer.TryGetComparisonOperatorsInterface(declaration.GetPsiModule()) is { } comparisonOperatorsInterface
            && declaration.DeclaredElement is { })
        {
            var type = TypeFactory.CreateType(declaration.DeclaredElement);

            var (_, _, declaresComparable, declaresComparisonOperators) = InterfaceImplementationAnalyzer.GetInterfaces(
                declaration,
                type,
                null,
                comparisonOperatorsInterface,
                TypeElementUtil.GetTypeElementByClrName(ClrTypeNames.IComparableGeneric, declaration.GetPsiModule()));

            this.declaration =
                declaration is IClassDeclaration or IStructDeclaration or IRecordDeclaration && declaresComparable && !declaresComparisonOperators
                    ? declaration
                    : null;

            this.comparisonOperatorsInterface = comparisonOperatorsInterface;
        }
        else
        {
            this.declaration = null;
            this.comparisonOperatorsInterface = null;
        }

        return this.declaration is { };
    }

    public override string Text
    {
        get
        {
            Debug.Assert(declaration is { });

            return $"Declare IComparisonOperators<{declaration.DeclaredName}, {declaration.DeclaredName}, bool> interface.";
        }
    }

    protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
    {
        try
        {
            Debug.Assert(declaration is { DeclaredElement: { } });
            Debug.Assert(comparisonOperatorsInterface is { });

            var psiModule = declaration.GetPsiModule();

            var type = TypeFactory.CreateType(declaration.DeclaredElement);

            declaration.AddSuperInterface(
                TypeFactory.CreateType(
                    comparisonOperatorsInterface,
                    new IType[] { type, type, TypeFactory.CreateTypeByCLRName(PredefinedType.BOOLEAN_FQN, psiModule) }),
                false);

            return _ => { };
        }
        finally
        {
            declaration = null;
            comparisonOperatorsInterface = null;
        }
    }
}