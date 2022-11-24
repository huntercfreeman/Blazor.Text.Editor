namespace BlazorTextEditor.Demo.Wasm.TestDataFolder;

public static partial class TestData
{
    public static class Html
    {
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