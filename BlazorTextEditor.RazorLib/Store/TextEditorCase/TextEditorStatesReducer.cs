using BlazorTextEditor.RazorLib.Store.TextEditorCase.Actions;
using Fluxor;

namespace BlazorTextEditor.RazorLib.Store.TextEditorCase;

public class TextEditorStatesReducer
{
    [ReducerMethod]
    public static TextEditorStates ReduceRegisterTextEditorBaseAction(
        TextEditorStates previousTextEditorStates,
        RegisterTextEditorBaseAction registerTextEditorBaseAction)
    {
        if (previousTextEditorStates.TextEditorList
            .Any(x => x.Key == registerTextEditorBaseAction.TextEditorBase.Key))
            return previousTextEditorStates;

        var nextList = previousTextEditorStates.TextEditorList
            .Add(registerTextEditorBaseAction.TextEditorBase);

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
            .Single(x => x.Key == forceRerenderAction.TextEditorKey);

        var nextTextEditor = textEditor.PerformForceRerenderAction(forceRerenderAction);

        var nextList = previousTextEditorStates.TextEditorList
            .Replace(textEditor, nextTextEditor);

        return previousTextEditorStates with
        {
            TextEditorList = nextList,
        };
    }

    [ReducerMethod]
    public static TextEditorStates ReduceInsertTextTextEditorBaseAction(
        TextEditorStates previousTextEditorStates,
        InsertTextTextEditorBaseAction insertTextTextEditorBaseAction)
    {
        var textEditor = previousTextEditorStates.TextEditorList
            .Single(x => x.Key == insertTextTextEditorBaseAction.TextEditorKey);

        var nextTextEditor = textEditor.PerformEditTextEditorAction(insertTextTextEditorBaseAction);

        var nextList = previousTextEditorStates.TextEditorList
            .Replace(textEditor, nextTextEditor);

        return previousTextEditorStates with
        {
            TextEditorList = nextList,
        };
    }
    
    [ReducerMethod]
    public static TextEditorStates ReduceKeyboardEventTextEditorBaseAction(
        TextEditorStates previousTextEditorStates,
        KeyboardEventTextEditorBaseAction keyboardEventTextEditorBaseAction)
    {
        var textEditor = previousTextEditorStates.TextEditorList
            .Single(x => x.Key == keyboardEventTextEditorBaseAction.TextEditorKey);

        var nextTextEditor = textEditor.PerformEditTextEditorAction(keyboardEventTextEditorBaseAction);

        var nextList = previousTextEditorStates.TextEditorList
            .Replace(textEditor, nextTextEditor);

        return previousTextEditorStates with
        {
            TextEditorList = nextList,
        };
    }
    
    [ReducerMethod]
    public static TextEditorStates ReduceDeleteTextByMotionTextEditorBaseAction(
        TextEditorStates previousTextEditorStates,
        DeleteTextByMotionTextEditorBaseAction deleteTextByMotionTextEditorBaseAction)
    {
        var textEditor = previousTextEditorStates.TextEditorList
            .Single(x => x.Key == deleteTextByMotionTextEditorBaseAction.TextEditorKey);

        var nextTextEditor = textEditor.PerformEditTextEditorAction(deleteTextByMotionTextEditorBaseAction);

        var nextList = previousTextEditorStates.TextEditorList
            .Replace(textEditor, nextTextEditor);

        return previousTextEditorStates with
        {
            TextEditorList = nextList,
        };
    }
    
    [ReducerMethod]
    public static TextEditorStates ReduceDeleteTextByRangeTextEditorBaseAction(
        TextEditorStates previousTextEditorStates,
        DeleteTextByRangeTextEditorBaseAction deleteTextByRangeTextEditorBaseAction)
    {
        var textEditor = previousTextEditorStates.TextEditorList
            .Single(x => x.Key == deleteTextByRangeTextEditorBaseAction.TextEditorKey);

        var nextTextEditor = textEditor.PerformEditTextEditorAction(deleteTextByRangeTextEditorBaseAction);

        var nextList = previousTextEditorStates.TextEditorList
            .Replace(textEditor, nextTextEditor);

        return previousTextEditorStates with
        {
            TextEditorList = nextList,
        };
    }
    
    [ReducerMethod]
    public static TextEditorStates ReduceEditTextEditorBaseAction(
        TextEditorStates previousTextEditorStates,
        UndoEditAction undoEditAction)
    {
        var textEditor = previousTextEditorStates.TextEditorList
            .Single(x => x.Key == undoEditAction.TextEditorKey);

        var nextTextEditor = textEditor.UndoEdit();

        var nextList = previousTextEditorStates.TextEditorList
            .Replace(textEditor, nextTextEditor);

        return previousTextEditorStates with
        {
            TextEditorList = nextList,
        };
    }
    
    [ReducerMethod]
    public static TextEditorStates ReduceEditTextEditorBaseAction(
        TextEditorStates previousTextEditorStates,
        RedoEditAction redoEditAction)
    {
        var textEditor = previousTextEditorStates.TextEditorList
            .Single(x => x.Key == redoEditAction.TextEditorKey);

        var nextTextEditor = textEditor.RedoEdit();

        var nextList = previousTextEditorStates.TextEditorList
            .Replace(textEditor, nextTextEditor);

        return previousTextEditorStates with
        {
            TextEditorList = nextList,
        };
    }

    [ReducerMethod]
    public static TextEditorStates ReduceDisposeTextEditorBaseAction(
        TextEditorStates previousTextEditorStates,
        DisposeTextEditorBaseAction disposeTextEditorBaseAction)
    {
        var textEditor = previousTextEditorStates.TextEditorList
            .Single(x => x.Key == disposeTextEditorBaseAction.TextEditorKey);

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
                x.Key == textEditorSetUsingRowEndingKindAction.TextEditorKey);

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