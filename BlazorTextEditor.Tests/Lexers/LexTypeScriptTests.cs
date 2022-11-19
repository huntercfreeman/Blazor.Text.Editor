using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Analysis.TypeScript;
using BlazorTextEditor.RazorLib.Lexing;
using BlazorTextEditor.Tests.TestDataFolder;

namespace BlazorTextEditor.Tests.Lexers;

public class LexTypeScriptTests
{
    [Fact]
    public async Task LexKeywords()
    {
        var text = TestData.TypeScript.EXAMPLE_TEXT_28_LINES;

        var expectedKeywordTextEditorTextSpans = new[]
        {
            new TextEditorTextSpan(0, 6, 1),
            new TextEditorTextSpan(31, 35, 1),
            new TextEditorTextSpan(60, 66, 1),
            new TextEditorTextSpan(82, 86, 1),
            new TextEditorTextSpan(108, 113, 1),
            new TextEditorTextSpan(148, 157, 1),
            new TextEditorTextSpan(201, 210, 1),
            new TextEditorTextSpan(412, 420, 1),
            new TextEditorTextSpan(487, 493, 1),
            new TextEditorTextSpan(670, 675, 1),
            new TextEditorTextSpan(756, 761, 1),
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
}