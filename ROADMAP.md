## An update every 2 weeks is the goal.

---

### 5.1.0 (2022-11-25)
- Scrolling Changes

### 5.2.0 (2022-12-09)
- Implement ILexers
- This includes:
    - C#
    - CSS
    - F#
    - HTML
    - JavaScript
    - Razor
    - TypeScript

### 6.0.0 (2022-12-30)
- Changes the already existing "TextEditorBase" class to instead be a model of a "unique file" if one uses the file system as an example use case.
- Added the class "TextEditorViewModel". This class acts contains any state one previously found on the Blazor Component: "TextEditorDisplay.razor". This allows one to maintain the TextEditorCursor position for example as its state is no longer tied to the lifecycle of the "TextEditorDisplay.razor" Blazor Component.
- Rename "TextEditorDisplay.razor" to "TextEditorViewModelDisplay.razor" and change the Blazor Parameter from "TextEditorKey" to "TextEditorViewModelKey"
- Added the class "TextEditorGroup". This class adds 'tab' functionality and maintains the state of the new Blazor component named "TextEditorGroupDisplay. razor".
- The TextEditor's gutter and body have separate parent HTML elements which they are positioned relative to.
    - (the gutter and body are position: absolute and previous versions had bugs involving the body being displayed within the gutter do to incorrectly calculating the CSS 'left' attribute value.)

### 6.1.0 (2023-01-04)
- Fix computed CSS style strings when dealing with localization and the locations decimal delimiter. (Example: An individual in Portugal did not have the text editor render correctly because the interpolated string did a .ToString() on a double with his local decimal delimiter. So the HTML element had a style attribute with value of "left: 5,33px" when it should be "left: 5.33px"
- Add Vim Emulation

### ?.?.?
- Autocomplete is to be finished
- Call tips is to be finished
- Brace highlighting is to be finished
- User lists is to be finished

### ?.?.?
- Folding
- Line Wrapping
- Long lines

### ?.?.?
- Notifications
- Searching and replacing

### ?.?.?
- Multiple views
- These multiple views can be of the same underlying TextEditorBase.
- Changes to the underlying TextEditorBase are to notify ALL Blazor components related to that TextEditorBase instance that they should re-render.
- Diff viewer

### ?.?.?
- Proportional fonts
- Change tab-width
- End of Line Annotations
- Above line annotation (code lens?)

### ?.?.?
- Insertion of 'widgets?' in the margin.
- An example widget would be the quick actions lightbulb icon.
- Popup edit menu. If I hit F2 perhaps it brings up a rename popup edit menu.

### ?.?.?
- Multi Cursor logic
- Virtual Space logic
- Rectangular selection logic
    
### ?.?.?
- All throughout the development of BlazorTextEditor be cognizant of Accessibility.
- In specific however, perhaps it is useful to dedicate time to making sure EVERYTHING is setup in terms of Accessibility.
    
### ?.?.?
- Support more natural languages
    beyond English.
- An example would be supporting
    Chinese characters.
