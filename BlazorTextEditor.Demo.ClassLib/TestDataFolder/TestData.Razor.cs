namespace BlazorTextEditor.Demo.ClassLib.TestDataFolder;

public static partial class TestData
{
	public static class Razor
	{
		public const string EXAMPLE_TEXT = @"<div class=""card"" style=""width:22rem"">
    <div class=""card-body"">
        <h3 class=""card-title"">@Title</h3>
        <p class=""card-text"">@ChildContent</p>
        <button @onclick=""OnYes"">Yes!</button>
    </div>

	@for (int i = 0; i < 10; i++)
	{
		<div>for loop</div>
	}

	@foreach (var entry in MyList)
	{
		<div>foreach loop</div>
	}

	@switch (colorName)
	{
		case ""Blue"":
			<div>
				Blue was the color name
			</div>
			break;
		case ""Red"":
			<div>
				Red was the color name
			</div>
			break;
		default:
			<div>
				You stumped me
			</div>
			break;
	}

	@do
	{
		<div>
			Do-While loop
		</div>
	} while (false)

	@while (false)
	{
		<div>
			While
		</div>
	}

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