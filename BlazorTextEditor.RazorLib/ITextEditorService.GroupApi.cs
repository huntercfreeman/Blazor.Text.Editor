using BlazorTextEditor.RazorLib.Group;
using BlazorTextEditor.RazorLib.Store.Group;
using BlazorTextEditor.RazorLib.ViewModel;
using Fluxor;
using System.Collections.Immutable;

namespace BlazorTextEditor.RazorLib;

public partial interface ITextEditorService
{
    public interface IGroupApi
    {
        public void AddViewModel(TextEditorGroupKey textEditorGroupKey, TextEditorViewModelKey textEditorViewModelKey);
        public TextEditorGroup? FindOrDefault(TextEditorGroupKey textEditorGroupKey);
        public void Register(TextEditorGroupKey textEditorGroupKey);
        public void Dispose(TextEditorGroupKey textEditorGroupKey);
        public void RemoveViewModel(TextEditorGroupKey textEditorGroupKey, TextEditorViewModelKey textEditorViewModelKey);
        public void SetActiveViewModel(TextEditorGroupKey textEditorGroupKey, TextEditorViewModelKey textEditorViewModelKey);
    }

    public class GroupApi : IGroupApi
    {
        private readonly ITextEditorService _textEditorService;
        private readonly IDispatcher _dispatcher;
        private readonly BlazorTextEditorOptions _blazorTextEditorOptions;

        public GroupApi(
            IDispatcher dispatcher,
            BlazorTextEditorOptions blazorTextEditorOptions,
            ITextEditorService textEditorService)
        {
            _dispatcher = dispatcher;
            _blazorTextEditorOptions = blazorTextEditorOptions;
            _textEditorService = textEditorService;
        }

        public void SetActiveViewModel(
            TextEditorGroupKey textEditorGroupKey,
            TextEditorViewModelKey textEditorViewModelKey)
        {
            _dispatcher.Dispatch(
                new TextEditorGroupsCollection.SetActiveViewModelOfGroupAction(
                    textEditorGroupKey,
                    textEditorViewModelKey));
        }

        public void RemoveViewModel(
            TextEditorGroupKey textEditorGroupKey,
            TextEditorViewModelKey textEditorViewModelKey)
        {
            _dispatcher.Dispatch(
                new TextEditorGroupsCollection.RemoveViewModelFromGroupAction(
                    textEditorGroupKey,
                    textEditorViewModelKey));
        }

        public void Register(
            TextEditorGroupKey textEditorGroupKey)
        {
            var textEditorGroup = new TextEditorGroup(
                textEditorGroupKey,
                TextEditorViewModelKey.Empty,
                ImmutableList<TextEditorViewModelKey>.Empty);

            _dispatcher.Dispatch(
                new TextEditorGroupsCollection.RegisterAction(
                    textEditorGroup));
        }
        
        public void Dispose(
            TextEditorGroupKey textEditorGroupKey)
        {
            _dispatcher.Dispatch(
                new TextEditorGroupsCollection.DisposeAction(
                    textEditorGroupKey));
        }

        public TextEditorGroup? FindOrDefault(
            TextEditorGroupKey textEditorGroupKey)
        {
            return _textEditorService.GroupsCollectionWrap.Value.GroupsList
                .FirstOrDefault(x =>
                    x.GroupKey == textEditorGroupKey);
        }

        public void AddViewModel(
            TextEditorGroupKey textEditorGroupKey,
            TextEditorViewModelKey textEditorViewModelKey)
        {
            _dispatcher.Dispatch(
                new TextEditorGroupsCollection.AddViewModelToGroupAction(
                    textEditorGroupKey,
                    textEditorViewModelKey));
        }
    }
}
