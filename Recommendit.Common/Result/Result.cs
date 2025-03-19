namespace Recommendit.Result
{
    public class Result
    {
        public  Result(bool isSuccess, Error error)
        {
            if (isSuccess && error != Error.None ||
                !isSuccess && error == Error.None)
            {
                throw new ArgumentException("Invalid error", nameof(error));
            }

            IsSuccess = isSuccess;
            Error = error;
        }

        public bool IsSuccess { get; }

        public bool IsFailure => !IsSuccess;

        public Error Error { get; }

        public static Result Success() => new(true, Error.None);

        public static Result Failure(Error error) => new(false, error);
    }

    public class Result<T> : Result
    {
        private Result(T value, bool isSuccess, Error error) : base(isSuccess, error)
        {
            Value = value;
        }

        public T Value { get; }

        public static Result<T> Success(T value) => new(value, true, Error.None);

        public static Result<T> Failure(Error error) => new(default, false, error);
    }

    public sealed record Error(string Code, string Description)
    {
        public static readonly Error None = new(string.Empty, string.Empty);
    }
}