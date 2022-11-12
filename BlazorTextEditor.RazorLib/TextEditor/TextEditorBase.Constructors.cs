using BlazorTextEditor.RazorLib.Character;
using BlazorTextEditor.RazorLib.Decoration;
using BlazorTextEditor.RazorLib.Editing;
using BlazorTextEditor.RazorLib.Keyboard;
using BlazorTextEditor.RazorLib.Keymap;
using BlazorTextEditor.RazorLib.Lexing;
using BlazorTextEditor.RazorLib.Row;

namespace BlazorTextEditor.RazorLib.TextEditor;

public partial class TextEditorBase
{
    public TextEditorBase(
        string content,
        ILexer? lexer,
        IDecorationMapper? decorationMapper,
        ITextEditorKeymap? textEditorKeymap)
    {
        Lexer = lexer ?? new LexerDefault();
        DecorationMapper = decorationMapper ?? new DecorationMapperDefault();
        TextEditorKeymap = textEditorKeymap ?? new TextEditorKeymapDefault();
        
        SetContent(content);
    }

    public TextEditorBase(
        string content,
        ILexer? lexer,
        IDecorationMapper? decorationMapper,
        ITextEditorKeymap? textEditorKeymap,
        TextEditorKey key)
        : this(
            content,
            lexer,
            decorationMapper,
            textEditorKeymap)
    {
        Key = key;
    }
}