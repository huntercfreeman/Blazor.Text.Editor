using BlazorTextEditor.RazorLib.Store.TextEditorCase.ViewModel;
using BlazorTextEditor.RazorLib.ViewModel;
using Fluxor;

namespace BlazorTextEditor.RazorLib.Store.TextEditorCase.Group;

public partial class TextEditorGroupsCollection
{
    private class Reducer
    {
        [ReducerMethod]
        public static TextEditorGroupsCollection ReduceRegisterTextEditorGroupAction(
            TextEditorGroupsCollection previousTextEditorGroupsCollection,
            RegisterGroupAction registerGroupAction)
        {
            var existingTextEditorGroup = previousTextEditorGroupsCollection.GroupsList
                .FirstOrDefault(x =>
                    x.TextEditorGroupKey ==
                    registerGroupAction.TextEditorGroup.TextEditorGroupKey);

            if (existingTextEditorGroup is not null)
                return previousTextEditorGroupsCollection;

            var nextList = previousTextEditorGroupsCollection.GroupsList
                .Add(registerGroupAction.TextEditorGroup);

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

            if (existingTextEditorGroup.ViewModelKeys.Contains(
                    addViewModelToGroupAction.TextEditorViewModelKey))
            {
                return previousTextEditorGroupsCollection;
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

        [ReducerMethod]
        public static TextEditorGroupsCollection ReduceRemoveViewModelFromGroupAction(
            TextEditorGroupsCollection previousTextEditorGroupsCollection,
            RemoveViewModelFromGroupAction removeViewModelFromGroupAction)
        {
            var existingTextEditorGroup = previousTextEditorGroupsCollection.GroupsList
                .FirstOrDefault(x =>
                    x.TextEditorGroupKey ==
                    removeViewModelFromGroupAction.TextEditorGroupKey);

            if (existingTextEditorGroup is null)
                return previousTextEditorGroupsCollection;

            var indexOfViewModelKeyToRemove = existingTextEditorGroup.ViewModelKeys.FindIndex(
                x =>
                    x == removeViewModelFromGroupAction.TextEditorViewModelKey);

            if (indexOfViewModelKeyToRemove == -1)
                return previousTextEditorGroupsCollection;

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

            TextEditorViewModelKey nextActiveTextEditorKey;

            // If removing the active will result in empty list set the active as an Empty TextEditorViewModelKey
            if (existingTextEditorGroup.ViewModelKeys.Count - 1 == 0)
                nextActiveTextEditorKey = TextEditorViewModelKey.Empty;
            else
                nextActiveTextEditorKey = existingTextEditorGroup.ViewModelKeys[activeViewModelKeyIndex];

            var nextGroup = existingTextEditorGroup with
            {
                ViewModelKeys = nextViewModelKeysList,
                ActiveTextEditorViewModelKey = nextActiveTextEditorKey
            };

            var nextGroupList = previousTextEditorGroupsCollection.GroupsList.Replace(
                existingTextEditorGroup,
                nextGroup);

            return new TextEditorGroupsCollection
            {
                GroupsList = nextGroupList
            };
        }

        [ReducerMethod]
        public static TextEditorGroupsCollection ReduceSetActiveViewModelOfGroupAction(
            TextEditorGroupsCollection previousTextEditorGroupsCollection,
            SetActiveViewModelOfGroupAction setActiveViewModelOfGroupAction)
        {
            var existingTextEditorGroup = previousTextEditorGroupsCollection.GroupsList
                .FirstOrDefault(x =>
                    x.TextEditorGroupKey ==
                    setActiveViewModelOfGroupAction.TextEditorGroupKey);

            if (existingTextEditorGroup is null)
                return previousTextEditorGroupsCollection;

            var nextGroup = existingTextEditorGroup with
            {
                ActiveTextEditorViewModelKey = setActiveViewModelOfGroupAction.TextEditorViewModelKey
            };

            var nextGroupList = previousTextEditorGroupsCollection.GroupsList.Replace(
                existingTextEditorGroup,
                nextGroup);

            return new TextEditorGroupsCollection
            {
                GroupsList = nextGroupList
            };
        }
    }
}