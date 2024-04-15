using System.Reflection;
using JetBrains.Application.UI.Extensions;
using JetBrains.ReSharper.Feature.Services.Daemon;
using NUnit.Framework;

namespace ReCommendedExtension.Tests;

[TestFixture]
public sealed class AssemblyTests
{
    [Test]
    [SuppressMessage("ReSharper", "StringIndexOfIsCultureSpecific.1")]
    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    public void TestCopyrightYear()
    {
        var assembly = typeof(IReCommendedExtensionZone).Assembly;
        var attribute = assembly.GetAttribute<AssemblyCopyrightAttribute>();

        Assert.IsTrue(attribute.Copyright.IndexOf(DateTime.Today.Year.ToString()) > -1);
    }

    [Test]
    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    public void TestVersions()
    {
        var assembly = typeof(IReCommendedExtensionZone).Assembly;

        var assemblyVersion = assembly.GetName().Version;
        Assert.AreEqual(0, assemblyVersion.Revision);

        var fileVersionAttribute = assembly.GetAttribute<AssemblyFileVersionAttribute>();

        Assert.AreEqual(
            new Version(assemblyVersion.Major, assemblyVersion.Minor, assemblyVersion.Build),
            new Version(fileVersionAttribute.Version));
    }

    [Test]
    public void TestDuplicateTexts()
    {
        var assembly = typeof(IReCommendedExtensionZone).Assembly;

        var attributes =
            from a in assembly.GetCustomAttributes<RegisterConfigurableSeverityAttribute>()
            group a by a.Title
            into groupings
            where groupings.Count() > 1
            select groupings.Key;

        var duplicateTitle = attributes.FirstOrDefault();

        Assert.IsNull(duplicateTitle, $"Duplicate {nameof(RegisterConfigurableSeverityAttribute.Title)} \"{duplicateTitle}\" detected.");
    }
}