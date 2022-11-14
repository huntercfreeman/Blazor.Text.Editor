namespace BlazorTextEditor.Tests;

/// <summary>
/// When one sees a method of the pattern
/// <see cref="SelectTextThenArrowLeft"/>.
/// <br/><br/>
/// This pattern is to mean the user selects text,
/// perhaps while holding the shift key,
/// BUT they then let go of the shift key and
/// hit ArrowLeft in this example.
/// </summary>
public class SelectionTests
{
    [Fact]
    public void SelectText()
    {
    }
    
    [Fact]
    public void SelectTextThenArrowLeft()
    {
    }
    
    [Fact]
    public void SelectTextThenArrowDown()
    {
    }
    
    [Fact]
    public void SelectTextThenArrowUp()
    {
    }
    
    [Fact]
    public void SelectTextThenArrowRight()
    {
    }
    
    [Fact]
    public void SelectTextThenHome()
    {
    }
    
    [Fact]
    public void SelectTextThenEnd()
    {
    }
    
    [Fact]
    public void SelectAll()
    {
    }
    
    [Fact]
    public void SelectAllThenArrowLeft()
    {
    }
    
    /// <summary>
    /// BUG: from [3.2.0] where this
    /// would position the user's cursor
    /// out of bounds. The next movement they made
    /// would then cause the app to crash.
    /// </summary>
    [Fact]
    public void SelectAllThenArrowRight()
    {
    }
    
    [Fact]
    public void ExpandSelection()
    {
    }
    
    [Fact]
    public void CopySelection()
    {
    }
    
    [Fact]
    public void CutSelection()
    {
    }
}