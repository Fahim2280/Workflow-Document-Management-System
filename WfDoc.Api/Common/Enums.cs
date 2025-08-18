namespace WfDoc.Api.Common;

public enum AccessLevel
{
    ReadOnly = 0,
    ReadWrite = 1
}

public enum WorkflowType
{
    Order = 0,
    Pool = 1
}

public enum DocumentStatus
{
    Pending = 0,
    Approved = 1,
    Rejected = 2
}

public static class PolicyNames
{
    public const string ReadWriteOnly = "ReadWriteOnly";
}

