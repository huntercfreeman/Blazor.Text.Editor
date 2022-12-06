using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Analysis.FSharp;
using BlazorTextEditor.RazorLib.Analysis.FSharp.Decoration;
using BlazorTextEditor.RazorLib.Analysis.FSharp.SyntaxActors;
using BlazorTextEditor.RazorLib.Lexing;
using BlazorTextEditor.Tests.TestDataFolder;

namespace BlazorTextEditor.Tests.Lexers;

public class LexFSharpTests
{
    [Fact]
    public async Task LexKeywords()
    {
        var text = TestData.FSharp.EXAMPLE_TEXT_21_LINES
            .ReplaceLineEndings("\n");

        var expectedKeywordTextEditorTextSpans = new[]
        {
            new TextEditorTextSpan(0, 3, 1),
            new TextEditorTextSpan(26, 30, 1),
            new TextEditorTextSpan(65, 68, 1),
            new TextEditorTextSpan(94, 97, 1),
            new TextEditorTextSpan(123, 126, 1),
            new TextEditorTextSpan(129, 131, 1),
            new TextEditorTextSpan(145, 147, 1),
            new TextEditorTextSpan(160, 163, 1),
            new TextEditorTextSpan(247, 250, 1),
        };
        
        var fSharpLexer = new TextEditorFSharpLexer();

        var textEditorTextSpans = 
            await fSharpLexer.Lex(text);

        textEditorTextSpans = textEditorTextSpans
            .Where(x => x.DecorationByte == (byte)FSharpDecorationKind.Keyword)
            .OrderBy(x => x.StartingIndexInclusive)
            .ToImmutableArray();

        Assert.Equal(expectedKeywordTextEditorTextSpans, textEditorTextSpans);
    }
}