using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Application.platforms;
using JetBrains.Metadata.Reader.API;
using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.FeaturesTestFramework.Intentions;

namespace ReCommendedExtension.Tests
{
    public abstract class ContextActionAvailabilityTestBase<T> : CSharpContextActionAvailabilityTestBase<T> where T : class, IContextAction
    {
        protected override IEnumerable<string> GetReferencedAssemblies(PlatformID platformId, TargetFrameworkId targetFrameworkId)
        {
            foreach (var assembly in base.GetReferencedAssemblies(platformId, targetFrameworkId) ?? Enumerable.Empty<string>())
            {
                yield return assembly;
            }

            yield return typeof(ValueTask<>).Assembly.Location;
        }

        protected override string ExtraPath => "";
    }
}