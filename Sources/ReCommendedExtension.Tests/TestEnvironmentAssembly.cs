using System.Diagnostics.CodeAnalysis;
using System.Threading;
using JetBrains.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Tests;

[assembly: Apartment(ApartmentState.STA)]

[SetUpFixture]
[SuppressMessage("ReSharper", "CheckNamespace", Justification = "Must be in the global namespace.")]
public sealed class TestEnvironmentAssembly : ExtensionTestEnvironmentAssembly<IReCommendedExtensionTestZone> { }