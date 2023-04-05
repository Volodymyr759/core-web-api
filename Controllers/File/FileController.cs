using CoreWebApi.Library.ResponseError;
using CoreWebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;

namespace CoreWebApi.Controllers.File
{
    [ApiController]
    [Authorize(Roles = "Admin")]
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public class FileController : ControllerBase
    {
        private readonly IFileService fileService;
        private readonly IResponseError responseBadRequestError;

        public FileController(IFileService fileService)
        {
            this.fileService = fileService;
            responseBadRequestError = ResponseErrorFactory.getBadRequestError("Wrong file data."); ;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Upload([FromForm] IFormFile uploadedFile)
        {
            if (uploadedFile == null) return BadRequest(responseBadRequestError);

            string path = "C:\\Users\\user\\source\\repos\\CoreWebApi\\wwwroot\\Uploads\\" + uploadedFile.FileName;

            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                await uploadedFile.CopyToAsync(fileStream);// save file to folder Uploads in the wwwroot - catalog
            }

            var fileDto = new FileModelDto
            {
                Name = uploadedFile.FileName,
                Path = path,
                FullPath = "https://volodymyr57.somee.com" + path,
                Thumbnail = "https://volodymyr57.somee.com" + path, // todo: create and save the small compressed file
                Type = "",
                Extention = "",
                Mime = uploadedFile.ContentType,
                Size = uploadedFile.Length,
                CreatedAt = DateTime.Now.ToUniversalTime()
            };

            var fileModelDto = await fileService.CreateAsync(fileDto);

            return Created("/api/file/upload", fileModelDto);
        }

    }
}
