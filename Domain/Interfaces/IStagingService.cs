using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IStagingService
    {
        Task SaveAsync<T>(string fileName, List<T> data);
        Task<List<T>> LoadAsync<T>(string fileName);
    }
}
