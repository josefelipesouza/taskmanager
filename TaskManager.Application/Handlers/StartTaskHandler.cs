using MediatR;
using TaskManager.Application.Commands;
using TaskManager.Application.Interfaces;
using TaskManager.Domain.Interfaces;

namespace TaskManager.Application.Handlers;

public class StartTaskHandler : IRequestHandler<StartTaskCommand, bool>
{
    private readonly ITaskRepository _repository;
    private readonly IMessageService _messageService;

    public StartTaskHandler(ITaskRepository repository, IMessageService messageService)
    {
        _repository = repository;
        _messageService = messageService;
    }

    public async Task<bool> Handle(StartTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await _repository.GetByIdAsync(request.Id);
        if (task is null) return false;

        task.Start();
        await _repository.UpdateAsync(task);

        await _messageService.PublishAsync("task.started", new
        {
            task.Id,
            task.Title,
            task.Status,
            task.UpdatedAt
        });

        return true;
    }
}