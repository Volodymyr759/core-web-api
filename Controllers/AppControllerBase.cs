using CoreWebApi.Library;
using Microsoft.AspNetCore.Mvc;

namespace CoreWebApi.Controllers
{
    public abstract class AppControllerBase : ControllerBase
    {
        public IResponseError responseBadRequestError = ResponseErrorFactory.getBadRequestError("Wrong given data.");
        public IResponseError responseNotFoundError = ResponseErrorFactory.getNotFoundError("Not Found.");
        public IResponseError responseServiceUnavailableError = ResponseErrorFactory.getServiceUnavailableError("Service Unavailable.");
    }
}
