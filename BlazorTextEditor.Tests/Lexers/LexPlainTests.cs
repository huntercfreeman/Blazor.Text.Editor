using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Analysis.Html.Decoration;
using BlazorTextEditor.RazorLib.Analysis.JavaScript;
using BlazorTextEditor.RazorLib.Analysis.Razor;
using BlazorTextEditor.RazorLib.Lexing;
using BlazorTextEditor.Tests.TestDataFolder;

namespace BlazorTextEditor.Tests.Lexers;

public class LexPlainTests
{
    [Fact]
    public async Task LexNothing()
    {
        var text = TestData.Plain.EXAMPLE_TEXT_25_LINES
            .ReplaceLineEndings("\n");

        var defaultLexer = new TextEditorLexerDefault();

        var textEditorTextSpans = 
            await defaultLexer.Lex(text);

        Assert.Empty(textEditorTextSpans);
    }
}