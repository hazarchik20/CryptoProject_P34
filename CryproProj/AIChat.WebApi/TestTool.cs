using Newtonsoft.Json.Linq;

namespace AIChat.WebApi;

public class TestTool : ITool
{
    public string Description => "Use this tool is your response contains c# code";

    public string Schema => """
        {
            "type": "object",
            "properties": {
                "code": { 
                    "type": "string",
                    "description": "C# code from your response"
                }
            }
        }
    """;
    public string Execute(string input)
    {
        var json = JObject.Parse(input);
        
        var code = json["code"];

        if (code != null)
        {
            File.WriteAllText("C:\\ITSTEP\\CryptoProj\\CryproProj\\AIChat.WebApi\\temp\\test.md", $"```csharp\n{code}\n```");
        }
        
        Console.WriteLine("From tool: " + input);
        return input;
    }
}