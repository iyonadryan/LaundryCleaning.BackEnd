namespace LaundryCleaning.Service.Services.Interfaces
{
    public interface IPublisherService
    {
        Task PublishAsync<T>(T message, CancellationToken cancellationToken);
    }
}
