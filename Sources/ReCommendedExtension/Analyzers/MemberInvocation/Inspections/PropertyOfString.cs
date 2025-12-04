using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using ReCommendedExtension.Extensions;

namespace ReCommendedExtension.Analyzers.MemberInvocation.Inspections;

internal sealed record PropertyOfString : Inspection
{
    public static PropertyOfString Arg0Empty { get; } = new()
    {
        Condition = args => args is [{ Value.AsStringConstant: "" }, ..], Message = propertyName => $"Use the '{propertyName}' property.",
    };

    public static PropertyOfString QualifierIsString { get; } = new()
    {
        Condition = args => args is [{ Value: { } qualifier }] && qualifier.Type().IsString(),
        Message = propertyName => $"Use the '{propertyName}' property.",
    };

    public Version? MinimumFrameworkVersion { get; init; }

    public required Func<TreeNodeCollection<ICSharpArgument?>, bool> Condition { get; init; }

    public string? Name { get; init; }

    public bool EnsureExtensionInvokedAsExtension { get; init; }

    public TargetType? EnsureTargetType { get; init; }
}