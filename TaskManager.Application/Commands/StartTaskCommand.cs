using MediatR;

namespace TaskManager.Application.Commands;

public record StartTaskCommand(Guid Id) : IRequest<bool>;