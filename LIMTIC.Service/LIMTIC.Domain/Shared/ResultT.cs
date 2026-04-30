namespace LIMTIC.Domain.Shared
{
    public class ResultT<TValue> : Result
    {
        protected internal ResultT(TValue? value, bool isSuccess, string? errorMessage = null) : base(isSuccess, errorMessage)
        {
            Value = value;
        }

        public TValue? Value { get; }

        public static ResultT<TValue> Success(TValue value) => new ResultT<TValue>(value, true);
        public static ResultT<TValue> Failure(string errorMessage) => new ResultT<TValue>(default, false, errorMessage);
    }
}
