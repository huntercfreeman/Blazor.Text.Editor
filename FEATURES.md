## Features of `Blazor.Text.Editor`
---

- `Insertion` of text
- `Deletion` of text
- `Selection` of text
- `Cursor` which shows the position within the text at which a user's action will occur.
- `Keyboard Movement`
    - `ArrowLeft`: Move cursor one column to the left, or wrap to the previous line if one exists.
        - `Control Modifier`: Move cursor to the left until the text for the current word finishes, or wrap to the previous line if one exists.
    - `ArrowDown`: Move cursor one row down.
        - `Control Modifier`: Scroll vertically down by the height of a row
    - `ArrowUp`: Move cursor one row up.
        - `Control Modifier`: Scroll vertically up by the height of a row
    - `ArrowRight`: Move cursor one column to the 
    right, or wrap to the next line if one exists.
        - `Control Modifier`:  Move cursor to the right until the text for the current word finishes, or wrap to the previous line if one exists..
    - `Home`: Move cursor to the first column of the current row.
        - `Control Modifier`: Move cursor to the first row. Afterwards, perform the `Home` keybind without the `Control Modifier`.
    - `End`: Move cursor to the last column of the current row.
        - `Control Modifier`: Move cursor to the last row. Afterwards, perform the `End` keybind without the `Control Modifier`.
- `Shift Key` Modified Keyboard Movement
    - If text is not already selected an anchor position will be made as the starting position of the cursor. Then the ending position will be where the cursor ends up after the movement. Should text already be selected then only the ending position modification occurs.
- Mouse Movement
    - OnMouseDown move `TextEditorCursor`
    - OnMouseMove move `TextEditorCursor`
- Mouse Selection of Text
    - No use of Shift Modifier: If text is not already selected, then OnMouseDown will make an anchor at the position of the cursor after it gets moved due to the OnMouseDown event. One can then move the mouse and place an ending position. The anchor and ending positions act as goal posts in regards to mentally imagining how they work. The text between them gets selected.
    - With use of Shift Modifier: If text is not already selected, then OnMouseDown will make an anchor at the position of the cursor PRIOR to the OnMouseDown event. Additionally an ending position will be made at the position that the OnMouseDown event occurred. One can then move the mouse and place an ending position. The anchor and ending positions act as goal posts in regards to mentally imagining how they work. The text between them gets selected.
    - Double Click Expand Selection: double click on a word to select the entirety of that word.

- Keyboard scroll position modification keymaps
    - `PageUp`: Scroll vertically up by the height of the text editor with 
    - `PageDown`: Scroll vertically down by the height of the text editor with 
    - `Control key` + ArrowUp
    - `Control key` + ArrowDown
