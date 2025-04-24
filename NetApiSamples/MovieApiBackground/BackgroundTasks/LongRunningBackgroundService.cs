using CommonLibrary.Services;

namespace MovieApiBackground.BackgroundTasks
{
    public class LongRunningBackgroundService(IBackgroundTaskQueue taskQueue, ITMDBService tmdbService) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var workItem = await taskQueue.DequeueAsync(stoppingToken);
                try
                {
                    workItem.Progress = 0.5;
                    Task.Delay(10000).Wait();

                    workItem.Results = await tmdbService.GetMoviesAsync(1);
                    workItem.Progress = 1.0;
                }
                catch (Exception ex)
                {
                    // Handle the exception (e.g., log it)
                    Console.WriteLine($"Error processing work item: {ex.Message}");
                }
            }
        }
    }
}
