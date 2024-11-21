using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using AzureWorkflowCommon;
using AzureWorkflowCommon.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace AzureWorkflowFunction
{
    public class Processor
    {
        private readonly ILogger<Processor> _logger;
        private readonly ITableStorageService _storageService;

        public Processor(ILogger<Processor> logger, ITableStorageService storageService)
        {
            _logger = logger;
            _storageService = storageService ?? throw new ArgumentNullException(nameof(storageService));
        }

        [Function(nameof(Processor))]
        public async Task Run(
            [ServiceBusTrigger("vslive", Connection = "ServiceBusConnection")]
            ServiceBusReceivedMessage message,
            ServiceBusMessageActions messageActions)
        {
            _logger.LogInformation("Message ID: {id}", message.MessageId);

            await _storageService.UpsertJobAsync(message.MessageId, GlobalConstants.StatusProcessing);

            Thread.Sleep(3000);

            // Complete the message
            await messageActions.CompleteMessageAsync(message);

            await _storageService.UpsertJobAsync(message.MessageId, GlobalConstants.StatusCompleted);
        }
    }
}
