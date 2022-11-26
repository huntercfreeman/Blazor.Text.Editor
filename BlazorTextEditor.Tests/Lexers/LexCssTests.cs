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
    public async Task LexCommentSingleValid()
    {
        var text = TestData.Css.EXAMPLE_TEXT_21_LINES
            .ReplaceLineEndings("\n");

        var cssLexer = new TextEditorCssLexer();

        var textEditorTextSpans = 
            await cssLexer.Lex(text);
        
        textEditorTextSpans = textEditorTextSpans
            .Where(x => x.DecorationByte == (byte)CssDecorationKind.Comment)
            .OrderBy(x => x.StartingIndexInclusive)
            .ToImmutableArray();

        Assert.Empty(textEditorTextSpans);
    }
    
    [Fact]
    public async Task LexCommentManyValid()
    {
        throw new NotImplementedException();
    }
    
    [Fact]
    public async Task LexCommentSingleInvalid()
    {
        throw new NotImplementedException();
    }
}