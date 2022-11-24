using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Analysis.JavaScript;
using BlazorTextEditor.RazorLib.Lexing;
using BlazorTextEditor.Tests.TestDataFolder;

namespace BlazorTextEditor.Tests.Lexers;

public class LexJavaScriptTests
{
    [Fact]
    public async Task LexKeywords()
    {
        var text = TestData.JavaScript.EXAMPLE_TEXT_28_LINES
            .ReplaceLineEndings("\n");

        var expectedKeywordTextEditorTextSpans = new[]
        {
            new TextEditorTextSpan(126, 128, 1),
            new TextEditorTextSpan(137, 145, 1),
            new TextEditorTextSpan(212, 217, 1),
            new TextEditorTextSpan(225, 228, 1),
            new TextEditorTextSpan(244, 249, 1),
            new TextEditorTextSpan(257, 260, 1),
            new TextEditorTextSpan(277, 280, 1),
            new TextEditorTextSpan(311, 314, 1),
            new TextEditorTextSpan(346, 348, 1),
            new TextEditorTextSpan(439, 445, 1),
            new TextEditorTextSpan(470, 475, 1),
            new TextEditorTextSpan(502, 507, 1),
            new TextEditorTextSpan(532, 537, 1),
        };
        
        var javaScriptLexer = new TextEditorJavaScriptLexer();

        var textEditorTextSpans = 
            await javaScriptLexer.Lex(text);

        textEditorTextSpans = textEditorTextSpans
            .Where(x => x.DecorationByte == (byte)JavaScriptDecorationKind.Keyword)
            .OrderBy(x => x.StartingIndexInclusive)
            .ToImmutableArray();
        
        Assert.Equal(expectedKeywordTextEditorTextSpans, textEditorTextSpans);
    }
}