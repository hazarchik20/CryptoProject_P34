namespace AIChat.WebApi;

public interface ITool
{
    string Description { get; }
    string Schema { get; }
    string Execute(string input);
}