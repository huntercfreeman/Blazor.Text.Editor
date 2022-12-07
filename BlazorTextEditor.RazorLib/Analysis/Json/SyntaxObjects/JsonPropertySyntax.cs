using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Analysis.Json.SyntaxEnums;
using BlazorTextEditor.RazorLib.Lexing;

namespace BlazorTextEditor.RazorLib.Analysis.Json.SyntaxObjects;

public class JsonPropertySyntax : IJsonSyntax
{
    public JsonPropertySyntax(
        TextEditorTextSpan textEditorTextSpan,
        JsonPropertyKeySyntax jsonPropertyKeySyntax,
        JsonPropertyValueSyntax jsonPropertyValueSyntax)
    {
        TextEditorTextSpan = textEditorTextSpan;
        JsonPropertyKeySyntax = jsonPropertyKeySyntax;
        JsonPropertyValueSyntax = jsonPropertyValueSyntax;
    }
    
    public TextEditorTextSpan TextEditorTextSpan { get; }
    public JsonPropertyKeySyntax JsonPropertyKeySyntax { get; }
    public JsonPropertyValueSyntax JsonPropertyValueSyntax { get; }
    public ImmutableArray<IJsonSyntax> ChildJsonSyntaxes => new IJsonSyntax[]
    {
        JsonPropertyKeySyntax,
        JsonPropertyValueSyntax
    }.ToImmutableArray();

    public JsonSyntaxKind JsonSyntaxKind => JsonSyntaxKind.Property;
}