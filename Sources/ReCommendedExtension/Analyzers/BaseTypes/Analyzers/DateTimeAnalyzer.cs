using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReCommendedExtension.Extensions;
using ReCommendedExtension.Extensions.MethodFinding;

namespace ReCommendedExtension.Analyzers.BaseTypes.Analyzers;

/// <remarks>
/// C# language version checks are only done when a quick fix would require it.
/// </remarks>
[ElementProblemAnalyzer(
    typeof(ICSharpExpression),
    HighlightingTypes = [typeof(UseExpressionResultSuggestion), typeof(UseDateTimePropertySuggestion)])]
public sealed class DateTimeAnalyzer : ElementProblemAnalyzer<ICSharpExpression>
{
    /// <remarks>
    /// <c>new DateTime(0)</c> → <c>DateTime.MinValue</c>
    /// </remarks>
    static void Analyze_Ctor_Int64(IHighlightingConsumer consumer, IObjectCreationExpression objectCreationExpression, ICSharpArgument ticksArgument)
    {
        if (!objectCreationExpression.IsUsedAsStatement() && ticksArgument.Value.TryGetInt64Constant() == 0)
        {
            consumer.AddHighlighting(
                new UseExpressionResultSuggestion(
                    $"The expression is always {nameof(DateTime)}.{nameof(DateTime.MinValue)}.",
                    objectCreationExpression,
                    $"{nameof(DateTime)}.{nameof(DateTime.MinValue)}"));
        }
    }

    /// <remarks>
    /// <c>DateTime.Now.Date</c> → <c>DateTime.Today</c>
    /// </remarks>
    static void AnalyzeDate(IHighlightingConsumer consumer, IReferenceExpression referenceExpression)
    {
        if (!referenceExpression.IsPropertyAssignment()
            && !referenceExpression.IsWithinNameofExpression()
            && referenceExpression.QualifierExpression is IReferenceExpression
            {
                Reference: var reference, QualifierExpression: var qualifierExpression,
            }
            && reference.Resolve().DeclaredElement is IProperty
            {
                AccessibilityDomain.DomainType: AccessibilityDomain.AccessibilityDomainType.PUBLIC,
                IsStatic: true,
                ShortName: nameof(DateTime.Now),
            } property
            && property.ContainingType.IsClrType(PredefinedType.DATETIME_FQN)
            && PredefinedType.DATETIME_FQN.HasProperty(
                new PropertySignature { Name = nameof(DateTime.Today), IsStatic = true },
                referenceExpression.GetPsiModule()))
        {
            consumer.AddHighlighting(
                new UseDateTimePropertySuggestion(
                    $"Use the '{nameof(DateTime.Today)}' property.",
                    reference,
                    qualifierExpression,
                    referenceExpression,
                    nameof(DateTime.Today)));
        }
    }

    /// <remarks>
    /// <c>dateTime.Equals(null)</c> → <c>false</c>
    /// </remarks>
    static void AnalyzeEquals_Object(IHighlightingConsumer consumer, IInvocationExpression invocationExpression, ICSharpArgument valueArgument)
    {
        if (!invocationExpression.IsUsedAsStatement() && valueArgument.Value.IsDefaultValue())
        {
            consumer.AddHighlighting(new UseExpressionResultSuggestion("The expression is always false.", invocationExpression, "false"));
        }
    }

    /// <remarks>
    /// <c>dateTime.GetTypeCode()</c> → <c>TypeCode.DateTime</c>
    /// </remarks>
    static void AnalyzeGetTypeCode(IHighlightingConsumer consumer, IInvocationExpression invocationExpression)
    {
        if (!invocationExpression.IsUsedAsStatement())
        {
            consumer.AddHighlighting(
                new UseExpressionResultSuggestion(
                    $"The expression is always {nameof(TypeCode)}.{nameof(TypeCode.DateTime)}.",
                    invocationExpression,
                    $"{nameof(TypeCode)}.{nameof(TypeCode.DateTime)}"));
        }
    }

    protected override void Run(ICSharpExpression element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
    {
        switch (element)
        {
            case IObjectCreationExpression { ConstructorReference: var reference } objectCreationExpression
                when reference.Resolve().DeclaredElement is IConstructor
                {
                    AccessibilityDomain.DomainType: AccessibilityDomain.AccessibilityDomainType.PUBLIC, IsStatic: false,
                } constructor
                && constructor.ContainingType.IsClrType(PredefinedType.DATETIME_FQN):

                switch (constructor.Parameters, objectCreationExpression.TryGetArgumentsInDeclarationOrder())
                {
                    case ([{ Type: var ticksType }], [{ } ticksArgument]) when ticksType.IsLong():
                        Analyze_Ctor_Int64(consumer, objectCreationExpression, ticksArgument);
                        break;
                }
                break;

            case IInvocationExpression
                {
                    InvokedExpression: IReferenceExpression { Reference: var reference } invokedExpression,
                } invocationExpression
                when reference.Resolve().DeclaredElement is IMethod
                {
                    AccessibilityDomain.DomainType: AccessibilityDomain.AccessibilityDomainType.PUBLIC, TypeParameters: [],
                } method
                && method.ContainingType.IsClrType(PredefinedType.DATETIME_FQN):

                switch (invokedExpression, method)
                {
                    case ({ QualifierExpression: { } }, { IsStatic: false }):
                        switch (method.ShortName)
                        {
                            case nameof(DateTime.Equals):
                                switch (method.Parameters, invocationExpression.TryGetArgumentsInDeclarationOrder())
                                {
                                    case ([{ Type: var valueType }], [{ } valueArgument]) when valueType.IsObject():
                                        AnalyzeEquals_Object(consumer, invocationExpression, valueArgument);
                                        break;
                                }
                                break;

                            case nameof(DateTime.GetTypeCode):
                                switch (method.Parameters, invocationExpression.TryGetArgumentsInDeclarationOrder())
                                {
                                    case ([], []): AnalyzeGetTypeCode(consumer, invocationExpression); break;
                                }
                                break;
                        }
                        break;
                }
                break;

            case IReferenceExpression { Reference: var reference } referenceExpression
                when reference.Resolve().DeclaredElement is IProperty
                {
                    AccessibilityDomain.DomainType: AccessibilityDomain.AccessibilityDomainType.PUBLIC, IsStatic: false,
                } property
                && property.ContainingType.IsClrType(PredefinedType.DATETIME_FQN):

                switch (property.ShortName)
                {
                    case nameof(DateTime.Date): AnalyzeDate(consumer, referenceExpression); break;
                }
                break;
        }
    }
}