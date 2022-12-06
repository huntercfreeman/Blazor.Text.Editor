using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Analysis.JavaScript.SyntaxActors;
using BlazorTextEditor.RazorLib.Analysis.TypeScript.Facts;
using BlazorTextEditor.RazorLib.Lexing;

namespace BlazorTextEditor.RazorLib.Analysis.TypeScript.SyntaxActors;

public class TextEditorTypeScriptLexer : ILexer
{
    public Task<ImmutableArray<TextEditorTextSpan>> Lex(string text)
    {
        var javaScriptLexerWithKeywordsOverriden = new TextEditorJavaScriptLexer(
            TypeScriptKeywords.ALL);

        return javaScriptLexerWithKeywordsOverriden.Lex(text);
    }
}