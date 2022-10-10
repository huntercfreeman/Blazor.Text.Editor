using Fluxor;

namespace BlazorTextEditor.ClassLib.Store.TextEditorCase;

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
            TextEditorList = nextList
        };
    }
    
    [ReducerMethod]
    public static TextEditorStates ReduceEditTextEditorBaseAction(
        TextEditorStates previousTextEditorStates,
        EditTextEditorBaseAction editTextEditorBaseAction)
    {
        var textEditor = previousTextEditorStates.TextEditorList
            .Single(x => x.Key == editTextEditorBaseAction.TextEditorKey);

        var nextTextEditor = textEditor.PerformEditTextEditorAction(editTextEditorBaseAction);

        var nextList = previousTextEditorStates.TextEditorList
            .Replace(textEditor, nextTextEditor);

        return previousTextEditorStates with
        {
            TextEditorList = nextList
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
            TextEditorList = nextList
        };
    }
}