using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Analysis.TypeScript;
using BlazorTextEditor.RazorLib.Analysis.TypeScript.Decoration;
using BlazorTextEditor.RazorLib.Analysis.TypeScript.SyntaxActors;
using BlazorTextEditor.RazorLib.Lexing;
using BlazorTextEditor.Tests.TestDataFolder;

namespace BlazorTextEditor.Tests.Lexers;

public class LexTypeScriptTests
{
    [Fact]
    public async Task LexKeywords()
    {
        var text = TestData.TypeScript.EXAMPLE_TEXT
            .ReplaceLineEndings("\n");

        var expectedKeywordTextEditorTextSpans = new[]
        {
            new TextEditorTextSpan(0, 6, 1),
            new TextEditorTextSpan(31, 35, 1),
            new TextEditorTextSpan(60, 66, 1),
            new TextEditorTextSpan(82, 86, 1),
            new TextEditorTextSpan(108, 113, 1),
            new TextEditorTextSpan(148, 157, 1),
            new TextEditorTextSpan(481, 489, 1),
            new TextEditorTextSpan(556, 562, 1),
            new TextEditorTextSpan(739, 744, 1),
            new TextEditorTextSpan(825, 830, 1),
        };
        
        var typeScriptLexer = new TextEditorTypeScriptLexer();

        var textEditorTextSpans = 
            await typeScriptLexer.Lex(text);

        textEditorTextSpans = textEditorTextSpans
            .Where(x => x.DecorationByte == (byte)TypeScriptDecorationKind.Keyword)
            .OrderBy(x => x.StartingIndexInclusive)
            .ToImmutableArray();
        
        Assert.Equal(expectedKeywordTextEditorTextSpans, textEditorTextSpans);
    }
    
    [Fact]
    public async Task LexComments()
    {
        var text = TestData.TypeScript.EXAMPLE_TEXT
            .ReplaceLineEndings("\n");

        var expectedKeywordTextEditorTextSpans = new[]
        {
            new TextEditorTextSpan(181, 241, 4),
            new TextEditorTextSpan(264, 479, 4),
        };
        
        var javaScriptLexer = new TextEditorTypeScriptLexer();

        var textEditorTextSpans = 
            await javaScriptLexer.Lex(text);

        textEditorTextSpans = textEditorTextSpans
            .Where(x => x.DecorationByte == (byte)TypeScriptDecorationKind.Comment)
            .OrderBy(x => x.StartingIndexInclusive)
            .ToImmutableArray();
        
        Assert.Equal(expectedKeywordTextEditorTextSpans, textEditorTextSpans);
    }
    
    [Fact]
    public async Task LexStrings()
    {
        var text = TestData.TypeScript.EXAMPLE_TEXT
            .ReplaceLineEndings("\n");

        var expectedKeywordTextEditorTextSpans = new[]
        {
            new TextEditorTextSpan(36, 57, 3),
            new TextEditorTextSpan(87, 105, 3),
            new TextEditorTextSpan(808, 822, 3),
            new TextEditorTextSpan(895, 911, 3),
        };
        
        var javaScriptLexer = new TextEditorTypeScriptLexer();

        var textEditorTextSpans = 
            await javaScriptLexer.Lex(text);

        textEditorTextSpans = textEditorTextSpans
            .Where(x => x.DecorationByte == (byte)TypeScriptDecorationKind.String)
            .OrderBy(x => x.StartingIndexInclusive)
            .ToImmutableArray();
        
        Assert.Equal(expectedKeywordTextEditorTextSpans, textEditorTextSpans);
    }
}