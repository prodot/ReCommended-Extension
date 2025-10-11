namespace ReCommendedExtension.Analyzers.Argument.Rules;

internal sealed record MethodSignature : MemberSignature
{
    public bool IsStatic { get; init; }

    [NonNegativeValue]
    public int GenericParametersCount { get; init; }
}