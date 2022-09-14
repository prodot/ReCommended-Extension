using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Test
{
    internal partial class SamplePage : Microsoft.UI.Xaml.Controls.ContentControl
    {
        async void OnLoaded()
        {
            OtherMethod();
        }
        
        async void OtherMethod() { }
    }

    partial class SamplePage : Microsoft.UI.Xaml.Markup.IComponentConnector // SamplePage.g.cs
    {
        [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.UI.Xaml.Markup.Compiler", " 1.0.0.0")]
        private class SamplePage_obj1_Bindings : Microsoft.UI.Xaml.Markup.IComponentConnector
        {
            private SamplePage dataRoot;
            private Microsoft.UI.Xaml.RoutedEventHandler obj1Loaded;

            public void Connect(int connectionId, object target)
            {
                switch (connectionId)
                {
                    case 1:
                        this.obj1Loaded = (object p0, Microsoft.UI.Xaml.RoutedEventArgs p1) =>
                        {
                            this.dataRoot.OnLoaded();
                        };
                        (WinRT.CastExtensions.As<Microsoft.UI.Xaml.Controls.ContentControl>(target)).Loaded += obj1Loaded;
                        break;
                }
            }
        }
    }
}