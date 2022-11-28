using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Analysis.Css;
using BlazorTextEditor.RazorLib.Lexing;
using BlazorTextEditor.Tests.TestDataFolder;

namespace BlazorTextEditor.Tests.Lexers;

public class LexCssTests
{
    [Fact]
    public async Task LexTagSelectors()
    {
        throw new NotImplementedException();
    }
    
    [Fact]
    public async Task LexCommentManyValid()
    {
        var text = TestData.Css.EXAMPLE_TEXT_21_LINES
            .ReplaceLineEndings("\n");

        var expectedTextEditorTextSpans = new[]
        {
            new TextEditorTextSpan(0, 107, 2),
            new TextEditorTextSpan(225, 268, 2),
            new TextEditorTextSpan(338, 407, 2),
            new TextEditorTextSpan(436, 498, 2),
        };
        
        var cssLexer = new TextEditorCssLexer();

        var textEditorTextSpans = 
            await cssLexer.Lex(text);
        
        textEditorTextSpans = textEditorTextSpans
            .Where(x => x.DecorationByte == (byte)CssDecorationKind.Comment)
            .OrderBy(x => x.StartingIndexInclusive)
            .ToImmutableArray();

        Assert.Equal(expectedTextEditorTextSpans, textEditorTextSpans);
    }
    
    [Fact]
    public async Task LexPropertyName()
    {
        var text = TestData.Css.EXAMPLE_TEXT_21_LINES
            .ReplaceLineEndings("\n");

        var expectedTextEditorTextSpans = new[]
        {
            new TextEditorTextSpan(117, 123, 3),
            new TextEditorTextSpan(133, 149, 3),
            new TextEditorTextSpan(171, 182, 3),
            new TextEditorTextSpan(205, 214, 3),
            new TextEditorTextSpan(276, 285, 3),
            new TextEditorTextSpan(295, 306, 3),
            new TextEditorTextSpan(318, 328, 3),
            new TextEditorTextSpan(421, 426, 3),
            new TextEditorTextSpan(509, 514, 3),
        };
        
        var cssLexer = new TextEditorCssLexer();

        var textEditorTextSpans = 
            await cssLexer.Lex(text);
        
        textEditorTextSpans = textEditorTextSpans
            .Where(x => x.DecorationByte == (byte)CssDecorationKind.PropertyName)
            .OrderBy(x => x.StartingIndexInclusive)
            .ToImmutableArray();

        Assert.Equal(expectedTextEditorTextSpans, textEditorTextSpans);
    }
    
    [Fact]
    public async Task LexPropertyValue()
    {
        var text = TestData.Css.EXAMPLE_TEXT_21_LINES
            .ReplaceLineEndings("\n");

        var expectedTextEditorTextSpans = new[]
        {
            new TextEditorTextSpan(125, 129, 4),
            new TextEditorTextSpan(151, 167, 4),
            new TextEditorTextSpan(184, 201, 4),
            new TextEditorTextSpan(216, 220, 4),
            new TextEditorTextSpan(287, 291, 4),
            new TextEditorTextSpan(308, 314, 4),
            new TextEditorTextSpan(330, 333, 4),
            new TextEditorTextSpan(428, 431, 4),
            new TextEditorTextSpan(516, 521, 4),
        };
        
        var cssLexer = new TextEditorCssLexer();

        var textEditorTextSpans = 
            await cssLexer.Lex(text);
        
        textEditorTextSpans = textEditorTextSpans
            .Where(x => x.DecorationByte == (byte)CssDecorationKind.PropertyValue)
            .OrderBy(x => x.StartingIndexInclusive)
            .ToImmutableArray();

        Assert.Equal(expectedTextEditorTextSpans, textEditorTextSpans);
    }
    
    [Fact]
    public async Task LexIdentifier()
    {
        var text = TestData.Css.EXAMPLE_TEXT_21_LINES
            .ReplaceLineEndings("\n");

        var expectedTextEditorTextSpans = new[]
        {
            new TextEditorTextSpan( 108, 112, 6),
            new TextEditorTextSpan( 269, 271, 6),
            new TextEditorTextSpan(409, 418, 6),
            new TextEditorTextSpan(500, 506, 6),
        };
        
        var cssLexer = new TextEditorCssLexer();

        var textEditorTextSpans = 
            await cssLexer.Lex(text);
        
        textEditorTextSpans = textEditorTextSpans
            .Where(x => x.DecorationByte == (byte)CssDecorationKind.Identifier)
            .OrderBy(x => x.StartingIndexInclusive)
            .ToImmutableArray();

        Assert.Equal(expectedTextEditorTextSpans, textEditorTextSpans);
    }
    
    [Fact]
    public async Task LexCommentSingleInvalid()
    {
        throw new NotImplementedException();
    }
}