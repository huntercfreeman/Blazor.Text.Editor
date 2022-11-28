using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Analysis.Html;
using BlazorTextEditor.RazorLib.Analysis.Html.Decoration;
using BlazorTextEditor.RazorLib.Lexing;
using BlazorTextEditor.Tests.TestDataFolder;

namespace BlazorTextEditor.Tests.Lexers;

public class LexHtmlTests
{
    [Fact]
    public async Task LexTagNames()
    {
        var text = TestData.Html.EXAMPLE_TEXT_19_LINES
            .ReplaceLineEndings("\n");

        var expectedTagNameTextEditorTextSpans = new[]
        {
            new TextEditorTextSpan(1, 5, 8),
            new TextEditorTextSpan(10, 15, 8),
            new TextEditorTextSpan(42, 47, 8),
            new TextEditorTextSpan(54, 59, 8),
            new TextEditorTextSpan(110, 115, 8),
            new TextEditorTextSpan(142, 147, 8),
            new TextEditorTextSpan(154, 159, 8),
            new TextEditorTextSpan(214, 219, 8),
            new TextEditorTextSpan(267, 269, 8),
            new TextEditorTextSpan(278, 283, 8),
            new TextEditorTextSpan(335, 337, 8),
            new TextEditorTextSpan(346, 351, 8),
            new TextEditorTextSpan(410, 415, 8),
            new TextEditorTextSpan(447, 455, 8),
            new TextEditorTextSpan(479, 485, 8),
            new TextEditorTextSpan(504, 510, 8),
            new TextEditorTextSpan(517, 523, 8),
            new TextEditorTextSpan(542, 548, 8),
            new TextEditorTextSpan(555, 561, 8),
            new TextEditorTextSpan(580, 586, 8),
            new TextEditorTextSpan(594, 602, 8),
            new TextEditorTextSpan(610, 615, 8),
            new TextEditorTextSpan(653, 658, 8),
            new TextEditorTextSpan(684, 689, 8),
            new TextEditorTextSpan(757, 761, 8),
        };
        
        var htmlLexer = new TextEditorHtmlLexer();

        var textEditorTextSpans = 
            await htmlLexer.Lex(text);

        textEditorTextSpans = textEditorTextSpans
            .Where(x => x.DecorationByte == (byte)HtmlDecorationKind.TagName)
            .OrderBy(x => x.StartingIndexInclusive)
            .ToImmutableArray();
        
        Assert.Equal(expectedTagNameTextEditorTextSpans, textEditorTextSpans);
    }
    
    [Fact]
    public async Task LexAttributeName()
    {
        throw new NotImplementedException();
    }
    
    [Fact]
    public async Task LexAttributeValue()
    {
        throw new NotImplementedException();
    }
    
    [Fact]
    public async Task LexComment()
    {
        throw new NotImplementedException();
    }
}