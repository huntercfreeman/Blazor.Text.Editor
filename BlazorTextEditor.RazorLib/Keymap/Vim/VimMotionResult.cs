using BlazorTextEditor.RazorLib.Commands;
using BlazorTextEditor.RazorLib.Cursor;

namespace BlazorTextEditor.RazorLib.Keymap.Vim;

public record VimMotionResult(
    ImmutableTextEditorCursor LowerPositionIndexImmutableCursor,
    int LowerPositionIndex,
    ImmutableTextEditorCursor HigherPositionIndexImmutableCursor,
    int HigherPositionIndex,
    int PositionIndexDisplacement)
{
    public static async Task<VimMotionResult> GetResultAsync(
        ITextEditorCommandParameter textEditorCommandParameter,
        TextEditorCursor textEditorCursorForMotion,
        Func<Task> motionCommandParameter)
    {
        await motionCommandParameter.Invoke();
        
        var beforeMotionImmutableCursor = textEditorCommandParameter.PrimaryCursorSnapshot.ImmutableCursor;
        
        var beforeMotionPositionIndex = textEditorCommandParameter.TextEditorModel
            .GetPositionIndex(
                beforeMotionImmutableCursor.RowIndex,
                beforeMotionImmutableCursor.ColumnIndex);
        
        var afterMotionImmutableCursor = new ImmutableTextEditorCursor(
            textEditorCursorForMotion);
        
        var afterMotionPositionIndex = textEditorCommandParameter.TextEditorModel
            .GetPositionIndex(
                afterMotionImmutableCursor.RowIndex,
                afterMotionImmutableCursor.ColumnIndex);

        if (beforeMotionPositionIndex > afterMotionPositionIndex)
        {
            return new VimMotionResult(
                afterMotionImmutableCursor,
                afterMotionPositionIndex,
                beforeMotionImmutableCursor,
                beforeMotionPositionIndex,
                beforeMotionPositionIndex - afterMotionPositionIndex);
        }
        
        return new VimMotionResult(
            beforeMotionImmutableCursor,
            beforeMotionPositionIndex,
            afterMotionImmutableCursor,
            afterMotionPositionIndex,
            afterMotionPositionIndex - beforeMotionPositionIndex);
    }
}