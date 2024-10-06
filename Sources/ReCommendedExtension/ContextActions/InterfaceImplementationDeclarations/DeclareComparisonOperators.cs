using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Feature.Services.CSharp.ContextActions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.TextControl;
using ReCommendedExtension.Analyzers.InterfaceImplementation;

namespace ReCommendedExtension.ContextActions.InterfaceImplementationDeclarations;

[ContextAction(
    GroupType = typeof(CSharpContextActions),
    Name = "Declare IComparisonOperators<T, T, bool>" + ZoneMarker.Suffix,
    Description = "Declare IComparisonOperators<T, T, bool>.")]
public sealed class DeclareComparisonOperators(ICSharpContextActionDataProvider provider) : ContextAction<IClassLikeDeclaration>(provider)
{
    IClassLikeDeclaration? declaration;
    ITypeElement? comparisonOperatorsInterface;

    [MemberNotNullWhen(true, nameof(declaration))]
    [MemberNotNullWhen(true, nameof(comparisonOperatorsInterface))]
    protected override bool IsAvailable(IClassLikeDeclaration selectedElement)
    {
        if (selectedElement.GetCSharpLanguageLevel() >= CSharpLanguageLevel.CSharp110
            && ClrTypeNames.IComparisonOperators.TryGetTypeElement(PsiModule) is { } comparisonOperatorsInterface
            && selectedElement.DeclaredElement is { })
        {
            var (_, _, declaresComparable, declaresComparisonOperators) = InterfaceImplementationAnalyzer.GetInterfaces(
                selectedElement.DeclaredElement,
                TypeFactory.CreateType(selectedElement.DeclaredElement),
                null,
                comparisonOperatorsInterface,
                PredefinedType.GENERIC_ICOMPARABLE_FQN.TryGetTypeElement(PsiModule));

            declaration = selectedElement is IClassDeclaration or IStructDeclaration or IRecordDeclaration
                && declaresComparable
                && !declaresComparisonOperators
                    ? selectedElement
                    : null;

            this.comparisonOperatorsInterface = comparisonOperatorsInterface;
        }
        else
        {
            declaration = null;
            this.comparisonOperatorsInterface = null;
        }

        return declaration is { };
    }

    public override string Text
    {
        get
        {
            Debug.Assert(declaration is { DeclaredElement: { } });
            Debug.Assert(CSharpLanguage.Instance is { });

            var name = TypeFactory.CreateType(declaration.DeclaredElement).GetPresentableName(CSharpLanguage.Instance);

            return $"Declare IComparisonOperators<{name}, {name}, bool> interface.";
        }
    }

    protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
    {
        try
        {
            Debug.Assert(declaration is { DeclaredElement: { } });
            Debug.Assert(comparisonOperatorsInterface is { });

            var type = TypeFactory.CreateType(declaration.DeclaredElement);

            declaration.AddSuperInterface(
                TypeFactory.CreateType(comparisonOperatorsInterface, [type, type, PredefinedType.BOOLEAN_FQN.GetType(PsiModule)]),
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