using System.Linq;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.Analyzers.ArgumentExceptionConstructorArgument
{
    [ElementProblemAnalyzer(typeof(IObjectCreationExpression), HighlightingTypes = new[] { typeof(ArgumentExceptionConstructorArgumentWarning) })]
    public sealed class ArgumentExceptionConstructorArgumentAnalyzer : ElementProblemAnalyzer<IObjectCreationExpression>
    {
        protected override void Run(IObjectCreationExpression element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
        {
            var declaredElement = element.ConstructorReference.Resolve().DeclaredElement;

            if (declaredElement is IConstructor constructor)
            {
                var typeName = constructor.GetContainingType().AssertNotNull().GetClrName();

                if (typeName.Equals(PredefinedType.ARGUMENTEXCEPTION_FQN) ||
                    typeName.Equals(PredefinedType.ARGUMENTNULLEXCEPTION_FQN) ||
                    typeName.Equals(PredefinedType.ARGUMENTOUTOFRANGEEXCEPTION_FQN))
                {
                    var messageArgument = element.Arguments.FirstOrDefault(a => a.AssertNotNull().MatchingParameter?.Element.ShortName == "message");
                    if (messageArgument != null)
                    {
                        var parameters = ((IParametersOwner)element.GetContainingTypeMemberDeclarationIgnoringClosures()?.DeclaredElement)
                            ?.Parameters;
                        if (parameters != null)
                        {
                            switch (messageArgument.Value)
                            {
                                case ILiteralExpression literalExpression:
                                    var literalText = literalExpression.Literal?.GetText();
                                    if (literalText != null &&
                                        literalText.Length > 2 &&
                                        literalText[0] == '\"' &&
                                        literalText[literalText.Length - 1] == '\"' &&
                                        parameters.Any(p => p.AssertNotNull().ShortName == literalText.Substring(1, literalText.Length - 2)))
                                    {
                                        consumer.AddHighlighting(
                                            new ArgumentExceptionConstructorArgumentWarning(
                                                "Parameter name used for the exception message.",
                                                messageArgument));
                                    }
                                    break;

                                case IInvocationExpression invocationExpression:
                                    if ((invocationExpression.InvokedExpression as IReferenceExpression)?.Reference.GetName() == @"nameof" &&
                                        invocationExpression.Arguments.Count == 1 &&
                                        (invocationExpression.Arguments[0].AssertNotNull().Value as IReferenceExpression)?.Reference.Resolve()
                                        .DeclaredElement is IParameter parameter &&
                                        parameters.Contains(parameter))
                                    {
                                        consumer.AddHighlighting(
                                            new ArgumentExceptionConstructorArgumentWarning(
                                                "Parameter name used for the exception message.",
                                                messageArgument));
                                    }

                                    break;
                            }
                        }
                    }
                }
            }
        }
    }
}