using Azure.Data.Tables;
using Azure.Messaging.ServiceBus;
using AzureWorkflowCommon.Models;
using Microsoft.Extensions.Azure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureWorkflowCommon.Services
{
    public class TableStorageService : ITableStorageService
    {
        private readonly TableClient _tableClient;

        private const string _primaryKey = "primary";

        public TableStorageService(IAzureClientFactory<TableServiceClient> tableServiceClientFactory) 
        {
            var tableServiceClient = tableServiceClientFactory.CreateClient("tableStorageClient");
            _tableClient = tableServiceClient.GetTableClient("vslive");

        }

        public async Task<string> GetStatusAsync(string id, CancellationToken cancellationToken = default)
        {
           var response = await _tableClient.GetEntityAsync<JobStatus>(_primaryKey, id, cancellationToken: cancellationToken);

            return response.Value == null ? GlobalConstants.StatusNotFound : response.Value.Status;
        }

        public async Task<bool> UpsertJobAsync(string id, string status, CancellationToken cancellationToken = default)
        {
            var jobStatus = new JobStatus()
            {
                PartitionKey = _primaryKey,
                RowKey = id,
                Status = status
            };

            var response = await _tableClient.UpsertEntityAsync<JobStatus>(jobStatus, cancellationToken: cancellationToken);

            return !response.IsError;
        }
    }
}
