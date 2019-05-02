using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;
using JetBrains.Metadata.Reader.API;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Util;

namespace ReCommendedExtension.Analyzers.LockOnObjectWithWeakIdentity
{
    [ElementProblemAnalyzer(typeof(ILockStatement), HighlightingTypes = new[] { typeof(LockOnObjectWithWeakIdentityHighlighting) })]
    public sealed class LockOnObjectWithWeakIdentityAnalyzer : ElementProblemAnalyzer<ILockStatement>
    {
        [NotNull]
        [ItemNotNull]
        static IEnumerable<IClrTypeName> GetClassTypes()
        {
            yield return PredefinedType.MARSHAL_BY_REF_OBJECT_FQN;

            yield return ClrTypeNames.MemberInfo;
            yield return ClrTypeNames.ParameterInfo;

            yield return ClrTypeNames.OutOfMemoryException;
            yield return ClrTypeNames.StackOverflowException;
            yield return ClrTypeNames.ExecutionEngineException;

            yield return PredefinedType.THREAD_FQN;
        }

        static string TryGetHighlightingMessage([NotNull] ICSharpExpression monitor)
        {
            Debug.Assert(CSharpLanguage.Instance != null);

            var monitorType = monitor.GetExpressionType().ToIType();

            if (monitorType.IsString())
            {
                return "Do not lock on strings.";
            }

            if (monitorType is IArrayType arrayType && arrayType.ElementType.IsValueType())
            {
                return "Do not lock on arrays of value types.";
            }

            var monitorTypeElement = monitorType.GetTypeElement();

            if (monitorTypeElement != null)
            {
                var psiModule = monitor.GetPsiModule();

                foreach (var type in GetClassTypes())
                {
                    var objectType = (IClass)TypeElementUtil.GetTypeElementByClrName(type, psiModule).AssertNotNull();
                    if (monitorTypeElement.IsDescendantOf(objectType))
                    {
                        var typeName = objectType.WithIdSubstitution().GetPresentableName(CSharpLanguage.Instance);

                        if (objectType.IsAbstract)
                        {
                            return $"Do not lock on objects derived from the '{typeName}' class.";
                        }

                        if (objectType.IsSealed)
                        {
                            return $"Do not lock on '{typeName}' objects.";
                        }

                        return $"Do not lock on '{typeName}' objects or objects derived from the '{typeName}' class.";
                    }
                }
            }

            return null;
        }

        protected override void Run(ILockStatement element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
        {
            if (element.Monitor == null)
            {
                return;
            }

            var message = TryGetHighlightingMessage(element.Monitor);
            if (message != null)
            {
                consumer.AddHighlighting(new LockOnObjectWithWeakIdentityHighlighting(message, element.Monitor));
            }
        }
    }
}