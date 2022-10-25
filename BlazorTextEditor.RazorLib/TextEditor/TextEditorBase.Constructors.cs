using BlazorTextEditor.RazorLib.Character;
using BlazorTextEditor.RazorLib.Decoration;
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
        var rowIndex = 0;
        var previousCharacter = '\0';

        var charactersOnRow = 0;

        var carriageReturnCount = 0;
        var linefeedCount = 0;
        var carriageReturnLinefeedCount = 0;
        
        for (var index = 0; index < content.Length; index++)
        {
            var character = content[index];

            charactersOnRow++;

            if (character == KeyboardKeyFacts.WhitespaceCharacters.CARRIAGE_RETURN)
            {
                _rowEndingPositions.Add((index + 1, RowEndingKind.CarriageReturn));
                rowIndex++;

                if (charactersOnRow > MostCharactersOnASingleRow)
                {
                    MostCharactersOnASingleRow = charactersOnRow;
                }

                charactersOnRow = 0;
                
                carriageReturnCount++;
            }
            else if (character == KeyboardKeyFacts.WhitespaceCharacters.NEW_LINE)
            {
                if (previousCharacter == KeyboardKeyFacts.WhitespaceCharacters.CARRIAGE_RETURN)
                {
                    var lineEnding = _rowEndingPositions[rowIndex - 1];

                    _rowEndingPositions[rowIndex - 1] =
                        (lineEnding.positionIndex + 1, RowEndingKind.CarriageReturnLinefeed);
                    
                    carriageReturnCount--;
                    carriageReturnLinefeedCount++;
                }
                else
                {
                    _rowEndingPositions.Add((index + 1, RowEndingKind.Linefeed));
                    rowIndex++;
                    
                    if (charactersOnRow > MostCharactersOnASingleRow)
                    {
                        MostCharactersOnASingleRow = charactersOnRow;
                    }

                    charactersOnRow = 0;
                    
                    linefeedCount++;
                }
            }

            if (character == KeyboardKeyFacts.WhitespaceCharacters.TAB)
                _tabKeyPositions.Add(index);

            previousCharacter = character;

            _content.Add(new RichCharacter
            {
                Value = character,
                DecorationByte = default
            });
        }

        _rowEndingKindCounts.AddRange(new List<(RowEndingKind rowEndingKind, int count)>
        {
            (RowEndingKind.CarriageReturn, carriageReturnCount),
            (RowEndingKind.Linefeed, linefeedCount),
            (RowEndingKind.CarriageReturnLinefeed, carriageReturnLinefeedCount),
        });

        CheckRowEndingPositions(true);
        
        _rowEndingPositions.Add((content.Length, RowEndingKind.EndOfFile));
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