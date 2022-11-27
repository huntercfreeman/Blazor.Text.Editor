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
            new TextEditorTextSpan(0, 107, 2),
            new TextEditorTextSpan(225, 268, 2),
            new TextEditorTextSpan(338, 407, 2),
            new TextEditorTextSpan(436, 498, 2),
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
    public async Task LexCommentSingleInvalid()
    {
        throw new NotImplementedException();
    }
}