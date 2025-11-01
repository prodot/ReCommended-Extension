using ReCommendedExtension.Extensions.MemberFinding;

namespace ReCommendedExtension.Analyzers.Argument.Rules;

internal abstract record MemberSignature
{
    public required IReadOnlyList<Parameter> Parameters { get; init; }
}