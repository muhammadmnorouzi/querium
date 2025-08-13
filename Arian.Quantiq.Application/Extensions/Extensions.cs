using Arian.Quantiq.Domain.Common.Results;

namespace Arian.Quantiq.Application.Extensions;

public static class Extensions
{
    public static ErrorContainer ToErrorContainer(this FluentValidation.Results.ValidationResult validationResult)
    {
        return validationResult == null
            ? throw new ArgumentNullException(nameof(validationResult))
            : new ErrorContainer(validationResult.Errors.Select(e => e.ErrorMessage));
    }
}