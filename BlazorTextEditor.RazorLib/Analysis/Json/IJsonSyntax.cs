using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Analysis.Json.SyntaxEnums;
using BlazorTextEditor.RazorLib.Lexing;

namespace BlazorTextEditor.RazorLib.Analysis.Json;

public interface IJsonSyntax
{
    public JsonSyntaxKind JsonSyntaxKind { get; }
    public TextEditorTextSpan TextEditorTextSpan { get; }
    public ImmutableArray<IJsonSyntax> ChildJsonSyntaxes { get; }
}