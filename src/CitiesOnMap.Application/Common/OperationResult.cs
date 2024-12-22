namespace CitiesOnMap.Application.Common;

public class OperationResult
{
    public OperationResult(bool succeeded)
    {
        Succeeded = succeeded;
        ResultType type = succeeded ? ResultType.Succeeded : ResultType.Undefined;
        Details = new ResultDetails(type);
    }

    public OperationResult(bool succeeded, ResultType type)
    {
        Succeeded = succeeded;
        Details = new ResultDetails(type);
    }

    public bool Succeeded { get; set; }
    public ResultDetails Details { get; set; }
}

public class OperationResult<T> : OperationResult
{
    public OperationResult(bool succeeded) : base(succeeded)
    {
    }

    public OperationResult(bool succeeded, ResultType type) : base(succeeded, type)
    {
    }

    public OperationResult(bool succeeded, T payload) : this(succeeded)
    {
        Payload = payload;
    }

    public OperationResult(bool succeeded, ResultType type, T payload) : this(succeeded, type)
    {
        Payload = payload;
    }

    public T? Payload { get; set; }
}