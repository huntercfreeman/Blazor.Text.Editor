using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Analysis.Json.SyntaxEnums;
using BlazorTextEditor.RazorLib.Lexing;

namespace BlazorTextEditor.RazorLib.Analysis.Json.SyntaxObjects;

public class JsonDocumentSyntax : IJsonSyntax
{
    public JsonDocumentSyntax(
        TextEditorTextSpan textEditorTextSpan,
        ImmutableArray<IJsonSyntax> childJsonSyntaxes)
    {
        ChildJsonSyntaxes = childJsonSyntaxes;
        TextEditorTextSpan = textEditorTextSpan;
    }
    
    public TextEditorTextSpan TextEditorTextSpan { get; }
    public ImmutableArray<IJsonSyntax> ChildJsonSyntaxes { get; }
    
    public JsonSyntaxKind JsonSyntaxKind => JsonSyntaxKind.Document;
}