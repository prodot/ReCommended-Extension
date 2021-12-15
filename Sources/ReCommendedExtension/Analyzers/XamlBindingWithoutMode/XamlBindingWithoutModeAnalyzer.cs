using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Xaml.Tree;
using JetBrains.ReSharper.Psi.Xaml.Tree.MarkupExtensions;
using JetBrains.ReSharper.Psi.Xml.Tree;

namespace ReCommendedExtension.Analyzers.XamlBindingWithoutMode
{
    [ElementProblemAnalyzer(typeof(IXmlTreeNode), HighlightingTypes = new[] { typeof(XamlBindingWithoutModeWarning) })]
    public sealed class XamlBindingWithoutModeAnalyzer : ElementProblemAnalyzer<IXmlTreeNode>
    {
        protected override void Run(IXmlTreeNode element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
        {
            switch (element)
            {
                case IBindingMarkup bindingMarkup when bindingMarkup.ModeAttribute == null && bindingMarkup.NameNode != null:
                    consumer.AddHighlighting(new XamlBindingWithoutModeWarning("Binding mode is not set explicitly.", bindingMarkup.NameNode));
                    break;

                case IBindingElement bindingElement when ClrTypeNames.Binding.Equals((bindingElement.Type as IDeclaredType)?.GetClrName())
                    && bindingElement.GetAttribute("Mode") == null:
                    consumer.AddHighlighting(new XamlBindingWithoutModeWarning("Binding mode is not set explicitly.", bindingElement.Header.Name));
                    break;

                case IXamlObjectElement objectElement when objectElement is ITypeOwnerDeclaration typeOwnerDeclaration
                    && ClrTypeNames.MultiBinding.Equals((typeOwnerDeclaration.Type as IDeclaredType)?.GetClrName())
                    && objectElement.GetAttribute("Mode") == null:
                    consumer.AddHighlighting(new XamlBindingWithoutModeWarning("Binding mode is not set explicitly.", objectElement.Header.Name));
                    break;
            }
        }
    }
}