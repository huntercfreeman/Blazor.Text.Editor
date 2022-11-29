using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Lexing;

namespace BlazorTextEditor.RazorLib.Analysis.Json.SyntaxItems;

public class JsonPropertyValueSyntax : IJsonSyntax
{
    public JsonPropertyValueSyntax(
        TextEditorTextSpan textEditorTextSpan,
        IJsonSyntax underlyingJsonSyntax)
    {
        UnderlyingJsonSyntax = underlyingJsonSyntax;
        TextEditorTextSpan = textEditorTextSpan;
    }

    public IJsonSyntax UnderlyingJsonSyntax { get; }
    public TextEditorTextSpan TextEditorTextSpan { get; }
    public ImmutableArray<IJsonSyntax> ChildJsonSyntaxes => new IJsonSyntax[]
    {
        // TODO: UnderlyingJsonSyntax
    }.ToImmutableArray();
    
    public JsonSyntaxKind JsonSyntaxKind => JsonSyntaxKind.PropertyValue;
}