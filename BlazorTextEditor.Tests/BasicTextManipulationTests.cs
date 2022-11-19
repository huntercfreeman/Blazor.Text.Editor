using BlazorTextEditor.RazorLib.Cursor;
using BlazorTextEditor.RazorLib.Store.TextEditorCase.Actions;
using BlazorTextEditor.Tests.TestDataFolder;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorTextEditor.Tests;

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
        
        _textEditorService.HandleKeyboardEvent(
            new KeyboardEventTextEditorBaseAction(
                _textEditorKey,
                TextEditorCursorSnapshot.TakeSnapshots(cursor),
                keyboardEventArgs,
                CancellationToken.None));
        
        Assert.Equal(
            keyboardEventArgs.Key, 
            _textEditor.GetAllText());
    }
    
    [Fact]
    public void InsertManyCharacterString()
    {
        var cursor = new TextEditorCursor((0, 0), true);

        var content = TestData.CSharp.EXAMPLE_TEXT_173_LINES;
        
        _textEditorService.InsertText(
            new InsertTextTextEditorBaseAction(
                _textEditorKey,
                TextEditorCursorSnapshot.TakeSnapshots(cursor),
                content,
                CancellationToken.None));
        
        Assert.Equal(
            content, 
            _textEditor.GetAllText());
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
        
        _textEditorService.InsertText(
            new InsertTextTextEditorBaseAction(
                _textEditorKey,
                TextEditorCursorSnapshot.TakeSnapshots(cursor),
                content,
                CancellationToken.None));
        
        Assert.Equal(
            content,
            _textEditor.GetAllText());
        
        _textEditorService.DeleteTextByRange(
            new DeleteTextByRangeTextEditorBaseAction(
                _textEditorKey,
                TextEditorCursorSnapshot.TakeSnapshots(cursor),
                1,
                CancellationToken.None));
        
        Assert.Equal(
            string.Empty,
            _textEditor.GetAllText());
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
        
        _textEditorService.InsertText(
            new InsertTextTextEditorBaseAction(
                _textEditorKey,
                TextEditorCursorSnapshot.TakeSnapshots(cursor),
                content,
                CancellationToken.None));
        
        Assert.Equal(
            content,
            _textEditor.GetAllText());
        
        cursor.IndexCoordinates = (0, startingPositionIndex);

        _textEditorService.DeleteTextByRange(
            new DeleteTextByRangeTextEditorBaseAction(
                _textEditorKey,
                TextEditorCursorSnapshot.TakeSnapshots(cursor),
                count,
                CancellationToken.None));
        
        Assert.Equal(
            content.Remove(startingPositionIndex, count),
            _textEditor.GetAllText());
    }
}