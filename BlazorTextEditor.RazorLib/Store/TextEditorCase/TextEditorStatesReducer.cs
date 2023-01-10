using BlazorTextEditor.RazorLib.Store.TextEditorCase.Actions;
using BlazorTextEditor.RazorLib.TextEditor;
using Fluxor;

namespace BlazorTextEditor.RazorLib.Store.TextEditorCase;

public class TextEditorStatesReducer
{
    [ReducerMethod]
    public static TextEditorStates ReduceRegisterTextEditorModelAction(
        TextEditorStates previousTextEditorStates,
        RegisterTextEditorModelAction registerTextEditorModelAction)
    {
        if (previousTextEditorStates.TextEditorList
            .Any(x => 
                x.ModelKey == registerTextEditorModelAction.TextEditorModel.ModelKey ||
                x.ResourceUri == registerTextEditorModelAction.TextEditorModel.ResourceUri))
        {
            return previousTextEditorStates;
        }

        var nextList = previousTextEditorStates.TextEditorList
            .Add(registerTextEditorModelAction.TextEditorModel);

        return previousTextEditorStates with
        {
            TextEditorList = nextList,
        };
    }
    
    [ReducerMethod]
    public static TextEditorStates ReduceForceRerenderAction(
        TextEditorStates previousTextEditorStates,
        ForceRerenderAction forceRerenderAction)
    {
        var textEditor = previousTextEditorStates.TextEditorList
            .Single(x => x.ModelKey == forceRerenderAction.TextEditorModelKey);

        var nextTextEditor = textEditor.PerformForceRerenderAction(forceRerenderAction);

        var nextList = previousTextEditorStates.TextEditorList
            .Replace(textEditor, nextTextEditor);

        return previousTextEditorStates with
        {
            TextEditorList = nextList,
        };
    }

    [ReducerMethod]
    public static TextEditorStates ReduceInsertTextTextEditorModelAction(
        TextEditorStates previousTextEditorStates,
        InsertTextTextEditorModelAction insertTextTextEditorModelAction)
    {
        var textEditor = previousTextEditorStates.TextEditorList
            .Single(x => x.ModelKey == insertTextTextEditorModelAction.TextEditorModelKey);

        var nextTextEditor = textEditor.PerformEditTextEditorAction(insertTextTextEditorModelAction);

        var nextList = previousTextEditorStates.TextEditorList
            .Replace(textEditor, nextTextEditor);

        return previousTextEditorStates with
        {
            TextEditorList = nextList,
        };
    }
    
    [ReducerMethod]
    public static TextEditorStates ReduceKeyboardEventTextEditorModelAction(
        TextEditorStates previousTextEditorStates,
        KeyboardEventTextEditorModelAction keyboardEventTextEditorModelAction)
    {
        var textEditor = previousTextEditorStates.TextEditorList
            .Single(x => x.ModelKey == keyboardEventTextEditorModelAction.TextEditorModelKey);

        var nextTextEditor = textEditor.PerformEditTextEditorAction(keyboardEventTextEditorModelAction);

        var nextList = previousTextEditorStates.TextEditorList
            .Replace(textEditor, nextTextEditor);

        return previousTextEditorStates with
        {
            TextEditorList = nextList,
        };
    }
    
    [ReducerMethod]
    public static TextEditorStates ReduceDeleteTextByMotionTextEditorModelAction(
        TextEditorStates previousTextEditorStates,
        DeleteTextByMotionTextEditorModelAction deleteTextByMotionTextEditorModelAction)
    {
        var textEditor = previousTextEditorStates.TextEditorList
            .Single(x => x.ModelKey == deleteTextByMotionTextEditorModelAction.TextEditorModelKey);

        var nextTextEditor = textEditor.PerformEditTextEditorAction(deleteTextByMotionTextEditorModelAction);

        var nextList = previousTextEditorStates.TextEditorList
            .Replace(textEditor, nextTextEditor);

        return previousTextEditorStates with
        {
            TextEditorList = nextList,
        };
    }
    
    [ReducerMethod]
    public static TextEditorStates ReduceDeleteTextByRangeTextEditorModelAction(
        TextEditorStates previousTextEditorStates,
        DeleteTextByRangeTextEditorModelAction deleteTextByRangeTextEditorModelAction)
    {
        var textEditor = previousTextEditorStates.TextEditorList
            .Single(x => x.ModelKey == deleteTextByRangeTextEditorModelAction.TextEditorModelKey);

        var nextTextEditor = textEditor.PerformEditTextEditorAction(deleteTextByRangeTextEditorModelAction);

        var nextList = previousTextEditorStates.TextEditorList
            .Replace(textEditor, nextTextEditor);

        return previousTextEditorStates with
        {
            TextEditorList = nextList,
        };
    }
    
    [ReducerMethod]
    public static TextEditorStates ReduceUndoEditAction(
        TextEditorStates previousTextEditorStates,
        UndoEditAction undoEditAction)
    {
        var textEditor = previousTextEditorStates.TextEditorList
            .Single(x => x.ModelKey == undoEditAction.TextEditorModelKey);

        var nextTextEditor = textEditor.UndoEdit();

        var nextList = previousTextEditorStates.TextEditorList
            .Replace(textEditor, nextTextEditor);

        return previousTextEditorStates with
        {
            TextEditorList = nextList,
        };
    }
    
    [ReducerMethod]
    public static TextEditorStates ReduceRedoEditAction(
        TextEditorStates previousTextEditorStates,
        RedoEditAction redoEditAction)
    {
        var textEditor = previousTextEditorStates.TextEditorList
            .Single(x => x.ModelKey == redoEditAction.TextEditorModelKey);

        var nextTextEditor = textEditor.RedoEdit();

        var nextList = previousTextEditorStates.TextEditorList
            .Replace(textEditor, nextTextEditor);

        return previousTextEditorStates with
        {
            TextEditorList = nextList,
        };
    }
    
    [ReducerMethod]
    public static TextEditorStates ReduceReloadTextEditorModelAction(
        TextEditorStates previousTextEditorStates,
        ReloadTextEditorModelAction reloadTextEditorModelAction)
    {
        var textEditor = previousTextEditorStates.TextEditorList
            .Single(x => x.ModelKey == reloadTextEditorModelAction.TextEditorModelKey);

        var nextTextEditor = new TextEditorModel(textEditor);
        nextTextEditor.SetContent(reloadTextEditorModelAction.Content);

        var nextList = previousTextEditorStates.TextEditorList
            .Replace(textEditor, nextTextEditor);

        return previousTextEditorStates with
        {
            TextEditorList = nextList,
        };
    }
    
    [ReducerMethod]
    public static TextEditorStates ReduceTextEditorSetResourceDataAction(
        TextEditorStates previousTextEditorStates,
        TextEditorSetResourceDataAction textEditorSetResourceDataAction)
    {
        var textEditor = previousTextEditorStates.TextEditorList
            .Single(x => x.ModelKey == textEditorSetResourceDataAction.TextEditorModelKey);

        var nextTextEditor = textEditor.SetResourceData(
            textEditorSetResourceDataAction.ResourceUri,
            textEditorSetResourceDataAction.ResourceLastWriteTime);

        var nextList = previousTextEditorStates.TextEditorList
            .Replace(textEditor, nextTextEditor);

        return previousTextEditorStates with
        {
            TextEditorList = nextList,
        };
    }

    [ReducerMethod]
    public static TextEditorStates ReduceDisposeTextEditorModelAction(
        TextEditorStates previousTextEditorStates,
        DisposeTextEditorModelAction disposeTextEditorModelAction)
    {
        var textEditor = previousTextEditorStates.TextEditorList
            .Single(x => x.ModelKey == disposeTextEditorModelAction.TextEditorModelKey);

        var nextList = previousTextEditorStates.TextEditorList
            .Remove(textEditor);

        return previousTextEditorStates with
        {
            TextEditorList = nextList,
        };
    }

    [ReducerMethod]
    public static TextEditorStates ReduceTextEditorSetFontSizeAction(
        TextEditorStates previousTextEditorStates,
        TextEditorSetFontSizeAction textEditorSetFontSizeAction)
    {
        var nextTextEditorOptions = previousTextEditorStates
                .GlobalTextEditorOptions with
            {
                FontSizeInPixels = textEditorSetFontSizeAction.FontSizeInPixels,
            };

        return previousTextEditorStates with
        {
            GlobalTextEditorOptions = nextTextEditorOptions,
        };
    }
    
    [ReducerMethod]
    public static TextEditorStates ReduceTextEditorSetCursorWidthAction(
        TextEditorStates previousTextEditorStates,
        TextEditorSetCursorWidthAction textEditorSetCursorWidthAction)
    {
        var nextTextEditorOptions = previousTextEditorStates
                .GlobalTextEditorOptions with
            {
                CursorWidthInPixels = textEditorSetCursorWidthAction.CursorWidthInPixels,
            };

        return previousTextEditorStates with
        {
            GlobalTextEditorOptions = nextTextEditorOptions,
        };
    }
    
    [ReducerMethod]
    public static TextEditorStates ReduceTextEditorSetHeightAction(
        TextEditorStates previousTextEditorStates,
        TextEditorSetHeightAction textEditorSetHeightAction)
    {
        var nextTextEditorOptions = previousTextEditorStates
                .GlobalTextEditorOptions with
            {
                HeightInPixels = textEditorSetHeightAction.HeightInPixels,
            };

        return previousTextEditorStates with
        {
            GlobalTextEditorOptions = nextTextEditorOptions,
        };
    }

    [ReducerMethod]
    public static TextEditorStates ReduceTextEditorSetThemeAction(
        TextEditorStates previousTextEditorStates,
        TextEditorSetThemeAction textEditorSetThemeAction)
    {
        var nextTextEditorOptions = previousTextEditorStates
                .GlobalTextEditorOptions with
            {
                Theme = textEditorSetThemeAction.Theme,
            };

        return previousTextEditorStates with
        {
            GlobalTextEditorOptions = nextTextEditorOptions,
        };
    }
    
    [ReducerMethod]
    public static TextEditorStates ReduceTextEditorSetKeymapAction(
        TextEditorStates previousTextEditorStates,
        TextEditorSetKeymapAction textEditorSetKeymapAction)
    {
        var nextTextEditorOptions = previousTextEditorStates
                .GlobalTextEditorOptions with
            {
                KeymapDefinition = textEditorSetKeymapAction.KeymapDefinition,
            };

        return previousTextEditorStates with
        {
            GlobalTextEditorOptions = nextTextEditorOptions,
        };
    }

    [ReducerMethod]
    public static TextEditorStates ReduceTextEditorSetShowWhitespaceAction(
        TextEditorStates previousTextEditorStates,
        TextEditorSetShowWhitespaceAction textEditorSetShowWhitespaceAction)
    {
        var nextTextEditorOptions = previousTextEditorStates
                .GlobalTextEditorOptions with
            {
                ShowWhitespace = textEditorSetShowWhitespaceAction.ShowWhitespace,
            };

        return previousTextEditorStates with
        {
            GlobalTextEditorOptions = nextTextEditorOptions,
        };
    }

    [ReducerMethod]
    public static TextEditorStates ReduceTextEditorSetShowNewlinesAction(
        TextEditorStates previousTextEditorStates,
        TextEditorSetShowNewlinesAction textEditorSetShowNewlinesAction)
    {
        var nextTextEditorOptions = previousTextEditorStates
                .GlobalTextEditorOptions with
            {
                ShowNewlines = textEditorSetShowNewlinesAction.ShowNewlines,
            };

        return previousTextEditorStates with
        {
            GlobalTextEditorOptions = nextTextEditorOptions,
        };
    }

    [ReducerMethod]
    public static TextEditorStates ReduceTextEditorSetUsingRowEndingKindAction(
        TextEditorStates previousTextEditorStates,
        TextEditorSetUsingRowEndingKindAction textEditorSetUsingRowEndingKindAction)
    {
        var textEditor = previousTextEditorStates.TextEditorList
            .Single(x =>
                x.ModelKey == textEditorSetUsingRowEndingKindAction.TextEditorModelKey);

        var nextTextEditor = textEditor
            .SetUsingRowEndingKind(textEditorSetUsingRowEndingKindAction.RowEndingKind);

        var nextList = previousTextEditorStates.TextEditorList
            .Replace(textEditor, nextTextEditor);

        return previousTextEditorStates with
        {
            TextEditorList = nextList,
        };
    }
}