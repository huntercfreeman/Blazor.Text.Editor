using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Lexing;

namespace BlazorTextEditor.RazorLib.Analysis.Json.SyntaxItems;

/// <summary>
/// Comments are not valid in Standard JSON.
/// </summary>
public class JsonBlockCommentSyntax : IJsonSyntax
{
    public JsonBlockCommentSyntax(
        TextEditorTextSpan textEditorTextSpan,
        ImmutableArray<IJsonSyntax> childJsonSyntaxes)
    {
        ChildJsonSyntaxes = childJsonSyntaxes;
        TextEditorTextSpan = textEditorTextSpan;
    }
    
    public TextEditorTextSpan TextEditorTextSpan { get; }
    public ImmutableArray<IJsonSyntax> ChildJsonSyntaxes { get; }
    
    public JsonSyntaxKind JsonSyntaxKind => JsonSyntaxKind.BlockComment;
}