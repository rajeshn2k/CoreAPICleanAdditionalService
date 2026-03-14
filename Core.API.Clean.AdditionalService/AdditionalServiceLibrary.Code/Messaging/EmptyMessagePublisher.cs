using Core.Library.ArivuTharavuThalam;

namespace Core.API.AdditionalServiceLibrary
{
    public class EmptyMessagePublisher : IMessagePublisher
    {
        public Task PublishAsync<TMessageContract>(TMessageContract message, string messageType, string messageAction, CancellationToken cancellationToken)
        {
            Logger.LogInformation(new string[] { "--> Not Connected to any Publish Messaging Queues" });
            return Task.CompletedTask;
        }

        public Task PublishAsync<TMessageContract>(IEnumerable<TMessageContract> message, string messageType, string messageAction, CancellationToken cancellationToken)
        {
            Logger.LogInformation(new string[] { "--> Not Connected to any Publish Messaging Queues" });
            return Task.CompletedTask;
        }
    }
}
