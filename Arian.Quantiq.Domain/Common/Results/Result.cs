using System.Text.Json.Serialization;

namespace Arian.Quantiq.Domain.Common.Results;

public abstract class Result<TData, TError>
{
    [JsonPropertyName("data")]
    public TData? Data { get; set; }

    [JsonPropertyName("error")]
    public TError? Error { get; set; }

    [JsonPropertyName("isSuccess")]
    public bool IsSuccess { get; set; }
    public bool IsFailure => !IsSuccess;

    protected Result()
    {
        // Default constructor for deserialization.
        // Values will be set by the deserializer.
    }

    protected Result(TData data)
    {
        if (data is null)
        {
            throw new ArgumentNullException(nameof(data), "Result data cannot be null");
        }

        Data = data;
        IsSuccess = true;
        Error = default;
    }

    protected Result(TError error)
    {
        if (error is null)
        {
            throw new ArgumentNullException(nameof(error), "Result error cannot be null");
        }

        Error = error;
        IsSuccess = false;
        Data = default;
    }
}