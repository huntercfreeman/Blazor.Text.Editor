using BlazorTextEditor.RazorLib.Find;
using BlazorTextEditor.RazorLib.Store.Find;
using Fluxor;

namespace BlazorTextEditor.RazorLib;

public partial interface ITextEditorService
{
    public interface IFindProviderApi
    {
        public void Register(ITextEditorFindProvider findProvider);
        public void DisposeAction(TextEditorFindProviderKey findProviderKey);
        public void SetActiveFindProvider(TextEditorFindProviderKey findProviderKey);
        public ITextEditorFindProvider? FindOrDefault(TextEditorFindProviderKey findProviderKey);
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
            TextEditorFindProviderKey findProviderKey)
        {
            _dispatcher.Dispatch(
                new TextEditorFindProviderState.DisposeAction(
                    findProviderKey));
        }

        public ITextEditorFindProvider? FindOrDefault(
            TextEditorFindProviderKey findProviderKey)
        {
            return _textEditorService.FindProviderState.Value.FindProvidersList
                .FirstOrDefault(x => x.FindProviderKey == findProviderKey);
        }

        public void Register(
            ITextEditorFindProvider findProvider)
        {
            _dispatcher.Dispatch(
                new TextEditorFindProviderState.RegisterAction(
                    findProvider));
        }

        public void SetActiveFindProvider(
            TextEditorFindProviderKey findProviderKey)
        {
            _dispatcher.Dispatch(
                new TextEditorFindProviderState.SetActiveFindProviderAction(
                    findProviderKey));
        }
    }
}
