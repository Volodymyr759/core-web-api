namespace CoreWebApi.Library.ResponseError
{
    public class ServiceUnavailableError : IResponseError
    {
        public int Status { get; } = 503;
        public string Title { get; set; } = string.Empty;
    }
}
