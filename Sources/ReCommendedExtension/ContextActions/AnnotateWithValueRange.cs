using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Feature.Services.CSharp.ContextActions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.TextControl;

namespace ReCommendedExtension.ContextActions;

[ContextAction(
    Group = "C#",
    Name = "Annotate with [ValueRange(...)] attribute" + ZoneMarker.Suffix,
    Description = "Annotates with the [ValueRange(...)] attribute.")]
public sealed class AnnotateWithValueRange(ICSharpContextActionDataProvider provider) : AnnotateWithCodeAnnotation(provider)
{
    [Pure]
    static bool IsAvailableForType(IType type)
        => type.IsInt() || type.IsLong() || type.IsShort() || type.IsSbyte() || type.IsUint() || type.IsUlong() || type.IsUshort() || type.IsByte();

    protected override bool AllowsMultiple => true;

    protected override string AnnotationAttributeTypeName => nameof(ValueRangeAttribute);

    protected override AttributeValue[] AnnotationArguments => new[] { new AttributeValue(ConstantValue.NOT_COMPILE_TIME_CONSTANT) };

    protected override bool CanBeAnnotated(IDeclaredElement? declaredElement, ITreeNode context)
        => declaredElement switch
        {
            IMethod method => IsAvailableForType(method.ReturnType),
            IProperty property => IsAvailableForType(property.Type),
            IField { IsConstant: false } field => IsAvailableForType(field.Type),
            IParameter parameter => IsAvailableForType(parameter.Type),
            IDelegate @delegate => IsAvailableForType(@delegate.InvokeMethod.ReturnType),

            _ => false,
        };

    protected override void ExecutePsiTransactionPostProcess(ITextControl textControl, IAttribute attribute)
    {
        Debug.Assert(attribute.Arguments is [{ }]);

        textControl.Caret.MoveTo(attribute.Arguments[0].GetDocumentRange().EndOffset, CaretVisualPlacement.DontScrollIfVisible);
        textControl.EmulateAction("TextControl.Backspace");
    }
}