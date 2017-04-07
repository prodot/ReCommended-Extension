using System;
using System.Diagnostics;
using JetBrains.Annotations;
using JetBrains.ReSharper.Daemon.Stages.Dispatcher;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReCommendedExtension.Highlightings;

namespace ReCommendedExtension.Analyzers
{
    [ElementProblemAnalyzer(typeof(IInvocationExpression),
        HighlightingTypes = new[] { typeof(NotifyPropertyChangedInvocatorFromConstructorHighlighting) })]
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

            var method = invocationExpression.Reference?.Resolve().DeclaredElement as IMethod;
            if (method == null)
            {
                return false; // cannot analyze
            }

            var notifyPropertyChangedAnnotationProvider =
                method.GetPsiServices().GetCodeAnnotationsCache().GetProvider<NotifyPropertyChangedAnnotationProvider>();

            return notifyPropertyChangedAnnotationProvider.ContainsNotifyPropetyChangedInvocatorAttribute(method); // true if annotated with [NotifyPropertyChangedInvocator]
        }

        protected override void Run(IInvocationExpression element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
        {
            if (IsNotifyPropertyChangedInvocatorFromConstructor(element))
            {
                var typeName = NotifyPropertyChangedAnnotationProvider.NotifyPropertyChangedInvocatorAttributeShortName;

                Debug.Assert(typeName != null);

                consumer.AddHighlighting(
                    new NotifyPropertyChangedInvocatorFromConstructorHighlighting(
                        element,
                        string.Format(
                            "Invocation of a method annotated with [{0}] from a constructor is redundant.",
                            typeName.EndsWith("Attribute", StringComparison.Ordinal)
                                ? typeName.Substring(0, typeName.Length - "Attribute".Length)
                                : typeName)));
            }
        }
    }
}