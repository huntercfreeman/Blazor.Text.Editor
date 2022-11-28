namespace BlazorTextEditor.Tests.TestDataFolder;

public static partial class TestData
{
    public static class Html
    {
        /// <summary>
        /// Copy and pasted from: https://html.com/tags/comment-tag/
        /// </summary>
        public const string EXAMPLE_TEXT_COMMENTS = @"You will be able to see this text.
<!-- You will not be able to see this text. -->
You can even comment out things in <!-- the middle of --> a sentence. 
<!-- Or you can comment out a large number of lines. --> 
<div class=""example-class"">
Another thing you can do is put comments after closing tags,
to help you find where a particular element ends. <br> 
(This can be helpful if you have a lot of nested elements.) 
</div> 
<!-- /.example-class -->

Read more: https://html.com/tags/comment-tag/#ixzz7lu6jpU00";
        
        public const string EXAMPLE_TEXT_19_LINES = @"<form>
  <label for=""username"">Username:</label>
    <input type=""text"" name=""username"" id=""username"" />
    <label for=""password"">Password:</label>
    <input type=""password"" name=""password"" id=""password"" />
    <input type=""radio"" name=""gender"" value=""male"" />Male<br />
    <input type=""radio"" name=""gender"" value=""female"" />Female<br />
    <input type=""radio"" name=""gender"" value=""other"" />Other
        <input list=""Options"" />
            <datalist id=""Options"">
        <option value=""Option1""></option>
    <option value=""Option2""></option>
    <option value=""Option3""></option>
    </datalist>

    <input type=""submit"" value=""Submit"" />
    <input type=""color"" />
        <input type=""checkbox"" name=""correct"" value=""correct"" />Correct
        </form>";
    }
}