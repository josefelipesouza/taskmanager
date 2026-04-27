using MediatR;
using TaskManager.Application.DTOs;

namespace TaskManager.Application.Queries;

public record GetTaskByIdQuery(Guid Id) : IRequest<TaskItemDto?>;