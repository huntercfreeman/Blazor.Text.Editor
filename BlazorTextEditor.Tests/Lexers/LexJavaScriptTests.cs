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
            new TextEditorTextSpan(246, 254, 1),
            new TextEditorTextSpan(321, 326, 1),
            new TextEditorTextSpan(334, 337, 1),
            new TextEditorTextSpan(353, 358, 1),
            new TextEditorTextSpan(366, 369, 1),
            new TextEditorTextSpan(386, 389, 1),
            new TextEditorTextSpan(420, 423, 1),
            new TextEditorTextSpan(446, 449, 1),
            new TextEditorTextSpan(451, 454, 1),
            new TextEditorTextSpan(481, 483, 1),
            new TextEditorTextSpan(574, 580, 1),
            new TextEditorTextSpan(605, 608, 1),
            new TextEditorTextSpan(627, 632, 1),
            new TextEditorTextSpan(659, 664, 1),
            new TextEditorTextSpan(689, 694, 1),
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
            new TextEditorTextSpan(0, 63, 4),
            new TextEditorTextSpan(64, 135, 4),
            new TextEditorTextSpan(185, 209, 4),
            new TextEditorTextSpan(211, 244, 4),
            new TextEditorTextSpan(294, 316, 4),
        };
        
        var javaScriptLexer = new TextEditorJavaScriptLexer();

        var textEditorTextSpans = 
            await javaScriptLexer.Lex(text);

        textEditorTextSpans = textEditorTextSpans
            .Where(x => x.DecorationByte == (byte)JavaScriptDecorationKind.Comment)
            .OrderBy(x => x.StartingIndexInclusive)
            .ToImmutableArray();
        
        Assert.Equal(expectedKeywordTextEditorTextSpans, textEditorTextSpans);
    }
    
    [Fact]
    public async Task LexStrings()
    {
        var text = TestData.JavaScript.EXAMPLE_TEXT
            .ReplaceLineEndings("\n");

        var expectedKeywordTextEditorTextSpans = new[]
        {
            new TextEditorTextSpan(154,182, 3),
            new TextEditorTextSpan(432,439, 3),
            new TextEditorTextSpan(617,624, 3),
        };
        
        var javaScriptLexer = new TextEditorJavaScriptLexer();

        var textEditorTextSpans = 
            await javaScriptLexer.Lex(text);

        textEditorTextSpans = textEditorTextSpans
            .Where(x => x.DecorationByte == (byte)JavaScriptDecorationKind.String)
            .OrderBy(x => x.StartingIndexInclusive)
            .ToImmutableArray();
        
        Assert.Equal(expectedKeywordTextEditorTextSpans, textEditorTextSpans);
    }
}