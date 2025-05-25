namespace ProjectApprovalAPI.Common
{
    public class ApiError(string message)
    {
        public string Message { get; set; } = message;
    }

}
