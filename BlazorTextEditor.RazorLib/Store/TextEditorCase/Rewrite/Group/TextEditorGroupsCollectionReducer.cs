using Fluxor;

namespace BlazorTextEditor.RazorLib.Store.TextEditorCase.Rewrite.Group;

public class TextEditorGroupsCollectionReducer
{
    [ReducerMethod]
    public static TextEditorGroupsCollection ReduceRegisterTextEditorGroupAction(
        TextEditorGroupsCollection previousTextEditorGroupsCollection,
        RegisterTextEditorGroupAction registerTextEditorGroupAction)
    {
        var existingTextEditorGroup = previousTextEditorGroupsCollection.GroupsList
            .FirstOrDefault(x =>
                x.TextEditorGroupKey == 
                registerTextEditorGroupAction.TextEditorGroup.TextEditorGroupKey);

        if (existingTextEditorGroup is not null)
            return previousTextEditorGroupsCollection;

        var nextList = previousTextEditorGroupsCollection.GroupsList
            .Add(registerTextEditorGroupAction.TextEditorGroup);

        return new TextEditorGroupsCollection
        {
            GroupsList = nextList
        };
    }
    
    [ReducerMethod]
    public static TextEditorGroupsCollection ReduceAddViewModelToGroupAction(
        TextEditorGroupsCollection previousTextEditorGroupsCollection,
        AddViewModelToGroupAction addViewModelToGroupAction)
    {
        var existingTextEditorGroup = previousTextEditorGroupsCollection.GroupsList
            .FirstOrDefault(x =>
                x.TextEditorGroupKey == 
                addViewModelToGroupAction.TextEditorGroupKey);

        if (existingTextEditorGroup is null)
            return previousTextEditorGroupsCollection;

        var nextViewModelKeysList = existingTextEditorGroup.ViewModelKeys.Add(
            addViewModelToGroupAction.TextEditorViewModelKey);

        var nextGroup = existingTextEditorGroup with
        {
            ViewModelKeys = nextViewModelKeysList
        };
        
        if (nextGroup.ViewModelKeys.Count == 1)
        {
            nextGroup = nextGroup with
            {
                ActiveTextEditorViewModelKey = addViewModelToGroupAction.TextEditorViewModelKey
            };  
        }

        var nextGroupList = previousTextEditorGroupsCollection.GroupsList.Replace(
            existingTextEditorGroup,
            nextGroup);

        return new TextEditorGroupsCollection
        {
            GroupsList = nextGroupList
        };
    }
}