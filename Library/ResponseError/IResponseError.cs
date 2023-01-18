namespace CoreWebApi.Library.ResponseError
{
    public interface IResponseError
    {
        public int Status { get; }
        public string Title { get; set; }
    }
}
