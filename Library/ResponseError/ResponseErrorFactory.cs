﻿namespace CoreWebApi.Library
{
    public class ResponseErrorFactory
    {
        public static IResponseError getBadRequestError(string title) => new BadRequestError() { Title = title };

        public static IResponseError getNotFoundError(string title) => new NotFoundError() { Title = title };

        public static IResponseError getServiceUnavailableError(string title) => new ServiceUnavailableError() { Title = title };
    }
}
