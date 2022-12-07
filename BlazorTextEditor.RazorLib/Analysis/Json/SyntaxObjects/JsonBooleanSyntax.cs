using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Analysis.Json.SyntaxEnums;
using BlazorTextEditor.RazorLib.Lexing;

namespace BlazorTextEditor.RazorLib.Analysis.Json.SyntaxObjects;

public class JsonBooleanSyntax : IJsonSyntax
{
    public JsonBooleanSyntax(
        TextEditorTextSpan textEditorTextSpan)
    {
        TextEditorTextSpan = textEditorTextSpan;
    }
    
    public TextEditorTextSpan TextEditorTextSpan { get; }
    public ImmutableArray<IJsonSyntax> ChildJsonSyntaxes => ImmutableArray<IJsonSyntax>.Empty;

    public JsonSyntaxKind JsonSyntaxKind => JsonSyntaxKind.Boolean;
}