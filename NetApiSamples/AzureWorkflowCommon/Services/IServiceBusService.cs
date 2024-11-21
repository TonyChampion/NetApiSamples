using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureWorkflowCommon.Services
{
    public interface IServiceBusService
    {
        Task<string> AddJobAsync(string data, CancellationToken cancellationToken = default);
    }
}
