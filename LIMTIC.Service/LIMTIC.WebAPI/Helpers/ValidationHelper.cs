using FluentValidation.Results;

namespace LIMTIC.WebAPI.Helpers
{
    public static class ValidationHelper
    {
        public static Dictionary<string, IEnumerable<string>> ParseValidationErrors(ValidationResult result)
        {
            return result.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage)
                );
        }
    }
}
