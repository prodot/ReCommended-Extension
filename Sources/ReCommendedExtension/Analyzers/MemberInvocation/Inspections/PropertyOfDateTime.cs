using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace ReCommendedExtension.Analyzers.MemberInvocation.Inspections;

internal sealed record PropertyOfDateTime : Inspection
{
    public static PropertyOfDateTime NowDate { get; } = new()
    {
        Condition = reference
            => reference.Resolve().DeclaredElement is IProperty
            {
                AccessibilityDomain.DomainType: AccessibilityDomain.AccessibilityDomainType.PUBLIC,
                IsStatic: true,
                ShortName: nameof(DateTime.Now),
            } property
            && property.ContainingType.IsClrType(PredefinedType.DATETIME_FQN),
        Message = propertyName => $"Use the '{propertyName}' property.",
    };

    public required Func<IReferenceExpressionReference, bool> Condition { get; init; }

    public string? Name { get; init; }
}