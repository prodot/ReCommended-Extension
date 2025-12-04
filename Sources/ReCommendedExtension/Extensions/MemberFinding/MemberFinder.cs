using JetBrains.ReSharper.Psi;

namespace ReCommendedExtension.Extensions.MemberFinding;

internal static class MemberFinder
{
    extension([InstantHandle] IEnumerable<IParametersOwner> members)
    {
        [Pure]
        bool HasMember(IReadOnlyList<Parameter> parameters, bool returnParameterNames, out string[] parameterNames)
        {
            parameterNames = [];

            foreach (var member in members)
            {
                if (parameters is [])
                {
                    return true;
                }

                var continueWithNextMember = false;

                if (returnParameterNames)
                {
                    parameterNames = new string[parameters.Count];
                }

                for (var i = 0; i < parameters.Count; i++)
                {
                    var parameter = parameters[i];
                    var memberParameter = member.Parameters[i];

                    if (parameter.Kind == memberParameter.Kind && parameter.IsType(memberParameter.Type))
                    {
                        if (returnParameterNames)
                        {
                            parameterNames[i] = memberParameter.ShortName;
                        }
                        continue;
                    }

                    continueWithNextMember = true;
                    break;
                }

                if (continueWithNextMember)
                {
                    parameterNames = [];
                    continue;
                }

                return true;
            }

            return false;
        }
    }

    extension(ITypeElement typeElement)
    {
        [Pure]
        public bool HasConstructor(ConstructorSignature signature, bool returnParameterNames, out string[] parameterNames)
            => (
                from constructor in typeElement.Constructors
                where constructor is { AccessibilityDomain.DomainType: AccessibilityDomain.AccessibilityDomainType.PUBLIC }
                    && constructor.Parameters.Count == signature.Parameters.Count
                select constructor).HasMember(signature.Parameters, returnParameterNames, out parameterNames);

        [Pure]
        public bool HasConstructor(ConstructorSignature signature) => typeElement.HasConstructor(signature, false, out _);

        [Pure]
        public bool HasProperty(PropertySignature signature)
            => (
                from property in typeElement.Properties
                where property is { AccessibilityDomain.DomainType: AccessibilityDomain.AccessibilityDomainType.PUBLIC }
                    && property.IsStatic == signature.IsStatic
                    && property.ShortName == signature.Name
                select property).HasMember([], false, out _);

        [Pure]
        public bool HasMethod(MethodSignature signature, bool returnParameterNames, out string[] parameterNames)
            => (
                from method in typeElement.Methods
                where method is { AccessibilityDomain.DomainType: AccessibilityDomain.AccessibilityDomainType.PUBLIC }
                    && method.IsStatic == signature.IsStatic
                    && method.ShortName == signature.Name
                    && method.TypeParametersCount == signature.GenericParametersCount
                    && method.Parameters.Count == signature.Parameters.Count
                select method).HasMember(signature.Parameters, returnParameterNames, out parameterNames);

        [Pure]
        public bool HasMethod(MethodSignature signature) => typeElement.HasMethod(signature, false, out _);
    }
}