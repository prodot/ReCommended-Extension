using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using ReCommendedExtension.Extensions;

namespace ReCommendedExtension.Analyzers.MemberInvocation.Inspections;

internal sealed record PropertyOfString : Inspection
{
    public static PropertyOfString Arg0Empty { get; } = new()
    {
        Condition = args => args[0]?.Value.TryGetStringConstant() == "", Message = propertyName => $"Use the '{propertyName}' property.",
    };

    public Version? MinimumFrameworkVersion { get; init; }

    public required Func<TreeNodeCollection<ICSharpArgument?>, bool> Condition { get; init; }

    public string? Name { get; init; }
}