using BlazorTextEditor.RazorLib.Diff;

namespace BlazorTextEditor.Tests.Diff._2023_03_15;

public class DiffInsertionTests
{
    [Fact]
    public void InsertionOfSingleCharacterAtStartOfBeforeText()
    {
        // Input
        var beforeText = "foo";
        var afterText = "afoo";
        
        // Expected
        var expectedLongestCommonSubsequence = "foo";

        // Calculate
        var diffResult = TextEditorDiffResult.Calculate(
            beforeText,
            afterText);

        // Assert
        Assert.Equal(
            expectedLongestCommonSubsequence,
            diffResult.LongestCommonSubsequence);
    }
    
    [Fact]
    public void InsertionOfSingleCharacterAtEndOfBeforeText()
    {
        // Input
        var beforeText = "foo";
        var afterText = "fooa";
        
        // Expected
        var expectedLongestCommonSubsequence = "foo";

        // Calculate
        var diffResult = TextEditorDiffResult.Calculate(
            beforeText,
            afterText);

        // Assert
        Assert.Equal(
            expectedLongestCommonSubsequence,
            diffResult.LongestCommonSubsequence);
    }
    
    [Fact]
    public void InsertionOfSingleCharacterBetweenTwoExistingCharacters()
    {
        // Input
        var beforeText = "foo";
        var afterText = "foao";
        
        // Expected
        var expectedLongestCommonSubsequence = "foo";

        // Calculate
        var diffResult = TextEditorDiffResult.Calculate(
            beforeText,
            afterText);

        // Assert
        Assert.Equal(
            expectedLongestCommonSubsequence,
            diffResult.LongestCommonSubsequence);
    }
}