using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ReCommendedExtension.Tests.test.data.Analyzers.Await
{
    public class TestMethods
    {
        [TestMethod]
        public async Task Method()
        {
            if (Environment.UserInteractive)
            {
                Console.WriteLine();
            }

            await Task.Delay(10);
        }

        [TestMethod]
        public async Task Method_WithConfigureAwait()
        {
            if (Environment.UserInteractive)
            {
                Console.WriteLine();
            }

            await Task.Delay(10).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task Method_AsExpressionBodied() => await Task.Delay(10);

        [TestMethod]
        public async Task Method_AsExpressionBodied_WithConfigureAwait() => await Task.Delay(10).ConfigureAwait(false);

        [TestMethod]
        public async Task Method4()
        {
            Console.WriteLine();
            await Task.FromResult("one");
        }

        [TestMethod]
        public async Task Method4_WithConfigureAwait()
        {
            Console.WriteLine();
            await Task.FromResult("one").ConfigureAwait(false);
        }

        [TestMethod]
        public async Task Method4_AsExpressionBodied() => await Task.FromResult("one");

        [TestMethod]
        public async Task Method4_AsExpressionBodied_WithConfigureAwait() => await Task.FromResult("one").ConfigureAwait(false);
    }

    public class TestInitializers
    {
        [TestInitialize]
        public async Task Method()
        {
            if (Environment.UserInteractive)
            {
                Console.WriteLine();
            }

            await Task.Delay(10);
        }

        [TestInitialize]
        public async Task Method_WithConfigureAwait()
        {
            if (Environment.UserInteractive)
            {
                Console.WriteLine();
            }

            await Task.Delay(10).ConfigureAwait(false);
        }

        [TestInitialize]
        public async Task Method_AsExpressionBodied() => await Task.Delay(10);

        [TestInitialize]
        public async Task Method_AsExpressionBodied_WithConfigureAwait() => await Task.Delay(10).ConfigureAwait(false);

        [TestInitialize]
        public async Task Method4()
        {
            Console.WriteLine();
            await Task.FromResult("one");
        }

        [TestInitialize]
        public async Task Method4_WithConfigureAwait()
        {
            Console.WriteLine();
            await Task.FromResult("one").ConfigureAwait(false);
        }

        [TestInitialize]
        public async Task Method4_AsExpressionBodied() => await Task.FromResult("one");

        [TestInitialize]
        public async Task Method4_AsExpressionBodied_WithConfigureAwait() => await Task.FromResult("one").ConfigureAwait(false);
    }

    public class ClassInitializers
    {
        [ClassInitialize]
        public static async Task Method(TestContext testContext)
        {
            if (Environment.UserInteractive)
            {
                Console.WriteLine();
            }

            await Task.Delay(10);
        }

        [ClassInitialize]
        public static async Task Method_WithConfigureAwait(TestContext testContext)
        {
            if (Environment.UserInteractive)
            {
                Console.WriteLine();
            }

            await Task.Delay(10).ConfigureAwait(false);
        }

        [ClassInitialize]
        public static async Task Method_AsExpressionBodied(TestContext testContext) => await Task.Delay(10);

        [ClassInitialize]
        public static async Task Method_AsExpressionBodied_WithConfigureAwait(TestContext testContext) => await Task.Delay(10).ConfigureAwait(false);

        [ClassInitialize]
        public static async Task Method4(TestContext testContext)
        {
            Console.WriteLine();
            await Task.FromResult("one");
        }

        [ClassInitialize]
        public static async Task Method4_WithConfigureAwait(TestContext testContext)
        {
            Console.WriteLine();
            await Task.FromResult("one").ConfigureAwait(false);
        }

        [ClassInitialize]
        public static async Task Method4_AsExpressionBodied(TestContext testContext) => await Task.FromResult("one");

        [ClassInitialize]
        public static async Task Method4_AsExpressionBodied_WithConfigureAwait(TestContext testContext)
            => await Task.FromResult("one").ConfigureAwait(false);
    }

    public class AssemblyInitializers
    {
        [AssemblyInitialize]
        public static async Task Method(TestContext testContext)
        {
            if (Environment.UserInteractive)
            {
                Console.WriteLine();
            }

            await Task.Delay(10);
        }

        [AssemblyInitialize]
        public static async Task Method_WithConfigureAwait(TestContext testContext)
        {
            if (Environment.UserInteractive)
            {
                Console.WriteLine();
            }

            await Task.Delay(10).ConfigureAwait(false);
        }

        [AssemblyInitialize]
        public static async Task Method_AsExpressionBodied(TestContext testContext) => await Task.Delay(10);

        [AssemblyInitialize]
        public static async Task Method_AsExpressionBodied_WithConfigureAwait(TestContext testContext) => await Task.Delay(10).ConfigureAwait(false);

        [AssemblyInitialize]
        public static async Task Method4(TestContext testContext)
        {
            Console.WriteLine();
            await Task.FromResult("one");
        }

        [AssemblyInitialize]
        public static async Task Method4_WithConfigureAwait(TestContext testContext)
        {
            Console.WriteLine();
            await Task.FromResult("one").ConfigureAwait(false);
        }

        [AssemblyInitialize]
        public static async Task Method4_AsExpressionBodied(TestContext testContext) => await Task.FromResult("one");

        [AssemblyInitialize]
        public static async Task Method4_AsExpressionBodied_WithConfigureAwait(TestContext testContext)
            => await Task.FromResult("one").ConfigureAwait(false);
    }

    public class TestCleanups
    {
        [TestCleanup]
        public async Task Method()
        {
            if (Environment.UserInteractive)
            {
                Console.WriteLine();
            }

            await Task.Delay(10);
        }

        [TestCleanup]
        public async Task Method_WithConfigureAwait()
        {
            if (Environment.UserInteractive)
            {
                Console.WriteLine();
            }

            await Task.Delay(10).ConfigureAwait(false);
        }

        [TestCleanup]
        public async Task Method_AsExpressionBodied() => await Task.Delay(10);

        [TestCleanup]
        public async Task Method_AsExpressionBodied_WithConfigureAwait() => await Task.Delay(10).ConfigureAwait(false);

        [TestCleanup]
        public async Task Method4()
        {
            Console.WriteLine();
            await Task.FromResult("one");
        }

        [TestCleanup]
        public async Task Method4_WithConfigureAwait()
        {
            Console.WriteLine();
            await Task.FromResult("one").ConfigureAwait(false);
        }

        [TestCleanup]
        public async Task Method4_AsExpressionBodied() => await Task.FromResult("one");

        [TestCleanup]
        public async Task Method4_AsExpressionBodied_WithConfigureAwait() => await Task.FromResult("one").ConfigureAwait(false);
    }

    public class ClassCleanups
    {
        [ClassCleanup]
        public static async Task Method()
        {
            if (Environment.UserInteractive)
            {
                Console.WriteLine();
            }

            await Task.Delay(10);
        }

        [ClassCleanup]
        public static async Task Method_WithConfigureAwait()
        {
            if (Environment.UserInteractive)
            {
                Console.WriteLine();
            }

            await Task.Delay(10).ConfigureAwait(false);
        }

        [ClassCleanup]
        public static async Task Method_AsExpressionBodied() => await Task.Delay(10);

        [ClassCleanup]
        public static async Task Method_AsExpressionBodied_WithConfigureAwait() => await Task.Delay(10).ConfigureAwait(false);

        [ClassCleanup]
        public static async Task Method4()
        {
            Console.WriteLine();
            await Task.FromResult("one");
        }

        [ClassCleanup]
        public static async Task Method4_WithConfigureAwait()
        {
            Console.WriteLine();
            await Task.FromResult("one").ConfigureAwait(false);
        }

        [ClassCleanup]
        public static async Task Method4_AsExpressionBodied() => await Task.FromResult("one");

        [ClassCleanup]
        public static async Task Method4_AsExpressionBodied_WithConfigureAwait() => await Task.FromResult("one").ConfigureAwait(false);
    }

    public class AssemblyCleanups
    {
        [AssemblyCleanup]
        public static async Task Method()
        {
            if (Environment.UserInteractive)
            {
                Console.WriteLine();
            }

            await Task.Delay(10);
        }

        [AssemblyCleanup]
        public static async Task Method_WithConfigureAwait()
        {
            if (Environment.UserInteractive)
            {
                Console.WriteLine();
            }

            await Task.Delay(10).ConfigureAwait(false);
        }

        [AssemblyCleanup]
        public static async Task Method_AsExpressionBodied() => await Task.Delay(10);

        [AssemblyCleanup]
        public static async Task Method_AsExpressionBodied_WithConfigureAwait() => await Task.Delay(10).ConfigureAwait(false);

        [AssemblyCleanup]
        public static async Task Method4()
        {
            Console.WriteLine();
            await Task.FromResult("one");
        }

        [AssemblyCleanup]
        public static async Task Method4_WithConfigureAwait()
        {
            Console.WriteLine();
            await Task.FromResult("one").ConfigureAwait(false);
        }

        [AssemblyCleanup]
        public static async Task Method4_AsExpressionBodied() => await Task.FromResult("one");

        [AssemblyCleanup]
        public static async Task Method4_AsExpressionBodied_WithConfigureAwait() => await Task.FromResult("one").ConfigureAwait(false);
    }
}