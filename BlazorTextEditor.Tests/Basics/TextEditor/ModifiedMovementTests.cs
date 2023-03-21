using BlazorCommon.RazorLib.Keyboard;
using BlazorTextEditor.RazorLib.Cursor;
using BlazorTextEditor.RazorLib.Store.Model;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorTextEditor.Tests.Basics.TextEditor;

public class ModifiedMovementTests : BlazorTextEditorTestingBase
{
    [Fact]
    public void CONTROL_ARROW_LEFT_EMPTY()
    {
        var text = TextEditorModel.GetAllText();

        Assert.Equal(string.Empty, text);
        Assert.Equal((0, 0), TextEditorViewModel.PrimaryCursor.IndexCoordinates);

        var controlArrowLeftKeyboardEventArg = new KeyboardEventArgs
        {
            Key = KeyboardKeyFacts.MovementKeys.ARROW_LEFT,
            CtrlKey = true
        };
        
        TextEditorCursor.MoveCursor(
            controlArrowLeftKeyboardEventArg,
            TextEditorViewModel.PrimaryCursor,
            TextEditorModel);
        
        Assert.Equal((0, 0), TextEditorViewModel.PrimaryCursor.IndexCoordinates);
    }
    
    [Fact]
    public void CONTROL_ARROW_LEFT_SHOULD_WRAP()
    {
        var content = "See you later!\nOh wait I forgot something!";
        
        var insertTextAction = new TextEditorModelsCollection.InsertTextAction(
            TextEditorModelKey,
            TextEditorCursorSnapshot.TakeSnapshots(TextEditorViewModel.PrimaryCursor),
            content,
            CancellationToken.None);
        
        TextEditorService.ModelInsertText(insertTextAction);
        
        var text = TextEditorModel.GetAllText();

        Assert.Equal(content, text);
        
        TextEditorViewModel.PrimaryCursor.PreferredColumnIndex = 0;
        
        TextEditorViewModel.PrimaryCursor.IndexCoordinates = 
            (1, TextEditorViewModel.PrimaryCursor.PreferredColumnIndex);
        
        var controlArrowLeftKeyboardEventArg = new KeyboardEventArgs
        {
            Key = KeyboardKeyFacts.MovementKeys.ARROW_LEFT,
            CtrlKey = true
        };
        
        TextEditorCursor.MoveCursor(
            controlArrowLeftKeyboardEventArg,
            TextEditorViewModel.PrimaryCursor,
            TextEditorModel);

        Assert.Equal(
            (0, content.IndexOf('\n')),
            TextEditorViewModel.PrimaryCursor.IndexCoordinates);
    }
    
    [Fact]
    public void CONTROL_ARROW_LEFT_SHOULD_NOT_WRAP()
    {
        var content = "See you later!\nOh wait I forgot something!";
        
        var insertTextAction = new TextEditorModelsCollection.InsertTextAction(
            TextEditorModelKey,
            TextEditorCursorSnapshot.TakeSnapshots(TextEditorViewModel.PrimaryCursor),
            content,
            CancellationToken.None);
        
        TextEditorService.ModelInsertText(insertTextAction);
        
        var text = TextEditorModel.GetAllText();

        Assert.Equal(content, text);
        
        TextEditorViewModel.PrimaryCursor.PreferredColumnIndex = 2;
        
        TextEditorViewModel.PrimaryCursor.IndexCoordinates = 
            (1, TextEditorViewModel.PrimaryCursor.PreferredColumnIndex);
        
        var controlArrowLeftKeyboardEventArg = new KeyboardEventArgs
        {
            Key = KeyboardKeyFacts.MovementKeys.ARROW_LEFT,
            CtrlKey = true
        };
        
        TextEditorCursor.MoveCursor(
            controlArrowLeftKeyboardEventArg,
            TextEditorViewModel.PrimaryCursor,
            TextEditorModel);

        Assert.Equal(
            (1, 0),
            TextEditorViewModel.PrimaryCursor.IndexCoordinates);
    }
    
    [Fact]
    public void CONTROL_ARROW_RIGHT_EMPTY()
    {
        var text = TextEditorModel.GetAllText();

        Assert.Equal(string.Empty, text);
        Assert.Equal((0, 0), TextEditorViewModel.PrimaryCursor.IndexCoordinates);

        var controlArrowRightKeyboardEventArg = new KeyboardEventArgs
        {
            Key = KeyboardKeyFacts.MovementKeys.ARROW_RIGHT,
            CtrlKey = true
        };
        
        TextEditorCursor.MoveCursor(
            controlArrowRightKeyboardEventArg,
            TextEditorViewModel.PrimaryCursor,
            TextEditorModel);
        
        Assert.Equal((0, 0), TextEditorViewModel.PrimaryCursor.IndexCoordinates);
    }
    
    [Fact]
    public void CONTROL_ARROW_RIGHT_SHOULD_WRAP()
    {
        var content = "See you later!\nOh wait I forgot something!";
        
        var insertTextAction = new TextEditorModelsCollection.InsertTextAction(
            TextEditorModelKey,
            TextEditorCursorSnapshot.TakeSnapshots(TextEditorViewModel.PrimaryCursor),
            content,
            CancellationToken.None);
        
        TextEditorService.ModelInsertText(insertTextAction);
        
        var text = TextEditorModel.GetAllText();

        Assert.Equal(content, text);
        
        TextEditorViewModel.PrimaryCursor.PreferredColumnIndex = content.IndexOf('\n');
        
        TextEditorViewModel.PrimaryCursor.IndexCoordinates = 
            (0, TextEditorViewModel.PrimaryCursor.PreferredColumnIndex);
        
        var controlArrowRightKeyboardEventArg = new KeyboardEventArgs
        {
            Key = KeyboardKeyFacts.MovementKeys.ARROW_RIGHT,
            CtrlKey = true
        };
        
        TextEditorCursor.MoveCursor(
            controlArrowRightKeyboardEventArg,
            TextEditorViewModel.PrimaryCursor,
            TextEditorModel);

        Assert.Equal(
            (1, 0),
            TextEditorViewModel.PrimaryCursor.IndexCoordinates);
    }
    
    [Fact]
    public void CONTROL_ARROW_RIGHT_SHOULD_NOT_WRAP()
    {
        var content = "See you later!\nOh wait I forgot something!";
        
        var insertTextAction = new TextEditorModelsCollection.InsertTextAction(
            TextEditorModelKey,
            TextEditorCursorSnapshot.TakeSnapshots(TextEditorViewModel.PrimaryCursor),
            content,
            CancellationToken.None);
        
        TextEditorService.ModelInsertText(insertTextAction);
        
        var text = TextEditorModel.GetAllText();

        Assert.Equal(content, text);
        
        TextEditorViewModel.PrimaryCursor.PreferredColumnIndex = 1;
        
        TextEditorViewModel.PrimaryCursor.IndexCoordinates = 
            (0, TextEditorViewModel.PrimaryCursor.PreferredColumnIndex);
        
        var controlArrowRightKeyboardEventArg = new KeyboardEventArgs
        {
            Key = KeyboardKeyFacts.MovementKeys.ARROW_RIGHT,
            CtrlKey = true
        };
        
        TextEditorCursor.MoveCursor(
            controlArrowRightKeyboardEventArg,
            TextEditorViewModel.PrimaryCursor,
            TextEditorModel);

        Assert.Equal(
            (0, 2),
            TextEditorViewModel.PrimaryCursor.IndexCoordinates);
    }

    [Fact]
    public void CONTROL_HOME()
    {
    }
    
    [Fact]
    public void CONTROL_END()
    {
    }
}