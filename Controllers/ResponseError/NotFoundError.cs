using System;

namespace CoreWebApi.Controllers.ResponseError
{
    public class NotFoundError : IResponseError
    {
        public int Status { get; } = 404;
        public string Title { get; set; } = String.Empty;
    }
}
