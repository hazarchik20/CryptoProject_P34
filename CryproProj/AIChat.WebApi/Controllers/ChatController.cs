using Microsoft.AspNetCore.Mvc;

namespace AIChat.WebApi.Controllers;

[ApiController]
[Route("api/v1/chat")]
public class ChatController : ControllerBase
{
    private readonly OpenAiChatService _openAiChatService;

    public ChatController(OpenAiChatService openAiChatService)
    {
        _openAiChatService = openAiChatService;
    }

    [HttpPost("message")]
    public async Task<IActionResult> PostMessage(ChatRequest request)
    {
        var response = await _openAiChatService.Message(request.Message);
        return Ok(response);
    }
    
    [HttpPost("stream")]
    public async Task<IActionResult> PostStream(ChatRequest request)
    {
        Response.ContentType = "text/event-stream";
        Response.Headers.Append("Cache-Control", "no-cache");
        Response.Headers.Append("Connection", "keep-alive");

        await Response.Body.FlushAsync();

        await foreach (var response in _openAiChatService.MessageStream(request.Message))
        {
            var ssePayload = $"data: {response}\n\n"; 
        
            await Response.WriteAsync(ssePayload);
            await Response.Body.FlushAsync();
        }
        
        Response.Body.Close();
        
        return Ok();
    }
}

public class ChatRequest
{
    public string Message { get; set; }
}