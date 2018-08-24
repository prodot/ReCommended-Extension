using System.Collections.Generic;
using System.Linq;
using JetBrains.ReSharper.FeaturesTestFramework.Daemon;
using JetBrains.Util.Dotnet.TargetFrameworkIds;

namespace ReCommendedExtension.Tests
{
    public abstract class HighlightingTestBaseWithAnnotationAssemblyReference : CSharpHighlightingTestBase
    {
        protected sealed override IEnumerable<string> GetReferencedAssemblies(TargetFrameworkId targetFrameworkId)
            => (base.GetReferencedAssemblies(targetFrameworkId) ?? Enumerable.Empty<string>()).EnsureValueTaskAssembly();
    }
}