using MediatR;
using TaskManager.Application.DTOs;

namespace TaskManager.Application.Commands;

public record CreateTaskCommand(
    string Title,
    string Description,
    DateTime DueDate
) : IRequest<TaskItemDto>;