namespace ReCommendedExtension.Analyzers.MemberInvocation.Rules;

public record struct BinaryOperatorOperands
{
    public required string Left { get; init; }

    public required string Right { get; init; }
}