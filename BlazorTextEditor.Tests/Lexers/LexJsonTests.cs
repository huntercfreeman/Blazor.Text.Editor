using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Analysis.Json;
using BlazorTextEditor.RazorLib.Analysis.Json.Decoration;
using BlazorTextEditor.RazorLib.Analysis.Json.SyntaxActors;
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
            new TextEditorTextSpan(5, 16, 1),
            new TextEditorTextSpan(30, 51, 1),
            new TextEditorTextSpan(70, 93, 1),
            new TextEditorTextSpan(111, 121, 1),
            new TextEditorTextSpan(139, 153, 1),
            new TextEditorTextSpan(195, 202, 1),
            new TextEditorTextSpan(233, 241, 1),
            new TextEditorTextSpan(255, 287, 1),
            new TextEditorTextSpan(305, 316, 1),
            new TextEditorTextSpan(343, 360, 1),
            new TextEditorTextSpan(382, 395, 1),
            new TextEditorTextSpan(417, 431, 1),
            new TextEditorTextSpan(495, 515, 1),
            new TextEditorTextSpan(537, 559, 1),
            new TextEditorTextSpan(610, 621, 1),
            new TextEditorTextSpan(639, 650, 1),
            new TextEditorTextSpan(680, 693, 1),
            new TextEditorTextSpan(715, 735, 1),
            new TextEditorTextSpan(757, 779, 1),
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
    public async Task LexValueString()
    {
        var text = TestData.Json.EXAMPLE_TEXT_LAUNCH_SETTINGS
            .ReplaceLineEndings("\n");

        var expectedTextEditorTextSpans = new[]
        {
            new TextEditorTextSpan(157, 179, 2),
            new TextEditorTextSpan(320, 327, 2),
            new TextEditorTextSpan(435, 479, 2),
            new TextEditorTextSpan(563, 574, 2),
            new TextEditorTextSpan(654, 664, 2),
            new TextEditorTextSpan(783, 794, 2),
        };
        
        var jsonLexer = new TextEditorJsonLexer();

        var textEditorTextSpans = 
            await jsonLexer.Lex(text);
        
        textEditorTextSpans = textEditorTextSpans
            .Where(x => x.DecorationByte == (byte)JsonDecorationKind.String)
            .OrderBy(x => x.StartingIndexInclusive)
            .ToImmutableArray();

        Assert.Equal(expectedTextEditorTextSpans, textEditorTextSpans);
    }
    
    [Fact]
    public async Task LexValueKeyword()
    {
        var text = TestData.Json.EXAMPLE_TEXT_LAUNCH_SETTINGS
            .ReplaceLineEndings("\n");

        var expectedTextEditorTextSpans = new[]
        {
            new TextEditorTextSpan(54, 59, 3),
            new TextEditorTextSpan(96, 100, 3),
            new TextEditorTextSpan(363, 367, 3),
            new TextEditorTextSpan(398, 402, 3),
            new TextEditorTextSpan(696, 700, 3),
        };
        
        var jsonLexer = new TextEditorJsonLexer();

        var textEditorTextSpans = 
            await jsonLexer.Lex(text);
        
        textEditorTextSpans = textEditorTextSpans
            .Where(x => x.DecorationByte == (byte)JsonDecorationKind.Keyword)
            .OrderBy(x => x.StartingIndexInclusive)
            .ToImmutableArray();

        Assert.Equal(expectedTextEditorTextSpans, textEditorTextSpans);
    }
    
    [Fact]
    public async Task LexValueNumber()
    {
        var text = TestData.Json.EXAMPLE_TEXT_LAUNCH_SETTINGS
            .ReplaceLineEndings("\n");

        var expectedTextEditorTextSpans = new[]
        {
            new TextEditorTextSpan(54, 59, 3),
            new TextEditorTextSpan(96, 100, 3),
            new TextEditorTextSpan(363, 367, 3),
            new TextEditorTextSpan(398, 402, 3),
            new TextEditorTextSpan(696, 700, 3),
        };
        
        var jsonLexer = new TextEditorJsonLexer();

        var textEditorTextSpans = 
            await jsonLexer.Lex(text);
        
        textEditorTextSpans = textEditorTextSpans
            .Where(x => x.DecorationByte == (byte)JsonDecorationKind.None)
            .OrderBy(x => x.StartingIndexInclusive)
            .ToImmutableArray();

        Assert.Equal(expectedTextEditorTextSpans, textEditorTextSpans);
    }
    
    [Fact]
    public async Task LexValueInteger()
    {
        var text = TestData.Json.EXAMPLE_TEXT_LAUNCH_SETTINGS
            .ReplaceLineEndings("\n");

        var expectedTextEditorTextSpans = new[]
        {
            new TextEditorTextSpan(54, 59, 3),
            new TextEditorTextSpan(96, 100, 3),
            new TextEditorTextSpan(363, 367, 3),
            new TextEditorTextSpan(398, 402, 3),
            new TextEditorTextSpan(696, 700, 3),
        };
        
        var jsonLexer = new TextEditorJsonLexer();

        var textEditorTextSpans = 
            await jsonLexer.Lex(text);
        
        textEditorTextSpans = textEditorTextSpans
            .Where(x => x.DecorationByte == (byte)JsonDecorationKind.Keyword)
            .OrderBy(x => x.StartingIndexInclusive)
            .ToImmutableArray();

        Assert.Equal(expectedTextEditorTextSpans, textEditorTextSpans);
    }
    
    [Fact]
    public async Task LexCommentLine()
    {
        var text = TestData.Json.EXAMPLE_TEXT_LAUNCH_SETTINGS
            .ReplaceLineEndings("\n");

        var expectedTextEditorTextSpans = new[]
        {
            new TextEditorTextSpan(54, 59, 3),
            new TextEditorTextSpan(96, 100, 3),
            new TextEditorTextSpan(363, 367, 3),
            new TextEditorTextSpan(398, 402, 3),
            new TextEditorTextSpan(696, 700, 3),
        };
        
        var jsonLexer = new TextEditorJsonLexer();

        var textEditorTextSpans = 
            await jsonLexer.Lex(text);
        
        textEditorTextSpans = textEditorTextSpans
            .Where(x => x.DecorationByte == (byte)JsonDecorationKind.Keyword)
            .OrderBy(x => x.StartingIndexInclusive)
            .ToImmutableArray();

        Assert.Equal(expectedTextEditorTextSpans, textEditorTextSpans);
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
    
    [Fact]
    public async Task AdhocTest()
    {
        var text = TestData.Json.EXAMPLE_ADHOC
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
}