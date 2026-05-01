using MediatR;
using TaskManager.Application.Commands;
using TaskManager.Application.DTOs;
using TaskManager.Application.Interfaces;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Interfaces;

namespace TaskManager.Application.Handlers;

public class CreateTaskHandler : IRequestHandler<CreateTaskCommand, TaskItemDto>
{
    private readonly ITaskRepository _repository;
    private readonly IMessageService _messageService;

    public CreateTaskHandler(ITaskRepository repository, IMessageService messageService)
    {
        _repository = repository;
        _messageService = messageService;
    }

    public async Task<TaskItemDto> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        var task = new TaskItem(request.Title, request.Description, request.DueDate);
        await _repository.AddAsync(task);

        await _messageService.PublishAsync("task.created", new
        {
            task.Id,
            task.Title,
            task.Description,
            task.Status,
            task.CreatedAt
        });

        return new TaskItemDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            DueDate = task.DueDate,
            Status = task.Status.ToString(),
            CreatedAt = task.CreatedAt,
            UpdatedAt = task.UpdatedAt
        };
    }
}