using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using JetBrains.Application.platforms;
using JetBrains.Metadata.Reader.API;
using JetBrains.ReSharper.FeaturesTestFramework.Daemon;

namespace ReCommendedExtension.Tests
{
    public abstract class HighlightingTestBaseWithAnnotationAssemblyReference : CSharpHighlightingTestBase
    {
        protected override IEnumerable<string> GetReferencedAssemblies(PlatformID platformId, TargetFrameworkId targetFrameworkId)
        {
            foreach (var assembly in base.GetReferencedAssemblies(platformId, targetFrameworkId) ?? Enumerable.Empty<string>())
            {
                yield return assembly;
            }

            yield return typeof(NotNullAttribute).Assembly.Location; // add reference to the annotations assembly
            yield return typeof(ValueTask<>).Assembly.Location;
        }
    }
}