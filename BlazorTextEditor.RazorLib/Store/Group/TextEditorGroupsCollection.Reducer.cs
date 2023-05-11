using BlazorTextEditor.RazorLib.ViewModel;
using Fluxor;

namespace BlazorTextEditor.RazorLib.Store.Group;

public partial class TextEditorGroupsCollection
{
    private class Reducer
    {
        [ReducerMethod]
        public static TextEditorGroupsCollection ReduceRegisterTextEditorGroupAction(
            TextEditorGroupsCollection inGroupsCollection,
            RegisterAction registerAction)
        {
            var existingTextEditorGroup = inGroupsCollection.GroupsList
                .FirstOrDefault(x =>
                    x.GroupKey ==
                    registerAction.TextEditorGroup.GroupKey);

            if (existingTextEditorGroup is not null)
                return inGroupsCollection;

            var nextList = inGroupsCollection.GroupsList
                .Add(registerAction.TextEditorGroup);

            return new TextEditorGroupsCollection
            {
                GroupsList = nextList
            };
        }

        [ReducerMethod]
        public static TextEditorGroupsCollection ReduceAddViewModelToGroupAction(
            TextEditorGroupsCollection inGroupsCollection,
            AddViewModelToGroupAction addViewModelToGroupAction)
        {
            var existingTextEditorGroup = inGroupsCollection.GroupsList
                .FirstOrDefault(x =>
                    x.GroupKey ==
                    addViewModelToGroupAction.TextEditorGroupKey);

            if (existingTextEditorGroup is null)
                return inGroupsCollection;

            if (existingTextEditorGroup.ViewModelKeys.Contains(
                    addViewModelToGroupAction.TextEditorViewModelKey))
            {
                return inGroupsCollection;
            }

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
                    ActiveViewModelKey = addViewModelToGroupAction.TextEditorViewModelKey
                };
            }

            var nextGroupList = inGroupsCollection.GroupsList.Replace(
                existingTextEditorGroup,
                nextGroup);

            return new TextEditorGroupsCollection
            {
                GroupsList = nextGroupList
            };
        }

        [ReducerMethod]
        public static TextEditorGroupsCollection ReduceRemoveViewModelFromGroupAction(
            TextEditorGroupsCollection inGroupsCollection,
            RemoveViewModelFromGroupAction removeViewModelFromGroupAction)
        {
            var existingTextEditorGroup = inGroupsCollection.GroupsList
                .FirstOrDefault(x =>
                    x.GroupKey ==
                    removeViewModelFromGroupAction.TextEditorGroupKey);

            if (existingTextEditorGroup is null)
                return inGroupsCollection;

            var indexOfViewModelKeyToRemove = existingTextEditorGroup.ViewModelKeys.FindIndex(
                x =>
                    x == removeViewModelFromGroupAction.TextEditorViewModelKey);

            if (indexOfViewModelKeyToRemove == -1)
                return inGroupsCollection;

            var nextViewModelKeysList = existingTextEditorGroup.ViewModelKeys.Remove(
                removeViewModelFromGroupAction.TextEditorViewModelKey);

            // This variable is done for renaming
            var activeViewModelKeyIndex = indexOfViewModelKeyToRemove;

            // If last item in list
            if (activeViewModelKeyIndex >= existingTextEditorGroup.ViewModelKeys.Count - 1)
            {
                activeViewModelKeyIndex--;
            }
            else
            {
                // ++ operation because nothing yet has been removed.
                // The new active TextEditor is set prior to actually removing the current active TextEditor.
                activeViewModelKeyIndex++;
            }

            TextEditorViewModelKey nextActiveTextEditorModelKey;

            // If removing the active will result in empty list set the active as an Empty TextEditorViewModelKey
            if (existingTextEditorGroup.ViewModelKeys.Count - 1 == 0)
                nextActiveTextEditorModelKey = TextEditorViewModelKey.Empty;
            else
                nextActiveTextEditorModelKey = existingTextEditorGroup.ViewModelKeys[activeViewModelKeyIndex];

            var nextGroup = existingTextEditorGroup with
            {
                ViewModelKeys = nextViewModelKeysList,
                ActiveViewModelKey = nextActiveTextEditorModelKey
            };

            var nextGroupList = inGroupsCollection.GroupsList.Replace(
                existingTextEditorGroup,
                nextGroup);

            return new TextEditorGroupsCollection
            {
                GroupsList = nextGroupList
            };
        }

        [ReducerMethod]
        public static TextEditorGroupsCollection ReduceSetActiveViewModelOfGroupAction(
            TextEditorGroupsCollection inGroupsCollection,
            SetActiveViewModelOfGroupAction setActiveViewModelOfGroupAction)
        {
            var existingTextEditorGroup = inGroupsCollection.GroupsList
                .FirstOrDefault(x =>
                    x.GroupKey ==
                    setActiveViewModelOfGroupAction.TextEditorGroupKey);

            if (existingTextEditorGroup is null)
                return inGroupsCollection;

            var nextGroup = existingTextEditorGroup with
            {
                ActiveViewModelKey = setActiveViewModelOfGroupAction.TextEditorViewModelKey
            };

            var nextGroupList = inGroupsCollection.GroupsList.Replace(
                existingTextEditorGroup,
                nextGroup);

            return new TextEditorGroupsCollection
            {
                GroupsList = nextGroupList
            };
        }
        
        [ReducerMethod]
        public static TextEditorGroupsCollection ReduceDisposeAction(
            TextEditorGroupsCollection inGroupsCollection,
            DisposeAction disposeAction)
        {
            var existingTextEditorGroup = inGroupsCollection.GroupsList
                .FirstOrDefault(x =>
                    x.GroupKey ==
                    disposeAction.TextEditorGroupKey);

            if (existingTextEditorGroup is null)
                return inGroupsCollection;

            var nextList = inGroupsCollection.GroupsList
                .Remove(existingTextEditorGroup);

            return new TextEditorGroupsCollection
            {
                GroupsList = nextList
            };
        }
    }
}