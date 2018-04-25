using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using JetBrains.Application.platforms;
using JetBrains.Metadata.Reader.API;
using JetBrains.ReSharper.FeaturesTestFramework.Intentions;

namespace ReCommendedExtension.Tests
{
    public abstract class QuickFixAvailabilityTestBaseWithAnnotationAssemblyReference : QuickFixAvailabilityTestBase
    {
        protected override IEnumerable<string> GetReferencedAssemblies(PlatformID platformId, TargetFrameworkId targetFrameworkId)
            => (base.GetReferencedAssemblies(platformId, targetFrameworkId) ?? Enumerable.Empty<string>()).GetReferencedAssemblies()
                .Append(typeof(NotNullAttribute).Assembly.Location);
    }
}