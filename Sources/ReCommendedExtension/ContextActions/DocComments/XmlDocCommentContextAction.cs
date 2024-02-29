using System.Linq.Expressions;
using System.Text;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Feature.Services.CSharp.ContextActions;
using JetBrains.ReSharper.Psi.Xml.CodeStyle;
using JetBrains.ReSharper.Psi.Xml.Tree;

namespace ReCommendedExtension.ContextActions.DocComments;

public abstract class XmlDocCommentContextAction : ContextActionBase
{
    protected sealed record Settings
    {
        [Pure]
        static T GetValue<T>(IContextBoundSettingsStore store, Expression<Func<XmlDocFormatterSettingsKey, T>> lambdaExpression)
            => store.GetValue(lambdaExpression);

        [Pure]
        public static Settings Load(ICSharpContextActionDataProvider provider)
        {
            var store = provider.PsiServices.SettingsStore.BindToContextTransient(ContextRange.ApplicationWide);

            return new Settings
            {
                IndentSize = GetValue(store, s => s.INDENT_SIZE),
                WrapLimit = GetValue(store, s => s.WRAP_LIMIT),
                TagSpacesAroundAttributeEq = GetValue(store, s => s.TagSpacesAroundAttributeEq),
                TagSpaceAfterLastAttr = GetValue(store, s => s.TagSpaceAfterLastAttr),
                TagSpaceBeforeHeaderEnd1 = GetValue(store, s => s.TagSpaceBeforeHeaderEnd1),
            };
        }

        [ValueRange(1, int.MaxValue)]
        public required int IndentSize { get; init; }

        [NonNegativeValue]
        public required int WrapLimit { get; init; }

        public required bool TagSpacesAroundAttributeEq { get; init; }

        public required bool TagSpaceAfterLastAttr { get; init; }

        public required bool TagSpaceBeforeHeaderEnd1 { get; init; }
    }

    protected enum TagOption
    {
        /// <remarks>
        /// E.g. <c><![CDATA[<Tag...></Tag>]]></c>
        /// </remarks>
        Expanded,

        /// <remarks>
        /// E.g. <c><![CDATA[<Tag.../>]]></c>
        /// </remarks>
        Collapsed,

        /// <remarks>
        /// E.g. <c><![CDATA[<Tag...>]]></c>
        /// </remarks>
        HeaderOnly,

        /// <remarks>
        /// E.g. <c><![CDATA[</Tag>]]></c>
        /// </remarks>
        FooterOnly,
    }

    static void AppendTag(
        StringBuilder builder,
        string tagName,
        (string name, string value)? attribute,
        TagOption option,
        Settings settings)
    {
        Debug.Assert(
            option is TagOption.Collapsed or TagOption.Expanded or TagOption.HeaderOnly || option == TagOption.FooterOnly && attribute is not { });

        builder.Append('<');

        if (option == TagOption.FooterOnly)
        {
            builder.Append('/');
        }

        builder.Append(tagName);

        if (attribute is var (attributeName, attributeValue))
        {
            builder.Append(' ');
            builder.Append(attributeName);

            if (settings.TagSpacesAroundAttributeEq)
            {
                builder.Append(" = ");
            }
            else
            {
                builder.Append('=');
            }

            builder.Append('"');
            builder.Append(attributeValue);
            builder.Append('"');

            if (settings.TagSpaceAfterLastAttr)
            {
                builder.Append(' ');
            }
        }

        switch (option)
        {
            case TagOption.Expanded:
                builder.Append("></");
                builder.Append(tagName);
                builder.Append('>');
                break;

            case TagOption.Collapsed:
                if (settings.TagSpaceBeforeHeaderEnd1 && (attribute is not { } || !settings.TagSpaceAfterLastAttr))
                {
                    builder.Append(' ');
                }
                builder.Append("/>");
                break;

            case TagOption.HeaderOnly or TagOption.FooterOnly:
                builder.Append('>');
                break;
        }
    }

    protected static void AppendTag(StringBuilder builder, string tagName, IXmlAttribute? attribute, TagOption option, Settings settings)
        => AppendTag(builder, tagName, attribute is { } ? (attribute.AttributeName, attribute.Value?.UnquotedValue ?? "") : null, option, settings);

    protected static void AppendTag(StringBuilder builder, string tagName, TagOption option, Settings settings)
        => AppendTag(builder, tagName, null as (string, string)?, option, settings);

    [Pure]
    protected static string BuildTag(string tagName, (string name, string value)? attribute, string? interior, TagOption option, Settings settings)
    {
        var builder = new StringBuilder();

        if (interior is { })
        {
            Debug.Assert(option == TagOption.Expanded);

            AppendTag(builder, tagName, attribute, TagOption.HeaderOnly, settings);
            builder.Append(interior);
            AppendTag(builder, tagName, null as (string, string)?, TagOption.FooterOnly, settings);
        }
        else
        {
            AppendTag(builder, tagName, attribute, option, settings);
        }

        return builder.ToString();
    }

    [Pure]
    protected static string BuildTag(string tagName, IXmlAttribute? attribute, string? interior, TagOption option, Settings settings)
        => BuildTag(tagName, attribute is { } ? (attribute.AttributeName, attribute.Value?.UnquotedValue ?? "") : null, interior, option, settings);

    [Pure]
    protected static string BuildTag(string tagName, string? interior, TagOption option, Settings settings)
        => BuildTag(tagName, null as (string, string)?, interior, option, settings);
}