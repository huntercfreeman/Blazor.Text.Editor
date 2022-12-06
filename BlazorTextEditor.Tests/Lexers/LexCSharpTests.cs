using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Analysis.CSharp.Decoration;
using BlazorTextEditor.RazorLib.Analysis.CSharp.SyntaxActors;
using BlazorTextEditor.RazorLib.Lexing;
using BlazorTextEditor.Tests.TestDataFolder;

namespace BlazorTextEditor.Tests.Lexers;

public class LexCSharpTests
{
    [Fact]
    public async Task LexKeywords()
    {
        var text = TestData.CSharp.EXAMPLE_TEXT_8_LINES
            .ReplaceLineEndings("\n");

        var expectedKeywordTextEditorTextSpans = new[]
        {
            new TextEditorTextSpan(0, 9, (byte)CSharpDecorationKind.Keyword), 
            new TextEditorTextSpan(35, 41, (byte)CSharpDecorationKind.Keyword), 
            new TextEditorTextSpan(42, 47, (byte)CSharpDecorationKind.Keyword), 
            new TextEditorTextSpan(62, 68, (byte)CSharpDecorationKind.Keyword), 
            new TextEditorTextSpan(69, 73, (byte)CSharpDecorationKind.Keyword), 
            new TextEditorTextSpan(99, 105, (byte)CSharpDecorationKind.Keyword),
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