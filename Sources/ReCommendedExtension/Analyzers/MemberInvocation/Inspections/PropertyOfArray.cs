using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using ReCommendedExtension.Extensions;

namespace ReCommendedExtension.Analyzers.MemberInvocation.Inspections;

internal sealed record PropertyOfArray : Inspection
{
    public static PropertyOfArray QualifierIsArray { get; } = new()
    {
        Condition = args => args is [{ Value: { } qualifier }] && qualifier.Type().IsGenericArray(),
        Message = propertyName => $"Use the '{propertyName}' property.",
    };

    public required Func<TreeNodeCollection<ICSharpArgument?>, bool> Condition { get; init; }

    public string? Name { get; init; }

    public bool EnsureExtensionInvokedAsExtension { get; init; }

    public TargetType? EnsureTargetType { get; init; }
}