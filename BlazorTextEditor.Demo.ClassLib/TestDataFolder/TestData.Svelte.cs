namespace BlazorTextEditor.Demo.ClassLib.TestDataFolder;

public static partial class TestData
{
    public static class Svelte
    {
        public const string EXAMPLE_TEXT = @"<script lang=""ts"">
	import type { DotNetSolutionFile } from ""../../../../out/FileSystem/Files/DotNetSolutionFile"";
	import { MessageReadSolutionIntoTreeView } from ""../../../../out/Messages/Read/MessageReadSolutionIntoTreeView"";
	import { onMount } from ""svelte"";
	import { MessageCategory } from ""../../../../out/Messages/MessageCategory"";
	import { MessageReadKind } from ""../../../../out/Messages/Read/MessageReadKind"";

	export let dotNetSolutionFiles: DotNetSolutionFile[];

	export let selectedDotNetSolutionFile: DotNetSolutionFile;

	function handleSelectOnChange() {
		if (selectedDotNetSolutionFile) {
			let messageReadSolutionIntoTreeView =
				new MessageReadSolutionIntoTreeView(selectedDotNetSolutionFile);

			tsVscode.postMessage({
				type: undefined,
				value: messageReadSolutionIntoTreeView,
			});
		}
	}

	onMount(async () => {
		window.addEventListener(""message"", async (event) => {
			const message = event.data;

			switch (message.messageCategory) {
				case MessageCategory.read:
					switch (message.messageReadKind) {
						case MessageReadKind.solutionIntoTreeView:
							selectedDotNetSolutionFile =
								message.dotNetSolutionFile;
							break;
						case MessageReadKind.messageReadUndefinedSolution:
							selectedDotNetSolutionFile =
								undefined;
							break;
					}
			}
		});
	});
</script>

<div>
	<select
		bind:value={selectedDotNetSolutionFile}
		on:change={handleSelectOnChange}
		on:keydown|stopPropagation
		class=""dni_select""
	>
		{#each dotNetSolutionFiles as solutionFile}
			<option value={solutionFile} class=""dni_option""
				>{solutionFile.absoluteFilePath.filenameWithExtension}</option
			>
		{/each}
	</select>
</div>

<style>
	.dni_select {
		color: var(--vscode-input-foreground);
		background-color: var(--vscode-input-background);
	}
</style>";
    }
}