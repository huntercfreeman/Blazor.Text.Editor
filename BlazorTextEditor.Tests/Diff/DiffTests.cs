using BlazorTextEditor.RazorLib.Diff;
using BlazorTextEditor.Tests.TestDataFolder;

namespace BlazorTextEditor.Tests.Diff;

public class DiffTests
{
    [Theory]
    [InlineData(
        // Test simple input NO line endings
        TestData.Diff.SingleLineBaseCases.NoLineEndings.SAMPLE_000,
        TestData.Diff.SingleLineBaseCases.NoLineEndings.SAMPLE_010,
        5)]
    [InlineData(
        // Test simple input with linefeed ending
        TestData.Diff.SingleLineBaseCases.WithLinefeedEnding.SAMPLE_000,
        TestData.Diff.SingleLineBaseCases.WithLinefeedEnding.SAMPLE_010,
        6)]
    [InlineData(
        // Test simple input with carriage return ending
        TestData.Diff.SingleLineBaseCases.WithCarriageReturnEnding.SAMPLE_000,
        TestData.Diff.SingleLineBaseCases.WithCarriageReturnEnding.SAMPLE_010,
        6)]
    [InlineData(
        // Test simple input with carriage return and linefeed ending
        TestData.Diff.SingleLineBaseCases.WithCarriageReturnLinefeedEnding.SAMPLE_000,
        TestData.Diff.SingleLineBaseCases.WithCarriageReturnLinefeedEnding.SAMPLE_010,
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
        TestData.Diff.MultiLineBaseCases.WithLinefeedEnding.SAMPLE_000,
        TestData.Diff.MultiLineBaseCases.WithLinefeedEnding.SAMPLE_010,
        10)]
    [InlineData(
        // Test multi-line input with carriage return endings
        TestData.Diff.MultiLineBaseCases.WithCarriageReturnEnding.SAMPLE_000,
        TestData.Diff.MultiLineBaseCases.WithCarriageReturnEnding.SAMPLE_010,
        10)]
    [InlineData(
        // Test multi-line input with carriage return and linefeed endings
        TestData.Diff.MultiLineBaseCases.WithCarriageReturnLinefeedEnding.SAMPLE_000,
        TestData.Diff.MultiLineBaseCases.WithCarriageReturnLinefeedEnding.SAMPLE_010,
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

    [Theory]
    [InlineData(
        // Test multi-line input with linefeed endings
        TestData.Diff.JustifiedCases.Bug_000.SAMPLE_000,
        TestData.Diff.JustifiedCases.Bug_000.SAMPLE_010,
        10)]
    public void JustifiedTests(
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