using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Tests
{
    public class AwaitForMethods
    {
        async Task Method()
        {
            await Task.Delay(10);
        }

        async Task Method_WithConfigureAwait(bool continueOnCapturedContext)
        {
            await Task.Delay(10).ConfigureAwait(continueOnCapturedContext);
        }

        async Task Method_WithConfigureAwait()
        {
            await Task.Delay(10).ConfigureAwait(false);
        }

        async Task Method_WithConfigureAwaitTrue()
        {
            await Task.Delay(10).ConfigureAwait(true);
        }

        async Task Method_WithConfigureAwait(ConfigureAwaitOptions options)
        {
            await Task.Delay(10).ConfigureAwait(options);
        }

        async Task Method_WithConfigureAwaitOptions_None()
        {
            await Task.Delay(10).ConfigureAwait(ConfigureAwaitOptions.None);
        }

        async Task Method_WithConfigureAwaitOptions_ContinueOnCapturedContext()
        {
            await Task.Delay(10).ConfigureAwait(ConfigureAwaitOptions.ContinueOnCapturedContext);
        }

        async Task Method_WithConfigureAwaitOptions_SuppressThrowing()
        {
            await Task.Delay(10).ConfigureAwait(ConfigureAwaitOptions.ContinueOnCapturedContext | ConfigureAwaitOptions.SuppressThrowing);
        }

        async Task Method_WithConfigureAwaitOptions_ForceYielding()
        {
            await Task.Delay(10).ConfigureAwait(ConfigureAwaitOptions.ForceYielding);
        }
    }
}