using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Analysis.Html.Decoration;
using BlazorTextEditor.RazorLib.Analysis.Razor;
using BlazorTextEditor.RazorLib.Analysis.Razor.SyntaxActors;
using BlazorTextEditor.RazorLib.Lexing;
using BlazorTextEditor.Tests.TestDataFolder;

namespace BlazorTextEditor.Tests.Lexers;

public class LexRazorTests
{
    [Fact]
    public async Task LexTagNames()
    {
        var text = TestData.Razor.EXAMPLE_TEXT_20_LINES
            .ReplaceLineEndings("\n");

        var expectedTagNameTextEditorTextSpans = new[]
        {
            new TextEditorTextSpan(1, 4, 8),
            new TextEditorTextSpan(44, 47, 8),
            new TextEditorTextSpan(76, 78, 8),
            new TextEditorTextSpan(106, 108, 8),
            new TextEditorTextSpan(119, 120, 8),
            new TextEditorTextSpan(154, 155, 8),
            new TextEditorTextSpan(166, 172, 8),
            new TextEditorTextSpan(196, 202, 8),
            new TextEditorTextSpan(210, 213, 8),
            new TextEditorTextSpan(217, 220, 8),
        };
        
        var razorLexer = new TextEditorRazorLexer();

        var textEditorTextSpans = 
            await razorLexer.Lex(text);

        textEditorTextSpans = textEditorTextSpans
            .Where(x => x.DecorationByte == (byte)HtmlDecorationKind.TagName)
            .OrderBy(x => x.StartingIndexInclusive)
            .ToImmutableArray();
        
        Assert.Equal(expectedTagNameTextEditorTextSpans, textEditorTextSpans);
    }
    
    [Fact]
    public async Task LexKeywords()
    {
        var text = TestData.Razor.EXAMPLE_TEXT_20_LINES
            .ReplaceLineEndings("\n");

        var expectedKeywordTextEditorTextSpans = new[]
        {
            new TextEditorTextSpan(250, 256, 13),
            new TextEditorTextSpan(288, 291, 13),
            new TextEditorTextSpan(293, 296, 13),
            new TextEditorTextSpan(321, 327, 13),
            new TextEditorTextSpan(328, 334, 13),
            new TextEditorTextSpan(344, 347, 13),
            new TextEditorTextSpan(349, 352, 13),
            new TextEditorTextSpan(361, 368, 13),
            new TextEditorTextSpan(369, 373, 13),
        };
        
        var razorLexer = new TextEditorRazorLexer();

        var textEditorTextSpans = 
            await razorLexer.Lex(text);

        textEditorTextSpans = textEditorTextSpans
            .Where(x => x.DecorationByte == (byte)HtmlDecorationKind.InjectedLanguageKeyword)
            .OrderBy(x => x.StartingIndexInclusive)
            .ToImmutableArray();
        
        Assert.Equal(expectedKeywordTextEditorTextSpans, textEditorTextSpans);
    }
}