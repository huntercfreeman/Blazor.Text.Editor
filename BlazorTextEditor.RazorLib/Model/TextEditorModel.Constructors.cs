using BlazorTextEditor.RazorLib.Decoration;
using BlazorTextEditor.RazorLib.Keymap;
using BlazorTextEditor.RazorLib.Keymap.Default;
using BlazorTextEditor.RazorLib.Lexing;

namespace BlazorTextEditor.RazorLib.Model;

public partial class TextEditorModel
{
    public TextEditorModel(
        string resourceUri,
        DateTime resourceLastWriteTime,
        string fileExtension,
        string content,
        ILexer? lexer,
        IDecorationMapper? decorationMapper,
        ITextEditorKeymap? textEditorKeymap)
    {
        ResourceUri = resourceUri;
        ResourceLastWriteTime = resourceLastWriteTime;
        FileExtension = fileExtension;
        Lexer = lexer ?? new TextEditorLexerDefault();
        DecorationMapper = decorationMapper ?? new TextEditorDecorationMapperDefault();
        TextEditorKeymap = textEditorKeymap ?? new TextEditorKeymapDefault();
        
        SetContent(content);
    }

    public TextEditorModel(
        string resourceUri,
        DateTime resourceLastWriteTime,
        string fileExtension,
        string content,
        ILexer? lexer,
        IDecorationMapper? decorationMapper,
        ITextEditorKeymap? textEditorKeymap,
        TextEditorModelKey modelKey)
        : this(
            resourceUri,
            resourceLastWriteTime,
            fileExtension,
            content,
            lexer,
            decorationMapper,
            textEditorKeymap)
    {
        ModelKey = modelKey;
    }

    /// <summary>
    /// Clone the TextEditorModel using shallow copy
    /// so that Fluxor will notify all the <see cref="TextEditorView"/>
    /// of the <see cref="TextEditorModel"/> having been replaced
    /// <br/><br/>
    /// Do not use a record would that do a deep value comparison
    /// and be incredibly slow? (i.e.) compare every RichCharacter in the list.
    /// </summary>
    public TextEditorModel(TextEditorModel original)
    {
        ResourceUri = original.ResourceUri;
        ResourceLastWriteTime = original.ResourceLastWriteTime;
        FileExtension = original.FileExtension;
        _content = original._content;
        _editBlocksPersisted = original._editBlocksPersisted;
        _rowEndingKindCounts = original._rowEndingKindCounts;
        _rowEndingPositions = original._rowEndingPositions;
        _tabKeyPositions = original._tabKeyPositions;
        ModelKey = original.ModelKey;

        OnlyRowEndingKind = original.OnlyRowEndingKind;
        UsingRowEndingKind = original.UsingRowEndingKind;
        Lexer = original.Lexer;
        DecorationMapper = original.DecorationMapper;
        TextEditorKeymap = original.TextEditorKeymap;
        EditBlockIndex = original.EditBlockIndex;
        MostCharactersOnASingleRowTuple = original.MostCharactersOnASingleRowTuple;
        TextEditorOptions = original.TextEditorOptions;
    }
}