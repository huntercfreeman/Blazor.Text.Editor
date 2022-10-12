using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Lexing;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace ExampleApplication.SyntaxHighlighting.JavaScript;

public class TextEditorCSharpLexer : ILexer
{
    public async Task<ImmutableArray<TextEditorTextSpan>> Lex(string content)
    {
        
    }
    
}