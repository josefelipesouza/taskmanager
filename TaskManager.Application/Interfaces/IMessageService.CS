namespace TaskManager.Application.Interfaces;

public interface IMessageService
{
    Task PublishAsync<T>(string eventName, T message);
}