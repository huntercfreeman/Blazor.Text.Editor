﻿using System.Collections.Immutable;
using BlazorCommon.RazorLib.Misc;
using BlazorTextEditor.RazorLib.Character;
using BlazorTextEditor.RazorLib.Decoration;
using BlazorTextEditor.RazorLib.Editing;
using BlazorTextEditor.RazorLib.HelperComponents;
using BlazorTextEditor.RazorLib.Keymap;
using BlazorTextEditor.RazorLib.Lexing;
using BlazorTextEditor.RazorLib.Options;
using BlazorTextEditor.RazorLib.Row;
using BlazorTextEditor.RazorLib.Semantics;
using BlazorTextEditor.RazorLib.ViewModel;

namespace BlazorTextEditor.RazorLib.Model;

public partial class TextEditorModel
{
    public const int TAB_WIDTH = 4;
    public const int GUTTER_PADDING_LEFT_IN_PIXELS = 5;
    public const int GUTTER_PADDING_RIGHT_IN_PIXELS = 15;
    public const int MAXIMUM_EDIT_BLOCKS = 10;
    public const int MOST_CHARACTERS_ON_A_SINGLE_ROW_MARGIN = 5;

    private readonly List<RichCharacter> _content = new();
    private readonly List<EditBlock> _editBlocksPersisted = new();
    private readonly List<(RowEndingKind rowEndingKind, int count)> _rowEndingKindCounts = new();

    /// <summary>To get the ending position of RowIndex _rowEndingPositions[RowIndex]<br /><br />_rowEndingPositions returns the start of the NEXT row</summary>
    private readonly List<(int positionIndex, RowEndingKind rowEndingKind)> _rowEndingPositions = new();

    /// <summary>Provides exact position index of a tab character</summary>
    private readonly List<int> _tabKeyPositions = new();

    public int RowCount => _rowEndingPositions.Count;
    public int DocumentLength => _content.Count;

    public ImmutableArray<EditBlock> EditBlocks => _editBlocksPersisted.ToImmutableArray();
    
    public ImmutableArray<(int positionIndex, RowEndingKind rowEndingKind)> RowEndingPositions =>
        _rowEndingPositions.ToImmutableArray();
    
    public ImmutableArray<(RowEndingKind rowEndingKind, int count)> RowEndingKindCounts =>
        _rowEndingKindCounts.ToImmutableArray();

    public TextEditorModelKey ModelKey { get; } = TextEditorModelKey.NewTextEditorModelKey();
    public RenderStateKey RenderStateKey { get; } = RenderStateKey.NewRenderStateKey();
    /// <summary>If there is a mixture of<br />-Carriage Return<br />-Linefeed<br />-CRLF<br />Then <see cref="OnlyRowEndingKind" /> will be null.<br /><br />If there are no line endingsthen <see cref="OnlyRowEndingKind" /> will be null.</summary>
    public RowEndingKind? OnlyRowEndingKind { get; private set; }
    public RowEndingKind UsingRowEndingKind { get; private set; }
    public ITextEditorLexer Lexer { get; private set; }
    public string ResourceUri { get; private set; }
    public DateTime ResourceLastWriteTime { get; private set; }
    /// <summary><see cref="FileExtension"/> is displayed as is within the<see cref="TextEditorFooter"/>.<br/><br/>The <see cref="TextEditorFooter"/> is only displayed if<see cref="TextEditorViewModelDisplay.IncludeFooterHelperComponent"/> is set to true.</summary>
    public string FileExtension { get; private set; }
    public IDecorationMapper DecorationMapper { get; private set; }
    public ISemanticModel? SemanticModel { get; }
    public ITextEditorKeymap TextEditorKeymap { get; }
    public int EditBlockIndex { get; private set; }

    public (int rowIndex, int rowLength) MostCharactersOnASingleRowTuple { get; private set; }

    public TextEditorOptions? TextEditorOptions { get; }
}