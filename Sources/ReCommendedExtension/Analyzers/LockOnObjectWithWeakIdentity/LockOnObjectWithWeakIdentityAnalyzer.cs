using JetBrains.Metadata.Reader.API;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.CSharp.Util;
using JetBrains.ReSharper.Psi.Util;

namespace ReCommendedExtension.Analyzers.LockOnObjectWithWeakIdentity;

[ElementProblemAnalyzer(typeof(ILockStatement), HighlightingTypes = new[] { typeof(LockOnObjectWithWeakIdentityWarning) })]
public sealed class LockOnObjectWithWeakIdentityAnalyzer : ElementProblemAnalyzer<ILockStatement>
{
    static readonly IClrTypeName[] classTypes =
    {
        // remoting
        PredefinedType.MARSHAL_BY_REF_OBJECT_FQN,

        // reflection
        ClrTypeNames.MemberInfo, ClrTypeNames.ParameterInfo,

        // exceptions
        ClrTypeNames.OutOfMemoryException, ClrTypeNames.StackOverflowException, ClrTypeNames.ExecutionEngineException,

        // threading
        PredefinedType.THREAD_FQN,
    };

    static string? TryGetHighlightingMessage(ICSharpExpression monitor)
    {
        if (monitor.GetOperandThroughParenthesis() is IThisExpression)
        {
            return "Do not lock on 'this'.";
        }

        var monitorType = monitor.GetExpressionType().ToIType();

        if (monitorType.IsString())
        {
            return "Do not lock on strings.";
        }

        if (monitorType is IArrayType arrayType && arrayType.ElementType.IsValueType())
        {
            return "Do not lock on arrays of value types.";
        }

        if (monitorType.GetTypeElement() is { } monitorTypeElement)
        {
            Debug.Assert(CSharpLanguage.Instance is { });

            var psiModule = monitor.GetPsiModule();

            foreach (var type in classTypes)
            {
                var objectType = (IClass?)type.TryGetTypeElement(psiModule);
                Debug.Assert(objectType is { });

                if (monitorTypeElement.IsDescendantOf(objectType))
                {
                    var typeName = objectType.WithIdSubstitution().GetPresentableName(CSharpLanguage.Instance);

                    return objectType switch
                    {
                        { IsAbstract: true } => $"Do not lock on objects derived from the '{typeName}' class.",
                        { IsSealed: true } => $"Do not lock on '{typeName}' objects.",
                        _ => $"Do not lock on '{typeName}' objects or objects derived from the '{typeName}' class.",
                    };
                }
            }
        }

        return null;
    }

    protected override void Run(ILockStatement element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
    {
        if (element.Monitor is { } && TryGetHighlightingMessage(element.Monitor) is { } message)
        {
            consumer.AddHighlighting(new LockOnObjectWithWeakIdentityWarning(message, element.Monitor));
        }
    }
}