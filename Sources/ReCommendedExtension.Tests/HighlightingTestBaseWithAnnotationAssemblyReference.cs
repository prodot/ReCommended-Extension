using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Metadata.Reader.API;
using JetBrains.ReSharper.FeaturesTestFramework.Daemon;
using PlatformID = JetBrains.Application.platforms.PlatformID;

namespace ReCommendedExtension.Tests
{
    public abstract class HighlightingTestBaseWithAnnotationAssemblyReference : CSharpHighlightingTestBase
    {
        protected override IEnumerable<string> GetReferencedAssemblies(PlatformID platformId, TargetFrameworkId targetFrameworkId)
            => (base.GetReferencedAssemblies(platformId, targetFrameworkId) ?? Enumerable.Empty<string>()).GetReferencedAssemblies()
                .Append(typeof(ValueTask<>).Assembly.Location);
    }
}