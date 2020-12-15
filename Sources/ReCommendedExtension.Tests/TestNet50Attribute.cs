using System;
using JetBrains.Annotations;
using JetBrains.Application.platforms;
using JetBrains.ReSharper.TestFramework;
using JetBrains.TestFramework;
using JetBrains.Util;
using JetBrains.Util.Dotnet.TargetFrameworkIds;

namespace ReCommendedExtension.Tests
{
    internal sealed class TestNet50Attribute // todo: remove this class when R# starts using correctly the non-RC package for the .NET 5
        : TestPackagesAttribute,
            ITestDataPackagesProvider,
            ITestPlatformProvider,
            ITestMsCorLibFlagProvider,
            ITestTargetFrameworkIdProvider,
            ITestFlavoursProvider
    {
        [NotNull]
        static readonly TargetFrameworkId ourTargetFrameworkId = TargetFrameworkId.Create(
            FrameworkIdentifier.NetFramework,
            new Version(4, 5, 1),
            ProfileIdentifier.Default);

        [NotNull]
        static readonly TargetFrameworkId TARGET_FRAMEWORK_ID = TargetFrameworkId.Create(".NETFramework", new Version(5, 0), null);

        public TestNet50Attribute([NotNull][ItemNotNull] params string[] packages) : base(
            ArrayUtil.Add("Microsoft.NETCore.App.Ref/5.0.0", packages)) { }

        TargetFrameworkId ITestPlatformProvider.GetTargetFrameworkId() => ourTargetFrameworkId;

        ReferenceDlls ITestMsCorLibFlagProvider.GetMsCorLibFlag() => ReferenceDlls.None;

        bool ITestFlavoursProvider.Inherits => false;

        Guid[] ITestFlavoursProvider.GetProjectTypeGuids() => EmptyArray<Guid>.Instance;

        TargetFrameworkId ITestTargetFrameworkIdProvider.GetTargetFrameworkId() => TARGET_FRAMEWORK_ID;

        bool ITestTargetFrameworkIdProvider.Inherits => false;
    }
}