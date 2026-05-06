namespace LIMTIC.Application.DTOs
{
    public class Result<T>
    {
        public bool Success { get; private set; }
        public T? Data { get; private set; }
        public string? Message { get; private set; }
        public Dictionary<string, IEnumerable<string>>? ValidationErrors { get; private set; }
        public bool IsFailure => !Success;

        public static Result<T> SuccessResult(T data, string? message = null) =>
            new Result<T> { Success = true, Data = data, Message = message };

        public static Result<T> FailureResult(string error) =>
            new Result<T> { Success = false, Message = error };

        public static Result<T> ValidationFailureResult(Dictionary<string, IEnumerable<string>> validationErrors) =>
            new Result<T> { Success = false, Message = "Validation Message", ValidationErrors = validationErrors };
    }
}