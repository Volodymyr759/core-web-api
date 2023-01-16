using System;

namespace CoreWebApi.Controllers.ResponseError
{
    public class BadRequestError : IResponseError
    {
        public int Status { get; } = 400;
        public string Title { get; set; } = String.Empty;
    }
}
