namespace LIMTIC.Domain.Shared
{
    public class Result<T>
    {
        public bool Success { get; private set; }
        public T? Data { get; private set; }
        public string? Error { get; private set; }
        public bool IsFailure => !Success;

        public static Result<T> SuccessResult(T data) =>
            new Result<T> { Success = true, Data = data };

        public static Result<T> FailureResult(string error) =>
            new Result<T> { Success = false, Error = error };
    }
}