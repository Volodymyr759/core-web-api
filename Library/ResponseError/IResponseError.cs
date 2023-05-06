namespace CoreWebApi.Library
{
    public interface IResponseError
    {
        public int Status { get; }
        public string Title { get; set; }
    }
}
