# BlazorTextEditor.Tests

Some common edge cases:
- Start of the text editor PositionIndex: (0, 0)
- End of the text editor
- CarriageReturn line ending being 2 characters as opposed to 1 (example: '\r\n' vs '\n')
- Start of a row where RowIndex > 0 and move cursor left.
- End of a row where RowIndex < Rows.Count and move cursor right.