using System.Collections.Generic;
using System.Linq;
using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.Util.Dotnet.TargetFrameworkIds;

namespace ReCommendedExtension.Tests
{
    public abstract class QuickFixAvailabilityTestBaseWithAnnotationAssemblyReference : QuickFixAvailabilityTestBase
    {
        protected sealed override IEnumerable<string> GetReferencedAssemblies(TargetFrameworkId targetFrameworkId)
            => (base.GetReferencedAssemblies(targetFrameworkId) ?? Enumerable.Empty<string>()).EnsureValueTaskAssembly().EnsureAnnotationsAssembly();
    }
}