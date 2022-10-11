# Change Log

All notable changes to the "Blazor.Text.Editor" nuget package will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.0] - 2022-10-11 (Initial Release)
### Added
- TextEditorDisplay Blazor Component
- Keyboard Movements
    - ArrowLeft
    - ArrowDown
    - ArrowUp
    - ArrowRight
    - Home
    - End
    - Ctrl + ArrowLeft
    - Ctrl + ArrowDown
    - Ctrl + ArrowUp
    - Ctrl + ArrowRight
    - Ctrl + Home
    - Ctrl + End
    - Shift + ArrowLeft
    - Shift + ArrowDown
    - Shift + ArrowUp
    - Shift + ArrowRight
    - Shift + Home
    - Shift + End
    - Allow both Ctrl and Shift modifiers simultaneously when using movements.
- Mouse Movements
    - OnMouseDown (colloquially one would likely say onclick in this situation)
    - OnMouseMove (when selecting text the cursor moves along with your mouse)
- Vertical Movement
    - When using ArrowDown or ArrowUp, in the case that the row one travels to has a length shorter than that of the column index they previously were at. The Editor in the background remembers their larger column index and further ArrowDown or ArrowUp movements try to match the original larger Column Index.
- Newline characters are displayed
- The gutter has a background color that can be separate from the 'file content'.
- The gutter contains line numbers.
- A caret row envelops the current row's line number and the text content on that row.
- Cursor
    - Movement with keyboard
    - Movement with mouse
    - Debounced Blinking Animation
- Virtualization
    - Only the content that is viewable within the TextEditorDisplay Blazor component gets rendered to the screen.
- Text Selection
    - One can use the mouse to select text by firing holding left click and dragging.
    - One can use the keyboard to select text by holding the 'Shift' key and combining that with any keyboard movement operation. For example: 'Shift' + 'ArrowLeft'.
- Clipboard
    - One can copy their active Text Selection using the keybind: { 'Ctrl' + 'c' }
    - One can paste the contents of their clipboard using the keybind: { 'Ctrl' + 'v' }
- ILexer API allows one to breakdown the string text within a TextEditorBase into TextEditorTextSpan(s) where each TextEditorTextSpan is the location of a token within the editor and a byte to indicate what color to make that location of the editor.
- IDecorationMapper API allows one to map a byte named 'DecorationByte' that exists on TextEditorTextSpan to a string that represents a CSS class to put on the rendered content.





