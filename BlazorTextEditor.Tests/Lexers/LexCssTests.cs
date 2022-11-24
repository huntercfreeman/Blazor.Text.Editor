using BlazorTextEditor.RazorLib.Analysis.Css;
using BlazorTextEditor.RazorLib.Lexing;
using BlazorTextEditor.Tests.TestDataFolder;

namespace BlazorTextEditor.Tests.Lexers;

public class LexCssTests
{
    [Fact]
    public async Task LexTagSelectors()
    {
        var text = TestData.Css.EXAMPLE_TEXT_21_LINES
            .ReplaceLineEndings("\n");

        var cssLexer = new TextEditorCssLexer();

        var textEditorTextSpans = 
            await cssLexer.Lex(text);

        Assert.Empty(textEditorTextSpans);
    }
}