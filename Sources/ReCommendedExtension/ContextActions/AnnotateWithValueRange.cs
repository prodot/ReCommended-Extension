using System.Diagnostics;
using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Feature.Services.CSharp.Analyses.Bulbs;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.TextControl;

namespace ReCommendedExtension.ContextActions
{
    [ContextAction(
        Group = "C#",
        Name = "Annotate with [ValueRange(...)] attribute" + ZoneMarker.Suffix,
        Description = "Annotates with the [ValueRange(...)] attribute.")]
    public sealed class AnnotateWithValueRange : AnnotateWithCodeAnnotation
    {
        [Pure]
        static bool IsAvailableForType([NotNull] IType type)
            => type.IsInt() ||
                type.IsLong() ||
                type.IsShort() ||
                type.IsSbyte() ||
                type.IsUint() ||
                type.IsUlong() ||
                type.IsUshort() ||
                type.IsByte();

        public AnnotateWithValueRange([NotNull] ICSharpContextActionDataProvider provider) : base(provider) { }

        protected override bool AllowsMultiple => true;

        protected override string AnnotationAttributeTypeName => nameof(ValueRangeAttribute);

        protected override AttributeValue[] AnnotationArguments => new[] { new AttributeValue(new ConstantValue(0, null as IType)) };

        protected override bool CanBeAnnotated(IDeclaredElement declaredElement, ITreeNode context, IPsiModule psiModule)
        {
            switch (declaredElement)
            {
                case IMethod method: return IsAvailableForType(method.ReturnType);

                case IProperty property: return IsAvailableForType(property.Type);

                case IField field: return IsAvailableForType(field.Type);

                case IParameter parameter: return IsAvailableForType(parameter.Type);

                case IDelegate @delegate: return IsAvailableForType(@delegate.InvokeMethod.ReturnType);
            }

            return false;
        }

        protected override void ExecutePsiTransactionPostProcess(ITextControl textControl, IAttribute attribute)
        {
            Debug.Assert(attribute.Arguments.Count == 1);
            Debug.Assert(attribute.Arguments[0] != null);

            textControl.Caret.MoveTo(attribute.Arguments[0].GetDocumentRange().EndOffset, CaretVisualPlacement.DontScrollIfVisible);
            textControl.EmulateAction("TextControl.Backspace");
        }
    }
}