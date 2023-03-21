using BlazorTextEditor.RazorLib.Diff;
using BlazorTextEditor.Tests.TestDataFolder;

namespace BlazorTextEditor.Tests.Basics.Diff;

public class DiffTests
{
    [Theory]
    [InlineData(
        TestData.Diff.SingleLineBaseCases.NoLineEndings.SAMPLE_000,
        TestData.Diff.SingleLineBaseCases.NoLineEndings.SAMPLE_010,
        5)]
    [InlineData(
        TestData.Diff.SingleLineBaseCases.WithLinefeedEnding.SAMPLE_000,
        TestData.Diff.SingleLineBaseCases.WithLinefeedEnding.SAMPLE_010,
        6)]
    [InlineData(
        TestData.Diff.SingleLineBaseCases.WithCarriageReturnEnding.SAMPLE_000,
        TestData.Diff.SingleLineBaseCases.WithCarriageReturnEnding.SAMPLE_010,
        6)]
    [InlineData(
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
        TestData.Diff.MultiLineBaseCases.WithLinefeedEnding.SAMPLE_000,
        TestData.Diff.MultiLineBaseCases.WithLinefeedEnding.SAMPLE_010,
        10)]
    [InlineData(
        TestData.Diff.MultiLineBaseCases.WithCarriageReturnEnding.SAMPLE_000,
        TestData.Diff.MultiLineBaseCases.WithCarriageReturnEnding.SAMPLE_010,
        10)]
    [InlineData(
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
        TestData.Diff.JustifiedCases.Bug_000.SAMPLE_000,
        TestData.Diff.JustifiedCases.Bug_000.SAMPLE_010,
        -1)]
    [InlineData(
        TestData.Diff.JustifiedCases.Bug_010.SAMPLE_000,
        TestData.Diff.JustifiedCases.Bug_010.SAMPLE_010,
        -1)]
    [InlineData(
        TestData.Diff.JustifiedCases.Bug_020.SAMPLE_000,
        TestData.Diff.JustifiedCases.Bug_020.SAMPLE_010,
        -1)]
    [InlineData(
        TestData.Diff.JustifiedCases.Bug_030.SAMPLE_000,
        TestData.Diff.JustifiedCases.Bug_030.SAMPLE_010,
        -1)]
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