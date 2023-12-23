using JetBrains.TestFramework;
using NUnit.Framework;

[assembly: Apartment(ApartmentState.STA)]

namespace ReCommendedExtension.Tests;

[SetUpFixture]
public sealed class TestEnvironmentAssembly : ExtensionTestEnvironmentAssembly<IReCommendedExtensionTestZone>;