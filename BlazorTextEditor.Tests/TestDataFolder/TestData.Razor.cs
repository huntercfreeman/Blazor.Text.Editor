namespace BlazorTextEditor.Tests.TestDataFolder;

public static partial class TestData
{
    public static class Razor
    {
        public const string EXAMPLE_TEXT_20_LINES = @"<div class=""card"" style=""width:22rem"">
    <div class=""card-body"">
        <h3 class=""card-title"">@Title</h3>
        <p class=""card-text"">@ChildContent</p>
        <button @onclick=""OnYes"">Yes!</button>
    </div>

	@if (true)
	{
		<div>
			My Text Content!
		</div>
	}
	else if (false)
	{
		<div>
			My Text Content!
		</div>
	}
	else
	{
		<div>
			My Text Content!
		</div>
	}

	@if (true)
	{
		<div>
			My Text Content!
		</div>
	}
	else
	{
		<div>
			My Text Content!
		</div>
	}



</div>

@code
{
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public string? Title { get; set; }

    private void OnYes()
    {
        Console.WriteLine(""Write to the console in C#! 'Yes' button selected."");
    }
}";
    }
}