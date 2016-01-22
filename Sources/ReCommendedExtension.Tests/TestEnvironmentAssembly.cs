using System.Diagnostics.CodeAnalysis;
using JetBrains.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Tests;

[SetUpFixture]
[SuppressMessage("ReSharper", "CheckNamespace", Justification = "Must be in the global namespace.")]
public sealed class TestEnvironmentAssembly : ExtensionTestEnvironmentAssembly<IReCommendedExtensionTestZone> { }