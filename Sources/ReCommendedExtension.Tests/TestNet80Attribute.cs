using JetBrains.ReSharper.TestFramework;
using JetBrains.TestFramework;
using JetBrains.Util;
using JetBrains.Util.Dotnet.TargetFrameworkIds;
using NuGet.Frameworks;

namespace ReCommendedExtension.Tests;

// todo: remove when [TestNet80] becomes available
// todo: also remove package reference "JetBrains.NuGet.Frameworks"

internal sealed class TestNet80Attribute
    : TestPackagesAttribute, ITestDataPackagesProvider, ITestMsCorLibFlagProvider, ITestTargetFrameworkIdProvider, ITestFlavoursProvider
{
    static readonly TargetFrameworkId TARGET_FRAMEWORK_ID = TargetFrameworkId.Create(
        new FallbackFramework(
            new NuGetFramework(FrameworkConstants.FrameworkIdentifiers.NetCoreApp, new Version(8, 0)),
            new[] { new NuGetFramework(".NETFramework", new Version(8, 0)) }));

    public TestNet80Attribute(params string[] packages) : base(ArrayUtil.Add("Microsoft.NETCore.App.Ref/8.0.0", packages)) { }

    ReferenceDlls ITestMsCorLibFlagProvider.GetMsCorLibFlag() => ReferenceDlls.None;

    bool ITestFlavoursProvider.Inherits => false;

    Guid[] ITestFlavoursProvider.GetProjectTypeGuids() => Array.Empty<Guid>();

    TargetFrameworkId ITestTargetFrameworkIdProvider.GetTargetFrameworkId() => TARGET_FRAMEWORK_ID;

    bool ITestTargetFrameworkIdProvider.Inherits => false;
}