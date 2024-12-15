using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.CSharp.ContextActions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;
using ReCommendedExtension.Extensions;

namespace ReCommendedExtension.ContextActions.Strings;

public abstract class ReplaceIsNullOrEmpty(ICSharpContextActionDataProvider provider) : ContextAction<IInvocationExpression>(provider)
{
    IInvocationExpression? selectedElement;
    ICSharpExpression? valueArgument;

    [MemberNotNullWhen(true, nameof(selectedElement))]
    [MemberNotNullWhen(true, nameof(valueArgument))]
    protected override bool IsAvailable(IInvocationExpression selectedElement)
    {
        this.valueArgument = !selectedElement.IsUsedAsStatement()
            && selectedElement is { InvokedExpression: IReferenceExpression { Reference: var reference }, Arguments: [{ Value: { } valueArgument }] }
            && reference.Resolve().DeclaredElement is IMethod
            {
                IsStatic: true,
                ShortName: nameof(string.IsNullOrEmpty),
                AccessibilityDomain.DomainType: AccessibilityDomain.AccessibilityDomainType.PUBLIC,
                TypeParameters: [],
                Parameters: [{ Type: IDeclaredType valueParameterType }],
            } method
            && method.ContainingType.IsClrType(PredefinedType.STRING_FQN)
            && valueParameterType.IsString()
                ? valueArgument
                : null;

        this.selectedElement = this.valueArgument is { } ? selectedElement : null;

        return this.valueArgument is { };
    }

    private protected abstract string ReplacementPattern { get; }

    public sealed override string Text
    {
        get
        {
            Debug.Assert(valueArgument is { });

            return $"Replace with '{valueArgument.GetText().TrimToSingleLineWithMaxLength(120)} is {ReplacementPattern}'";
        }
    }

    protected sealed override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
    {
        try
        {
            Debug.Assert(selectedElement is { });
            Debug.Assert(valueArgument is { });

            using (WriteLockCookie.Create())
            {
                var factory = CSharpElementFactory.GetInstance(selectedElement);

                ModificationUtil
                    .ReplaceChild(selectedElement, factory.CreateExpression($"($0 is {ReplacementPattern})", valueArgument))
                    .TryRemoveParentheses(factory);
            }

            return _ => { };
        }
        finally
        {
            selectedElement = null;
            valueArgument = null;
        }
    }
}