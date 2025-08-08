using System.Text.Json.Serialization;

namespace Arian.Querium.Common.Results;

public class ErrorContainer(params IEnumerable<string> errorMessages)
{
    public ErrorContainer() : this([])
    {
    }

    private IList<string> _errorMessages = [.. errorMessages ?? []];

    [JsonPropertyName("messages")]
    public IReadOnlyList<string> Messages
    {
        get => _errorMessages.AsReadOnly();
        set => _errorMessages = [.. value ?? Enumerable.Empty<string>()];
    }

    public void AddError(string v)
    {
        if (!string.IsNullOrWhiteSpace(v))
        {
            _errorMessages.Add(v);
        }
    }
}
