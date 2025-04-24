using CommonLibrary.Models.TMDB;
using System.Collections.Concurrent;

namespace MovieApiBackground.BackgroundTasks
{
    public class BackgroundTaskItem
    {
        public string Name { get; set; }
        public double Progress { get; set; }
        public int MaxCount { get; set; }

        public MovieListPage? Results { get; set; } // Assuming this is the type of the result
    }

    public interface IBackgroundTaskQueue
    {
        string QueueBackgroundWorkItem();
        Task<BackgroundTaskItem> DequeueAsync(CancellationToken cancellationToken);
        BackgroundTaskItem GetBackgroundTaskItem(string name);
    }

    public class BackgroundTaskQueue : IBackgroundTaskQueue
    {
        private readonly SemaphoreSlim _signal = new(0);
        private readonly ConcurrentQueue<BackgroundTaskItem> _workItems = new();
        private readonly ConcurrentDictionary<string, BackgroundTaskItem> _processingItems = new();

        public string QueueBackgroundWorkItem()
        {
            BackgroundTaskItem workItem = new BackgroundTaskItem
            {
                Name = Guid.NewGuid().ToString(),
                Progress = 0,
                MaxCount = 100 // Example max count
            };

            if (workItem == null) throw new ArgumentNullException(nameof(workItem));

            _workItems.Enqueue(workItem);

            _signal.Release(); // Signal that a new item is available

            return workItem.Name;
        }

        public async Task<BackgroundTaskItem> DequeueAsync(CancellationToken cancellationToken)
        {
            await _signal.WaitAsync(cancellationToken);

            _workItems.TryDequeue(out var workItem);

            if (workItem != null)
            {
                _processingItems.AddOrUpdate(workItem.Name, workItem, (key, existingItem) =>
                {
                    existingItem.Progress = workItem.Progress;
                    existingItem.Results = workItem.Results;
                    return existingItem;
                });
            }

            return workItem!;
        }

        public BackgroundTaskItem GetBackgroundTaskItem(string name)
        {
            // Check if the item is being or has been processed
            if (_processingItems.TryGetValue(name, out var item))
            {
                // If completed, remove it from the processing items
                if (item.Progress >= 1.0)
                {
                    _processingItems.Remove(name, out _); // Remove it from the completed items
                }
                return item;
            }

            // If not completed, check the work items queue
            var workItem = _workItems.FirstOrDefault(item => item.Name == name);

            if (workItem != null)
            {
                return workItem;
            }

            return null;
        }
    }
}