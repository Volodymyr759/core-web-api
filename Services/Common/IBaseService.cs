using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreWebApi.Services
{
    public interface IBaseService<T> where T: class
    {
        Task<T> GetByIdAsync(int id);
    }
}
