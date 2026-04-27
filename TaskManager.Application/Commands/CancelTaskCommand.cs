using MediatR;

namespace TaskManager.Application.Commands;

public record CancelTaskCommand(Guid Id) : IRequest<bool>;