using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureWorkflowCommon.Services
{
    public interface ITableStorageService
    {
        Task<string> GetStatusAsync(string id, CancellationToken cancellationToken = default);
        Task<bool> UpsertJobAsync(string id, string status, CancellationToken cancellationToken = default);
    }
}
