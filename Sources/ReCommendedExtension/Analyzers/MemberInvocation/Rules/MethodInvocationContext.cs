namespace ReCommendedExtension.Analyzers.MemberInvocation.Rules;

public enum MethodInvocationContext
{
    Standalone,
    BinaryLeftOperand,
    BinaryRightOperand,
}