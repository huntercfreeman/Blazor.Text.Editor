using BlazorTextEditor.RazorLib.Decoration;
using BlazorTextEditor.RazorLib.Keymap;
using BlazorTextEditor.RazorLib.Keymap.Default;
using BlazorTextEditor.RazorLib.Lexing;

namespace BlazorTextEditor.RazorLib.TextEditor;

public partial class TextEditorBase
{
    public TextEditorBase(
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

    public TextEditorBase(
        string resourceUri,
        DateTime resourceLastWriteTime,
        string fileExtension,
        string content,
        ILexer? lexer,
        IDecorationMapper? decorationMapper,
        ITextEditorKeymap? textEditorKeymap,
        TextEditorKey key)
        : this(
            resourceUri,
            resourceLastWriteTime,
            fileExtension,
            content,
            lexer,
            decorationMapper,
            textEditorKeymap)
    {
        Key = key;
    }

    /// <summary>
    /// Clone the TextEditorBase using shallow copy
    /// so that Fluxor will notify all the <see cref="TextEditorView"/>
    /// of the <see cref="TextEditorBase"/> having been replaced
    /// <br/><br/>
    /// Do not use a record would that do a deep value comparison
    /// and be incredibly slow? (i.e.) compare every RichCharacter in the list.
    /// </summary>
    public TextEditorBase(TextEditorBase original)
    {
        ResourceUri = original.ResourceUri;
        ResourceLastWriteTime = original.ResourceLastWriteTime;
        FileExtension = original.FileExtension;
        _content = original._content;
        _editBlocksPersisted = original._editBlocksPersisted;
        _rowEndingKindCounts = original._rowEndingKindCounts;
        _rowEndingPositions = original._rowEndingPositions;
        _tabKeyPositions = original._tabKeyPositions;
        Key = original.Key;

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