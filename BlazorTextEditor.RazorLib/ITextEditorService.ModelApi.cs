using BlazorTextEditor.RazorLib.Analysis.CSharp.SyntaxActors;
using BlazorTextEditor.RazorLib.Analysis.Css.Decoration;
using BlazorTextEditor.RazorLib.Analysis.Css.SyntaxActors;
using BlazorTextEditor.RazorLib.Analysis.FSharp.SyntaxActors;
using BlazorTextEditor.RazorLib.Analysis.GenericLexer.Decoration;
using BlazorTextEditor.RazorLib.Analysis.Html.Decoration;
using BlazorTextEditor.RazorLib.Analysis.Html.SyntaxActors;
using BlazorTextEditor.RazorLib.Analysis.JavaScript.SyntaxActors;
using BlazorTextEditor.RazorLib.Analysis.Json.Decoration;
using BlazorTextEditor.RazorLib.Analysis.Json.SyntaxActors;
using BlazorTextEditor.RazorLib.Analysis.Razor.SyntaxActors;
using BlazorTextEditor.RazorLib.Analysis.TypeScript.SyntaxActors;
using BlazorTextEditor.RazorLib.Decoration;
using BlazorTextEditor.RazorLib.Lexing;
using BlazorTextEditor.RazorLib.Model;
using BlazorTextEditor.RazorLib.Row;
using BlazorTextEditor.RazorLib.Store.Model;
using BlazorTextEditor.RazorLib.ViewModel;
using Fluxor;
using System.Collections.Immutable;

namespace BlazorTextEditor.RazorLib;

public partial interface ITextEditorService
{
    public interface IModelApi
    {
        public void DeleteTextByMotion(TextEditorModelsCollection.DeleteTextByMotionAction deleteTextByMotionAction);
        public void DeleteTextByRange(TextEditorModelsCollection.DeleteTextByRangeAction deleteTextByRangeAction);
        public void Dispose(TextEditorModelKey textEditorModelKey);
        public TextEditorModel? ModelFindOrDefault(TextEditorModelKey textEditorModelKey);
        public string? ModelGetAllText(TextEditorModelKey textEditorModelKey);
        public ImmutableArray<TextEditorViewModel> ModelGetViewModelsOrEmpty(TextEditorModelKey textEditorModelKey);
        public void ModelHandleKeyboardEvent(TextEditorModelsCollection.KeyboardEventAction keyboardEventAction);
        public void ModelInsertText(TextEditorModelsCollection.InsertTextAction insertTextAction);
        public void ModelRedoEdit(TextEditorModelKey textEditorModelKey);
        /// <summary>It is recommended to use the <see cref="ModelRegisterTemplatedModel" /> method as it will internally reference the <see cref="ILexer" /> and <see cref="IDecorationMapper" /> that correspond to the desired text editor.</summary>
        public void ModelRegisterCustomModel(TextEditorModel model);
        /// <summary>As an example, for a C# Text Editor one would pass in a <see cref="WellKnownModelKind" /> of <see cref="WellKnownModelKind.CSharp" />.<br /><br />For a Plain Text Editor one would pass in a <see cref="WellKnownModelKind" /> of <see cref="WellKnownModelKind.Plain" />.</summary>
        public void ModelRegisterTemplatedModel(TextEditorModelKey textEditorModelKey, WellKnownModelKind wellKnownModelKind, string resourceUri, DateTime resourceLastWriteTime, string fileExtension, string initialContent);
        public void ModelReload(TextEditorModelKey textEditorModelKey, string content, DateTime resourceLastWriteTime);
        public void ModelSetResourceData(TextEditorModelKey textEditorModelKey, string resourceUri, DateTime resourceLastWriteTime);
        public void ModelSetUsingRowEndingKind(TextEditorModelKey textEditorModelKey, RowEndingKind rowEndingKind);
        public void ModelUndoEdit(TextEditorModelKey textEditorModelKey);
        public TextEditorModel? FindOrDefaultByResourceUri(string resourceUri);
    }

    public class ModelApi : IModelApi
    {
        private readonly ITextEditorService _textEditorService;
        private readonly IDispatcher _dispatcher;

        public ModelApi(
            IDispatcher dispatcher,
            ITextEditorService textEditorService)
        {
            _dispatcher = dispatcher;
            _textEditorService = textEditorService;
        }

        public TextEditorModel? FindOrDefaultByResourceUri(
            string resourceUri)
        {
            return _textEditorService.ModelsCollectionWrap.Value.TextEditorList
                .FirstOrDefault(x =>
                    x.ResourceUri == resourceUri);
        }

        public void ModelUndoEdit(
            TextEditorModelKey textEditorModelKey)
        {
            var undoEditAction = new TextEditorModelsCollection.UndoEditAction(
                textEditorModelKey);

            _dispatcher.Dispatch(undoEditAction);
        }

        public void ModelSetUsingRowEndingKind(
            TextEditorModelKey textEditorModelKey,
            RowEndingKind rowEndingKind)
        {
            _dispatcher.Dispatch(
                new TextEditorModelsCollection.SetUsingRowEndingKindAction(
                    textEditorModelKey,
                    rowEndingKind));
        }

        public void ModelSetResourceData(
            TextEditorModelKey textEditorModelKey,
            string resourceUri,
            DateTime resourceLastWriteTime)
        {
            _dispatcher.Dispatch(
                new TextEditorModelsCollection.SetResourceDataAction(
                    textEditorModelKey,
                    resourceUri,
                    resourceLastWriteTime));
        }

        public void ModelReload(
            TextEditorModelKey textEditorModelKey,
            string content,
            DateTime resourceLastWriteTime)
        {
            _dispatcher.Dispatch(
                new TextEditorModelsCollection.ReloadAction(
                    textEditorModelKey,
                    content,
                    resourceLastWriteTime));
        }

        public void ModelRegisterTemplatedModel(
            TextEditorModelKey textEditorModelKey,
            WellKnownModelKind wellKnownModelKind,
            string resourceUri,
            DateTime resourceLastWriteTime,
            string fileExtension,
            string initialContent)
        {
            ILexer? lexer = null;
            IDecorationMapper? decorationMapper = null;

            switch (wellKnownModelKind)
            {
                case WellKnownModelKind.CSharp:
                    lexer = new TextEditorCSharpLexer();
                    decorationMapper = new GenericDecorationMapper();
                    break;
                case WellKnownModelKind.Html:
                    lexer = new TextEditorHtmlLexer();
                    decorationMapper = new TextEditorHtmlDecorationMapper();
                    break;
                case WellKnownModelKind.Css:
                    lexer = new TextEditorCssLexer();
                    decorationMapper = new TextEditorCssDecorationMapper();
                    break;
                case WellKnownModelKind.Json:
                    lexer = new TextEditorJsonLexer();
                    decorationMapper = new TextEditorJsonDecorationMapper();
                    break;
                case WellKnownModelKind.FSharp:
                    lexer = new TextEditorFSharpLexer();
                    decorationMapper = new GenericDecorationMapper();
                    break;
                case WellKnownModelKind.Razor:
                    lexer = new TextEditorRazorLexer();
                    decorationMapper = new TextEditorHtmlDecorationMapper();
                    break;
                case WellKnownModelKind.JavaScript:
                    lexer = new TextEditorJavaScriptLexer();
                    decorationMapper = new GenericDecorationMapper();
                    break;
                case WellKnownModelKind.TypeScript:
                    lexer = new TextEditorTypeScriptLexer();
                    decorationMapper = new GenericDecorationMapper();
                    break;
            }

            var textEditorModel = new TextEditorModel(
                resourceUri,
                resourceLastWriteTime,
                fileExtension,
                initialContent,
                lexer,
                decorationMapper,
                null,
                null,
                textEditorModelKey);

            // IBackgroundTaskQueue does not work well here because
            // this Task does not need to be tracked.
            _ = Task.Run(async () =>
            {
                try
                {
                    await textEditorModel.ApplySyntaxHighlightingAsync();

                    _dispatcher.Dispatch(
                        new TextEditorModelsCollection.ForceRerenderAction(
                            textEditorModel.ModelKey));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }, CancellationToken.None);

            _dispatcher.Dispatch(
                new TextEditorModelsCollection.RegisterAction(
                    textEditorModel));
        }

        public void ModelRegisterCustomModel(
            TextEditorModel model)
        {
            _dispatcher.Dispatch(
                new TextEditorModelsCollection.RegisterAction(
                    model));
        }

        public void ModelRedoEdit(
            TextEditorModelKey textEditorModelKey)
        {
            var redoEditAction = new TextEditorModelsCollection.RedoEditAction(
                textEditorModelKey);

            _dispatcher.Dispatch(redoEditAction);
        }

        public void ModelInsertText(
            TextEditorModelsCollection.InsertTextAction insertTextAction)
        {
            _dispatcher.Dispatch(insertTextAction);
        }

        public void ModelHandleKeyboardEvent(
            TextEditorModelsCollection.KeyboardEventAction keyboardEventAction)
        {
            _dispatcher.Dispatch(keyboardEventAction);
        }

        public ImmutableArray<TextEditorViewModel> ModelGetViewModelsOrEmpty(
            TextEditorModelKey textEditorModelKey)
        {
            return _textEditorService.ViewModelsCollectionWrap.Value.ViewModelsList
                .Where(x => x.ModelKey == textEditorModelKey)
                .ToImmutableArray();
        }

        public string? ModelGetAllText(
            TextEditorModelKey textEditorModelKey)
        {
            return _textEditorService.ModelsCollectionWrap.Value.TextEditorList
                .FirstOrDefault(x => x.ModelKey == textEditorModelKey)
                ?.GetAllText();
        }

        public TextEditorModel? ModelFindOrDefault(
            TextEditorModelKey textEditorModelKey)
        {
            return _textEditorService.ModelsCollectionWrap.Value.TextEditorList
                .FirstOrDefault(x => x.ModelKey == textEditorModelKey);
        }

        public void Dispose(
            TextEditorModelKey textEditorModelKey)
        {
            _dispatcher.Dispatch(
                new TextEditorModelsCollection.DisposeAction(
                    textEditorModelKey));
        }

        public void DeleteTextByRange(
            TextEditorModelsCollection.DeleteTextByRangeAction deleteTextByRangeAction)
        {
            _dispatcher.Dispatch(deleteTextByRangeAction);
        }

        public void DeleteTextByMotion(
            TextEditorModelsCollection.DeleteTextByMotionAction deleteTextByMotionAction)
        {
            _dispatcher.Dispatch(deleteTextByMotionAction);
        }
    }
}
