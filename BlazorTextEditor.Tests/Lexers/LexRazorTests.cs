using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Analysis.Html.Decoration;
using BlazorTextEditor.RazorLib.Analysis.Razor.SyntaxActors;
using BlazorTextEditor.RazorLib.Lexing;
using BlazorTextEditor.Tests.TestDataFolder;

namespace BlazorTextEditor.Tests.Lexers;

public class LexRazorTests
{
    [Fact]
    public async Task LexTagNames()
    {
        var text = TestData.Razor.EXAMPLE_TEXT
            .ReplaceLineEndings("\n");

        var expectedTagNameTextEditorTextSpans = new[]
        {
            new TextEditorTextSpan(1, 4, (byte)HtmlDecorationKind.TagName),
            new TextEditorTextSpan(44, 47, (byte)HtmlDecorationKind.TagName),
            new TextEditorTextSpan(76, 78, (byte)HtmlDecorationKind.TagName),
            new TextEditorTextSpan(106, 108, (byte)HtmlDecorationKind.TagName),
            new TextEditorTextSpan(119, 120, (byte)HtmlDecorationKind.TagName),
            new TextEditorTextSpan(154, 155, (byte)HtmlDecorationKind.TagName),
            new TextEditorTextSpan(166, 172, (byte)HtmlDecorationKind.TagName),
            new TextEditorTextSpan(196, 202, (byte)HtmlDecorationKind.TagName),
            new TextEditorTextSpan(210, 213, (byte)HtmlDecorationKind.TagName),
            new TextEditorTextSpan(217, 220, (byte)HtmlDecorationKind.TagName),
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
    public async Task LexInjectedLanguageKeywords()
    {
        var text = TestData.Razor.EXAMPLE_TEXT
            .ReplaceLineEndings("\n");

        var expectedKeywordTextEditorTextSpans = new[]
        {
            new TextEditorTextSpan(250, 256, (byte)HtmlDecorationKind.InjectedLanguageKeyword),
            new TextEditorTextSpan(288, 291, (byte)HtmlDecorationKind.InjectedLanguageKeyword),
            new TextEditorTextSpan(293, 296, (byte)HtmlDecorationKind.InjectedLanguageKeyword),
            new TextEditorTextSpan(321, 327, (byte)HtmlDecorationKind.InjectedLanguageKeyword),
            new TextEditorTextSpan(328, 334, (byte)HtmlDecorationKind.InjectedLanguageKeyword),
            new TextEditorTextSpan(344, 347, (byte)HtmlDecorationKind.InjectedLanguageKeyword),
            new TextEditorTextSpan(349, 352, (byte)HtmlDecorationKind.InjectedLanguageKeyword),
            new TextEditorTextSpan(361, 368, (byte)HtmlDecorationKind.InjectedLanguageKeyword),
            new TextEditorTextSpan(369, 373, (byte)HtmlDecorationKind.InjectedLanguageKeyword),
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