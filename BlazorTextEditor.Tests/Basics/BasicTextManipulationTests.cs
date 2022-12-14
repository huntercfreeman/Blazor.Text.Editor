using BlazorTextEditor.RazorLib.Cursor;
using BlazorTextEditor.RazorLib.Store.TextEditorCase.Actions;
using BlazorTextEditor.Tests.TestDataFolder;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorTextEditor.Tests.Basics;

public class BasicTextManipulationTests : BlazorTextEditorTestingBase
{
    [Fact]
    public void InsertByKeyboardEvent()
    {
        var cursor = new TextEditorCursor((0, 0), true);

        var keyboardEventArgs = new KeyboardEventArgs
        {
            Key = "A"
        };
        
        TextEditorService.HandleKeyboardEvent(
            new KeyboardEventTextEditorBaseAction(
                TextEditorKey,
                TextEditorCursorSnapshot.TakeSnapshots(cursor),
                keyboardEventArgs,
                CancellationToken.None));
        
        Assert.Equal(
            keyboardEventArgs.Key, 
            TextEditor.GetAllText());
    }
    
    [Fact]
    public void InsertManyCharacterString()
    {
        var cursor = new TextEditorCursor((0, 0), true);

        var content = TestData.CSharp.EXAMPLE_TEXT_173_LINES;
        
        TextEditorService.InsertText(
            new InsertTextTextEditorBaseAction(
                TextEditorKey,
                TextEditorCursorSnapshot.TakeSnapshots(cursor),
                content,
                CancellationToken.None));
        
        Assert.Equal(
            content, 
            TextEditor.GetAllText());
    }
    
    /// <summary>
    /// TODO: Insert '\r' then '\n' this would erroneously result in two spaces if typed manually as the two would not combine to "\r\n"
    /// </summary>
    [Fact]
    public void InsertCarriageReturnThenNewLine()
    {
        throw new NotImplementedException();
    }
    
    [Fact]
    public void DeleteSingleCharacter()
    {
        var cursor = new TextEditorCursor((0, 0), true);

        var content = "A";
        
        TextEditorService.InsertText(
            new InsertTextTextEditorBaseAction(
                TextEditorKey,
                TextEditorCursorSnapshot.TakeSnapshots(cursor),
                content,
                CancellationToken.None));
        
        Assert.Equal(
            content,
            TextEditor.GetAllText());
        
        TextEditorService.DeleteTextByRange(
            new DeleteTextByRangeTextEditorBaseAction(
                TextEditorKey,
                TextEditorCursorSnapshot.TakeSnapshots(cursor),
                1,
                CancellationToken.None));
        
        Assert.Equal(
            string.Empty,
            TextEditor.GetAllText());
    }
    
    [Fact]
    public void DeleteManyCharacters()
    {
        var startingPositionIndex = 2;
        var count = 3;
        
        var cursor = new TextEditorCursor(
            (0, 0),
            true);

        var content = "Abcdefg";
        
        TextEditorService.InsertText(
            new InsertTextTextEditorBaseAction(
                TextEditorKey,
                TextEditorCursorSnapshot.TakeSnapshots(cursor),
                content,
                CancellationToken.None));
        
        Assert.Equal(
            content,
            TextEditor.GetAllText());
        
        cursor.IndexCoordinates = (0, startingPositionIndex);

        TextEditorService.DeleteTextByRange(
            new DeleteTextByRangeTextEditorBaseAction(
                TextEditorKey,
                TextEditorCursorSnapshot.TakeSnapshots(cursor),
                count,
                CancellationToken.None));
        
        Assert.Equal(
            content.Remove(startingPositionIndex, count),
            TextEditor.GetAllText());
    }
}