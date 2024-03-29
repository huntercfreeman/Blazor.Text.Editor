using BlazorCommon.RazorLib.Misc;
using BlazorTextEditor.RazorLib.Lexing;
using BlazorTextEditor.Tests.TestDataFolder;

namespace BlazorTextEditor.Tests.Basics.Lexers;

public class LexPlainTests
{
    [Fact]
    public async Task LexNothing()
    {
        var text = TestData.Plain.EXAMPLE_TEXT_25_LINES
            .ReplaceLineEndings("\n");

        var defaultLexer = new TextEditorLexerDefault();

        var textEditorTextSpans = await defaultLexer.Lex(
            text,
            RenderStateKey.NewRenderStateKey());

        Assert.Empty(textEditorTextSpans);
    }
}