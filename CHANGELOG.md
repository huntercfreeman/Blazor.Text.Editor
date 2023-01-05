# Change Log

All notable changes to the "Blazor.Text.Editor" nuget package will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.0] - 2022-10-11 (Initial Release)

<details>
  <summary>Click to show changes</summary>

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
    - When using ArrowDown or ArrowUp, in the case that the row one travels to has a length shorter than that of the
      column index they previously were at. The Editor in the background remembers their larger column index and further
      ArrowDown or ArrowUp movements try to match the original larger Column Index.
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
    - One can use the keyboard to select text by holding the 'Shift' key and combining that with any keyboard movement
      operation. For example: 'Shift' + 'ArrowLeft'.
- Clipboard
    - One can copy their active Text Selection using the keybind: { 'Ctrl' + 'c' }
    - One can paste the contents of their clipboard using the keybind: { 'Ctrl' + 'v' }
- ILexer API allows one to breakdown the string text within a TextEditorBase into TextEditorTextSpan(s) where each
  TextEditorTextSpan is the location of a token within the editor and a byte to indicate what color to make that
  location of the editor.
- IDecorationMapper API allows one to map a byte named 'DecorationByte' that exists on TextEditorTextSpan to a string
  that represents a CSS class to put on the rendered content.
</details>

---

## [3.2.0]

<details>
  <summary>Click to show changes</summary>

  ### Bug Was Erroneously Added

- A bug was erroneously added where when user selected the last character of the document and then pressed ArrowRight.
  The cursor would move 1 character out of the document. Then you get an index out of bounds exception if you hit arrow
  right 1 more time.
</details>

---

## [4.1.0]

<details>
  <summary>Click to show changes</summary>

  ### Bug Fix

- Exception was being thrown when user selected the last character of the document and then pressed ArrowRight.
    - This bug was occurring starting 3.2.0
</details>

---

## [5.0.0]

<details>
  <summary>Click to show changes</summary>

  ### Added

- Razor Syntax Highlighting
- Automatic LocalStorage integration (Optional)
    - int? FontSizeInPixels
    - Theme? Theme
    - bool? ShowWhitespace
    - bool? ShowNewlines
- Helper Components
    - TreeViewFooter.razor
    - TextEditorInputFontSize.razor
    - TextEditorInputShowNewLines.razor
    - TextEditorInputShowWhitespace.razor
    - TextEditorInputTheme.razor
- Text Manipulation
  - When text is selected typing a character will delete the selection before inserting the character.
- Text Selection
  - { Shift + LeftClick } will move the
        text selection ending position index
        to the clicked position. If the user does not have a text selection anchor set, the text selection anchor is set to where the cursor is prior to the movement. Then the text selecting ending position is the clicked position and in between the two points gets selected.
  - DoubleClick will expand select at the
      double-clicked position. In otherwords
      double clicking a word will now select that
      word.
- Cursor movement
    - If the cursor is not in the viewable area upon an OnMouseDown event, the TextEditorDisplay will no longer do a jarring scroll bug.
- Fix the JavaScript disposing of IntersectionObservers
- Keymap
    - { Ctrl + X } -> cut
    - { Ctrl + Z } -> undo
    - { Ctrl + Y } -> redo
- Blazor Component Parameter changes
    - TextEditorDisplay can be given
        the TextEditorKey of a yet to be registered
        TextEditorBase. The component will then render on its own once the TextEditorKey gets registered.
    - Only a TextEditorKey is required to be passed in to a TextEditorDisplay. Everything else will be done internally.
    - One can modify default behavior through the various other Blazor parameters other than the TextEditorKey. The others are used for customization.
- Theme changes
  - The 'Unset' theme is now a Visual Studio Dark Clone
  - Removed BlazorTextEditor custom themes
  - Only Unset, Visual Studio Dark Clone, and Visual Studio Light Clone remain.
- { Ctrl + C } to Copy no longer results in  one losing focus
- Allow the user to set the initial theme when invoking AddBlazorTextEditor() service collection extension method.
- BUG: Cut selection on content that starts with column index 0 will put cursor at column index -1
- Out of the box static settings dialog component containing all the individual Setting helper components.
- Control + Backspace
- Control + Delete
- Fix the Constants for punctuation to no longer be missing some.
- Change light theme caret row color
- Fix GetColumnIndexOfCharacterWithDifferingKind() index out of range
- Default context menu
  - Cut
  - Copy
  - Paste
- TextEditorHeader.razor Blazor component's buttons.
    - Undo
    - Redo
    - Cut
    - Copy
    - Paste
    - Save
    - Select All
    - Refresh
- Move over all Analysis code (the ILexers and IDecorationMappers) to BlazorTextEditor
    - Not enough analysis logic exists to justify the extra step of installing a separate nuget package.
- Move over some code from BlazorStudio to the BlazorTextEditor
    - CustomEvents
    - Dimensions
    - Dropdown
    - Menu
    - OutOfBoundsClick
    - Resize
    - Notifications
- Move over some code from BlazorTreeView  to the BlazorTextEditor
- When cursor accesses out of bounds location return largest available RowIndex and largest available ColumnIndex
- Virtualization move to separate files the many classes in one
- Move rerender logic to the reducer having a replaced value needing rendered. Instead of manually.
- Make TextEditorView to be a base class for all Blazor components wanting to be notified of re instantiations of their given TextEditorKey's TextEditorBase
- Visual Indicator whether TextEditorDisplay has focus
- Disabled button background color change
- Added AutocompleteIndexer.cs
- Index on whitespace, and punctuation.
- Look for autocomplete results when user types a LetterOrDigit
- Register methods for common TextEditorBase uses cases like C# and Razor
- { Ctrl + Space } -> bring up auto complete menu
- ArrowDown or ArrowUp to set focus to autocomplete menu if it is open
- Esc to close AutocompleteMenu when focusing it
- Fix out of bounds error when deleting after selecting all
- Bug fix: Pasting carriage return will now paste the LineEnding being used in the TextEditor instead of being treated as two separate newline characters.
- Throttle syntax highlighting 1 second
- 50 ms on mouse move throttle
</details>

---

## [5.1.0] - 2022-11-25 (Scrolling Changes)

<details>
  <summary>Click to show changes</summary>

  ### Added

- Keymap
    - PageDown: Scroll vertically down at most the height of the view
    - PageUp: Scroll vertically up at most the height of the view
    - Ctrl + PageDown: Move cursor to the last viewable row given the current view.
    - Ctrl + PageUp: Move cursor to the first viewable row given the current view.
    - Ctrl + ArrowDown: Scroll the view down by the height of one line without moving the cursor.
    - Ctrl + ArrowUp: Scroll the view up by the height of one line without moving the cursor.
- Scrollbar changes
    - The scrollbars are now custom made by way of overflow: hidden as opposed to the native scrollbars that were used by way of overflow: auto.
    - Now will prevent propagation on all mouse events when interacting with the scrollbar. Explanation: previously clicking the native scrollbar would propagate that click event to the text editor causing a janky and mostly unusable scrolling behavior to occur.
    - Scrolling the cursor into view if it is out of view is now done using C# as opposed to the way it was of a JavaScript intersection observer.
    - A keyboard event which results in the cursor going out of view will automatically scroll the cursor into view. Explanation: previously a cursor going out of view would sometimes not scroll the cursor into view
                    until the next keyboard event after having gone out of view.
    - The line numbers are no longer part of the horizontal scrollbar.
    - Fixed cursor rendering erroneously within the gutter. Explanation: When scrolling horizontally beyond a scrollLeft of 0. It previously was the case that a keyboard event which brought the cursor to column 0 would display the cursor inside the gutter/margin because the scrollLeft was not accounting for the gutter width.
- ILexer changes
    - F# ILexer added: Keywords are syntax highlighted
    - JavaScript ILexer: Keywords are syntax highlighted
    - TypeScript ILexer: Keywords are syntax highlighted
- TextEditorLexerDefault rename
- TextEditorDecorationMapperDefault rename

### Bugs

- This bug exists in previous versions as well: When doing horizontal virtualization it seems tab key width is not accounted for?
    - Seeing nothing when using tab key to put text horizontally out of view then horizontally scrolling that text into view.
    - Proceeding to put an enter key to split the line and all the text appears again.
- This bug exists in previous versions as well: 
    - Pick line ending of CR and then proceed to hit Enter key. 
    - Afterwards pick line ending of LF then proceed to hit Enter key. 
    - Something weird regarding carriage return line feed is happening here. 
    - I saw the length of the document go up by 1 for the CR. 
    - But then the LF which has a length of 1 added 2 length.
</details>

---

## [5.2.0] - 2022-12-07 (ILexer and Syntax Highlighting Changes)

<details>
  <summary>Click to show changes</summary>

  ### Added

- Helper Components
    - TextEditorInputHeight.razor provides an input of type number to allow setting of text editor height to a pixel value.
        - Local storage integration is set up for TextEditorInputHeight.razor
- ILexer changes
    - C#
        - Method parameters are syntax highlighted
            first. Prior to this change any invocation
            of a method would color all text between the 
            parenthesis the same color (the light blue color
            in visual studio dark theme for variables). Now
            this wide sweep of all text between the parenthesis
            is looked at. Following that the text between the parenthesis goes through looking for keywords
            and other syntax after the fact instead of first
            then being override by the more general parameter
            syntax highlighting.
    - CSS Syntax Highlighting
        - Tag selectors (identifiers)
        - Comments
        - PropertyName
        - PropertyValue
    - F# Syntax Highlighting
        - Keywords
        - Strings
        - Comments
    - HTML Syntax Highlighting
        - Tag Names
        - Injected Language Fragments (The .razor '@' transition character)
        - Attribute Names
        - Attribute Values
        - Comments
    - JavaScript Syntax Highlighting
        - Keywords
        - Strings
        - Comments
    - JSON Syntax Highlighting
        - Property Key
        - Boolean
        - Integer (No decimals place)
        - Number (Has decimals place)
        - Null
        - String
    - Razor Syntax Highlighting
        - Razor Keywords (Example: @page "/counter")
        - C# Razor Keywords (Example: @if () { })
        - Inline Expressions
        - Razor Code Blocks
        - All things listed in "HTML Syntax Highlighting" as
            the Razor Lexer internally swaps between various inner
            lexers. One of which is the HTML Lexer.
    - TypeScript Syntax Highlighting
        - Keywords
        - Strings
        - Comments

### FixedBugs

- FIXED: Previous applied syntax highlighting which becomes marked as default text (DecorationKind of None) will correctly re-render with the previous syntax highlighting removed.
    - Explanation: Add a multi line comment at the start of the file but do not close the comment. All the file will be commented out. Now remove the multi line comment. Previously some text would stay syntax highlighted as a comment erroneously. This has been fixed.
- FIXED: When doing horizontal virtualization it seems tab key width is not accounted for?
    - Seeing nothing when using tab key to put text horizontally out of view then horizontally scrolling that text into view.
    - Proceeding to put an enter key to split the line and all the text appears again.
</details>

---

### [6.0.0] 2022-12-23 (ViewModels)

<details>
  <summary>Click to show changes</summary>

  - Instead of only having `TextEditorBase.cs` see the following child bullet points for all previous and new C# Classes that directly relate to a Text Editor.
    - `TextEditorBase.cs` maps to a unique file. Perhaps it might be a file on ones filesystem.
    - `TextEditorViewModel.cs` maps to the user interface state for a `TextEditorViewModelDisplay.razor`.
    - `TextEditorGroup.cs` maps to the tab state of a `TextEditorGroupDisplay.razor`.
- FIXBUG: User's text editor cursor appearing in the gutter, and any other {blank in gutter erroneously} situation
    - Details: The TextEditor's gutter and body have the css attribute `position: absolute`. Succinctly speaking to render the user interface correctly the `left: {WIDTH_OF_GUTTER}` had to be used on varying Blazor components.
</details>

---

### [6.1.0] 2023-01-04 (Vim Emulator)

<details>
  <summary>Click to show changes</summary>

  - Fix computed CSS style strings when dealing with localization and the locations decimal delimiter. Example: An individual in Portugal did not have the text editor render correctly because the interpolated string did a .ToString() on a double with his local decimal delimiter. So the HTML element had a style attribute with value of "left: 5,33px" when it should be "left: 5.33px"
- Add Vim Emulation
- `JavaScript Intersection Observer` returns for:
    - Identifying if a `Virtualization Boundary` is intersecting.
    - Identifying if the `Text Editor Cursor` is intersecting. (more specifically in the case of the cursor we are looking to see if it is not intersecting -- then we can `scroll it into view`).
    - Details: Previously the JavaScript Intersection Observer was being used for both the virtualization and the text editor cursor. However, I thought I could mimic its behavior using C# and I realize now there are a variety of reasons for that having been a bad idea. So Adding the JavaScript Intersection Observer back in this update.
- FIXBUG: `Scrollbar Vertical` would "jitter" so to speak when one tried scrolling to the very bottom of a Text Editor. That is to say, the scrollTop would +- 10 pixels every second.
- Added: `ScrollHeightMarginBottom` so one can scroll `40%` of the TextEditor's height beyond the end of the TextEditor content.
- Added: "`Preview settings here`" display in the settings dialog. It is a mini text editor so one can see what changes they're making more easily.
- Added: "`Text Editor Cursor-Width (px):`" input helper component.
- Changed: `Default height` of a `TextEditorViewModelDisplay.razor`. Now the default is `height: 100%;`. So it will get the height of whatever its parent element is.
- Changed: An underscore '\_' no longer counts as punctuation. 
    - Details: Private fields in C# have one of their conventions to be prepending the private field with an underscore '\_'. Therefore it used to be the case that expanding selection on a private field which followed this convention would either highlight only the underscore or only the text following the underscore depending on where one had double clicked.
</details>
