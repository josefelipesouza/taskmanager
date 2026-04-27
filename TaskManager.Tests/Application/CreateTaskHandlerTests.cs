using FluentAssertions;
using Moq;
using TaskManager.Application.Commands;
using TaskManager.Application.Handlers;
using TaskManager.Application.Interfaces;
using TaskManager.Domain.Interfaces;
using TaskManager.Domain.Entities;

namespace TaskManager.Tests.Application;

public class CreateTaskHandlerTests
{
    private readonly Mock<ITaskRepository> _repositoryMock;
    private readonly Mock<IMessageService> _messageServiceMock;
    private readonly CreateTaskHandler _handler;

    public CreateTaskHandlerTests()
    {
        _repositoryMock = new Mock<ITaskRepository>();
        _messageServiceMock = new Mock<IMessageService>();
        _handler = new CreateTaskHandler(_repositoryMock.Object, _messageServiceMock.Object);
    }

    [Fact]
    public async Task Handle_DeveCriarTarefa_EPublicarEvento()
    {
        // Arrange
        var command = new CreateTaskCommand(
            "Tarefa teste",
            "Descrição teste",
            DateTime.UtcNow.AddDays(1)
        );

        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<TaskItem>()))
            .Returns(Task.CompletedTask);

        _messageServiceMock
            .Setup(m => m.PublishAsync(It.IsAny<string>(), It.IsAny<object>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be(command.Title);
        result.Description.Should().Be(command.Description);
        result.Status.Should().Be("Pending");

        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<TaskItem>()), Times.Once);
        _messageServiceMock.Verify(m => m.PublishAsync("task.created", It.IsAny<object>()), Times.Once);
    }
}