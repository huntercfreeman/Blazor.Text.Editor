using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Analysis.JavaScript;
using BlazorTextEditor.RazorLib.Analysis.JavaScript.Decoration;
using BlazorTextEditor.RazorLib.Analysis.JavaScript.Facts;
using BlazorTextEditor.RazorLib.Analysis.JavaScript.SyntaxActors;
using BlazorTextEditor.RazorLib.Lexing;
using BlazorTextEditor.Tests.TestDataFolder;

namespace BlazorTextEditor.Tests.Lexers;

public class LexJavaScriptTests
{
    [Fact]
    public async Task LexKeywords()
    {
        var text = TestData.JavaScript.EXAMPLE_TEXT
            .ReplaceLineEndings("\n");

        var expectedKeywordTextEditorTextSpans = new[]
        {
            new TextEditorTextSpan(137, 140, 1),
            new TextEditorTextSpan(185, 193, 1),
            new TextEditorTextSpan(260, 265, 1),
            new TextEditorTextSpan(273, 276, 1),
            new TextEditorTextSpan(292, 297, 1),
            new TextEditorTextSpan(305, 308, 1),
            new TextEditorTextSpan(325, 328, 1),
            new TextEditorTextSpan(359, 362, 1),
            new TextEditorTextSpan(385, 388, 1),
            new TextEditorTextSpan(390, 393, 1),
            new TextEditorTextSpan(420, 422, 1),
            new TextEditorTextSpan(513, 519, 1),
            new TextEditorTextSpan(544, 547, 1),
            new TextEditorTextSpan(566, 571, 1),
            new TextEditorTextSpan(598, 603, 1),
            new TextEditorTextSpan(628, 633, 1),
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
    
    [Fact]
    public async Task LexComments()
    {
        var text = TestData.JavaScript.EXAMPLE_TEXT
            .ReplaceLineEndings("\n");

        var expectedKeywordTextEditorTextSpans = new[]
        {
            new TextEditorTextSpan(137, 145,1),
            new TextEditorTextSpan(212, 217,1),
            new TextEditorTextSpan(225, 228,1),
            new TextEditorTextSpan(244, 249,1),
            new TextEditorTextSpan(257, 260,1),
            new TextEditorTextSpan(277, 280,1),
            new TextEditorTextSpan(311, 314,1),
            new TextEditorTextSpan(316, 319,1),
            new TextEditorTextSpan(346, 348,1),
            new TextEditorTextSpan(439, 445,1),
            new TextEditorTextSpan(470, 475,1),
            new TextEditorTextSpan(502, 507,1),
            new TextEditorTextSpan(532, 537,1),
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