using BlazorTextEditor.RazorLib.Diff;

namespace BlazorTextEditor.Tests.Diff._2023_03_15;

public class DiffEmptyTests
{
    [Fact]
    public void BeforeIsEmptyAfterIsEmpty()
    {
        // Input
        var beforeText = string.Empty;
        var afterText = string.Empty;
        
        // Expected
        var expectedLongestCommonSubsequence = string.Empty;

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
    public void BeforeIsEmptyAfterIsNotEmpty()
    {
        // Input
        var beforeText = string.Empty;
        var afterText = "lorem ipsum";
        
        // Expected
        var expectedLongestCommonSubsequence = string.Empty;

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
    public void BeforeIsNotEmptyAfterIsEmpty()
    {
        // Input
        var beforeText = "lorem ipsum";
        var afterText = string.Empty;
        
        // Expected
        var expectedLongestCommonSubsequence = string.Empty;

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