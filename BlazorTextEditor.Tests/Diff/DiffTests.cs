using BlazorTextEditor.RazorLib.Diff;
using BlazorTextEditor.Tests.TestDataFolder;

namespace BlazorTextEditor.Tests.Diff;

public class DiffTests
{
    [Theory]
    [InlineData(
        // Test simple input NO line endings
        TestData.Diff.Simple.NoLineEndings.SAMPLE_000,
        TestData.Diff.Simple.NoLineEndings.SAMPLE_010,
        5)]
    [InlineData(
        // Test simple input with linefeed ending
        TestData.Diff.Simple.WithLinefeedEnding.SAMPLE_000,
        TestData.Diff.Simple.WithLinefeedEnding.SAMPLE_010,
        6)]
    [InlineData(
        // Test simple input with carriage return ending
        TestData.Diff.Simple.WithCarriageReturnEnding.SAMPLE_000,
        TestData.Diff.Simple.WithCarriageReturnEnding.SAMPLE_010,
        6)]
    [InlineData(
        // Test simple input with carriage return and linefeed ending
        TestData.Diff.Simple.WithCarriageReturnLinefeedEnding.SAMPLE_000,
        TestData.Diff.Simple.WithCarriageReturnLinefeedEnding.SAMPLE_010,
        7)]
    public void SimpleTests(
        string beforeText,
        string afterText,
        int lengthOfLongestCommonSubsequence)
    {
        var diffResult = TextEditorDiffResult.Calculate(
            beforeText,
            afterText);

        Assert.Equal(
            lengthOfLongestCommonSubsequence,
            diffResult.LongestCommonSubsequence.Length);
        
        AssertSumOfAllTextSpanLengthsIsEqualToLengthOfLongestCommonSubsequence(
            diffResult);
    }
    
    [Theory]
    [InlineData(
        // Test multi-line input with linefeed endings
        TestData.Diff.MultiLine.WithLinefeedEnding.SAMPLE_000,
        TestData.Diff.MultiLine.WithLinefeedEnding.SAMPLE_010,
        10)]
    [InlineData(
        // Test multi-line input with carriage return endings
        TestData.Diff.MultiLine.WithCarriageReturnEnding.SAMPLE_000,
        TestData.Diff.MultiLine.WithCarriageReturnEnding.SAMPLE_010,
        10)]
    [InlineData(
        // Test multi-line input with carriage return and linefeed endings
        TestData.Diff.MultiLine.WithCarriageReturnLinefeedEnding.SAMPLE_000,
        TestData.Diff.MultiLine.WithCarriageReturnLinefeedEnding.SAMPLE_010,
        11)]
    public void MultiLineTests(
        string beforeText,
        string afterText,
        int lengthOfLongestCommonSubsequence)
    {
        var diffResult = TextEditorDiffResult.Calculate(
            beforeText,
            afterText);

        Assert.Equal(
            lengthOfLongestCommonSubsequence,
            diffResult.LongestCommonSubsequence.Length);
        
        AssertSumOfAllTextSpanLengthsIsEqualToLengthOfLongestCommonSubsequence(
            diffResult);
    }

    private void AssertSumOfAllTextSpanLengthsIsEqualToLengthOfLongestCommonSubsequence(
        TextEditorDiffResult diffResult)
    {
        var sumOfAllBeforeTextSpanLengths = diffResult.BeforeLongestCommonSubsequenceTextSpans
            .Sum(x => x.EndingIndexExclusive - x.StartingIndexInclusive);

        Assert.Equal(
            sumOfAllBeforeTextSpanLengths,
            diffResult.LongestCommonSubsequence.Length);
        
        var sumOfAllAfterTextSpanLengths = diffResult.AfterLongestCommonSubsequenceTextSpans
            .Sum(x => x.EndingIndexExclusive - x.StartingIndexInclusive);

        Assert.Equal(
            sumOfAllAfterTextSpanLengths,
            diffResult.LongestCommonSubsequence.Length);
    }
}