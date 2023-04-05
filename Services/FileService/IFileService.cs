using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoreWebApi.Services
{
    public interface IFileService
    {
        Task<FileModelDto> CreateAsync(FileModelDto fileModelDto);

        Task<IEnumerable<FileModelDto>> UploadMultiFileAsync(List<IFormFile> files);

        Task Download(int id);
    }
}
