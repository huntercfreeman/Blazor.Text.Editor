using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Analysis.Json;
using BlazorTextEditor.RazorLib.Lexing;
using BlazorTextEditor.Tests.TestDataFolder;

namespace BlazorTextEditor.Tests.Lexers;

public class LexJsonTests
{
    [Fact]
    public async Task LexPropertyKey()
    {
        var text = TestData.Json.EXAMPLE_TEXT_LAUNCH_SETTINGS
            .ReplaceLineEndings("\n");

        var expectedTextEditorTextSpans = new[]
        {
            // TODO: Replace this placeholder data with the real TextEditorTextSpan(s)
            new TextEditorTextSpan( 108, 112, 6),
        };
        
        var jsonLexer = new TextEditorJsonLexer();

        var textEditorTextSpans = 
            await jsonLexer.Lex(text);
        
        textEditorTextSpans = textEditorTextSpans
            .Where(x => x.DecorationByte == (byte)JsonDecorationKind.PropertyKey)
            .OrderBy(x => x.StartingIndexInclusive)
            .ToImmutableArray();

        Assert.Equal(expectedTextEditorTextSpans, textEditorTextSpans);
    }
    
    [Fact]
    public async Task LexString()
    {
        throw new NotImplementedException();
    }
    
    [Fact]
    public async Task LexKeyword()
    {
        throw new NotImplementedException();
    }
    
    [Fact]
    public async Task LexLineComment()
    {
        throw new NotImplementedException();
    }
    
    [Fact]
    public async Task LexBlockComment()
    {
        throw new NotImplementedException();
    }
}