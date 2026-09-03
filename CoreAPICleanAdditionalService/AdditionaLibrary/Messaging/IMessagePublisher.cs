namespace Core.Library.Clean.AdditionalService
{
    public interface IMessagePublisher
    {
        /// <summary>
        /// if "where TMessageContract : class" needed change 
        /// should also be done at implementaion class method and also may not be able to push primitive types like string, int
        /// </summary>
        /// <typeparam name="TMessageContract"></typeparam>
        /// <param name="message"></param>
        /// <param name="messageType"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task PublishAsync<TMessageContract>(TMessageContract message, string messageType, string messageAction, CancellationToken cancellationToken);
        //where TMessageContract : class;

        /// <summary>
        /// PublishWithSessionAsync
        /// if "where TMessageContract : class" needed change 
        /// should also be done at implementaion class method and also may not be able to push primitive types like string, int
        /// </summary>
        /// <typeparam name="TMessageContract"></typeparam>
        /// <param name="message"></param>
        /// <param name="messageType"></param>
        /// <param name="messageAction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task PublishAsync<TMessageContract>(IEnumerable<TMessageContract> message, string messageType, string messageAction, CancellationToken cancellationToken);
    }
}