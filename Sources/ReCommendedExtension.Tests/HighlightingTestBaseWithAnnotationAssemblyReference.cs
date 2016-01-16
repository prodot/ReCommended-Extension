using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using JetBrains.ReSharper.FeaturesTestFramework.Daemon;

namespace ReCommendedExtension.Tests
{
    public abstract class HighlightingTestBaseWithAnnotationAssemblyReference : CSharpHighlightingTestBase
    {
        protected override IEnumerable<string> GetReferencedAssemblies()
        {
            foreach (var assembly in base.GetReferencedAssemblies() ?? Enumerable.Empty<string>())
            {
                yield return assembly;
            }

            yield return typeof(NotNullAttribute).Assembly.Location; // add reference to the annotations assembly
        }
    }
}