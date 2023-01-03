using System.Collections.Immutable;
using BlazorALaCarte.Shared.JavaScriptObjects;
using BlazorALaCarte.Shared.Keyboard;
using BlazorTextEditor.RazorLib.Commands;
using BlazorTextEditor.RazorLib.Cursor;
using BlazorTextEditor.RazorLib.Keymap.VimKeymapSpecifics;
using BlazorTextEditor.RazorLib.Store.TextEditorCase.ViewModels;
using BlazorTextEditor.RazorLib.TextEditor;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorTextEditor.RazorLib.Keymap;

public class TextEditorKeymapVim : ITextEditorKeymap
{
    public KeymapKey KeymapKey => KeymapFacts.VimKeymapDefinition.KeymapKey;
    public string KeymapDisplayName => KeymapFacts.VimKeymapDefinition.DisplayName;

    private readonly TextEditorKeymapDefault _textEditorKeymapDefault = new();

    public VimMode ActiveVimMode { get; set; } = VimMode.Normal;
    
    public VimSentence VimSentence { get; } = new();
    
    public string GetCursorCssClassString()
    {
        switch (ActiveVimMode)
        {
            case VimMode.Normal:
            case VimMode.Visual:
            case VimMode.VisualLine:
                return TextCursorKindFacts.BlockCssClassString;
            default:
                return string.Empty;
        }
    }
    
    public string GetCursorCssStyleString(
        TextEditorBase textEditorBase,
        TextEditorViewModel textEditorViewModel,
        TextEditorOptions textEditorOptions)
    {
        var characterWidthInPixels = textEditorViewModel.VirtualizationResult.CharacterWidthAndRowHeight.CharacterWidthInPixels;

        switch (ActiveVimMode)
        {
            case VimMode.Normal:
            case VimMode.Visual:
            case VimMode.VisualLine:
            {
                var characterWidthInPixelsInvariantCulture = characterWidthInPixels
                    .ToString(System.Globalization.CultureInfo.InvariantCulture);
                
                return $"width: {characterWidthInPixelsInvariantCulture}px;";
            }
        }

        return string.Empty;
    }
    
    public TextEditorCommand? Map(
        KeyboardEventArgs keyboardEventArgs,
        bool hasTextSelection)
    {
        if (KeyboardKeyFacts.IsMovementKey(keyboardEventArgs.Key))
        {
            return new TextEditorCommand(
                textEditorCommandParameter =>
                {
                    var previousAnchorPositionIndex = textEditorCommandParameter
                        .PrimaryCursorSnapshot.UserCursor.TextEditorSelection.AnchorPositionIndex;

                    var previousEndingPositionIndex = textEditorCommandParameter
                        .PrimaryCursorSnapshot.UserCursor.TextEditorSelection.EndingPositionIndex;

                    if (ActiveVimMode == VimMode.VisualLine ||
                        ActiveVimMode == VimMode.Visual)
                    {
                        keyboardEventArgs.ShiftKey = true;
                    }
                    
                    TextEditorCommand? modifiedCommand = null;
                        
                    if (keyboardEventArgs.CtrlKey)
                    {
                        modifiedCommand = _textEditorKeymapDefault.DefaultCtrlModifiedKeymap(
                            keyboardEventArgs,
                            hasTextSelection);
                    }
                    
                    if (modifiedCommand is null &&
                        keyboardEventArgs.AltKey)
                    {
                        modifiedCommand = _textEditorKeymapDefault.DefaultAltModifiedKeymap(
                            keyboardEventArgs,
                            hasTextSelection);
                    }
                    
                    if (modifiedCommand is not null)
                    {
                        modifiedCommand.DoAsyncFunc.Invoke(textEditorCommandParameter);
                    }
                    else
                    {
                        TextEditorCursor.MoveCursor(
                            keyboardEventArgs,
                            textEditorCommandParameter.PrimaryCursorSnapshot.UserCursor,
                            textEditorCommandParameter.TextEditorBase);
                    }

                    if (ActiveVimMode == VimMode.VisualLine)
                    {
                        var nextEndingPositionIndex = textEditorCommandParameter
                            .PrimaryCursorSnapshot.UserCursor.TextEditorSelection.EndingPositionIndex;

                        if (nextEndingPositionIndex < textEditorCommandParameter
                                .PrimaryCursorSnapshot.UserCursor.TextEditorSelection.AnchorPositionIndex)
                        {
                            if (previousAnchorPositionIndex < previousEndingPositionIndex)
                            {
                                // Anchor went from being the lower bound to the upper bound.
                                
                                var rowDataAnchorIsOn = textEditorCommandParameter.TextEditorBase
                                    .FindRowIndexRowStartRowEndingTupleFromPositionIndex(
                                        previousAnchorPositionIndex.Value);
                            
                                textEditorCommandParameter
                                        .PrimaryCursorSnapshot.UserCursor.TextEditorSelection.AnchorPositionIndex = 
                                    textEditorCommandParameter
                                        .TextEditorBase.RowEndingPositions[rowDataAnchorIsOn.rowIndex]
                                        .positionIndex;
                            }

                            var startingPositionOfRow = textEditorCommandParameter.TextEditorBase
                                .GetStartOfRowTuple(textEditorCommandParameter.PrimaryCursorSnapshot.UserCursor
                                    .IndexCoordinates.rowIndex)
                                .positionIndex;
                            
                            textEditorCommandParameter.PrimaryCursorSnapshot.UserCursor.TextEditorSelection
                                .EndingPositionIndex = startingPositionOfRow;
                        }
                        else if (nextEndingPositionIndex >= textEditorCommandParameter
                                     .PrimaryCursorSnapshot.UserCursor.TextEditorSelection.AnchorPositionIndex)
                        {
                            if (previousAnchorPositionIndex > previousEndingPositionIndex)
                            {
                                // Anchor went from being the upper bound to the lower bound.
                                
                                var rowDataAnchorIsOn = textEditorCommandParameter.TextEditorBase
                                    .FindRowIndexRowStartRowEndingTupleFromPositionIndex(
                                        previousAnchorPositionIndex.Value);
                                
                                textEditorCommandParameter
                                    .PrimaryCursorSnapshot.UserCursor.TextEditorSelection.AnchorPositionIndex = 
                                        textEditorCommandParameter.TextEditorBase.GetStartOfRowTuple(
                                                rowDataAnchorIsOn.rowIndex - 1)
                                        .positionIndex; 
                            } 
                            
                            var endingPositionOfRow = textEditorCommandParameter
                                .TextEditorBase.RowEndingPositions[
                                    textEditorCommandParameter.PrimaryCursorSnapshot.UserCursor.IndexCoordinates.rowIndex]
                                .positionIndex;

                            textEditorCommandParameter.PrimaryCursorSnapshot.UserCursor.TextEditorSelection
                                .EndingPositionIndex = endingPositionOfRow;
                        }
                    }
                    
                    return Task.CompletedTask;
                },
                true,
                keyboardEventArgs.Key,
                keyboardEventArgs.Key);
        }
        
        if (TryMapToVimKeymap(
                keyboardEventArgs,
                hasTextSelection,
                out var command))
        {
            return command;
        }
        
        if (keyboardEventArgs.CtrlKey)
        {
            return _textEditorKeymapDefault.DefaultCtrlModifiedKeymap(
                keyboardEventArgs,
                hasTextSelection);
        }

        if (keyboardEventArgs.AltKey)
        {
            return _textEditorKeymapDefault.DefaultAltModifiedKeymap(
                keyboardEventArgs,
                hasTextSelection);
        }
            
        if (hasTextSelection)
        {
            return _textEditorKeymapDefault.DefaultHasSelectionModifiedKeymap(
                keyboardEventArgs,
                hasTextSelection);
        }
            
        return keyboardEventArgs.Key switch
        {
            KeyboardKeyFacts.MetaKeys.PAGE_DOWN => TextEditorCommandFacts.ScrollPageDown,
            KeyboardKeyFacts.MetaKeys.PAGE_UP => TextEditorCommandFacts.ScrollPageUp,
            _ => null,
        };
    }

    public bool TryMapToVimKeymap(
        KeyboardEventArgs keyboardEventArgs,
        bool hasTextSelection,
        out TextEditorCommand? command)
    {
        switch (ActiveVimMode)
        {
            case VimMode.Normal:
            case VimMode.Visual:
            case VimMode.VisualLine:
            case VimMode.Command:
            {
                if (TryMapToVimNormalModeKeymap(
                        keyboardEventArgs,
                        hasTextSelection,
                        out command))
                {
                    return true;
                }

                goto default;
            }
            case VimMode.Insert:
            {
                if (TryMapToVimInsertModeKeymap(
                        keyboardEventArgs,
                        hasTextSelection,
                        out command))
                {
                    return true;
                }

                goto default;
            }
            default:
            {
                command = null;
                return false;
            }
        }
    }

    public bool TryMapToVimNormalModeKeymap(
        KeyboardEventArgs keyboardEventArgs,
        bool hasTextSelection,
        out TextEditorCommand? command)
    {
        switch (keyboardEventArgs.Key)
        {
            case "i":
            {
                ActiveVimMode = VimMode.Insert;
                command = TextEditorCommandFacts.DoNothingDiscard;
                return true;
            }
            case "v":
            {
                ActiveVimMode = VimMode.Visual;
                
                command = new TextEditorCommand(
                    textEditorCommandParameter =>
                    {
                        var positionIndex =
                            textEditorCommandParameter.TextEditorBase.GetPositionIndex(
                                textEditorCommandParameter.PrimaryCursorSnapshot.ImmutableCursor.RowIndex,
                                textEditorCommandParameter.PrimaryCursorSnapshot.ImmutableCursor.ColumnIndex);

                        textEditorCommandParameter.PrimaryCursorSnapshot.UserCursor.TextEditorSelection.AnchorPositionIndex =
                            positionIndex;

                        textEditorCommandParameter.PrimaryCursorSnapshot.UserCursor.TextEditorSelection
                                .EndingPositionIndex =
                            positionIndex + 1;
                    
                        return Task.CompletedTask;
                    },
                    true,
                    keyboardEventArgs.Key,
                    keyboardEventArgs.Key); 
                
                return true;
            }
            case "V":
            {
                ActiveVimMode = VimMode.VisualLine;
                
                command = new TextEditorCommand(
                    textEditorCommandParameter =>
                    {
                        var startOfRowPositionIndexInclusive =
                            textEditorCommandParameter.TextEditorBase.GetPositionIndex(
                                textEditorCommandParameter.PrimaryCursorSnapshot.ImmutableCursor.RowIndex,
                                0);

                        textEditorCommandParameter.PrimaryCursorSnapshot.UserCursor.TextEditorSelection.AnchorPositionIndex =
                            startOfRowPositionIndexInclusive;

                        var endOfRowPositionIndexExclusive = textEditorCommandParameter.TextEditorBase.RowEndingPositions[
                                textEditorCommandParameter.PrimaryCursorSnapshot.ImmutableCursor.RowIndex]
                            .positionIndex;
                        
                        textEditorCommandParameter.PrimaryCursorSnapshot.UserCursor.TextEditorSelection
                                .EndingPositionIndex =
                            endOfRowPositionIndexExclusive;
                    
                        return Task.CompletedTask;
                    },
                    true,
                    keyboardEventArgs.Key,
                    keyboardEventArgs.Key); 
                
                return true;
            }
            case ":":
            {
                command = new TextEditorCommand(
                    async textEditorCommandParameter =>
                    {
                        textEditorCommandParameter.TextEditorService.SetViewModelWith(
                            textEditorCommandParameter.TextEditorViewModel.TextEditorViewModelKey,
                            previousViewModel => previousViewModel with
                            {
                                DisplayCommandBar = true
                            });
                    },
                    false,
                    "Command Mode",
                    "Command Mode");

                return true;
            }
            case "u":
            {
                command = new TextEditorCommand(
                    async textEditorCommandParameter =>
                    {
                        textEditorCommandParameter.TextEditorService.UndoEdit(
                            textEditorCommandParameter.TextEditorBase.Key);
                    },
                    false,
                    "Undo",
                    "undo");

                return true;
            }
            case "r":
            {
                if (keyboardEventArgs.CtrlKey)
                {
                    command = new TextEditorCommand(
                        async textEditorCommandParameter =>
                        {
                            textEditorCommandParameter.TextEditorService.RedoEdit(
                                textEditorCommandParameter.TextEditorBase.Key);
                        },
                        false,
                        "Redo",
                        "redo");

                    return true;
                }

                goto default;
            }
            default:
            {
                if (keyboardEventArgs.Key == "Shift")
                {
                    command = TextEditorCommandFacts.DoNothingDiscard;
                    return false;
                }
                
                var success = VimSentence.TryLex(
                    keyboardEventArgs,
                    hasTextSelection,
                    out command);

                return success;
            }
        }
    }
    
    public bool TryMapToVimInsertModeKeymap(
        KeyboardEventArgs keyboardEventArgs,
        bool hasTextSelection,
        out TextEditorCommand command)
    {
        switch (keyboardEventArgs.Key)
        {
            case KeyboardKeyFacts.MetaKeys.ESCAPE:
            {
                ActiveVimMode = VimMode.Normal;
                command = TextEditorCommandFacts.DoNothingDiscard;
                return true;
            }
            default:
            {
                command = null;
                return false;
            }
        }
    }
}