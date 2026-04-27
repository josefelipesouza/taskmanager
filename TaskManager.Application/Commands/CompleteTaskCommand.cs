using MediatR;

namespace TaskManager.Application.Commands;

public record CompleteTaskCommand(Guid Id) : IRequest<bool>;