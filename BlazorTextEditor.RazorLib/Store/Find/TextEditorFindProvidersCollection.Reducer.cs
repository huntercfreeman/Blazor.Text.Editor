using BlazorCommon.RazorLib.Misc;
using BlazorTextEditor.RazorLib.Character;
using BlazorTextEditor.RazorLib.Find;
using BlazorTextEditor.RazorLib.Store.ViewModel;
using BlazorTextEditor.RazorLib.ViewModel;
using BlazorTextEditor.RazorLib.Virtualization;
using Fluxor;
using static BlazorTextEditor.RazorLib.Store.Find.TextEditorFindProvidersCollection;

namespace BlazorTextEditor.RazorLib.Store.Find;

public partial class TextEditorFindProvidersCollection
{
    private class Reducer
    {
        [ReducerMethod]
        public static TextEditorFindProvidersCollection ReduceRegisterAction(
            TextEditorFindProvidersCollection inFindProvidersCollection,
            RegisterAction registerAction)
        {
            var existingFindProvider = inFindProvidersCollection.FindProvidersList.FirstOrDefault(x =>
                x.FindProviderKey == registerAction.FindProvider.FindProviderKey);

            if (existingFindProvider is not null)
                return inFindProvidersCollection;

            var outFindProvidersList = inFindProvidersCollection.FindProvidersList
                .Add(registerAction.FindProvider);

            return new TextEditorFindProvidersCollection(
                outFindProvidersList,
                inFindProvidersCollection.ActiveTextEditorFindProviderKey);
        }

        [ReducerMethod]
        public static TextEditorFindProvidersCollection ReduceDisposeAction(
            TextEditorFindProvidersCollection inFindProvidersCollection,
            DisposeAction disposeAction)
        {
            var existingFindProvider = inFindProvidersCollection.FindProvidersList.FirstOrDefault(x =>
                x.FindProviderKey == disposeAction.FindProviderKey);

            if (existingFindProvider is null)
                return inFindProvidersCollection;

            var outFindProvidersList = inFindProvidersCollection.FindProvidersList
                .Remove(existingFindProvider);

            return new TextEditorFindProvidersCollection(
                outFindProvidersList,
                inFindProvidersCollection.ActiveTextEditorFindProviderKey);
        }

        [ReducerMethod]
        public static TextEditorFindProvidersCollection ReduceSetActiveFindProviderAction(
            TextEditorFindProvidersCollection inFindProvidersCollection,
            SetActiveFindProviderAction setActiveFindProviderAction)
        {
            return new TextEditorFindProvidersCollection(
                inFindProvidersCollection.FindProvidersList,
                setActiveFindProviderAction.FindProviderKey);
        }
    }
}
