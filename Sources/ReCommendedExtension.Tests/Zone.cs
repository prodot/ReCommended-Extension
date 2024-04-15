using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.ReSharper.TestFramework;
using JetBrains.TestFramework.Application.Zones;

namespace ReCommendedExtension.Tests;

[ZoneDefinition]
public interface IReCommendedExtensionTestZone : ITestsEnvZone, IRequire<IReCommendedExtensionZone>, IRequire<PsiFeatureTestZone>;

[ZoneMarker]
public sealed class ZoneMarker : IRequire<IReCommendedExtensionZone>;