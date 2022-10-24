using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Decoration;
using BlazorTextEditor.RazorLib.Keyboard;
using BlazorTextEditor.RazorLib.Lexing;
using BlazorTextEditor.RazorLib.Store.TextEditorCase;

namespace BlazorTextEditor.RazorLib.TextEditor;

public class TextEditorBase
{
    public const int TabWidth = 4;
    public const int GutterPaddingLeftInPixels = 5;
    public const int GutterPaddingRightInPixels = 15;
    public const int MaximumEditBlocks = 10;

    /// <summary>
    /// To get the ending position of RowIndex _rowEndingPositions[RowIndex]
    /// <br/><br/>
    /// _rowEndingPositions returns the start of the NEXT row
    /// </summary>
    private readonly List<(int positionIndex, RowEndingKind rowEndingKind)> _rowEndingPositions = new();

    /// <summary>
    /// Provides exact position index of a tab character
    /// </summary>
    private readonly List<int> _tabKeyPositions = new();

    private readonly List<RichCharacter> _content = new();
    private readonly List<EditBlock> _editBlocks = new();
    private readonly List<(RowEndingKind rowEndingKind, int count)> _rowEndingKindCounts = new();
    
    public TextEditorBase(string content, ILexer? lexer, IDecorationMapper? decorationMapper)
    {
        Lexer = lexer ?? new LexerDefault();
        DecorationMapper = decorationMapper ?? new DecorationMapperDefault();
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

    public TextEditorBase(string content, ILexer? lexer, IDecorationMapper? decorationMapper, TextEditorKey key)
        : this(content, lexer, decorationMapper)
    {
        Key = key;
    }

    public TextEditorKey Key { get; } = TextEditorKey.NewTextEditorKey();
    public int RowCount => _rowEndingPositions.Count;
    public int DocumentLength => _content.Count;

    public ImmutableArray<EditBlock> EditBlocks => _editBlocks.ToImmutableArray();

    /// <summary>
    /// If there is a mixture of<br/>
    ///     -Carriage Return<br/>
    ///     -Linefeed<br/>
    ///     -CRLF<br/>
    /// Then <see cref="OnlyRowEndingKind"/> will be null.
    /// <br/><br/>
    /// If there are no line endings
    /// then <see cref="OnlyRowEndingKind"/> will be null.
    /// </summary>
    public RowEndingKind? OnlyRowEndingKind { get; private set; }
    public RowEndingKind UsingRowEndingKind { get; private set; }
    public ILexer Lexer { get; private set; }
    public IDecorationMapper DecorationMapper { get; private set; }
    
    public int MostCharactersOnASingleRow { get; private set; }

    public TextEditorOptions TextEditorOptions { get; } = TextEditorOptions.UnsetTextEditorOptions();

    public ImmutableArray<(int positionIndex, RowEndingKind rowEndingKind)> RowEndingPositions =>
        _rowEndingPositions.ToImmutableArray();

    public (int positionIndex, RowEndingKind rowEndingKind) GetStartOfRowTuple(int rowIndex)
    {
        return rowIndex > 0
            ? _rowEndingPositions[rowIndex - 1]
            : (0, RowEndingKind.StartOfFile);
    }

    /// <summary>
    /// Returns the Length of a row however it does not include the line ending characters by default.
    /// To include line ending characters the parameter <see cref="includeLineEndingCharacters"/> must be true.
    /// </summary>
    public int GetLengthOfRow(
        int rowIndex,
        bool includeLineEndingCharacters = false)
    {
        if (!_rowEndingPositions.Any())
            return 0;

        (int positionIndex, RowEndingKind rowEndingKind) startOfRowTupleInclusive = GetStartOfRowTuple(rowIndex);

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

        for (int i = startingRowIndex;
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

    public TextEditorBase PerformEditTextEditorAction(EditTextEditorBaseAction editTextEditorBaseAction)
    {
        if (KeyboardKeyFacts.IsMetaKey(editTextEditorBaseAction.KeyboardEventArgs))
        {
            if (KeyboardKeyFacts.MetaKeys.BACKSPACE == editTextEditorBaseAction.KeyboardEventArgs.Key ||
                KeyboardKeyFacts.MetaKeys.DELETE == editTextEditorBaseAction.KeyboardEventArgs.Key)
            {
                PerformDeletions(editTextEditorBaseAction);
            }
        }
        else
        {
            PerformInsertions(editTextEditorBaseAction);
        }

        return this;
    }

    private void EnsureUndoPoint(TextEditKind textEditKind)
    {
        var mostRecentEditBlock = _editBlocks.LastOrDefault();

        if (mostRecentEditBlock is null ||
            mostRecentEditBlock.TextEditKind != textEditKind)
        {
            _editBlocks.Add(new EditBlock(
                textEditKind,
                textEditKind.ToString(),
                GetAllText()));
        }

        while (_editBlocks.Count > MaximumEditBlocks &&
               _editBlocks.Count != 0)
        {
            _editBlocks.RemoveAt(0);
        }
    }

    private void PerformInsertions(EditTextEditorBaseAction editTextEditorBaseAction)
    {
        EnsureUndoPoint(TextEditKind.Insertion);

        foreach (var cursorTuple in editTextEditorBaseAction.TextCursorTuples)
        {
            var startOfRowPositionIndex =
                GetStartOfRowTuple(cursorTuple.immutableTextEditorCursor.RowIndex)
                    .positionIndex;

            var cursorPositionIndex =
                startOfRowPositionIndex + cursorTuple.immutableTextEditorCursor.ColumnIndex;

            var wasTabCode = false;
            var wasEnterCode = false;

            var characterValueToInsert = editTextEditorBaseAction.KeyboardEventArgs.Key
                .First();

            if (KeyboardKeyFacts.IsWhitespaceCode(editTextEditorBaseAction.KeyboardEventArgs.Code))
            {
                characterValueToInsert =
                    KeyboardKeyFacts.ConvertWhitespaceCodeToCharacter(editTextEditorBaseAction.KeyboardEventArgs.Code);

                wasTabCode = KeyboardKeyFacts.WhitespaceCodes.TAB_CODE ==
                             editTextEditorBaseAction.KeyboardEventArgs.Code;

                wasEnterCode = KeyboardKeyFacts.WhitespaceCodes.ENTER_CODE ==
                               editTextEditorBaseAction.KeyboardEventArgs.Code;
            }

            var characterCountInserted = 1;

            if (wasEnterCode)
            {
                var rowEndingKindToInsert = UsingRowEndingKind;
                
                var richCharacters = rowEndingKindToInsert
                    .AsCharacters()
                    .Select(character => new RichCharacter
                    {
                        Value = character,
                        DecorationByte = default(byte)
                    });
                
                characterCountInserted = rowEndingKindToInsert.AsCharacters().Length;
                
                _content.InsertRange(cursorPositionIndex, richCharacters);
                
                _rowEndingPositions.Insert(cursorTuple.immutableTextEditorCursor.RowIndex,
                    (cursorPositionIndex + characterCountInserted, rowEndingKindToInsert));

                MutateRowEndingKindCount(
                    UsingRowEndingKind, 
                    1);
                
                var indexCoordinates = cursorTuple.textEditorCursor.IndexCoordinates;

                cursorTuple.textEditorCursor.IndexCoordinates = (indexCoordinates.rowIndex + 1, 0);

                cursorTuple.textEditorCursor.PreferredColumnIndex =
                    cursorTuple.textEditorCursor.IndexCoordinates.columnIndex;
            }
            else
            {
                if (wasTabCode)
                {
                    var index = _tabKeyPositions
                        .FindIndex(x =>
                            x >= cursorPositionIndex);

                    if (index == -1)
                    {
                        _tabKeyPositions.Add(cursorPositionIndex);
                    }
                    else
                    {
                        for (int i = index; i < _tabKeyPositions.Count; i++)
                        {
                            _tabKeyPositions[i]++;
                        }

                        _tabKeyPositions.Insert(index, cursorPositionIndex);
                    }
                }
                
                var richCharacterToInsert = new RichCharacter
                {
                    Value = characterValueToInsert,
                    DecorationByte = default(byte)
                };

                _content.Insert(cursorPositionIndex, richCharacterToInsert);

                var indexCoordinates = cursorTuple.textEditorCursor.IndexCoordinates;

                cursorTuple.textEditorCursor.IndexCoordinates =
                    (indexCoordinates.rowIndex, indexCoordinates.columnIndex + 1);
                cursorTuple.textEditorCursor.PreferredColumnIndex =
                    cursorTuple.textEditorCursor.IndexCoordinates.columnIndex;
            }

            var firstRowIndexToModify = wasEnterCode
                ? cursorTuple.immutableTextEditorCursor.RowIndex + 1
                : cursorTuple.immutableTextEditorCursor.RowIndex;

            for (int i = firstRowIndexToModify; i < _rowEndingPositions.Count; i++)
            {
                var rowEndingTuple = _rowEndingPositions[i];

                _rowEndingPositions[i] = (rowEndingTuple.positionIndex + characterCountInserted, rowEndingTuple.rowEndingKind);
            }

            if (!wasTabCode)
            {
                var firstTabKeyPositionIndexToModify = _tabKeyPositions
                    .FindIndex(x => x >= cursorPositionIndex);

                if (firstTabKeyPositionIndexToModify != -1)
                {
                    for (int i = firstTabKeyPositionIndexToModify; i < _tabKeyPositions.Count; i++)
                    {
                        _tabKeyPositions[i] += characterCountInserted;
                    }
                }
            }
        }
    }

    private void PerformDeletions(EditTextEditorBaseAction editTextEditorBaseAction)
    {
        EnsureUndoPoint(TextEditKind.Deletion);

        foreach (var cursorTuple in editTextEditorBaseAction.TextCursorTuples)
        {
            var startOfRowPositionIndex =
                GetStartOfRowTuple(cursorTuple.immutableTextEditorCursor.RowIndex)
                    .positionIndex;

            var cursorPositionIndex =
                startOfRowPositionIndex + cursorTuple.immutableTextEditorCursor.ColumnIndex;

            int startingPositionIndexToRemoveInclusive;
            int countToRemove;
            bool moveBackwards;

            // Cannot calculate this after text was deleted - it would be wrong
            int? selectionUpperBoundRowIndex = null;
            // Needed for when text selection is deleted
            (int rowIndex, int columnIndex)? selectionLowerBoundIndexCoordinates = null;

            if (cursorTuple.immutableTextEditorCursor.ImmutableTextEditorSelection.HasSelectedText())
            {
                var lowerBound = cursorTuple.immutableTextEditorCursor.ImmutableTextEditorSelection
                    .AnchorPositionIndex ?? 0; 
                
                var upperBound = cursorTuple.immutableTextEditorCursor.ImmutableTextEditorSelection
                    .EndingPositionIndex;

                if (lowerBound > upperBound)
                    (lowerBound, upperBound) = (upperBound, lowerBound);
                
                var lowerRowMetaData = 
                    FindRowIndexRowStartRowEndingTupleFromPositionIndex(
                        lowerBound);
                
                var upperRowMetaData = 
                    FindRowIndexRowStartRowEndingTupleFromPositionIndex(
                        upperBound);

                // Value is needed when knowing what row ending positions
                // to update after deletion is done
                selectionUpperBoundRowIndex = upperRowMetaData.rowIndex;
                
                // Value is needed when knowing where to position the
                // cursor after deletion is done
                selectionLowerBoundIndexCoordinates = 
                    (lowerRowMetaData.rowIndex,
                        lowerBound - lowerRowMetaData.rowStartPositionIndex);

                startingPositionIndexToRemoveInclusive = upperBound - 1;
                countToRemove = upperBound - lowerBound;
                moveBackwards = true;

                cursorTuple.textEditorCursor.TextEditorSelection.AnchorPositionIndex = null;
            }
            else if (KeyboardKeyFacts.MetaKeys.BACKSPACE == editTextEditorBaseAction.KeyboardEventArgs.Key)
            {
                startingPositionIndexToRemoveInclusive = cursorPositionIndex - 1;
                countToRemove = 1;
                moveBackwards = true;
            }
            else if (KeyboardKeyFacts.MetaKeys.DELETE == editTextEditorBaseAction.KeyboardEventArgs.Key)
            {
                startingPositionIndexToRemoveInclusive = cursorPositionIndex;
                countToRemove = 1;
                moveBackwards = false;
            }
            else
            {
                throw new ApplicationException(
                    $"The keyboard key: {editTextEditorBaseAction.KeyboardEventArgs.Key} was not recognized");
            }

            int charactersRemovedCount = 0;
            int rowsRemovedCount = 0;

            var indexToRemove = startingPositionIndexToRemoveInclusive;

            while (countToRemove-- > 0)
            {
                if (indexToRemove < 0 ||
                    indexToRemove > _content.Count - 1)
                {
                    break;
                }

                var characterToDelete = _content[indexToRemove];

                int startingIndexToRemoveRange;
                int countToRemoveRange;

                if (KeyboardKeyFacts.IsLineEndingCharacter(characterToDelete.Value))
                {
                    rowsRemovedCount++;

                    // rep.positionIndex == indexToRemove + 1
                    //     ^is for backspace
                    //
                    // rep.positionIndex == indexToRemove + 2
                    //     ^is for delete
                    var rowEndingTupleIndex = _rowEndingPositions
                        .FindIndex(rep =>
                            rep.positionIndex == indexToRemove + 1 ||
                            rep.positionIndex == indexToRemove + 2);

                    var rowEndingTuple = _rowEndingPositions[rowEndingTupleIndex];

                    _rowEndingPositions.RemoveAt(rowEndingTupleIndex);

                    var lengthOfRowEnding = rowEndingTuple.rowEndingKind
                        .AsCharacters().Length;

                    if (moveBackwards)
                    {
                        startingIndexToRemoveRange = indexToRemove - (lengthOfRowEnding - 1);
                    }
                    else
                    {
                        startingIndexToRemoveRange = indexToRemove;
                    }

                    countToRemove -= (lengthOfRowEnding - 1);
                    countToRemoveRange = lengthOfRowEnding;
                    
                    MutateRowEndingKindCount(
                        rowEndingTuple.rowEndingKind, 
                        -1);
                }
                else
                {
                    if (characterToDelete.Value == KeyboardKeyFacts.WhitespaceCharacters.TAB)
                        _tabKeyPositions.Remove(indexToRemove);

                    startingIndexToRemoveRange = indexToRemove;
                    countToRemoveRange = 1;
                }

                charactersRemovedCount += countToRemoveRange;

                _content.RemoveRange(startingIndexToRemoveRange, countToRemoveRange);

                if (moveBackwards)
                    indexToRemove -= countToRemoveRange;
            }
            
            if (charactersRemovedCount == 0 &&
                rowsRemovedCount == 0)
            {
                return;
            }
            
            if (moveBackwards && !selectionUpperBoundRowIndex.HasValue)
            {
                var modifyRowsBy = -1 * rowsRemovedCount;

                var startOfCurrentRowPositionIndex = GetStartOfRowTuple(
                        cursorTuple.immutableTextEditorCursor.RowIndex + modifyRowsBy)
                    .positionIndex;

                var modifyPositionIndexBy = -1 * charactersRemovedCount;

                var endingPositionIndex = cursorPositionIndex + modifyPositionIndexBy;

                var columnIndex = endingPositionIndex - startOfCurrentRowPositionIndex;
            
                var indexCoordinates = cursorTuple.textEditorCursor.IndexCoordinates;
            
                cursorTuple.textEditorCursor.IndexCoordinates = 
                    (indexCoordinates.rowIndex + modifyRowsBy, 
                        columnIndex);
            }
            
            int firstRowIndexToModify;

            if (selectionUpperBoundRowIndex.HasValue)
            {
                firstRowIndexToModify = selectionLowerBoundIndexCoordinates!.Value.rowIndex;
                
                cursorTuple.textEditorCursor.IndexCoordinates = 
                    selectionLowerBoundIndexCoordinates!.Value;
            }
            else if (moveBackwards)
            {
                firstRowIndexToModify = cursorTuple.immutableTextEditorCursor.RowIndex - rowsRemovedCount;
            }
            else
            {
                firstRowIndexToModify = cursorTuple.immutableTextEditorCursor.RowIndex;
            }
            
            for (int i = firstRowIndexToModify; i < _rowEndingPositions.Count; i++)
            {
                var rowEndingTuple = _rowEndingPositions[i];
            
                _rowEndingPositions[i] = (rowEndingTuple.positionIndex - charactersRemovedCount, rowEndingTuple.rowEndingKind);
            }
            
            var firstTabKeyPositionIndexToModify = _tabKeyPositions
                .FindIndex(x => x >= startingPositionIndexToRemoveInclusive);

            if (firstTabKeyPositionIndexToModify != -1)
            {
                for (int i = firstTabKeyPositionIndexToModify; i < _tabKeyPositions.Count; i++)
                {
                    _tabKeyPositions[i] -= charactersRemovedCount;
                }
            }
        }
    }

    private void MutateRowEndingKindCount(RowEndingKind rowEndingKind, int changeBy)
    {
        var indexOfRowEndingKindCount = _rowEndingKindCounts
            .FindIndex(x => 
                x.rowEndingKind == rowEndingKind);

        var currentRowEndingKindCount = _rowEndingKindCounts[indexOfRowEndingKindCount]
            .count;
                    
        _rowEndingKindCounts[indexOfRowEndingKindCount] = 
            (rowEndingKind, currentRowEndingKindCount + changeBy);
                    
        CheckRowEndingPositions(false);
    }
    
    private void CheckRowEndingPositions(bool setUsingRowEndingKind)
    {
        var existingRowEndings = _rowEndingKindCounts
            .Where(x => x.count > 0)
            .ToArray();

        if (!existingRowEndings.Any())
        {
            OnlyRowEndingKind = RowEndingKind.Unset;
            UsingRowEndingKind = RowEndingKind.Linefeed;
        }
        else
        {
            if (existingRowEndings.Length == 1)
            {
                var rowEndingKind = existingRowEndings
                    .Single()
                    .rowEndingKind;

                if (setUsingRowEndingKind)
                    UsingRowEndingKind = rowEndingKind;

                OnlyRowEndingKind = rowEndingKind;
            }
            else
            {
                if (setUsingRowEndingKind)
                {
                    UsingRowEndingKind = existingRowEndings
                        .MaxBy(x => x.count)
                        .rowEndingKind;
                }
                
                OnlyRowEndingKind = null;
            }
        }
    }
    
    /// <summary>
    /// If applying syntax highlighting it may be preferred to use
    /// <see cref="ApplySyntaxHighlightingAsync"/>. It is effectively
    /// just invoking the lexer and then <see cref="ApplyDecorationRange"/>
    /// </summary>
    public void ApplyDecorationRange(IEnumerable<TextEditorTextSpan> textEditorTextSpans)
    {
        foreach (var textEditorTextSpan in textEditorTextSpans)
        {
            for (int i = textEditorTextSpan.StartingIndexInclusive; i < textEditorTextSpan.EndingIndexExclusive; i++)
            {
                _content[i].DecorationByte = textEditorTextSpan.DecorationByte;
            }
        }
    }
    
    public async Task ApplySyntaxHighlightingAsync(bool clearSyntaxHighlightingBeforeApplication = true)
    {
        if (clearSyntaxHighlightingBeforeApplication)
        {
            ApplyDecorationRange(new []
            {
                new TextEditorTextSpan(
                    0,
                    _content.Count,
                    // 0 is decoration none
                    0)
            });
        }
        
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
    
    public int GetPositionIndex(int rowIndex, int columnIndex)
    {
        var startOfRowPositionIndex =
            GetStartOfRowTuple(rowIndex)
                .positionIndex;

        return startOfRowPositionIndex + columnIndex;
    }
    
    public string GetTextRange(int startingPositionIndex, int count)
    {
        return new string(_content
            .Skip(startingPositionIndex)
            .Take(count)
            .Select(rc => rc.Value)
            .ToArray());
    }
    
    public (int rowIndex, int rowStartPositionIndex, (int positionIndex, RowEndingKind rowEndingKind) rowEndingTuple) 
        FindRowIndexRowStartRowEndingTupleFromPositionIndex(int positionIndex)
    {
        for (int i = _rowEndingPositions.Count - 1; i >= 0; i--)
        {
            var rowEndingTuple = _rowEndingPositions[i];
            
            if (positionIndex >= rowEndingTuple.positionIndex)
                return (i + 1, rowEndingTuple.positionIndex, 
                    i == _rowEndingPositions.Count - 1
                        ? rowEndingTuple
                        : _rowEndingPositions[i + 1]);
        }

        return (0, 0, _rowEndingPositions[0]);
    }

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

        var lastPositionIndexOnRow = _rowEndingPositions[rowIndex].positionIndex - 1;
        
        var positionIndex = GetPositionIndex(rowIndex, columnIndex);

        if (moveBackwards)
        {
            if (positionIndex <= startOfRowPositionIndex)
                return -1;

            positionIndex -= 1;
        }
        
        var startingCharacterKind = _content[positionIndex].GetCharacterKind();
        
        while (true)
        {
            if (positionIndex >= _content.Count ||
                positionIndex > lastPositionIndexOnRow ||
                positionIndex < startOfRowPositionIndex)
            {
                return -1;
            }
            
            var currentCharacterKind = _content[positionIndex].GetCharacterKind();

            if (currentCharacterKind != startingCharacterKind)
                break;

            positionIndex += iterateBy;
        }
        
        if (moveBackwards)
        {
            positionIndex += 1;
        }

        return positionIndex - startOfRowPositionIndex;
    }

    public void SetDecorationMapper(IDecorationMapper? decorationMapper)
    {
        DecorationMapper = decorationMapper ?? new DecorationMapperDefault();
        
        // TODO: Invoke an event to reapply the CSS classes?
    }
    
    public void SetLexerMapper(ILexer? lexer)
    {
        Lexer = lexer ?? new LexerDefault();
        
        // TODO: Invoke an event to reapply the CSS classes?
    }
    
    public TextEditorBase SetUsingRowEndingKind(RowEndingKind rowEndingKind)
    {
        UsingRowEndingKind = rowEndingKind;
        return this;
    }

    public ImmutableArray<RichCharacter> GetAllRichCharacters()
    {
        return _content.ToImmutableArray();
    }
    
    public void ClearEditBlocks()
    {
        _editBlocks.Clear();
    }
}