using MediatR;
using TaskManager.Application.DTOs;

namespace TaskManager.Application.Queries;

public record GetAllTasksQuery() : IRequest<IEnumerable<TaskItemDto>>;