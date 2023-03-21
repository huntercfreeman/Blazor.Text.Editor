using BlazorTextEditor.RazorLib.Cursor;
using BlazorTextEditor.RazorLib.Store.Model;
using BlazorTextEditor.Tests.TestDataFolder;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorTextEditor.Tests.Basics.TextEditor;

public class TextManipulationTests : BlazorTextEditorTestingBase
{
    [Fact]
    public void INSERT_BY_KEYBOARD_EVENT()
    {
        var cursor = new TextEditorCursor((0, 0), true);

        var keyboardEventArgs = new KeyboardEventArgs
        {
            Key = "A"
        };
        
        TextEditorService.ModelHandleKeyboardEvent(
            new TextEditorModelsCollection.KeyboardEventAction(
                TextEditorModelKey,
                TextEditorCursorSnapshot.TakeSnapshots(cursor),
                keyboardEventArgs,
                CancellationToken.None));
        
        Assert.Equal(
            keyboardEventArgs.Key, 
            TextEditorModel.GetAllText());
    }
    
    [Fact]
    public void INSERT_MANY_CHARACTER_STRING()
    {
        var cursor = new TextEditorCursor((0, 0), true);

        var content = TestData.CSharp.EXAMPLE_TEXT_173_LINES;
        
        TextEditorService.ModelInsertText(
            new TextEditorModelsCollection.InsertTextAction(
                TextEditorModelKey,
                TextEditorCursorSnapshot.TakeSnapshots(cursor),
                content,
                CancellationToken.None));
        
        Assert.Equal(
            content, 
            TextEditorModel.GetAllText());
    }
    
    /// <summary>
    /// TODO: Insert '\r' then '\n' this would erroneously result in two spaces if typed manually as the two would not combine to "\r\n"
    /// </summary>
    [Fact]
    public void INSERT_CARRIAGE_RETURN_THEN_NEW_LINE()
    {
        throw new NotImplementedException();
    }
    
    [Fact]
    public void DELETE_SINGLE_CHARACTER()
    {
        var cursor = new TextEditorCursor((0, 0), true);

        var content = "A";
        
        TextEditorService.ModelInsertText(
            new TextEditorModelsCollection.InsertTextAction(
                TextEditorModelKey,
                TextEditorCursorSnapshot.TakeSnapshots(cursor),
                content,
                CancellationToken.None));
        
        Assert.Equal(
            content,
            TextEditorModel.GetAllText());
        
        TextEditorService.ModelDeleteTextByRange(
            new TextEditorModelsCollection.DeleteTextByRangeAction(
                TextEditorModelKey,
                TextEditorCursorSnapshot.TakeSnapshots(cursor),
                1,
                CancellationToken.None));
        
        Assert.Equal(
            string.Empty,
            TextEditorModel.GetAllText());
    }
    
    [Fact]
    public void DELETE_MANY_CHARACTERS()
    {
        var startingPositionIndex = 2;
        var count = 3;
        
        var cursor = new TextEditorCursor(
            (0, 0),
            true);

        var content = "Abcdefg";
        
        TextEditorService.ModelInsertText(
            new TextEditorModelsCollection.InsertTextAction(
                TextEditorModelKey,
                TextEditorCursorSnapshot.TakeSnapshots(cursor),
                content,
                CancellationToken.None));
        
        Assert.Equal(
            content,
            TextEditorModel.GetAllText());
        
        cursor.IndexCoordinates = (0, startingPositionIndex);

        TextEditorService.ModelDeleteTextByRange(
            new TextEditorModelsCollection.DeleteTextByRangeAction(
                TextEditorModelKey,
                TextEditorCursorSnapshot.TakeSnapshots(cursor),
                count,
                CancellationToken.None));
        
        Assert.Equal(
            content.Remove(startingPositionIndex, count),
            TextEditorModel.GetAllText());
    }
}