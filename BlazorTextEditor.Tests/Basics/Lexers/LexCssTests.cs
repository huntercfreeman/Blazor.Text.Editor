using System.Collections.Immutable;
using BlazorCommon.RazorLib.Misc;
using BlazorTextEditor.RazorLib.Analysis.Css.Decoration;
using BlazorTextEditor.RazorLib.Analysis.Css.SyntaxActors;
using BlazorTextEditor.RazorLib.Lexing;
using BlazorTextEditor.Tests.TestDataFolder;

namespace BlazorTextEditor.Tests.Basics.Lexers;

public class LexCssTests
{
    [Fact]
    public async Task LexCommentManyValid()
    {
        var text = TestData.Css.EXAMPLE_TEXT_21_LINES
            .ReplaceLineEndings("\n");

        var expectedTextEditorTextSpans = new[]
        {
            new TextEditorTextSpan(0, 107, (byte)CssDecorationKind.Comment),
            new TextEditorTextSpan(225, 268, (byte)CssDecorationKind.Comment),
            new TextEditorTextSpan(338, 407, (byte)CssDecorationKind.Comment),
            new TextEditorTextSpan(436, 498, (byte)CssDecorationKind.Comment),
        };
        
        var cssLexer = new TextEditorCssLexer();

        var textEditorTextSpans = await cssLexer.Lex(
            text,
            RenderStateKey.NewRenderStateKey());
        
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
            new TextEditorTextSpan(117, 123, (byte)CssDecorationKind.PropertyName),
            new TextEditorTextSpan(133, 149, (byte)CssDecorationKind.PropertyName),
            new TextEditorTextSpan(171, 182, (byte)CssDecorationKind.PropertyName),
            new TextEditorTextSpan(205, 214, (byte)CssDecorationKind.PropertyName),
            new TextEditorTextSpan(276, 285, (byte)CssDecorationKind.PropertyName),
            new TextEditorTextSpan(295, 306, (byte)CssDecorationKind.PropertyName),
            new TextEditorTextSpan(318, 328, (byte)CssDecorationKind.PropertyName),
            new TextEditorTextSpan(421, 426, (byte)CssDecorationKind.PropertyName),
            new TextEditorTextSpan(509, 514, (byte)CssDecorationKind.PropertyName),
        };
        
        var cssLexer = new TextEditorCssLexer();

        var textEditorTextSpans = await cssLexer.Lex(
            text,
            RenderStateKey.NewRenderStateKey());
        
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
            new TextEditorTextSpan(125, 129, (byte)CssDecorationKind.PropertyValue),
            new TextEditorTextSpan(151, 167, (byte)CssDecorationKind.PropertyValue),
            new TextEditorTextSpan(184, 201, (byte)CssDecorationKind.PropertyValue),
            new TextEditorTextSpan(216, 220, (byte)CssDecorationKind.PropertyValue),
            new TextEditorTextSpan(287, 291, (byte)CssDecorationKind.PropertyValue),
            new TextEditorTextSpan(308, 314, (byte)CssDecorationKind.PropertyValue),
            new TextEditorTextSpan(330, 333, (byte)CssDecorationKind.PropertyValue),
            new TextEditorTextSpan(428, 431, (byte)CssDecorationKind.PropertyValue),
            new TextEditorTextSpan(516, 521, (byte)CssDecorationKind.PropertyValue),
        };
        
        var cssLexer = new TextEditorCssLexer();

        var textEditorTextSpans = await cssLexer.Lex(
            text,
            RenderStateKey.NewRenderStateKey());
        
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
            new TextEditorTextSpan( 108, 112, (byte)CssDecorationKind.Identifier),
            new TextEditorTextSpan( 269, 271, (byte)CssDecorationKind.Identifier),
            new TextEditorTextSpan(409, 418, (byte)CssDecorationKind.Identifier),
            new TextEditorTextSpan(500, 506, (byte)CssDecorationKind.Identifier),
        };
        
        var cssLexer = new TextEditorCssLexer();

        var textEditorTextSpans = await cssLexer.Lex(
            text,
            RenderStateKey.NewRenderStateKey());
        
        textEditorTextSpans = textEditorTextSpans
            .Where(x => x.DecorationByte == (byte)CssDecorationKind.Identifier)
            .OrderBy(x => x.StartingIndexInclusive)
            .ToImmutableArray();

        Assert.Equal(expectedTextEditorTextSpans, textEditorTextSpans);
    }
}