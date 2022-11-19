using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Analysis.CSharp;
using BlazorTextEditor.RazorLib.Analysis.JavaScript;
using BlazorTextEditor.RazorLib.Lexing;
using BlazorTextEditor.Tests.TestDataFolder;

namespace BlazorTextEditor.Tests.Lexers;

public class LexCSharpTests
{
    [Fact]
    public async Task LexKeywords()
    {
        var text = TestData.CSharp.EXAMPLE_TEXT_8_LINES;

        var expectedKeywordTextEditorTextSpans = new[]
        {
            new TextEditorTextSpan(0, 9, 5), 
            new TextEditorTextSpan(35, 41, 5), 
            new TextEditorTextSpan(42, 47, 5), 
            new TextEditorTextSpan(62, 68, 5), 
            new TextEditorTextSpan(69, 73, 5), 
            new TextEditorTextSpan(99, 105, 5),
        };
        
        var cSharpLexer = new TextEditorCSharpLexer();

        var textEditorTextSpans = 
            await cSharpLexer.Lex(text);

        textEditorTextSpans = textEditorTextSpans
            .Where(x => x.DecorationByte == (byte)CSharpDecorationKind.Keyword)
            .OrderBy(x => x.StartingIndexInclusive)
            .ToImmutableArray();

        Assert.Equal(expectedKeywordTextEditorTextSpans, textEditorTextSpans);
    }
}