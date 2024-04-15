using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.Analyzers.NotifyPropertyChangedInvocatorFromConstructor;

[ElementProblemAnalyzer(typeof(IInvocationExpression), HighlightingTypes = [typeof(NotifyPropertyChangedInvocatorFromConstructorWarning)])]
public sealed class NotifyPropertyChangedInvocatorFromConstructorAnalyzer : ElementProblemAnalyzer<IInvocationExpression>
{
    [Pure]
    static bool IsUnderAnonymousMethod(ITreeNode? element)
    {
        foreach (var treeNode in element.ContainingNodes())
        {
            if (treeNode is IAnonymousFunctionExpression or IQueryParameterPlatform)
            {
                return true;
            }
        }

        return false;
    }

    [Pure]
    static bool IsNotifyPropertyChangedInvocatorFromConstructor(IInvocationExpression invocationExpression)
    {
        var containingTypeMemberDeclaration = invocationExpression.GetContainingTypeMemberDeclarationIgnoringClosures();
        if (containingTypeMemberDeclaration is not IConstructorDeclaration)
        {
            return false; // not a constructor => do not highlight
        }

        if (IsUnderAnonymousMethod(invocationExpression))
        {
            return false; // called from an anonymous method or a lambda expression => do not highlight
        }

        if (invocationExpression.Reference.Resolve().DeclaredElement is not IMethod method)
        {
            return false; // cannot analyze
        }

        var notifyPropertyChangedAnnotationProvider =
            method.GetPsiServices().GetCodeAnnotationsCache().GetProvider<NotifyPropertyChangedAnnotationProvider>();

        return notifyPropertyChangedAnnotationProvider
            .HasNotifyPropertyChangedInvocatorAttribute(method); // true if annotated with [NotifyPropertyChangedInvocator]
    }

    protected override void Run(IInvocationExpression element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
    {
        if (IsNotifyPropertyChangedInvocatorFromConstructor(element))
        {
            var name = nameof(NotifyPropertyChangedInvocatorAttribute).WithoutSuffix();

            consumer.AddHighlighting(
                new NotifyPropertyChangedInvocatorFromConstructorWarning(
                    element,
                    $"Invocation of a method annotated with [{name}] from a constructor is redundant."));
        }
    }
}