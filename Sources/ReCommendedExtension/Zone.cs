using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.Application.Environment;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.Xaml;
using ReCommendedExtension;

namespace ReCommendedExtension
{
    [ZoneDefinition]
    [ZoneDefinitionConfigurableFeature(
        ZoneMarker.ExtensionName,
        ZoneMarker.ExtensionDescription,
        false /* true -> in "Products", false -> in "Features" */)]
    public interface IReCommendedExtensionZone : IZone, IRequire<ILanguageCSharpZone>, IRequire<ILanguageXamlZone>;

    [ZoneMarker]
    public sealed class ZoneMarker : IRequire<IReCommendedExtensionZone>
    {
        internal const string ExtensionId = "ReCommendedExtension"
#if DEBUG
                + "_DEBUG"
#endif
            ;

        internal const string ExtensionName = "ReCommended Extension for ReSharper"
#if DEBUG
                + " (DEBUG)"
#endif
            ;

        internal const string ExtensionDescription = "Code analysis improvements and context actions.";
        internal const string Suffix = " (ReCommended Extension)";
    }
}

namespace ExtensionActivator
{
    [ZoneActivator]
    [ZoneMarker]
    public sealed class ReCommendedExtensionActivator : IActivate<IReCommendedExtensionZone>
    {
        public bool ActivatorEnabled() => true;
    }
}