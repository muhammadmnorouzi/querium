using System.Net;
using System.Text.Json.Serialization;

namespace Arian.Quantiq.Domain.Common.Results;

public class ApplicationResult<TData> : Result<TData, ErrorContainer>
{
    public ApplicationResult() : base() { }

    [JsonPropertyName("httpStatusCode")]
    public HttpStatusCode HttpStatusCode { get; set; }

    public ApplicationResult(TData data, HttpStatusCode httpStatusCode) : base(data) => HttpStatusCode = httpStatusCode;

    public ApplicationResult(ErrorContainer error, HttpStatusCode httpStatusCode) : base(error) => HttpStatusCode = httpStatusCode;

    public static implicit operator ApplicationResult<TData>((TData Data, HttpStatusCode StatusCode) tuple)
    {
        return new(tuple.Data, tuple.StatusCode);
    }

    public static implicit operator ApplicationResult<TData>((ErrorContainer Error, HttpStatusCode StatusCode) tuple)
    {
        return new(tuple.Error, tuple.StatusCode);
    }
}
