using System.Reflection;
using JetBrains.Application.UI.Extensions;
using JetBrains.Collections;
using JetBrains.Metadata.Reader.API;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
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

    [Test]
    public void TestClrTypeNames()
    {
        var predefinedTypes =
        (
            from field in typeof(PredefinedType).GetFields()
            where field.IsStatic && typeof(IClrTypeName).IsAssignableFrom(field.FieldType)
            select new { ClrTypeName = (IClrTypeName)field.GetValue(null), FieldName = field.Name }).ToDictionary(
            item => item.ClrTypeName,
            item => item.FieldName,
            ClrTypeNameEqualityComparer.Default);

        var fields =
            from field in typeof(ClrTypeNames).GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
            where typeof(IClrTypeName).IsAssignableFrom(field.FieldType)
            select new { ClrTypeName = (IClrTypeName)field.GetValue(null), FieldName = field.Name };

        var types = new Dictionary<IClrTypeName, string>(ClrTypeNameEqualityComparer.Default);

        foreach (var field in fields)
        {
            Assert.True(types.TryAdd(field.ClrTypeName, field.FieldName), $"Duplicate type '{field.ClrTypeName.FullName}' detected.");
        }

        var redundantFields = new List<(string fieldName, string predefinedTypeFieldName)>();

        foreach (var (clrTypeName, fieldName) in types)
        {
            if (predefinedTypes.TryGetValue(clrTypeName, out var predefinedTypeFieldName))
            {
                redundantFields.Add((fieldName, predefinedTypeFieldName));
            }
        }

        Assert.IsEmpty(
            redundantFields,
            $"Redundant fields detected:{
                string.Join(
                    "",
                    from t in redundantFields
                    select $"{Environment.NewLine}    {t.fieldName}: use {nameof(PredefinedType)}.{t.predefinedTypeFieldName}")
            }");
    }
}