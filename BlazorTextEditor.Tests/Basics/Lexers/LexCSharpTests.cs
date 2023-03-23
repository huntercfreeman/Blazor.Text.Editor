namespace BlazorTextEditor.Tests.Basics.Lexers;

public class LexCSharpTests
{
    // 2023-02-23: Tests need to be rewritten with the new GenericSyntaxTree
    //
    // [Fact]
    // public async Task LexKeywords()
    // {
    //     var text = TestData.CSharp.EXAMPLE_TEXT_8_LINES
    //         .ReplaceLineEndings("\n");
    //
    //     var expectedKeywordTextEditorTextSpans = new[]
    //     {
    //         new TextEditorTextSpan(0, 9, (byte)CSharpDecorationKind.Keyword), 
    //         new TextEditorTextSpan(35, 41, (byte)CSharpDecorationKind.Keyword), 
    //         new TextEditorTextSpan(42, 47, (byte)CSharpDecorationKind.Keyword), 
    //         new TextEditorTextSpan(62, 68, (byte)CSharpDecorationKind.Keyword), 
    //         new TextEditorTextSpan(69, 73, (byte)CSharpDecorationKind.Keyword), 
    //         new TextEditorTextSpan(99, 105, (byte)CSharpDecorationKind.Keyword),
    //     };
    //     
    //     var cSharpLexer = new TextEditorCSharpLexer();
    //
    //     var textEditorTextSpans = 
    //         await cSharpLexer.Lex(text);
    //
    //     textEditorTextSpans = textEditorTextSpans
    //         .Where(x => x.DecorationByte == (byte)CSharpDecorationKind.Keyword)
    //         .OrderBy(x => x.StartingIndexInclusive)
    //         .ToImmutableArray();
    //
    //     Assert.Equal(expectedKeywordTextEditorTextSpans, textEditorTextSpans);
    // }
}