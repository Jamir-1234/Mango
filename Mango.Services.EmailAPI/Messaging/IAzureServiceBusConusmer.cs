namespace Mango.Services.EmailAPI.Messaging
{
    public interface IAzureServiceBusConusmer
    {
        Task Start();
        Task Stop();
    }
}
