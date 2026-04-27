using MediatR;
using TaskManager.Application.Commands;
using TaskManager.Application.Interfaces;
using TaskManager.Domain.Interfaces;

namespace TaskManager.Application.Handlers;

public class CompleteTaskHandler : IRequestHandler<CompleteTaskCommand, bool>
{
    private readonly ITaskRepository _repository;
    private readonly IMessageService _messageService;

    public CompleteTaskHandler(ITaskRepository repository, IMessageService messageService)
    {
        _repository = repository;
        _messageService = messageService;
    }

    public async Task<bool> Handle(CompleteTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await _repository.GetByIdAsync(request.Id);
        if (task is null) return false;

        task.Complete();
        await _repository.UpdateAsync(task);

        await _messageService.PublishAsync("task.completed", new
        {
            task.Id,
            task.Title,
            task.Status,
            task.UpdatedAt
        });

        return true;
    }
}