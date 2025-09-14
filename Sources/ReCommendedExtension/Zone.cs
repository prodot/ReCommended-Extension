using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.Application.Environment;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.Xaml;
using ReCommendedExtension;

#if RESHARPER
using JetBrains.Platform.VisualStudio.Protocol.BuildScript;
using JetBrains.VsIntegration.Env;
using JetBrains.VsIntegration.Zones;
#endif

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
#if RESHARPER
        const string platform = "ReSharper";
#endif
#if RIDER
        const string platform = "Rider";
#endif

        internal const string ExtensionName = $"ReCommended Extension for {platform}"
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
#if RIDER
    [ZoneActivator]
    [ZoneMarker]
    public sealed class ReCommendedExtensionActivator : IActivate<IReCommendedExtensionZone>
    {
        public bool ActivatorEnabled() => true;
    }
#endif

#if RESHARPER
    /// <summary>
    /// Activator for the In-Process mode
    /// </summary>
    [ZoneActivator]
    [ZoneMarker(typeof(IVisualStudioFrontendEnvZone))]
    public sealed class ReCommendedExtensionActivator(VisualStudioProtocolConnector protocolConnector) : IActivateDynamic<IReCommendedExtensionZone>
    {
        public bool ActivatorEnabled() => !protocolConnector.IsOutOfProcess;
    }

    /// <summary>
    /// Activator for the Out-of-Process mode
    /// </summary>
    [ZoneActivator]
    [ZoneMarker(typeof(IVisualStudioBackendOutOfProcessEnvZone))]
    public sealed class ReCommendedExtensionActivatorOop : IActivate<IReCommendedExtensionZone>;
#endif
}