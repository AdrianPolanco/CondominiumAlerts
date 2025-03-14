namespace CondominiumAlerts.Infrastructure.Services.AI.MessagesSummary;

public class Choice
{
    public Logprobs Logprobs { get; set; }
    public string FinishReason { get; set; }
    public string NativeFinishReason { get; set; }
    public int Index { get; set; }
    public Message Message { get; set; }
}

public class Content
{
    public double Logprob { get; set; }
    public string Token { get; set; }
    public List<object> Top_Logprobs { get; set; }
}

public class Logprobs
{
    public List<Content> Content { get; set; }
    public List<object> Refusal { get; set; }
}

public class Message
{
    public string Role { get; set; }
    public string Content { get; set; }
    public object Refusal { get; set; }
}

public class DeepSeekResponse
{
    public string Id { get; set; }
    public string Provider { get; set; }
    public string Model { get; set; }
    public string Object { get; set; }
    public int Created { get; set; }
    public List<Choice> Choices { get; set; }
    public Usage Usage { get; set; }
}

public class Usage
{
    public int Prompt_Tokens { get; set; }
    public int Completion_Tokens { get; set; }
    public int Total_Tokens { get; set; }
}