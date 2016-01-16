using JetBrains.TestFramework;
using NUnit.Framework;

namespace ReCommendedExtension.Tests
{
    [SetUpFixture]
    public sealed class TestEnvironmentAssembly : ExtensionTestEnvironmentAssembly<IReCommendedExtensionTestZone> { }
}