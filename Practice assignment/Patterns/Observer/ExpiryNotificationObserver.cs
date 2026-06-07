using Practice_assignment.Models;

namespace Practice_assignment.Patterns.Observer
{
    public class ExpiryNotificationObserver : IContractObserver
    {
        private readonly ILogger<ExpiryNotificationObserver> _logger;
        public ExpiryNotificationObserver(ILogger<ExpiryNotificationObserver> logger)
            => _logger = logger;

        public Task OnContractStatusChangedAsync(ContractStatusChangedEvent e)
        {
            if (e.NewStatus == ContractStatus.Expired)
                _logger.LogWarning(
                    "[NOTIFY] Contract {Id} has EXPIRED. Client should be notified.", e.ContractId);
            return Task.CompletedTask;
        }
    }
}
