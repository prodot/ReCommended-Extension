using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.Analyzers.ArgumentExceptionConstructorArgument;

[ElementProblemAnalyzer(typeof(IObjectCreationExpression), HighlightingTypes = new[] { typeof(ArgumentExceptionConstructorArgumentWarning) })]
public sealed class ArgumentExceptionConstructorArgumentAnalyzer : ElementProblemAnalyzer<IObjectCreationExpression>
{
    protected override void Run(IObjectCreationExpression element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
    {
        var declaredElement = element.ConstructorReference.Resolve().DeclaredElement;

        if (declaredElement is IConstructor constructor)
        {
            Debug.Assert(constructor.ContainingType is { });

            var typeName = constructor.ContainingType.GetClrName();

            if ((typeName.Equals(PredefinedType.ARGUMENTEXCEPTION_FQN)
                    || typeName.Equals(PredefinedType.ARGUMENTNULLEXCEPTION_FQN)
                    || typeName.Equals(PredefinedType.ARGUMENTOUTOFRANGEEXCEPTION_FQN))
                && element.Arguments.FirstOrDefault(a => a.MatchingParameter is { Element.ShortName : "message" }) is { } messageArgument
                && element.GetContainingTypeMemberDeclarationIgnoringClosures() is
                {
                    DeclaredElement: IParametersOwner { Parameters: { } parameters },
                })
            {
                switch (messageArgument.Value)
                {
                    case ILiteralExpression literalExpression:
                        if (literalExpression.Literal.GetText() is ['\"', .. var parameterName, '\"']
                            && parameters.Any(p => p.ShortName == parameterName))
                        {
                            consumer.AddHighlighting(
                                new ArgumentExceptionConstructorArgumentWarning(
                                    "Parameter name used for the exception message.",
                                    messageArgument));
                        }
                        break;

                    case IInvocationExpression invocationExpression:
                        if ((invocationExpression.InvokedExpression as IReferenceExpression)?.Reference.GetName() == "nameof"
                            && invocationExpression.Arguments is [{ Value: IReferenceExpression referenceExpression }]
                            && referenceExpression.Reference.Resolve().DeclaredElement is IParameter parameter
                            && parameters.Contains(parameter))
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