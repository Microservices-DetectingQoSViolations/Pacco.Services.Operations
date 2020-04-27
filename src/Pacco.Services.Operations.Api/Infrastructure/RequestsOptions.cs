namespace Pacco.Services.Operations.Api.Infrastructure
{
    public class RequestsOptions
    {
        public int ExpirySeconds { get; set; }
        public string CheckRequestCronExpression { get; set; }
        public int MaxHandlingOperationSeconds { get; set; }
    }
}