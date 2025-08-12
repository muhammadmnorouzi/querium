using System.Text.Json.Serialization;

namespace Arian.Quantiq.Domain.Common.Results;

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


    public void AddErrorIf(bool condition,string v)
    {
        if (!string.IsNullOrWhiteSpace(v) && condition)
        {
            _errorMessages.Add(v);
        }
    }

    public bool IsEmpty()
    {
        return _errorMessages.Count == 0;
    }

    public bool IsNotEmpty()
    {
        return _errorMessages.Count >= 1;
    }
}
