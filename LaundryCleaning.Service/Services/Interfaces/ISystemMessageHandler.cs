namespace LaundryCleaning.Service.Services.Interfaces
{
    public interface ISystemMessageHandler<in T>
    {
        Task HandleAsync(T message, CancellationToken cancellationToken);
    }
}
