namespace ReCommendedExtension.Analyzers.Method.Rules;

public enum MethodInvocationContext
{
    Standalone,
    BinaryLeftOperand,
    BinaryRightOperand,
}