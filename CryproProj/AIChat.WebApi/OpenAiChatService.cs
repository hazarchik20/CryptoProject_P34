using System.ClientModel;
using System.Text.Json;
using AIChat.WebApi.Controllers;
using OpenAI;
using OpenAI.Chat;

namespace AIChat.WebApi;

public class OpenAiChatService
{
    private readonly OpenAIClient _openAIClient;
    private readonly PromptProvider _promptProvider = new();
    private readonly HistoryService _historyService;
    private readonly string _model;
    private readonly ITool[] _tools;

    public OpenAiChatService(IConfiguration configuration, HistoryService historyService)
    {
        _historyService = historyService;
        _tools = new ITool[] { new TestTool() };
        _openAIClient = new OpenAIClient(new ApiKeyCredential(configuration["OpenAi:ApiKey"]), 
            new OpenAIClientOptions
            {
                Endpoint = new Uri(configuration["OpenAi:Endpoint"])
            });
        _model = configuration["OpenAi:Model"];
    }
    
    public async Task<string> Message(string message)
    {
        var client = _openAIClient.GetChatClient(_model);
        
        // system message
        var systemMessage = ChatMessage.CreateSystemMessage(_promptProvider.GetPrompt());

        var userMessage = ChatMessage.CreateUserMessage(message);
        
        // chat history
        var history = _historyService.GetHistory()
            .Select(h => h.IsUser 
                ? ChatMessage.CreateUserMessage(h.Message)
                : (ChatMessage)ChatMessage.CreateAssistantMessage(h.Message));
        
        var options = new ChatCompletionOptions
        {
            Temperature = 0,
            ToolChoice = ChatToolChoice.CreateAutoChoice(),
            AllowParallelToolCalls = true,
            MaxOutputTokenCount = 2000
        };

        // tools
        foreach (var tool in _tools)
        {
            options.Tools.Add(ChatTool.CreateFunctionTool(tool.GetType().Name.ToLower(), tool.Description, BinaryData.FromString(tool.Schema)));
        }

        List<ChatMessage> messages = [systemMessage, userMessage, ..history];

        while (true)
        {
            var response = await client.CompleteChatAsync(messages, options);

            if (response.Value.ToolCalls.Count > 0)
            {
                foreach (var tool in response.Value.ToolCalls)
                {
                    var toolCall = _tools.First(t => t.GetType().Name.ToLower() == tool.FunctionName.ToLower());
                    toolCall.Execute(tool.FunctionArguments.ToString());

                    messages.Add(ChatMessage.CreateToolMessage(tool.Id, tool.FunctionArguments.ToString()));
                }
            }
            else
            {
                _historyService.SaveHistory(message, response.Value.Content.First().Text);
        
                return JsonSerializer.Serialize(response.Value);
            }
        }
    }
    
    // message streaming
    public async IAsyncEnumerable<string> MessageStream(string message)
    {
        var client = _openAIClient.GetChatClient(_model);
        
        var systemMessage = ChatMessage.CreateSystemMessage(_promptProvider.GetPrompt());
        var userMessage = ChatMessage.CreateUserMessage(message);

        ChatMessage[] messages = [systemMessage, userMessage];

        var options = new ChatCompletionOptions
        {
            Temperature = 0,
        };
        
        await foreach(var response in client.CompleteChatStreamingAsync(messages, options))
        {
            foreach (var update in response.ContentUpdate)
            {
                if(!string.IsNullOrEmpty(update.Text))
                    yield return update.Text;
            }
        }
    }
}