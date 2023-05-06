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
        private readonly IMapper mapper;
        private readonly IRepository<FileModel> repository;

        public FileService(IRepository<FileModel> repository, IMapper mapper)
        {
            this.mapper = mapper;
            this.repository = repository;
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
