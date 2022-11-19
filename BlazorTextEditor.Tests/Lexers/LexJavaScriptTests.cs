using BlazorTextEditor.RazorLib.Analysis.JavaScript;
using BlazorTextEditor.Tests.TestDataFolder;

namespace BlazorTextEditor.Tests.Lexers;

public class LexJavaScriptTests
{
    [Fact]
    public void TestSingularKeywordRecognition()
    {
        var content = TestData.JavaScript.EXAMPLE_TEXT_28_LINES;
        
        var foundKeywords = JavaScriptSyntaxTree
            .ParseText(content);

        var z = 2;

        // Assert.Contains(
        //     foundKeywords, 
        //     x => x == keyword);
    }
    
    // [Fact]
    // public void TestSingularKeywordRecognition()
    // {
    //     var keyword = JavaScriptKeywords.FalseKeyword;
    //     
    //     var foundKeywords = JavaScriptSyntaxTree
    //         .ParseText(keyword);
    //
    //     Assert.Contains(
    //         foundKeywords, 
    //         x => x == keyword);
    // }
    //
    // [Fact]
    // public void TestAllKeywordsRecognition()
    // {
    //     var content = string.Join(' ', JavaScriptKeywords.All);
    //     var expectedCountOfKeywords = JavaScriptKeywords.All.Length;
    //     
    //     var foundKeywords = JavaScriptSyntaxTree
    //         .ParseText(content);
    //
    //     Assert.Equal(expectedCountOfKeywords, foundKeywords.Count);
    // }
}