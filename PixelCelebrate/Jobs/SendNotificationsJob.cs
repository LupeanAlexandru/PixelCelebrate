using Quartz;

namespace PixelCelebrate.Jobs
{
    public class SendNotificationsJob : IJob
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<SendNotificationsJob> _logger;

        public SendNotificationsJob(IHttpClientFactory httpClientFactory, ILogger<SendNotificationsJob> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var client = _httpClientFactory.CreateClient();

            try
            {
                var response = await client.PostAsync("http://localhost:5277/api/user/send-notifications", null);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("SendBirthdayNotifications executed successfully at {time}", DateTime.UtcNow);
                }
                else
                {
                    _logger.LogWarning("SendBirthdayNotifications failed with status code: {status}", response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while executing SendBirthdayNotifications");
            }
        }
    }
}
