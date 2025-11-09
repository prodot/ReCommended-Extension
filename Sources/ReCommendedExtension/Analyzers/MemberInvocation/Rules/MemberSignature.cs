namespace ReCommendedExtension.Analyzers.MemberInvocation.Rules;

internal abstract record MemberSignature
{
    public bool IsStatic { get; init; }
}