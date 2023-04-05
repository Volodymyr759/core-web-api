using AutoMapper;
using CoreWebApi.Data;
using CoreWebApi.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoreWebApi.Services
{
    public class FileService : IFileService
    {
        private readonly IRepository<FileModel> repository;
        private readonly IMapper mapper;

        public FileService(IRepository<FileModel> repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public async Task<FileModelDto> CreateAsync(FileModelDto fileModelDto)
        {
            var fileModel = mapper.Map<FileModel>(fileModelDto);

            return mapper.Map<FileModelDto>(await repository.CreateAsync(fileModel));
        }

        public Task<IEnumerable<FileModelDto>> UploadMultiFileAsync(List<IFormFile> files)
        {
            throw new NotImplementedException();
        }

        public Task Download(int id)
        {
            throw new NotImplementedException();
        }
    }
}
