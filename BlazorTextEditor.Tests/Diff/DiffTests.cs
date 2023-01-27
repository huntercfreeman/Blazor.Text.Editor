using BlazorTextEditor.RazorLib.Diff;
using BlazorTextEditor.Tests.TestDataFolder;

namespace BlazorTextEditor.Tests.Diff;

public class DiffTests
{
    [Fact]
    public async Task AdhocTesting()
    {
        var beforeText = TestData.Diff.BEFORE_TEXT
            .ReplaceLineEndings("\n");
        
        var afterText = TestData.Diff.AFTER_TEXT
            .ReplaceLineEndings("\n");
        
        DiffService.CalculateDiff(beforeText, afterText);
    }
}