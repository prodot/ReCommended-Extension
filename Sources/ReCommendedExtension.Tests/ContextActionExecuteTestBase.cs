using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.FeaturesTestFramework.Intentions;

namespace ReCommendedExtension.Tests
{
    public abstract class ContextActionExecuteTestBase<T> : CSharpContextActionExecuteTestBase<T> where T : class, IContextAction
    {
        protected override string ExtraPath => "";
    }
}