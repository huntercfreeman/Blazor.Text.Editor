namespace BlazorTextEditor.Tests.TestDataFolder;

public static partial class TestData
{
    public static class Json
    {
        /// <summary>
        /// launchSettings.json of a Blazor ServerSide application
        /// </summary>
        public const string EXAMPLE_TEXT_LAUNCH_SETTINGS = @"{
  ""iisSettings"": {
        ""windowsAuthentication"": false,
        ""anonymousAuthentication"": true,
        ""iisExpress"": {
            ""applicationUrl"": ""http://localhost:62895"",
            ""sslPort"": 44378
        }
    },
    ""profiles"": {
        ""BlazorTextEditor.Demo.ServerSide"": {
            ""commandName"": ""Project"",
            ""dotnetRunMessages"": true,
            ""launchBrowser"": true,
            ""applicationUrl"": ""https://localhost:7250;http://localhost:5106"",
            ""environmentVariables"": {
                ""ASPNETCORE_ENVIRONMENT"": ""Development""
            }
        },
        ""IIS Express"": {
            ""commandName"": ""IISExpress"",
            ""launchBrowser"": true,
            ""environmentVariables"": {
                ""ASPNETCORE_ENVIRONMENT"": ""Development""
            }
        }
    }
}
";
    }
}