using System.Linq.Expressions;
using System.Text;
using JetBrains.Application.Progress;
using JetBrains.Application.Settings;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Feature.Services.CSharp.ContextActions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Xml.CodeStyle;
using JetBrains.ReSharper.Psi.Xml.Tree;
using JetBrains.ReSharper.Psi.Xml.XmlDocComments;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;

namespace ReCommendedExtension.ContextActions;

[ContextAction(
    Group = "C#",
    Name = "Reflow XML doc comments" + ZoneMarker.Suffix,
    Description = "Reflow XML doc comments XML doc comments, i.e. apply smart formatting, tag reordering, etc.")]
public sealed class ReflowDocComments(ICSharpContextActionDataProvider provider) : ContextActionBase
{
    sealed record Settings
    {
        [Pure]
        static T GetValue<T>(IContextBoundSettingsStore store, Expression<Func<XmlDocFormatterSettingsKey, T>> lambdaExpression)
            => store.GetValue(lambdaExpression);

        [Pure]
        public static Settings Load(IContextBoundSettingsStore store)
            => new()
            {
                IndentSize = GetValue(store, s => s.INDENT_SIZE),
                WrapLimit = GetValue(store, s => s.WRAP_LIMIT),
                TagSpacesAroundAttributeEq = GetValue(store, s => s.TagSpacesAroundAttributeEq),
                TagSpaceAfterLastAttr = GetValue(store, s => s.TagSpaceAfterLastAttr),
                TagSpaceBeforeHeaderEnd1 = GetValue(store, s => s.TagSpaceBeforeHeaderEnd1),
            };

        [ValueRange(1, int.MaxValue)]
        public required int IndentSize { get; init; }

        [NonNegativeValue]
        public required int WrapLimit { get; init; }

        public required bool TagSpacesAroundAttributeEq { get; init; }

        public required bool TagSpaceAfterLastAttr { get; init; }

        public required bool TagSpaceBeforeHeaderEnd1 { get; init; }
    }

    enum TopLevelTagAttribute
    {
        /// <summary>
        /// No attributes allowed, the tag cannot be repeated.
        /// </summary>
        None,

        /// <summary>
        /// Only a single <c>name</c> attribute is expected, the tag can be repeated, but with different <c>name</c> values.
        /// </summary>
        Name,

        /// <summary>
        /// Only a single <c>cref</c> attribute is expected, the tag can be repeated.
        /// </summary>
        Cref,

        /// <summary>
        /// Only a single <c>cref</c> or as single <c>href</c> attribute is expected, the tag can be repeated. The tag is also collapsed if empty.
        /// </summary>
        CrefOrHref,
    }

    enum NestedTagAttribute
    {
        /// <summary>
        /// No attributes allowed.
        /// </summary>
        None,

        /// <summary>
        /// Only a single <c>name</c> attribute is expected.
        /// </summary>
        Name,

        /// <summary>
        /// Only a single <c>type</c> attribute is expected.
        /// </summary>
        Type,

        /// <summary>
        /// Only a single <c>cref</c> or as single <c>href</c> attribute is expected.
        /// </summary>
        CrefOrHref,
    }

    enum TopLevelTagFlow
    {
        Multiline,
        FirstTryOneLine,
    }

    enum NestedTagFlow
    {
        Inline,
        InlineWithLineBreak,
        Multiline,
        FirstTryOneLine,
    }

    enum NestedTagInterior
    {
        /// <summary>
        /// No content allowed. The tag is collapsed.
        /// </summary>
        None,

        /// <summary>
        /// Only text is allowed. The tag is collapsed if empty and has attributes. The tag is expanded if it doesn't have any attribute.
        /// </summary>
        TextOnly,

        /// <summary>
        /// Only text is allowed (original line breaks are kept). The tag is collapsed if empty and has attributes. The tag is expanded if it doesn't
        /// have any attribute.
        /// </summary>
        TextOnlyKeepingLineBreaks,

        /// <summary>
        /// Only an optional nested tag <c>listheader</c> followed by zero or more <c>item</c> nested tags are allowed. The tag is expanded.
        /// </summary>
        ListHeaderAndItems,

        /// <summary>
        /// Any content is allowed. The tag is collapsed if empty and has attributes. The tag is expanded if it doesn't have any attribute.
        /// </summary>
        Any,
    }

    abstract record TagInfo
    {
        public required string Name { get; init; }
    }

    sealed record TopLevelTagInfo : TagInfo
    {
        public TopLevelTagAttribute Attribute { get; init; } = TopLevelTagAttribute.None;

        public Func<ITreeNode?, IReadOnlyList<IDeclaration>?>? TryGetDeclarations { get; init; }

        public required TopLevelTagFlow Flow { get; init; }
    }

    sealed record NestedTagInfo : TagInfo
    {
        public NestedTagAttribute Attribute { get; init; } = NestedTagAttribute.None;

        public required NestedTagFlow Flow { get; init; }

        public required NestedTagInterior Interior { get; init; }
    }

    abstract record Token
    {
        public static TextToken Lexeme(string text, bool hasSpaceBefore = false) => new(text, TextTokenKind.Lexeme, hasSpaceBefore);

        public static TextToken Code(string text, bool hasSpaceBefore = false) => new(text, TextTokenKind.Code, hasSpaceBefore);

        public static TextToken Tag(string text, NestedTagInfo tagInfo, bool hasSpaceBefore = false)
            => new(text, TextTokenKind.Tag, hasSpaceBefore, tagInfo);

        public static InstructionToken ForceLineBreak { get; } = new(InstructionTokenKind.ForceLineBreak);

        public static InstructionToken BeginMultiline { get; } = new(InstructionTokenKind.BeginMultiline);

        public static InstructionToken EndMultiline { get; } = new(InstructionTokenKind.EndMultiline);

        public static InstructionToken BeginOneLineAttempt { get; } = new(InstructionTokenKind.BeginOneLineAttempt);

        public static InstructionToken EndOneLineAttempt { get; } = new(InstructionTokenKind.EndOneLineAttempt);

        public static InstructionToken BeginIndentation { get; } = new(InstructionTokenKind.BeginIndentation);

        public static InstructionToken EndIndentation { get; } = new(InstructionTokenKind.EndIndentation);
    }

    sealed record TextToken : Token
    {
        internal TextToken(string text, TextTokenKind kind, bool hasSpaceBefore, NestedTagInfo? tagInfo = null)
        {
            Debug.Assert(kind is TextTokenKind.Lexeme or TextTokenKind.Code && tagInfo is not { } || kind == TextTokenKind.Tag && tagInfo is { });

            Text = text;
            Kind = kind;
            HasSpaceBefore = hasSpaceBefore;
            TagInfo = tagInfo;
        }

        public string Text { get; }

        public TextTokenKind Kind { get; }

        public bool HasSpaceBefore { get; }

        [SuppressMessage("ReSharper", "MemberHidesStaticFromOuterClass", Justification = "Intentional: the base type should not be accessed here.")]
        public NestedTagInfo? TagInfo { get; }
    }

    enum TextTokenKind
    {
        Lexeme,
        Code,
        Tag,
    }

    sealed record InstructionToken : Token
    {
        internal InstructionToken(InstructionTokenKind kind) => Kind = kind;

        public InstructionTokenKind Kind { get; }
    }

    enum InstructionTokenKind
    {
        ForceLineBreak,
        BeginMultiline,
        EndMultiline,
        BeginOneLineAttempt,
        EndOneLineAttempt,
        BeginIndentation,
        EndIndentation,
    }

    /// <remarks>
    /// Ordered list of supported top-level tags.
    /// </remarks>
    static readonly IReadOnlyList<TopLevelTagInfo> topLevelTags =
    [
        new TopLevelTagInfo { Name = "summary", Flow = TopLevelTagFlow.Multiline },
        new TopLevelTagInfo
        {
            Name = @"typeparam",
            Attribute = TopLevelTagAttribute.Name,
            TryGetDeclarations = TryGetTypeParameters,
            Flow = TopLevelTagFlow.FirstTryOneLine,
        },
        new TopLevelTagInfo
        {
            Name = "param", Attribute = TopLevelTagAttribute.Name, TryGetDeclarations = TryGetParameters, Flow = TopLevelTagFlow.FirstTryOneLine,
        },
        new TopLevelTagInfo { Name = "returns", Flow = TopLevelTagFlow.FirstTryOneLine },
        new TopLevelTagInfo { Name = "value", Flow = TopLevelTagFlow.FirstTryOneLine },
        new TopLevelTagInfo { Name = "exception", Attribute = TopLevelTagAttribute.Cref, Flow = TopLevelTagFlow.FirstTryOneLine },
        new TopLevelTagInfo { Name = "remarks", Flow = TopLevelTagFlow.Multiline },
        new TopLevelTagInfo { Name = "example", Flow = TopLevelTagFlow.FirstTryOneLine },
        new TopLevelTagInfo { Name = @"seealso", Attribute = TopLevelTagAttribute.CrefOrHref, Flow = TopLevelTagFlow.FirstTryOneLine },
    ];

    static readonly IReadOnlyDictionary<string, TopLevelTagInfo> topLevelTagsByName = topLevelTags.ToDictionary(
        tag => tag.Name,
        StringComparer.Ordinal);

    static readonly IReadOnlyDictionary<string, NestedTagInfo> nestedTagByName = new[]
    {
        new NestedTagInfo { Name = "para", Interior = NestedTagInterior.Any, Flow = NestedTagFlow.InlineWithLineBreak },
        new NestedTagInfo { Name = "br", Interior = NestedTagInterior.None, Flow = NestedTagFlow.InlineWithLineBreak },
        new NestedTagInfo { Name = "c", Interior = NestedTagInterior.TextOnlyKeepingLineBreaks, Flow = NestedTagFlow.Inline },
        new NestedTagInfo { Name = "code", Interior = NestedTagInterior.TextOnlyKeepingLineBreaks, Flow = NestedTagFlow.Multiline },
        new NestedTagInfo
        {
            Name = "paramref", Attribute = NestedTagAttribute.Name, Interior = NestedTagInterior.None, Flow = NestedTagFlow.Inline,
        },
        new NestedTagInfo
        {
            Name = "typeparamref", Attribute = NestedTagAttribute.Name, Interior = NestedTagInterior.None, Flow = NestedTagFlow.Inline,
        },
        new NestedTagInfo
        {
            Name = "see", Interior = NestedTagInterior.TextOnly, Attribute = NestedTagAttribute.CrefOrHref, Flow = NestedTagFlow.Inline,
        },
        new NestedTagInfo
        {
            Name = "list",
            Attribute = NestedTagAttribute.Type,
            Interior = NestedTagInterior.ListHeaderAndItems,
            Flow = NestedTagFlow.Multiline,
        },
        new NestedTagInfo { Name = @"listheader", Interior = NestedTagInterior.Any, Flow = NestedTagFlow.FirstTryOneLine },
        new NestedTagInfo { Name = "item", Interior = NestedTagInterior.Any, Flow = NestedTagFlow.FirstTryOneLine },
        new NestedTagInfo { Name = "term", Interior = NestedTagInterior.Any, Flow = NestedTagFlow.FirstTryOneLine },
        new NestedTagInfo { Name = "description", Interior = NestedTagInterior.Any, Flow = NestedTagFlow.FirstTryOneLine },
    }.ToDictionary(tag => tag.Name, StringComparer.Ordinal);

    static readonly XmlTokenTypes xmlTokenTypes = XmlTokenTypes.GetInstance<XmlDocLanguage>();

    enum TagOption
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

    static void AppendTag(StringBuilder builder, string tagName, IXmlAttribute? attribute, TagOption option, Settings settings)
    {
        Debug.Assert(
            option is TagOption.Collapsed or TagOption.Expanded or TagOption.HeaderOnly || option == TagOption.FooterOnly && attribute is not { });

        builder.Append('<');

        if (option == TagOption.FooterOnly)
        {
            builder.Append('/');
        }

        builder.Append(tagName);

        if (attribute is { })
        {
            builder.Append(' ');
            builder.Append(attribute.AttributeName);

            if (settings.TagSpacesAroundAttributeEq)
            {
                builder.Append(" = ");
            }
            else
            {
                builder.Append('=');
            }

            builder.Append('"');
            builder.Append(attribute.Value?.UnquotedValue);
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

    [Pure]
    static string BuildTag(string tagName, IXmlAttribute? attribute, string? interior, TagOption option, Settings settings)
    {
        var builder = new StringBuilder();

        if (interior is { })
        {
            Debug.Assert(option == TagOption.Expanded);

            AppendTag(builder, tagName, attribute, TagOption.HeaderOnly, settings);
            builder.Append(interior);
            AppendTag(builder, tagName, null, TagOption.FooterOnly, settings);
        }
        else
        {
            AppendTag(builder, tagName, attribute, option, settings);
        }

        return builder.ToString();
    }

    [Pure]
    static IReadOnlyList<ITypeParameterDeclaration>? TryGetTypeParameters(ITreeNode? treeNode)
        => treeNode switch
        {
            IMethodDeclaration methodDeclaration => methodDeclaration.TypeParameterDeclarations,
            ILocalFunctionDeclaration localFunctionDeclaration => localFunctionDeclaration.TypeParameterDeclarations,
            IClassLikeDeclaration classLikeDeclaration => classLikeDeclaration.TypeParameters,
            IDelegateDeclaration delegateDeclaration => delegateDeclaration.TypeParameters,

            _ => null,
        };

    [Pure]
    static IReadOnlyList<IParameterDeclaration>? TryGetParameters(ITreeNode? treeNode)
        => (treeNode as IParametersOwnerDeclaration)?.ParameterDeclarations as IReadOnlyList<IParameterDeclaration>;

    [Pure]
    static bool AreEqual(NodeType x, NodeType y) => x.Index == y.Index;

    [Pure]
    static IEnumerable<Token> Tokenize(IXmlTag tag, Settings settings)
    {
        var space = false;

        foreach (var node in tag.InnerXml)
        {
            if (AreEqual(node.NodeType, xmlTokenTypes.SPACE) || AreEqual(node.NodeType, xmlTokenTypes.NEW_LINE))
            {
                space = true;
                continue;
            }

            if (AreEqual(node.NodeType, xmlTokenTypes.TEXT))
            {
                yield return Token.Lexeme(node.GetText(), space);
                space = false;
                continue;
            }

            if (node is IXmlTag nestedTag)
            {
                var tagName = nestedTag.GetFullTagName();

                if (nestedTagByName.TryGetValue(tagName, out var nestedTagInfo))
                {
                    switch (nestedTagInfo.Attribute, nestedTagInfo.Interior, nestedTagInfo.Flow)
                    {
                        case (NestedTagAttribute.None, NestedTagInterior.Any, NestedTagFlow.InlineWithLineBreak):
                        {
                            // typical case: <para/>

                            if (nestedTag.IsEmptyTag
                                || !Tokenize(nestedTag, settings)
                                    .Any(
                                        t => t is TextToken
                                            or InstructionToken
                                            {
                                                Kind: InstructionTokenKind.ForceLineBreak
                                                or InstructionTokenKind.BeginMultiline
                                                or InstructionTokenKind.EndMultiline
                                                or InstructionTokenKind.BeginOneLineAttempt
                                                or InstructionTokenKind.EndOneLineAttempt,
                                            }))
                            {
                                yield return Token.Tag(BuildTag(tagName, null, null, TagOption.Collapsed, settings), nestedTagInfo);
                            }
                            else
                            {
                                yield return Token.Tag(BuildTag(tagName, null, null, TagOption.HeaderOnly, settings), nestedTagInfo);

                                foreach (var nestedToken in Tokenize(nestedTag, settings))
                                {
                                    yield return nestedToken;
                                }

                                yield return Token.Tag(BuildTag(tagName, null, null, TagOption.FooterOnly, settings), nestedTagInfo);
                            }

                            yield return Token.ForceLineBreak;

                            space = false;
                            break;
                        }

                        case (NestedTagAttribute.None, NestedTagInterior.None, NestedTagFlow.InlineWithLineBreak):
                        {
                            // typical case: <br/>

                            yield return Token.Tag(BuildTag(tagName, null, null, TagOption.Collapsed, settings), nestedTagInfo);
                            yield return Token.ForceLineBreak;

                            space = false;

                            break;
                        }

                        case (NestedTagAttribute.None, NestedTagInterior.TextOnlyKeepingLineBreaks, NestedTagFlow.Inline):
                        {
                            // typical case: <c>...</c>

                            yield return Token.Tag(BuildTag(tagName, null, nestedTag.InnerValue, TagOption.Expanded, settings), nestedTagInfo, space);

                            space = false;
                            break;
                        }

                        case (NestedTagAttribute.None, NestedTagInterior.TextOnlyKeepingLineBreaks, NestedTagFlow.Multiline):
                        {
                            // typical case: <code>...</code>

                            yield return Token.BeginMultiline;
                            yield return Token.Tag(BuildTag(tagName, null, null, TagOption.HeaderOnly, settings), nestedTagInfo);
                            yield return Token.BeginIndentation;

                            var lines = nestedTag.InnerText.Split(["\r\n", "\n", "\r"], StringSplitOptions.None);

                            var (start, end) = (0, lines.Length - 1);

                            // skip leading empty strings
                            while (start < lines.Length && lines[start] == "")
                            {
                                start++;
                            }

                            // skip trailing empty strings
                            while (end >= 0 && lines[end] == "")
                            {
                                end--;
                            }

                            if (start <= end)
                            {
                                for (var i = start; i <= end; i++)
                                {
                                    yield return Token.Code(lines[i]);
                                }
                            }

                            yield return Token.EndIndentation;
                            yield return Token.Tag(BuildTag(tagName, null, null, TagOption.FooterOnly, settings), nestedTagInfo);
                            yield return Token.EndMultiline;

                            space = false;
                            break;
                        }

                        case (NestedTagAttribute.Name, NestedTagInterior.None, NestedTagFlow.Inline):
                        {
                            // typical case: <paramref name="..."/> or <typeparamref name="..."/>

                            yield return Token.Tag(
                                BuildTag(tagName, nestedTag.GetAttribute("name"), null, TagOption.Collapsed, settings),
                                nestedTagInfo,
                                space);

                            space = false;
                            break;
                        }

                        case (NestedTagAttribute.CrefOrHref, NestedTagInterior.TextOnly, NestedTagFlow.Inline):
                        {
                            // typical case: <see cref="...">...</see>

                            var attribute = nestedTag.GetAttribute("cref") ?? nestedTag.GetAttribute("href");
                            Debug.Assert(attribute is { });

                            yield return Token.Tag(
                                nestedTag.IsEmptyTag || nestedTag.InnerValue == ""
                                    ? BuildTag(tagName, attribute, null, TagOption.Collapsed, settings)
                                    : BuildTag(tagName, attribute, nestedTag.InnerValue, TagOption.Expanded, settings),
                                nestedTagInfo,
                                space);

                            space = false;
                            break;
                        }

                        case (NestedTagAttribute.Type, NestedTagInterior.ListHeaderAndItems, NestedTagFlow.Multiline):
                        {
                            // typical case: <list type="...">...</list>

                            yield return Token.BeginMultiline;
                            yield return Token.Tag(
                                BuildTag(tagName, nestedTag.GetAttribute("type"), null, TagOption.HeaderOnly, settings),
                                nestedTagInfo);
                            yield return Token.BeginIndentation;

                            foreach (var nestedToken in Tokenize(nestedTag, settings))
                            {
                                yield return nestedToken;
                            }

                            yield return Token.EndIndentation;
                            yield return Token.Tag(BuildTag(tagName, null, null, TagOption.FooterOnly, settings), nestedTagInfo);
                            yield return Token.EndMultiline;

                            space = false;
                            break;
                        }

                        case (NestedTagAttribute.None, NestedTagInterior.Any, NestedTagFlow.FirstTryOneLine):
                        {
                            // typical case: <listheader>...</listheader> or <item>...</item> or <term>...</term> or <description>...</description>

                            yield return Token.BeginOneLineAttempt;
                            yield return Token.Tag(BuildTag(tagName, null, null, TagOption.HeaderOnly, settings), nestedTagInfo);
                            yield return Token.BeginIndentation;

                            foreach (var nestedToken in Tokenize(nestedTag, settings))
                            {
                                yield return nestedToken;
                            }

                            yield return Token.EndIndentation;
                            yield return Token.Tag(BuildTag(tagName, null, null, TagOption.FooterOnly, settings), nestedTagInfo);
                            yield return Token.EndOneLineAttempt;

                            space = false;
                            break;
                        }
                    }
                }
                else
                {
                    // return unsupported tags "as is"
                    var attributes = string.Join(
                        " ",
                        from attribute in nestedTag.GetAttributes()
                        select settings.TagSpacesAroundAttributeEq
                            ? $"{attribute.AttributeName} = \"{attribute.Value?.UnquotedValue}\""
                            : $"{attribute.AttributeName}=\"{attribute.Value?.UnquotedValue}\"");
                    if (attributes != "")
                    {
                        attributes = $" {attributes}";
                    }

                    if (attributes != "" && settings.TagSpaceAfterLastAttr)
                    {
                        attributes = $"{attributes} ";
                    }

                    var spaceBeforeHeaderEnd = settings.TagSpaceBeforeHeaderEnd1 && (attributes == "" || !settings.TagSpaceAfterLastAttr) ? " " : "";

                    yield return nestedTag.IsEmptyTag
                        ? Token.Lexeme($"<{tagName}{attributes}{spaceBeforeHeaderEnd}/>")
                        : Token.Lexeme($"<{tagName}{attributes}>{nestedTag.InnerText}</{tagName}>");
                }
            }
        }
    }

    [Pure]
    static string? TryGetValidAttributeValue(IXmlTag tag, string primaryAttributeName, string? secondaryAttributeName = null)
    {
        var attributeValue = null as string;
        foreach (var attribute in tag.GetAttributes())
        {
            if (attribute.AttributeName == primaryAttributeName || attribute.AttributeName == secondaryAttributeName)
            {
                if (attribute.Value is { })
                {
                    if (attributeValue is { })
                    {
                        return null; // duplicate attribute
                    }

                    attributeValue = attribute.Value.UnquotedValue;
                }
            }
            else
            {
                return null; // unknown attribute
            }
        }

        return attributeValue; // null if attribute was not found
    }

    /// <remarks>
    /// <list type="bullet">
    /// <listheader><description>D</description></listheader>
    /// </list>
    /// </remarks>
    [Pure]
    static bool AreTopLevelTagsValid(TreeNodeCollection<IXmlTag> tags, ITreeNode? parentDeclaration)
    {
        var detectedTags = null as HashSet<string>;

        foreach (var tag in tags)
        {
            if (topLevelTagsByName.TryGetValue(tag.GetFullTagName(), out var tagInfo))
            {
                switch (tagInfo.Attribute)
                {
                    case TopLevelTagAttribute.None:
                    {
                        if (tag.GetAttributes().Any())
                        {
                            return false; // the tag should not have any attribute
                        }

                        detectedTags ??= new HashSet<string>(StringComparer.Ordinal);

                        if (detectedTags.Add(tagInfo.Name))
                        {
                            break;
                        }

                        return false; // there should only be one tag
                    }

                    case TopLevelTagAttribute.Name:
                    {
                        if (TryGetValidAttributeValue(tag, "name") is not { } attributeValue)
                        {
                            return false; // attribute not found, or duplicate, or an unknown attribute detected
                        }

                        if (tagInfo.TryGetDeclarations?.Invoke(parentDeclaration) is not { } declarations)
                        {
                            return false; // no declarations
                        }

                        if (declarations.All(declaration => declaration.DeclaredName != attributeValue))
                        {
                            return false; // unknown value
                        }

                        detectedTags ??= new HashSet<string>(StringComparer.Ordinal);

                        if (detectedTags.Add($"{tagInfo.Name}.{attributeValue}"))
                        {
                            break;
                        }

                        return false; // there should only be one tag with the attribute value
                    }

                    case TopLevelTagAttribute.Cref:
                    {
                        if (TryGetValidAttributeValue(tag, "cref") is not { })
                        {
                            return false; // attribute not found, or duplicate, or an unknown attribute detected
                        }

                        break;
                    }

                    case TopLevelTagAttribute.CrefOrHref:
                    {
                        if (TryGetValidAttributeValue(tag, "cref", "href") is not { })
                        {
                            return false; // attribute not found, or duplicate, or an unknown attribute detected
                        }

                        break;
                    }

                    default: throw new NotSupportedException();
                }

                if (!AreNestedTagsValid(tag.InnerTags))
                {
                    return false;
                }
            }
        }

        return true;
    }

    [Pure]
    static bool AreNestedTagsValid(TreeNodeCollection<IXmlTag> tags)
    {
        foreach (var tag in tags)
        {
            if (nestedTagByName.TryGetValue(tag.GetFullTagName(), out var tagInfo))
            {
                switch (tagInfo.Attribute)
                {
                    case NestedTagAttribute.None:
                    {
                        if (tag.GetAttributes().Any())
                        {
                            return false; // the tag should not have any attribute
                        }

                        break;
                    }

                    case NestedTagAttribute.Name:
                    {
                        if (TryGetValidAttributeValue(tag, "name") is not { })
                        {
                            return false; // attribute not found, or duplicate, or an unknown attribute detected
                        }

                        break;
                    }

                    case NestedTagAttribute.Type:
                    {
                        if (TryGetValidAttributeValue(tag, "type") is not { })
                        {
                            return false; // attribute not found, or duplicate, or an unknown attribute detected
                        }

                        break;
                    }

                    case NestedTagAttribute.CrefOrHref:
                    {
                        if (TryGetValidAttributeValue(tag, "cref", "href") is not { })
                        {
                            return false; // attribute not found, or duplicate, or an unknown attribute detected
                        }

                        break;
                    }

                    default: throw new NotSupportedException();
                }

                switch (tagInfo.Interior)
                {
                    case NestedTagInterior.None:
                    {
                        if (tag.InnerValue != "")
                        {
                            return false; // inner text or tag detected
                        }

                        break;
                    }

                    case NestedTagInterior.TextOnly or NestedTagInterior.TextOnlyKeepingLineBreaks:
                    {
                        if (tag.InnerTags.Any())
                        {
                            return false; // inner tag detected
                        }

                        break;
                    }

                    case NestedTagInterior.ListHeaderAndItems:
                    {
                        if (tag.InnerTextTokens.Any())
                        {
                            return false; // inner text detected
                        }

                        if (tag.InnerTags.Any(t => t.GetFullTagName() is not (@"listheader" or "item")))
                        {
                            return false; // unknown tags detected
                        }

                        if (tag.InnerTags.Count(t => t.GetFullTagName() == @"listheader") > 1)
                        {
                            return false; // multiple "listheader" tags detected
                        }

                        break;
                    }
                }

                if (!AreNestedTagsValid(tag.InnerTags))
                {
                    return false;
                }
            }
        }

        return true;
    }

    static void ReflowTokens(
        [InstantHandle] IEnumerable<Token> tokens,
        StringBuilder builder,
        [NonNegativeValue] int maxLength,
        [NonNegativeValue] int baseIndentation,
        Settings settings)
    {
        var indentation = baseIndentation;
        var pendingLineBreak = false;
        var tokenRecorder = null as List<Token>;

        var length = 0;

        var stack = null as Stack<(StringBuilder, int indentation, bool pendingLineBreak, List<Token>? tokenRecorder)>;

        foreach (var token in tokens)
        {
            tokenRecorder?.Add(token);

            switch (token)
            {
                case InstructionToken { Kind: InstructionTokenKind.ForceLineBreak }:
                    pendingLineBreak = true;
                    break;

                case InstructionToken { Kind: InstructionTokenKind.BeginMultiline }:
                    pendingLineBreak = true;
                    break;

                case InstructionToken { Kind: InstructionTokenKind.EndMultiline }:
                    pendingLineBreak = true;
                    break;

                case InstructionToken { Kind: InstructionTokenKind.BeginOneLineAttempt }:
                    stack ??= [];

                    stack.Push((builder, indentation, pendingLineBreak, tokenRecorder));
                    (builder, indentation, pendingLineBreak, tokenRecorder) = (new StringBuilder(), 0, false, []);

                    length = 0;
                    break;

                case InstructionToken { Kind: InstructionTokenKind.EndOneLineAttempt }:
                    Debug.Assert(stack is { Count: > 0 });
                    Debug.Assert(tokenRecorder is [_, ..]);

                    tokenRecorder.RemoveAt(tokenRecorder.Count - 1); // remove this instruction

                    var oneLineText = builder.ToString();
                    var recordedTokens = tokenRecorder;

                    (builder, indentation, pendingLineBreak, tokenRecorder) = stack.Pop();

                    if (tokenRecorder is { })
                    {
                        tokenRecorder.AddRange(recordedTokens); // add recorded nested tokens
                        tokenRecorder.Add(Token.EndOneLineAttempt); // add this instruction to the stacked token recorder
                    }

                    // todo: replace ".Skip(1).Take(...Count - 2)" with ".Take(1..^1)" when it becomes available
                    if (indentation + oneLineText.Length > maxLength
                        || recordedTokens
                            .Skip(1)
                            .Take(recordedTokens.Count - 2)
                            .Any(
                                t => t is TextToken
                                {
                                    TagInfo.Flow: NestedTagFlow.InlineWithLineBreak or NestedTagFlow.Multiline or NestedTagFlow.FirstTryOneLine,
                                }))
                    {
                        ReflowTokens(
                            recordedTokens.Prepend(Token.BeginMultiline).Append(Token.EndMultiline),
                            builder,
                            maxLength,
                            indentation,
                            settings);
                    }
                    else
                    {
                        if (pendingLineBreak)
                        {
                            builder.AppendLine();
                        }
                        builder.Append(' ', indentation);
                        builder.Append(oneLineText);
                    }

                    pendingLineBreak = true;
                    break;

                case InstructionToken { Kind: InstructionTokenKind.BeginIndentation }:
                    if (stack is not { Count: > 0 })
                    {
                        // not within a "one line attempt"

                        indentation += settings.IndentSize;
                        pendingLineBreak = true;
                    }
                    break;

                case InstructionToken { Kind: InstructionTokenKind.EndIndentation }:
                    if (stack is not { Count: > 0 })
                    {
                        // not within a "one line attempt"

                        indentation -= settings.IndentSize;
                        pendingLineBreak = true;
                    }
                    break;

                case TextToken { Kind: var kind, Text: var text, HasSpaceBefore: var hasSpaceBefore }:
                    if (pendingLineBreak || kind == TextTokenKind.Code || length + text.Length >= maxLength)
                    {
                        builder.AppendLine();

                        builder.Append(' ', indentation);
                        builder.Append(text);

                        length = indentation + text.Length;
                        pendingLineBreak = false;
                    }
                    else
                    {
                        if (length > 0)
                        {
                            if (hasSpaceBefore)
                            {
                                builder.Append(' ');
                                length++;
                            }
                        }
                        else
                        {
                            builder.Append(' ', indentation);
                            length += indentation;
                        }

                        builder.Append(text);
                        length += text.Length;
                    }

                    break;
            }
        }
    }

    /// <exception cref="NotSupportedException"></exception>
    static void ReflowTag(StringBuilder builder, IXmlTag tag, TopLevelTagInfo tagInfo, [NonNegativeValue] int maxLength, Settings settings)
    {
        var attribute = tagInfo.Attribute switch
        {
            TopLevelTagAttribute.None => null,
            TopLevelTagAttribute.Name => tag.GetAttribute("name"),
            TopLevelTagAttribute.Cref => tag.GetAttribute("cref"),
            TopLevelTagAttribute.CrefOrHref => tag.GetAttribute("cref") ?? tag.GetAttribute("href"),

            _ => throw new NotSupportedException(),
        };

        Debug.Assert(
            (RequiredAttribute: tagInfo.Attribute, attribute)
            is (TopLevelTagAttribute.Name or TopLevelTagAttribute.Cref or TopLevelTagAttribute.CrefOrHref, { Value.UnquotedValue: { } })
            or (TopLevelTagAttribute.None, not { }));

        var textBuilder = null as StringBuilder;

        if (tagInfo.Flow == TopLevelTagFlow.FirstTryOneLine)
        {
            if (tagInfo.Attribute == TopLevelTagAttribute.CrefOrHref)
            {
                textBuilder = new StringBuilder();
                ReflowTokens(Tokenize(tag, settings), textBuilder, maxLength, 0, settings);

                if (textBuilder.Length == 0)
                {
                    AppendTag(builder, tagInfo.Name, attribute, TagOption.Collapsed, settings);
                    builder.AppendLine();
                    return;
                }
            }

            if (tag.InnerTags.All(
                nestedTag => nestedTagByName.TryGetValue(nestedTag.GetTagName(), out var nestedTagInfo)
                    && nestedTagInfo.Flow == NestedTagFlow.Inline))
            {
                var oneLineBuilder = new StringBuilder();
                AppendTag(oneLineBuilder, tagInfo.Name, attribute, TagOption.HeaderOnly, settings);

                if (textBuilder is { })
                {
                    oneLineBuilder.Append(textBuilder);
                }
                else
                {
                    ReflowTokens(Tokenize(tag, settings), oneLineBuilder, maxLength, 0, settings);
                }

                AppendTag(oneLineBuilder, tagInfo.Name, null, TagOption.FooterOnly, settings);

                if (oneLineBuilder.Length <= maxLength)
                {
                    builder.AppendLine(oneLineBuilder.ToString());
                    return;
                }
            }
        }

        AppendTag(builder, tagInfo.Name, attribute, TagOption.HeaderOnly, settings);
        builder.AppendLine();
        ReflowTokens(Tokenize(tag, settings), builder, maxLength, tagInfo.Flow == TopLevelTagFlow.Multiline ? 0 : settings.IndentSize, settings);
        builder.AppendLine();
        AppendTag(builder, tagInfo.Name, null, TagOption.FooterOnly, settings);
        builder.AppendLine();
    }

    ICSharpDocCommentBlock? docCommentBlock;

    [MemberNotNullWhen(true, nameof(docCommentBlock))]
    public override bool IsAvailable(IUserDataHolder cache)
    {
        docCommentBlock = provider.GetSelectedElement<ICSharpDocCommentBlock>();

        if (docCommentBlock is { })
        {
            if (docCommentBlock.DocComments.All(docComment => docComment.CommentType == CommentType.DOC_COMMENT)
                && AreTopLevelTagsValid(docCommentBlock.GetXmlPsi().XmlFile.InnerTags, docCommentBlock.Parent))
            {
                return true; // supporting only "///"-style doc comments
            }

            docCommentBlock = null;
        }

        return false;
    }

    public override string Text => "Reflow";

    protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
    {
        try
        {
            Debug.Assert(docCommentBlock is { });

            using (WriteLockCookie.Create())
            {
                var settings = Settings.Load(provider.PsiServices.SettingsStore.BindToContextTransient(ContextRange.ApplicationWide));

                var position = (int)docCommentBlock.GetDocumentRange().StartOffset.ToDocumentCoords().Column;

                var prefixLength = "/// ".Length;

                var maxLength = settings.WrapLimit - prefixLength - position;

                var tags = docCommentBlock.GetXmlPsi().XmlFile.InnerTags;

                var builder = new StringBuilder();

                // add supported tags first

                foreach (var tagInfo in topLevelTags)
                {
                    var relevantTags = from tag in tags where tag.GetFullTagName() == tagInfo.Name select tag;

                    if (tagInfo.Attribute == TopLevelTagAttribute.Name
                        && tagInfo.TryGetDeclarations?.Invoke(docCommentBlock.Parent) is { } declarations)
                    {
                        // use the declaration order

                        var relevantTagsByName = relevantTags.ToDictionary(
                            tag => tag.GetAttribute("name")?.Value?.UnquotedValue,
                            StringComparer.Ordinal);

                        foreach (var declaration in declarations)
                        {
                            ReflowTag(builder, relevantTagsByName[declaration.DeclaredName], tagInfo, maxLength, settings);
                        }
                    }
                    else
                    {
                        foreach (var tag in relevantTags)
                        {
                            ReflowTag(builder, tag, tagInfo, maxLength, settings);
                        }
                    }
                }

                // add remaining tags "as is"

                foreach (var tag in tags)
                {
                    if (!topLevelTagsByName.ContainsKey(tag.GetFullTagName()))
                    {
                        builder.AppendLine(tag.GetText());
                    }
                }

                if (builder.Length > 0)
                {
                    var factory = CSharpElementFactory.GetInstance(docCommentBlock);

                    ModificationUtil.ReplaceChild(docCommentBlock, factory.CreateDocCommentBlock(builder.ToString()));
                }
            }

            return _ => { };
        }
        finally
        {
            docCommentBlock = null;
        }
    }
}