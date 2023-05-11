using BlazorTextEditor.RazorLib.Find;
using BlazorTextEditor.RazorLib.Group;
using BlazorTextEditor.RazorLib.Model;
using BlazorTextEditor.RazorLib.Store.Find;
using BlazorTextEditor.RazorLib.Store.Group;
using BlazorTextEditor.RazorLib.ViewModel;
using Fluxor;
using System.Collections.Immutable;

namespace BlazorTextEditor.RazorLib;

public partial interface ITextEditorService
{
    public interface IFindProviderApi
    {
        public void Register(ITextEditorFindProvider textEditorFindProvider);
        public void DisposeAction(TextEditorFindProviderKey textEditorFindProviderKey);
        public void SetActiveFindProvider(TextEditorFindProviderKey textEditorFindProviderKey);
        public ITextEditorFindProvider? FindOrDefault(TextEditorFindProviderKey textEditorFindProviderKey);
    }

    public class FindProviderApi : IFindProviderApi
    {
        private readonly ITextEditorService _textEditorService;
        private readonly IDispatcher _dispatcher;
        private readonly BlazorTextEditorOptions _blazorTextEditorOptions;

        public FindProviderApi(
            IDispatcher dispatcher,
            BlazorTextEditorOptions blazorTextEditorOptions,
            ITextEditorService textEditorService)
        {
            _dispatcher = dispatcher;
            _blazorTextEditorOptions = blazorTextEditorOptions;
            _textEditorService = textEditorService;
        }

        public void DisposeAction(
            TextEditorFindProviderKey textEditorFindProviderKey)
        {
            _dispatcher.Dispatch(
                new TextEditorFindProvidersCollection.DisposeAction(
                    textEditorFindProviderKey));
        }

        public ITextEditorFindProvider? FindOrDefault(
            TextEditorFindProviderKey textEditorFindProviderKey)
        {
            return _textEditorService.TextEditorFindProvidersCollectionWrap.Value.FindProvidersList
                .FirstOrDefault(x => x.FindProviderKey == textEditorFindProviderKey);
        }

        public void Register(
            ITextEditorFindProvider textEditorFindProvider)
        {
            _dispatcher.Dispatch(
                new TextEditorFindProvidersCollection.RegisterAction(
                    textEditorFindProvider));
        }

        public void SetActiveFindProvider(
            TextEditorFindProviderKey textEditorFindProviderKey)
        {
            _dispatcher.Dispatch(
                new TextEditorFindProvidersCollection.SetActiveFindProviderAction(
                    textEditorFindProviderKey));
        }
    }
}
