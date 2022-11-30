using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Lexing;

namespace BlazorTextEditor.RazorLib.Analysis.Json.SyntaxItems;

public class JsonIntegerSyntax : IJsonSyntax
{
    public JsonIntegerSyntax(
        TextEditorTextSpan textEditorTextSpan)
    {
        TextEditorTextSpan = textEditorTextSpan;
    }
    
    public TextEditorTextSpan TextEditorTextSpan { get; }
    public ImmutableArray<IJsonSyntax> ChildJsonSyntaxes => ImmutableArray<IJsonSyntax>.Empty;
    
    public JsonSyntaxKind JsonSyntaxKind => JsonSyntaxKind.Integer;
}