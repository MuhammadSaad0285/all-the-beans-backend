using AllTheBeans.Api.Contracts.Beans;
using AllTheBeans.Application.Errors;

namespace AllTheBeans.Api.Validation;

public static class BeanValidators
{
    public static void ValidateForCreate(BeanCreateRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ValidationException("Bean name is required.");
        if (request.Cost < 0)
            throw new ValidationException("Bean cost must be non-negative.");
        if (string.IsNullOrWhiteSpace(request.Currency))
            throw new ValidationException("Currency is required for cost.");
    }

    public static void ValidateForUpdate(BeanUpdateRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ValidationException("Bean name is required.");
        if (request.Cost < 0)
            throw new ValidationException("Bean cost must be non-negative.");
        if (string.IsNullOrWhiteSpace(request.Currency))
            throw new ValidationException("Currency is required for cost.");
    }
}
