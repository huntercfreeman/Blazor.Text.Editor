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
    public static TextEditorStates ReduceEditTextEditorBaseAction(
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