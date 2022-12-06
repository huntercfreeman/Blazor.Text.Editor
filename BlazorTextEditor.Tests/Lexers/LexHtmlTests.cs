using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Analysis.Html;
using BlazorTextEditor.RazorLib.Analysis.Html.Decoration;
using BlazorTextEditor.RazorLib.Analysis.Html.SyntaxActors;
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

        var expectedTextEditorTextSpans = new[]
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
        
        Assert.Equal(expectedTextEditorTextSpans, textEditorTextSpans);
    }
    
    [Fact]
    public async Task LexAttributeName()
    {
        var text = TestData.Html.EXAMPLE_TEXT_19_LINES
            .ReplaceLineEndings("\n");

        var expectedTextEditorTextSpans = new[]
        {
            new TextEditorTextSpan(16, 19, 1),
            new TextEditorTextSpan(60, 64, 1),
            new TextEditorTextSpan(72, 76, 1),
            new TextEditorTextSpan(88, 90, 1),
            new TextEditorTextSpan(116, 119, 1),
            new TextEditorTextSpan(160, 164, 1),
            new TextEditorTextSpan(176, 180, 1),
            new TextEditorTextSpan(192, 194, 1),
            new TextEditorTextSpan(220, 224, 1),
            new TextEditorTextSpan(233, 237, 1),
            new TextEditorTextSpan(247, 252, 1),
            new TextEditorTextSpan(284, 288, 1),
            new TextEditorTextSpan(297, 301, 1),
            new TextEditorTextSpan(311, 316, 1),
            new TextEditorTextSpan(352, 356, 1),
            new TextEditorTextSpan(365, 369, 1),
            new TextEditorTextSpan(379, 384, 1),
            new TextEditorTextSpan(416, 420, 1),
            new TextEditorTextSpan(456, 458, 1),
            new TextEditorTextSpan(486, 491, 1),
            new TextEditorTextSpan(524, 529, 1),
            new TextEditorTextSpan(562, 567, 1),
            new TextEditorTextSpan(616, 620, 1),
            new TextEditorTextSpan(630, 635, 1),
            new TextEditorTextSpan(659, 663, 1),
            new TextEditorTextSpan(690, 694, 1),
            new TextEditorTextSpan(706, 710, 1),
            new TextEditorTextSpan(721, 726, 1),
        };
        
        var htmlLexer = new TextEditorHtmlLexer();

        var textEditorTextSpans = 
            await htmlLexer.Lex(text);

        textEditorTextSpans = textEditorTextSpans
            .Where(x => x.DecorationByte == (byte)HtmlDecorationKind.AttributeName)
            .OrderBy(x => x.StartingIndexInclusive)
            .ToImmutableArray();
        
        Assert.Equal(expectedTextEditorTextSpans, textEditorTextSpans);
    }
    
    [Fact]
    public async Task LexAttributeValue()
    {
        var text = TestData.Html.EXAMPLE_TEXT_19_LINES
            .ReplaceLineEndings("\n");

        var expectedTextEditorTextSpans = new[]
        {
            new TextEditorTextSpan(21, 29, 2),
            new TextEditorTextSpan(66, 70, 2),
            new TextEditorTextSpan(78, 86, 2),
            new TextEditorTextSpan(92, 100, 2),
            new TextEditorTextSpan(121, 129, 2),
            new TextEditorTextSpan(166, 174, 2),
            new TextEditorTextSpan(182, 190, 2),
            new TextEditorTextSpan(196, 204, 2),
            new TextEditorTextSpan(226, 231, 2),
            new TextEditorTextSpan(239, 245, 2),
            new TextEditorTextSpan(254, 258, 2),
            new TextEditorTextSpan(290, 295, 2),
            new TextEditorTextSpan(303, 309, 2),
            new TextEditorTextSpan(318, 324, 2),
            new TextEditorTextSpan(358, 363, 2),
            new TextEditorTextSpan(371, 377, 2),
            new TextEditorTextSpan(386, 391, 2),
            new TextEditorTextSpan(422, 429, 2),
            new TextEditorTextSpan(460, 467, 2),
            new TextEditorTextSpan(493, 500, 2),
            new TextEditorTextSpan(531, 538, 2),
            new TextEditorTextSpan(569, 576, 2),
            new TextEditorTextSpan(622, 628, 2),
            new TextEditorTextSpan(637, 643, 2),
            new TextEditorTextSpan(665, 670, 2),
            new TextEditorTextSpan(696, 704, 2),
            new TextEditorTextSpan(712, 719, 2),
            new TextEditorTextSpan(728, 735, 2),
        };
        
        var htmlLexer = new TextEditorHtmlLexer();

        var textEditorTextSpans = 
            await htmlLexer.Lex(text);

        textEditorTextSpans = textEditorTextSpans
            .Where(x => x.DecorationByte == (byte)HtmlDecorationKind.AttributeValue)
            .OrderBy(x => x.StartingIndexInclusive)
            .ToImmutableArray();
        
        Assert.Equal(expectedTextEditorTextSpans, textEditorTextSpans);
    }
    
    // TODO: A more extensive test should be made this is only top level comment tags. I checked manually and a HTML element containing a comment works fine but there should be a test for deeply nested comments.
    [Fact]
    public async Task LexComment()
    {
        var text = TestData.Html.EXAMPLE_TEXT_COMMENTS
            .ReplaceLineEndings("\n");

        var expectedTextEditorTextSpans = new[]
        {
            new TextEditorTextSpan(35, 82, 3 ),
            new TextEditorTextSpan(118, 140, 3 ),
            new TextEditorTextSpan(154, 210, 3 ),
            new TextEditorTextSpan(426, 450, 3 ),
        };
        
        var htmlLexer = new TextEditorHtmlLexer();

        var textEditorTextSpans = 
            await htmlLexer.Lex(text);

        textEditorTextSpans = textEditorTextSpans
            .Where(x => x.DecorationByte == (byte)HtmlDecorationKind.Comment)
            .OrderBy(x => x.StartingIndexInclusive)
            .ToImmutableArray();
        
        Assert.Equal(expectedTextEditorTextSpans, textEditorTextSpans);
    }
}