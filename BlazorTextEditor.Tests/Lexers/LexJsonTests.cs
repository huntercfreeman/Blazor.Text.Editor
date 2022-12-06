using System.Collections.Immutable;
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
            new TextEditorTextSpan(5, 16, (byte)JsonDecorationKind.PropertyKey),
            new TextEditorTextSpan(30, 51, (byte)JsonDecorationKind.PropertyKey),
            new TextEditorTextSpan(70, 93, (byte)JsonDecorationKind.PropertyKey),
            new TextEditorTextSpan(111, 121, (byte)JsonDecorationKind.PropertyKey),
            new TextEditorTextSpan(139, 153, (byte)JsonDecorationKind.PropertyKey),
            new TextEditorTextSpan(195, 202, (byte)JsonDecorationKind.PropertyKey),
            new TextEditorTextSpan(233, 241, (byte)JsonDecorationKind.PropertyKey),
            new TextEditorTextSpan(255, 287, (byte)JsonDecorationKind.PropertyKey),
            new TextEditorTextSpan(305, 316, (byte)JsonDecorationKind.PropertyKey),
            new TextEditorTextSpan(343, 360, (byte)JsonDecorationKind.PropertyKey),
            new TextEditorTextSpan(382, 395, (byte)JsonDecorationKind.PropertyKey),
            new TextEditorTextSpan(417, 431, (byte)JsonDecorationKind.PropertyKey),
            new TextEditorTextSpan(495, 515, (byte)JsonDecorationKind.PropertyKey),
            new TextEditorTextSpan(537, 559, (byte)JsonDecorationKind.PropertyKey),
            new TextEditorTextSpan(610, 621, (byte)JsonDecorationKind.PropertyKey),
            new TextEditorTextSpan(639, 650, (byte)JsonDecorationKind.PropertyKey),
            new TextEditorTextSpan(680, 693, (byte)JsonDecorationKind.PropertyKey),
            new TextEditorTextSpan(715, 735, (byte)JsonDecorationKind.PropertyKey),
            new TextEditorTextSpan(757, 779, (byte)JsonDecorationKind.PropertyKey),
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
            new TextEditorTextSpan(157, 179, (byte)JsonDecorationKind.String),
            new TextEditorTextSpan(320, 327, (byte)JsonDecorationKind.String),
            new TextEditorTextSpan(435, 479, (byte)JsonDecorationKind.String),
            new TextEditorTextSpan(563, 574, (byte)JsonDecorationKind.String),
            new TextEditorTextSpan(654, 664, (byte)JsonDecorationKind.String),
            new TextEditorTextSpan(783, 794, (byte)JsonDecorationKind.String),
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
            new TextEditorTextSpan(54, 59, (byte)JsonDecorationKind.Keyword),
            new TextEditorTextSpan(96, 100, (byte)JsonDecorationKind.Keyword),
            new TextEditorTextSpan(363, 367, (byte)JsonDecorationKind.Keyword),
            new TextEditorTextSpan(398, 402, (byte)JsonDecorationKind.Keyword),
            new TextEditorTextSpan(696, 700, (byte)JsonDecorationKind.Keyword),
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
    public async Task LexValueInteger()
    {
        var text = TestData.Json.EXAMPLE_TEXT_WITH_COMMENTS
            .ReplaceLineEndings("\n");

        var expectedTextEditorTextSpans = new[]
        {
            new TextEditorTextSpan(78, 81, (byte)JsonDecorationKind.Integer),
            new TextEditorTextSpan(390, 395, (byte)JsonDecorationKind.Integer),
        };
        
        var jsonLexer = new TextEditorJsonLexer();

        var textEditorTextSpans = 
            await jsonLexer.Lex(text);
        
        textEditorTextSpans = textEditorTextSpans
            .Where(x => x.DecorationByte == (byte)JsonDecorationKind.Integer)
            .OrderBy(x => x.StartingIndexInclusive)
            .ToImmutableArray();

        Assert.Equal(expectedTextEditorTextSpans, textEditorTextSpans);
    }
    
    [Fact]
    public async Task LexValueNumber()
    {
        var text = TestData.Json.EXAMPLE_TEXT_WITH_COMMENTS
            .ReplaceLineEndings("\n");

        var expectedTextEditorTextSpans = new[]
        {
            new TextEditorTextSpan(36, 41, (byte)JsonDecorationKind.Number),
        };
        
        var jsonLexer = new TextEditorJsonLexer();

        var textEditorTextSpans = 
            await jsonLexer.Lex(text);
        
        textEditorTextSpans = textEditorTextSpans
            .Where(x => x.DecorationByte == (byte)JsonDecorationKind.Number)
            .OrderBy(x => x.StartingIndexInclusive)
            .ToImmutableArray();

        Assert.Equal(expectedTextEditorTextSpans, textEditorTextSpans);
    }
    
    [Fact]
    public async Task LexCommentLine()
    {
        throw new NotImplementedException();
    }
    
    [Fact]
    public async Task LexCommentBlock()
    {
        throw new NotImplementedException();
    }
}