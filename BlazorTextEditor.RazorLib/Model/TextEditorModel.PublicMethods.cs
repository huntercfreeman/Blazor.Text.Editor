using System.Collections.Immutable;
using BlazorCommon.RazorLib.Keyboard;
using BlazorTextEditor.RazorLib.Analysis;
using BlazorTextEditor.RazorLib.Character;
using BlazorTextEditor.RazorLib.Cursor;
using BlazorTextEditor.RazorLib.Decoration;
using BlazorTextEditor.RazorLib.Editing;
using BlazorTextEditor.RazorLib.Lexing;
using BlazorTextEditor.RazorLib.Row;
using BlazorTextEditor.RazorLib.Store;
using BlazorTextEditor.RazorLib.Store.Model;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorTextEditor.RazorLib.Model;

public partial class TextEditorModel
{
    /// <summary>
    /// The cursor is a separate element
    /// and at times will try to access out of bounds locations.
    /// <br/><br/>
    /// When cursor accesses out of bounds location
    /// return largest available RowIndex and
    /// largest available ColumnIndex
    /// </summary>
    public (int positionIndex, RowEndingKind rowEndingKind) GetStartOfRowTuple(int rowIndex)
    {
        if (rowIndex > _rowEndingPositions.Count - 1)
            rowIndex = _rowEndingPositions.Count - 1;
        
        if (rowIndex > 0)
            return _rowEndingPositions[rowIndex - 1];
        
        return (0, RowEndingKind.StartOfFile);
    }

    /// <summary>
    ///     Returns the Length of a row however it does not include the line ending characters by default.
    ///     To include line ending characters the parameter <see cref="includeLineEndingCharacters" /> must be true.
    /// </summary>
    public int GetLengthOfRow(
        int rowIndex,
        bool includeLineEndingCharacters = false)
    {
        if (!_rowEndingPositions.Any())
            return 0;
        
        if (rowIndex > _rowEndingPositions.Count - 1)
            rowIndex = _rowEndingPositions.Count - 1;
        
        if (rowIndex < 0)
            rowIndex = 0;
        
        var startOfRowTupleInclusive = GetStartOfRowTuple(rowIndex);

        // TODO: Index was out of range exception on 2023-04-18
        var endOfRowTupleExclusive = _rowEndingPositions[rowIndex];

        var lengthOfRowWithLineEndings = endOfRowTupleExclusive.positionIndex
                                         - startOfRowTupleInclusive.positionIndex;

        if (includeLineEndingCharacters)
            return lengthOfRowWithLineEndings;

        return lengthOfRowWithLineEndings - endOfRowTupleExclusive.rowEndingKind.AsCharacters().Length;
    }

    /// <param name="startingRowIndex">The starting index of the rows to return</param>
    /// <param name="count">count of 0 returns 0 rows. count of 1 returns the startingRowIndex.</param>
    public List<List<RichCharacter>> GetRows(int startingRowIndex, int count)
    {
        var rowCountAvailable = _rowEndingPositions.Count - startingRowIndex;

        var rowCountToReturn = count < rowCountAvailable
            ? count
            : rowCountAvailable;

        var endingRowIndexExclusive = startingRowIndex + rowCountToReturn;

        var rows = new List<List<RichCharacter>>();

        for (var i = startingRowIndex;
             i < endingRowIndexExclusive;
             i++)
        {
            // Previous row's line ending position is this row's start.
            var startOfRowInclusive = GetStartOfRowTuple(i)
                .positionIndex;

            var endOfRowExclusive = _rowEndingPositions[i].positionIndex;

            var row = _content
                .Skip(startOfRowInclusive)
                .Take(endOfRowExclusive - startOfRowInclusive)
                .ToList();

            rows.Add(row);
        }

        return rows;
    }

    public int GetTabsCountOnSameRowBeforeCursor(int rowIndex, int columnIndex)
    {
        var startOfRowPositionIndex = GetStartOfRowTuple(rowIndex)
            .positionIndex;

        var tabs = _tabKeyPositions
            .SkipWhile(positionIndex => positionIndex < startOfRowPositionIndex)
            .TakeWhile(positionIndex => positionIndex < startOfRowPositionIndex + columnIndex);

        return tabs.Count();
    }

    public TextEditorModel PerformForceRerenderAction(TextEditorModelsCollection.ForceRerenderAction forceRerenderAction)
    {
        return new TextEditorModel(this);
    }

    public TextEditorModel PerformEditTextEditorAction(
        TextEditorModelsCollection.KeyboardEventAction keyboardEventAction)
    {
        if (KeyboardKeyFacts.IsMetaKey(keyboardEventAction.KeyboardEventArgs))
        {
            if (KeyboardKeyFacts.MetaKeys.BACKSPACE == keyboardEventAction.KeyboardEventArgs.Key ||
                KeyboardKeyFacts.MetaKeys.DELETE == keyboardEventAction.KeyboardEventArgs.Key)
                PerformDeletions(keyboardEventAction);
        }
        else
        {
            var cursorSnapshots = TextEditorCursorSnapshot
                .TakeSnapshots(
                    keyboardEventAction.CursorSnapshots
                        .Select(x => x.UserCursor)
                        .ToArray())
                .ToImmutableArray();

            var primaryCursorSnapshot = cursorSnapshots
                .FirstOrDefault(x =>
                    x.UserCursor.IsPrimaryCursor);

            if (primaryCursorSnapshot is null)
                return new TextEditorModel(this);

            /*
             * TODO: 2022-11-18 one must not use the UserCursor while
             * calculating but instead make a copy of the mutable cursor
             * by looking at the snapshot and mutate that local 'userCursor'
             * then once the transaction is done offer the 'userCursor' to the
             * user interface such that it can choose to move the actual user cursor
             * to that position
             */

            // TODO: start making a mutable copy of their immutable cursor snapshot
            // so if the user moves the cursor
            // while calculating nothing can go wrong causing exception
            //
            // See the var localCursor in this contiguous code block.
            //
            // var localCursor = new TextEditorCursor(
            //     (primaryCursorSnapshot.ImmutableCursor.RowIndex, primaryCursorSnapshot.ImmutableCursor.ColumnIndex), 
            //     true);

            if (TextEditorSelectionHelper.HasSelectedText(
                    primaryCursorSnapshot
                        .ImmutableCursor.ImmutableTextEditorSelection))
            {
                PerformDeletions(new TextEditorModelsCollection.KeyboardEventAction(
                    keyboardEventAction.TextEditorModelKey,
                    cursorSnapshots,
                    new KeyboardEventArgs
                    {
                        Code = KeyboardKeyFacts.MetaKeys.DELETE,
                        Key = KeyboardKeyFacts.MetaKeys.DELETE,
                    },
                    CancellationToken.None));
            }
            
            var innerCursorSnapshots = TextEditorCursorSnapshot
                .TakeSnapshots(
                    keyboardEventAction.CursorSnapshots
                        .Select(x => x.UserCursor)
                        .ToArray())
                .ToImmutableArray();
            
            PerformInsertions(keyboardEventAction with
            {
                CursorSnapshots = innerCursorSnapshots
            });
        }

        return new TextEditorModel(this);
    }

    public TextEditorModel PerformEditTextEditorAction(TextEditorModelsCollection.InsertTextAction insertTextAction)
    {
        var cursorSnapshots = TextEditorCursorSnapshot
            .TakeSnapshots(
                insertTextAction.CursorSnapshots
                    .Select(x => x.UserCursor)
                    .ToArray())
            .ToImmutableArray();

        var primaryCursorSnapshot = cursorSnapshots
            .FirstOrDefault(x =>
                x.UserCursor.IsPrimaryCursor);

        if (primaryCursorSnapshot is null)
            return new TextEditorModel(this);

        /*
         * TODO: 2022-11-18 one must not use the UserCursor while
         * calculating but instead make a copy of the mutable cursor
         * by looking at the snapshot and mutate that local 'userCursor'
         * then once the transaction is done offer the 'userCursor' to the
         * user interface such that it can choose to move the actual user cursor
         * to that position
         */

        // TODO: start making a mutable copy of their immutable cursor snapshot
        // so if the user moves the cursor
        // while calculating nothing can go wrong causing exception
        //
        // See the var localCursor in this contiguous code block.
        //
        // var localCursor = new TextEditorCursor(
        //     (primaryCursorSnapshot.ImmutableCursor.RowIndex, primaryCursorSnapshot.ImmutableCursor.ColumnIndex), 
        //     true);

        if (TextEditorSelectionHelper.HasSelectedText(
                primaryCursorSnapshot
                    .ImmutableCursor.ImmutableTextEditorSelection))
        {
            PerformDeletions(new TextEditorModelsCollection.KeyboardEventAction(
                insertTextAction.TextEditorModelKey,
                cursorSnapshots,
                new KeyboardEventArgs
                {
                    Code = KeyboardKeyFacts.MetaKeys.DELETE,
                    Key = KeyboardKeyFacts.MetaKeys.DELETE,
                },
                CancellationToken.None));
        }
        
        var localContent = insertTextAction.Content
            .Replace("\r\n", "\n");
        
        foreach (var character in localContent)
        {
            // TODO: This needs to be rewritten everything should be inserted at the same time not a foreach loop insertion for each character
            //
            // Need innerCursorSnapshots because need
            // after every loop of the foreach that the
            // cursor snapshots are updated
            var innerCursorSnapshots = TextEditorCursorSnapshot
                .TakeSnapshots(
                    insertTextAction.CursorSnapshots
                        .Select(x => x.UserCursor)
                        .ToArray())
                .ToImmutableArray();

            var code = character switch
            {
                '\r' => KeyboardKeyFacts.WhitespaceCodes.CARRIAGE_RETURN_CODE,
                '\n' => KeyboardKeyFacts.WhitespaceCodes.ENTER_CODE,
                '\t' => KeyboardKeyFacts.WhitespaceCodes.TAB_CODE,
                ' ' => KeyboardKeyFacts.WhitespaceCodes.SPACE_CODE,
                _ => character.ToString(),
            };

            var keyboardEventTextEditorModelAction =
                new TextEditorModelsCollection.KeyboardEventAction(
                    insertTextAction.TextEditorModelKey,
                    innerCursorSnapshots,
                    new KeyboardEventArgs
                    {
                        Code = code,
                        Key = character.ToString(),
                    },
                    CancellationToken.None);

            PerformEditTextEditorAction(keyboardEventTextEditorModelAction);
        }

        return new TextEditorModel(this);
    }

    public TextEditorModel PerformEditTextEditorAction(
        TextEditorModelsCollection.DeleteTextByMotionAction deleteTextByMotionAction)
    {
        var keyboardEventArgs = deleteTextByMotionAction.MotionKind switch
        {
            MotionKind.Backspace => new KeyboardEventArgs
            {
                Key = KeyboardKeyFacts.MetaKeys.BACKSPACE
            },
            MotionKind.Delete => new KeyboardEventArgs
            {
                Key = KeyboardKeyFacts.MetaKeys.DELETE
            },
            _ => throw new ApplicationException(
                $"The {nameof(MotionKind)}:" +
                $" {deleteTextByMotionAction.MotionKind}" +
                " was not recognized.")
        };

        var keyboardEventTextEditorModelAction =
            new TextEditorModelsCollection.KeyboardEventAction(
                deleteTextByMotionAction.TextEditorModelKey,
                deleteTextByMotionAction.CursorSnapshots,
                keyboardEventArgs,
                CancellationToken.None);

        PerformEditTextEditorAction(keyboardEventTextEditorModelAction);

        return new TextEditorModel(this);
    }

    public TextEditorModel PerformEditTextEditorAction(
        TextEditorModelsCollection.DeleteTextByRangeAction deleteTextByRangeAction)
    {
        // TODO: This needs to be rewritten everything should be deleted at the same time not a foreach loop for each character
        for (var i = 0; i < deleteTextByRangeAction.Count; i++)
        {
            // Need innerCursorSnapshots because need
            // after every loop of the foreach that the
            // cursor snapshots are updated
            var innerCursorSnapshots = TextEditorCursorSnapshot
                .TakeSnapshots(
                    deleteTextByRangeAction.CursorSnapshots
                        .Select(x => x.UserCursor)
                        .ToArray())
                .ToImmutableArray();

            var keyboardEventTextEditorModelAction =
                new TextEditorModelsCollection.KeyboardEventAction(
                    deleteTextByRangeAction.TextEditorModelKey,
                    innerCursorSnapshots,
                    new KeyboardEventArgs
                    {
                        Code = KeyboardKeyFacts.MetaKeys.DELETE,
                        Key = KeyboardKeyFacts.MetaKeys.DELETE,
                    },
                    CancellationToken.None);

            PerformEditTextEditorAction(keyboardEventTextEditorModelAction);
        }

        return new TextEditorModel(this);
    }

    /// <summary>
    ///     If applying syntax highlighting it may be preferred to use
    ///     <see cref="ApplySyntaxHighlightingAsync" />. It is effectively
    ///     just invoking the lexer and then <see cref="ApplyDecorationRange" />
    /// </summary>
    public void ApplyDecorationRange(IEnumerable<TextEditorTextSpan> textEditorTextSpans)
    {
        var positionsPainted = new HashSet<int>();
        
        foreach (var textEditorTextSpan in textEditorTextSpans)
        {
            for (var i = textEditorTextSpan.StartingIndexInclusive; i < textEditorTextSpan.EndingIndexExclusive; i++)
            {
                _content[i].DecorationByte = textEditorTextSpan.DecorationByte;

                positionsPainted.Add(i);
            }
        }

        for (var i = 0; i < _content.Count - 1; i++)
        {
            if (!positionsPainted.Contains(i))
            {
                // DecorationByte of 0 is to be 'None'
                _content[i].DecorationByte = 0;
            }
        }
    }

    public async Task ApplySyntaxHighlightingAsync()
    {
        var textEditorTextSpans = await Lexer.Lex(GetAllText());

        ApplyDecorationRange(textEditorTextSpans);
    }

    public string GetAllText()
    {
        return new string(_content
            .Select(rc => rc.Value)
            .ToArray());
    }

    public int GetCursorPositionIndex(TextEditorCursor textEditorCursor)
    {
        return GetPositionIndex(
            textEditorCursor.IndexCoordinates.rowIndex,
            textEditorCursor.IndexCoordinates.columnIndex);
    }
    
    public int GetCursorPositionIndex(ImmutableTextEditorCursor immutableTextEditorCursor)
    {
        return GetPositionIndex(
            immutableTextEditorCursor.RowIndex,
            immutableTextEditorCursor.ColumnIndex);
    }

    public int GetPositionIndex(int rowIndex, int columnIndex)
    {
        var startOfRowPositionIndex =
            GetStartOfRowTuple(rowIndex)
                .positionIndex;

        return startOfRowPositionIndex + columnIndex;
    }

    public char GetTextAt(int positionIndex)
    {
        if (positionIndex < 0 || 
            positionIndex >= _content.Count)
        {
            return ParserFacts.END_OF_FILE;
        }

        return _content[positionIndex].Value;
    }
    
    public string GetTextRange(int startingPositionIndex, int count)
    {
        return new string(_content
            .Skip(startingPositionIndex)
            .Take(count)
            .Select(rc => rc.Value)
            .ToArray());
    }
    
    public string GetLinesRange(int startingRowIndex, int count)
    {
        var startingPositionIndexInclusive = GetPositionIndex(
            startingRowIndex,
            0);

        var lastRowIndexExclusive = startingRowIndex + count;
        
        var endingPositionIndexExclusive = GetPositionIndex(
            lastRowIndexExclusive,
            0);

        return GetTextRange(
            startingPositionIndexInclusive,
            endingPositionIndexExclusive - startingPositionIndexInclusive);
    }
    
    /// <summary>
    /// Given a <see cref="TextEditorModel"/> with a preference for the right side
    /// of the cursor, the following conditional branch will play out.
    /// <br/><br/>
    /// -IF the cursor is amongst a word, that word will be returned.
    /// <br/><br/>
    /// -ELSE IF the start of the word is to the right of the cursor that word will be returned.
    /// <br/><br/>
    /// -ELSE IF the end of the word is to the left of the cursor that word will be returned.
    /// </summary>
    public TextEditorTextSpan? GetWordAt(int positionIndex)
    {
        var previousCharacter = GetTextAt(
            positionIndex - 1);
            
        var currentCharacter = GetTextAt(
            positionIndex);
        
        var previousCharacterKind = CharacterKindHelper.CharToCharacterKind(
            previousCharacter);
            
        var currentCharacterKind = CharacterKindHelper.CharToCharacterKind(
            currentCharacter);
        
        var rowInformation =
            FindRowInformation(positionIndex);

        var columnIndex = positionIndex -
                          rowInformation.rowStartPositionIndex;
        
        if (previousCharacterKind == CharacterKind.LetterOrDigit &&
            currentCharacterKind == CharacterKind.LetterOrDigit)
        {
            var wordColumnIndexStartInclusive =
                GetColumnIndexOfCharacterWithDifferingKind(
                    rowInformation.rowIndex,
                    columnIndex,
                    true);

            if (wordColumnIndexStartInclusive == -1)
                wordColumnIndexStartInclusive = 0;
            
            var wordColumnIndexEndExclusive =
                GetColumnIndexOfCharacterWithDifferingKind(
                    rowInformation.rowIndex,
                    columnIndex,
                    false);

            if (wordColumnIndexEndExclusive == -1)
                wordColumnIndexEndExclusive = GetLengthOfRow(rowInformation.rowIndex);

            return new TextEditorTextSpan(
                wordColumnIndexStartInclusive + rowInformation.rowStartPositionIndex,
                wordColumnIndexEndExclusive + rowInformation.rowStartPositionIndex,
                0);
        }
        else if (currentCharacterKind == CharacterKind.LetterOrDigit)
        {
            var wordColumnIndexEndExclusive =
                GetColumnIndexOfCharacterWithDifferingKind(
                    rowInformation.rowIndex,
                    columnIndex,
                    false);

            if (wordColumnIndexEndExclusive == -1)
                wordColumnIndexEndExclusive = GetLengthOfRow(rowInformation.rowIndex);
                
            return new TextEditorTextSpan(
                columnIndex + rowInformation.rowStartPositionIndex,
                wordColumnIndexEndExclusive + rowInformation.rowStartPositionIndex,
                0);
        }
        else if (previousCharacterKind == CharacterKind.LetterOrDigit)
        {
            var wordColumnIndexStartInclusive =
                GetColumnIndexOfCharacterWithDifferingKind(
                    rowInformation.rowIndex,
                    columnIndex,
                    true);

            if (wordColumnIndexStartInclusive == -1)
                wordColumnIndexStartInclusive = 0;
                
            return new TextEditorTextSpan(
                wordColumnIndexStartInclusive + rowInformation.rowStartPositionIndex,
                columnIndex + rowInformation.rowStartPositionIndex,
                0);
        }

        return null;
    }

    public (int rowIndex, int rowStartPositionIndex, (int positionIndex, RowEndingKind rowEndingKind) rowEndingTuple)
        FindRowInformation(int positionIndex)
    {
        for (var i = _rowEndingPositions.Count - 1; i >= 0; i--)
        {
            var rowEndingTuple = _rowEndingPositions[i];

            if (positionIndex >= rowEndingTuple.positionIndex)
            {
                return (i + 1, rowEndingTuple.positionIndex,
                    i == _rowEndingPositions.Count - 1
                        ? rowEndingTuple
                        : _rowEndingPositions[i + 1]);
            }
        }

        return (0, 0, _rowEndingPositions[0]);
    }

    /// <summary>
    /// <see cref="moveBackwards"/> is to mean earlier in the document
    /// (lower column index or lower row index depending on position) 
    /// </summary>
    /// <returns>Will return -1 if no valid result was found.</returns>
    public int GetColumnIndexOfCharacterWithDifferingKind(
        int rowIndex,
        int columnIndex,
        bool moveBackwards)
    {
        var iterateBy = moveBackwards
            ? -1
            : 1;

        var startOfRowPositionIndex = GetStartOfRowTuple(
                rowIndex)
            .positionIndex;

        if (rowIndex > _rowEndingPositions.Count - 1)
            return -1;
        
        var lastPositionIndexOnRow = _rowEndingPositions[rowIndex].positionIndex - 1;

        var positionIndex = GetPositionIndex(rowIndex, columnIndex);

        if (moveBackwards)
        {
            if (positionIndex <= startOfRowPositionIndex)
                return -1;

            positionIndex -= 1;
        }

        if (positionIndex < 0 ||
            positionIndex >= _content.Count)
        {
            return -1;
        }

        var startingCharacterKind = _content[positionIndex].GetCharacterKind();

        while (true)
        {
            if (positionIndex >= _content.Count ||
                positionIndex > lastPositionIndexOnRow ||
                positionIndex < startOfRowPositionIndex)
                return -1;

            var currentCharacterKind = _content[positionIndex].GetCharacterKind();

            if (currentCharacterKind != startingCharacterKind)
                break;

            positionIndex += iterateBy;
        }

        if (moveBackwards) positionIndex += 1;

        return positionIndex - startOfRowPositionIndex;
    }

    public void SetDecorationMapper(IDecorationMapper? decorationMapper)
    {
        DecorationMapper = decorationMapper ?? new TextEditorDecorationMapperDefault();

        // TODO: Invoke an event to reapply the CSS classes?
    }

    public void SetLexer(ILexer? lexer)
    {
        Lexer = lexer ?? new TextEditorLexerDefault();

        // TODO: Invoke an event to reapply the CSS classes?
    }
    
    public TextEditorModel SetResourceData(
        string resourceUri,
        DateTime resourceLastWriteTime)
    {
        var nextTextEditor = new TextEditorModel(this);

        nextTextEditor.ResourceUri = resourceUri;
        nextTextEditor.ResourceLastWriteTime = resourceLastWriteTime;

        return nextTextEditor;
    }

    public TextEditorModel SetUsingRowEndingKind(RowEndingKind rowEndingKind)
    {
        UsingRowEndingKind = rowEndingKind;
        return new TextEditorModel(this);
    }

    public ImmutableArray<RichCharacter> GetAllRichCharacters()
    {
        return _content.ToImmutableArray();
    }

    public void ClearEditBlocks()
    {
        EditBlockIndex = 0;
        _editBlocksPersisted.Clear();
    }

    public bool CanUndoEdit()
    {
        return EditBlockIndex > 0;
    }

    public bool CanRedoEdit()
    {
        return EditBlockIndex < _editBlocksPersisted.Count - 1;
    }

    /// <summary>
    /// The "if (EditBlockIndex == _editBlocksPersisted.Count)"
    /// <br/><br/>
    /// Is done because the active EditBlock is not yet persisted.
    /// <br/><br/>
    /// The active EditBlock is instead being 'created' as the user
    /// continues to make edits of the same <see cref="TextEditKind"/>
    /// <br/><br/>
    /// For complete clarity, this comment refers to one possibly expecting
    /// to see "if (EditBlockIndex == _editBlocksPersisted.Count - 1)"
    /// </summary>
    public TextEditorModel UndoEdit()
    {
        if (!CanUndoEdit())
            return this;

        if (EditBlockIndex == _editBlocksPersisted.Count)
        {
            // If the edit block is pending then persist it
            // before reverting back to the previous persisted edit block.

            EnsureUndoPoint(TextEditKind.ForcePersistEditBlock);
            EditBlockIndex--;
        }

        EditBlockIndex--;

        var restoreEditBlock = _editBlocksPersisted[EditBlockIndex];

        SetContent(restoreEditBlock.ContentSnapshot);

        return new TextEditorModel(this);
    }

    public TextEditorModel RedoEdit()
    {
        if (!CanRedoEdit())
            return this;

        EditBlockIndex++;

        var restoreEditBlock = _editBlocksPersisted[EditBlockIndex];

        SetContent(restoreEditBlock.ContentSnapshot);

        return new TextEditorModel(this);
    }

    public CharacterKind GetCharacterKindAt(int positionIndex)
    {
        try
        {
            return _content[positionIndex].GetCharacterKind();
        }
        catch (ArgumentOutOfRangeException)
        {
            // The text editor's cursor is is likely
            // to have this occur at times
        }

        return CharacterKind.Bad;
    }

    public string? ReadPreviousWordOrDefault(
        int rowIndex,
        int columnIndex)
    {
        var wordPositionIndexEndExclusive = GetPositionIndex(
            rowIndex,
            columnIndex);

        var wordCharacterKind = GetCharacterKindAt(
            wordPositionIndexEndExclusive - 1);

        if (wordCharacterKind == CharacterKind.LetterOrDigit)
        {
            var wordColumnIndexStartInclusive =
                GetColumnIndexOfCharacterWithDifferingKind(
                    rowIndex,
                    columnIndex,
                    true);

            if (wordColumnIndexStartInclusive == -1)
                wordColumnIndexStartInclusive = 0;

            var wordLength = columnIndex -
                             wordColumnIndexStartInclusive;

            var wordPositionIndexStartInclusive =
                wordPositionIndexEndExclusive - wordLength;

            return GetTextRange(
                wordPositionIndexStartInclusive,
                wordLength);
        }

        return null;
    }
    
    public string? ReadNextWordOrDefault(
        int rowIndex,
        int columnIndex)
    {
        var wordPositionIndexStartInclusive = GetPositionIndex(
            rowIndex,
            columnIndex);
            
        var wordCharacterKind = GetCharacterKindAt(
            wordPositionIndexStartInclusive);

        if (wordCharacterKind == CharacterKind.LetterOrDigit)
        {
            var wordColumnIndexEndExclusive =
                GetColumnIndexOfCharacterWithDifferingKind(
                    rowIndex,
                    columnIndex,
                    false);

            if (wordColumnIndexEndExclusive == -1)
                wordColumnIndexEndExclusive = GetLengthOfRow(rowIndex);

            var wordLength = wordColumnIndexEndExclusive - 
                             columnIndex;

            return GetTextRange(
                wordPositionIndexStartInclusive,
                wordLength);
        }

        return null;
    }
}