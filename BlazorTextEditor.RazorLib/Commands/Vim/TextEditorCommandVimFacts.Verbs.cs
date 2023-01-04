using BlazorTextEditor.RazorLib.Commands.Default;
using BlazorTextEditor.RazorLib.Cursor;
using BlazorTextEditor.RazorLib.Keymap.Vim;
using BlazorTextEditor.RazorLib.Store.TextEditorCase.Actions;

namespace BlazorTextEditor.RazorLib.Commands.Vim;

public static partial class TextEditorCommandVimFacts
{
    public static class Verbs
    {
        public static readonly TextEditorCommand DeleteLine = new(
            async textEditorCommandParameter =>
            {
                await TextEditorCommandDefaultFacts.Cut.DoAsyncFunc
                    .Invoke(textEditorCommandParameter);
            },
            true,
            "Vim::Delete(Line)",
            "Vim::Delete(Line)");

        public static readonly TextEditorCommand ChangeLine = new(
            async textEditorCommandParameter =>
            {
                await DeleteLine.DoAsyncFunc
                    .Invoke(textEditorCommandParameter);

                if (textEditorCommandParameter.TextEditorService.GlobalKeymapDefinition.Keymap
                    is TextEditorKeymapVim vimKeymap)
                {
                    vimKeymap.ActiveVimMode = VimMode.Insert;
                }
            },
            true,
            "Vim::Change(Line)",
            "Vim::Change(Line)");

        public static TextEditorCommand GetDeleteMotion(TextEditorCommand innerTextEditorCommand) => new(
            async textEditorCommandParameter =>
            {
                var textEditorCursorForMotion = new TextEditorCursor(
                    textEditorCommandParameter
                        .PrimaryCursorSnapshot.UserCursor.IndexCoordinates,
                    true);

                var textEditorCommandParameterForMotion = new TextEditorCommandParameter(
                    textEditorCommandParameter.TextEditorBase,
                    TextEditorCursorSnapshot.TakeSnapshots(textEditorCursorForMotion),
                    textEditorCommandParameter.ClipboardProvider,
                    textEditorCommandParameter.TextEditorService,
                    textEditorCommandParameter.TextEditorViewModel);

                var motionResult = await VimMotionResult
                    .GetResultAsync(
                        textEditorCommandParameter,
                        textEditorCursorForMotion,
                        async () =>
                            await innerTextEditorCommand.DoAsyncFunc
                                .Invoke(textEditorCommandParameterForMotion));

                var cursorForDeletion = new TextEditorCursor(
                    (motionResult.LowerPositionIndexImmutableCursor.RowIndex,
                        motionResult.LowerPositionIndexImmutableCursor.ColumnIndex),
                    true);

                var deleteTextTextEditorBaseAction = new DeleteTextByRangeTextEditorBaseAction(
                    textEditorCommandParameter.TextEditorBase.Key,
                    TextEditorCursorSnapshot.TakeSnapshots(cursorForDeletion),
                    motionResult.PositionIndexDisplacement,
                    CancellationToken.None);

                textEditorCommandParameter
                    .TextEditorService
                    .DeleteTextByRange(deleteTextTextEditorBaseAction);
            },
            true,
            $"Vim::Delete({innerTextEditorCommand.DisplayName})",
            $"Vim::Delete({innerTextEditorCommand.DisplayName})");
        
        public static TextEditorCommand GetChangeMotion(TextEditorCommand innerTextEditorCommand) => new(
            async textEditorCommandParameter =>
            {
                var deleteMotion = GetDeleteMotion(innerTextEditorCommand);

                await deleteMotion.DoAsyncFunc
                    .Invoke(textEditorCommandParameter);
                
                if (textEditorCommandParameter.TextEditorService.GlobalKeymapDefinition.Keymap
                    is TextEditorKeymapVim textEditorKeymapVim)
                {
                    textEditorKeymapVim.ActiveVimMode = VimMode.Insert;
                }
            },
            true,
            $"Vim::Change({innerTextEditorCommand.DisplayName})",
            $"Vim::Change({innerTextEditorCommand.DisplayName})");
        
        public static readonly TextEditorCommand ChangeSelection = new(
            async textEditorCommandParameter =>
            {
                await TextEditorCommandDefaultFacts.Cut.DoAsyncFunc
                    .Invoke(textEditorCommandParameter);
                
                if (textEditorCommandParameter.TextEditorService.GlobalKeymapDefinition.Keymap
                    is TextEditorKeymapVim textEditorKeymapVim)
                {
                    textEditorKeymapVim.ActiveVimMode = VimMode.Insert;
                }
            },
            true,
            "Vim::Change(Selection)",
            "Vim::Change(Selection)");
        
        public static readonly TextEditorCommand Yank = new(
            async textEditorCommandParameter =>
            {
                await TextEditorCommandDefaultFacts.Copy.DoAsyncFunc
                    .Invoke(textEditorCommandParameter);
                
                await TextEditorCommandDefaultFacts.ClearTextSelection.DoAsyncFunc
                    .Invoke(textEditorCommandParameter);
            },
            true,
            "Vim::Change(Selection)",
            "Vim::Change(Selection)");
        
        public static readonly TextEditorCommand NewLineBelow = new(
            async textEditorCommandParameter =>
            {
                await TextEditorCommandDefaultFacts.NewLineBelow.DoAsyncFunc
                    .Invoke(textEditorCommandParameter);
                
                if (textEditorCommandParameter.TextEditorService.GlobalKeymapDefinition.Keymap
                    is TextEditorKeymapVim textEditorKeymapVim)
                {
                    textEditorKeymapVim.ActiveVimMode = VimMode.Insert;
                }
            },
            true,
            "Vim::NewLineBelow()",
            "Vim::NewLineBelow()");
        
        public static readonly TextEditorCommand NewLineAbove = new(
            async textEditorCommandParameter =>
            {
                await TextEditorCommandDefaultFacts.NewLineAbove.DoAsyncFunc
                    .Invoke(textEditorCommandParameter);
                
                if (textEditorCommandParameter.TextEditorService.GlobalKeymapDefinition.Keymap
                    is TextEditorKeymapVim textEditorKeymapVim)
                {
                    textEditorKeymapVim.ActiveVimMode = VimMode.Insert;
                }
            },
            true,
            "Vim::NewLineAbove()",
            "Vim::NewLineAbove()");
    }
}