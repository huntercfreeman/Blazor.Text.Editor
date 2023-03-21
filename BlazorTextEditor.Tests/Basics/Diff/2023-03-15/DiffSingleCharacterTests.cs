using BlazorTextEditor.RazorLib.Diff;

namespace BlazorTextEditor.Tests.Basics.Diff._2023_03_15;

public class DiffSingleCharacterTests
{
    [Fact]
    public void InputsAreNotEqual()
    {
        // Input
        var beforeText = "a";
        var afterText = "b";
        
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
    public void InputsAreEqual()
    {
        // Input
        var beforeText = "a";
        var afterText = "a";
        
        // Expected
        var expectedLongestCommonSubsequence = "a";

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