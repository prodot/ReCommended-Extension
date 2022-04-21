using System;
using System.Diagnostics;
using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace ReCommendedExtension.Analyzers.NotifyPropertyChangedInvocatorFromConstructor
{
    [ElementProblemAnalyzer(
        typeof(IInvocationExpression),
        HighlightingTypes = new[] { typeof(NotifyPropertyChangedInvocatorFromConstructorWarning) })]
    public sealed class NotifyPropertyChangedInvocatorFromConstructorAnalyzer : ElementProblemAnalyzer<IInvocationExpression>
    {
        static bool IsNotifyPropertyChangedInvocatorFromConstructor([NotNull] IInvocationExpression invocationExpression)
        {
            var containingTypeMemberDeclaration = invocationExpression.GetContainingTypeMemberDeclarationIgnoringClosures();
            if (!(containingTypeMemberDeclaration is IConstructorDeclaration))
            {
                return false; // not a constructor => do not highlight
            }

            if (invocationExpression.IsUnderAnonymousMethod())
            {
                return false; // called from an anonymous method or a lambda expression => do not highlight
            }

            if (!(invocationExpression.Reference.Resolve().DeclaredElement is IMethod method))
            {
                return false; // cannot analyze
            }

            var notifyPropertyChangedAnnotationProvider =
                method.GetPsiServices().GetCodeAnnotationsCache().GetProvider<NotifyPropertyChangedAnnotationProvider>();

            return notifyPropertyChangedAnnotationProvider
                .ContainsNotifyPropertyChangedInvocatorAttribute(method); // true if annotated with [NotifyPropertyChangedInvocator]
        }

        protected override void Run(IInvocationExpression element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
        {
            if (IsNotifyPropertyChangedInvocatorFromConstructor(element))
            {
                var typeName = NotifyPropertyChangedAnnotationProvider.NotifyPropertyChangedInvocatorAttributeShortName;

                Debug.Assert(typeName != null);

                var attributeName = typeName.EndsWith("Attribute", StringComparison.Ordinal)
                    ? typeName.Substring(0, typeName.Length - "Attribute".Length)
                    : typeName;

                consumer.AddHighlighting(
                    new NotifyPropertyChangedInvocatorFromConstructorWarning(
                        element,
                        $"Invocation of a method annotated with [{attributeName}] from a constructor is redundant."));
            }
        }
    }
}