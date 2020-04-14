namespace Pacco.Services.Operations.Api.Services.TimeProviding
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public System.DateTime Now() => System.DateTime.UtcNow;
    }
}
