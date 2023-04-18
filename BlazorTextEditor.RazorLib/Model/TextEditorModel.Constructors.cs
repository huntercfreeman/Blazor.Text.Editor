using BlazorTextEditor.RazorLib.Character;
using BlazorTextEditor.RazorLib.Decoration;
using BlazorTextEditor.RazorLib.Keymap;
using BlazorTextEditor.RazorLib.Keymap.Default;
using BlazorTextEditor.RazorLib.Lexing;
using BlazorTextEditor.RazorLib.Semantics;

namespace BlazorTextEditor.RazorLib.Model;

/// <summary>Stores the <see cref="RichCharacter"/> class instances that represent the text.<br/><br/>Each TextEditorModel has a unique underlying resource uri.<br/><br/>Therefore, if one has a text file named "myHomework.txt", then only one TextEditorModel can exist with the resource uri of "myHomework.txt".</summary>
public partial class TextEditorModel
{
    public TextEditorModel(
        string resourceUri,
        DateTime resourceLastWriteTime,
        string fileExtension,
        string content,
        ILexer? lexer,
        IDecorationMapper? decorationMapper,
        ISemanticModel? semanticModel,
        ITextEditorKeymap? textEditorKeymap)
    {
        ResourceUri = resourceUri;
        ResourceLastWriteTime = resourceLastWriteTime;
        FileExtension = fileExtension;
        Lexer = lexer ?? new TextEditorLexerDefault();
        DecorationMapper = decorationMapper ?? new TextEditorDecorationMapperDefault();
        SemanticModel = semanticModel ?? new SemanticModelDefault();
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
        ISemanticModel? semanticModel,
        ITextEditorKeymap? textEditorKeymap,
        TextEditorModelKey modelKey)
        : this(
            resourceUri,
            resourceLastWriteTime,
            fileExtension,
            content,
            lexer,
            decorationMapper,
            semanticModel,
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