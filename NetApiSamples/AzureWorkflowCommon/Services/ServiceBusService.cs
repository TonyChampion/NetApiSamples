using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Azure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureWorkflowCommon.Services
{
    public class ServiceBusService : IServiceBusService
    {
       
        private ServiceBusSender _sender;
        private ITableStorageService _storageService;

        public ServiceBusService(IAzureClientFactory<ServiceBusClient> serviceBusClientFactory, 
                                 ITableStorageService tableStorageService) 
        {
            _storageService = tableStorageService ?? throw new ArgumentNullException(nameof(tableStorageService));  

            var client = serviceBusClientFactory.CreateClient("servicebusClient");
            _sender = client.CreateSender("vslive");
        }

        public async Task<string> AddJobAsync(string data, CancellationToken cancellationToken = default)
        {
            var message = new ServiceBusMessage(data);
            message.MessageId = Guid.NewGuid().ToString();

            await _storageService.UpsertJobAsync(message.MessageId, GlobalConstants.StatusQueued);

            await _sender.SendMessageAsync(message);

            return message.MessageId;

        }
    }
}
