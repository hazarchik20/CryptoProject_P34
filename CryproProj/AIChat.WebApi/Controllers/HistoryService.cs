namespace AIChat.WebApi.Controllers;

public class HistoryService
{
    private readonly List<HistoryItem> _history = new List<HistoryItem>();

    public void SaveHistory(string message, string answer)
    {
        _history.Add(new HistoryItem(message, true));
        _history.Add(new HistoryItem(answer, false));
    }
    
    public IEnumerable<HistoryItem> GetHistory() => _history;
}

public record HistoryItem(string Message, bool IsUser);