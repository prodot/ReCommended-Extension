// ReSharper disable once CheckNamespace
namespace System.Diagnostics.CodeAnalysis;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
[AttributeUsage(AttributeTargets.Parameter)]
internal sealed class DoesNotReturnIfAttribute(bool parameterValue) : Attribute
{
    public bool ParameterValue { get; } = parameterValue;
}