using FluentAssertions;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;

namespace TaskManager.Tests.Domain;

public class TaskItemTests
{
    [Fact]
    public void TaskItem_DeveCriarComSucesso()
    {
        // Arrange
        var title = "Tarefa teste";
        var description = "Descrição teste";
        var dueDate = DateTime.UtcNow.AddDays(1);

        // Act
        var task = new TaskItem(title, description, dueDate);

        // Assert
        task.Id.Should().NotBeEmpty();
        task.Title.Should().Be(title);
        task.Description.Should().Be(description);
        task.Status.Should().Be(TaskItemStatus.Pending);
        task.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void TaskItem_DeveLancarExcecao_QuandoTituloVazio()
    {
        // Act
        var act = () => new TaskItem("", "Descrição", DateTime.UtcNow.AddDays(1));

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("Título não pode ser vazio.");
    }

    [Fact]
    public void TaskItem_DeveLancarExcecao_QuandoDataNoPassado()
    {
        // Act
        var act = () => new TaskItem("Título", "Descrição", DateTime.UtcNow.AddDays(-1));

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("Data de vencimento não pode ser no passado.");
    }

    [Fact]
    public void TaskItem_DeveCompletar_QuandoPendente()
    {
        // Arrange
        var task = new TaskItem("Título", "Descrição", DateTime.UtcNow.AddDays(1));

        // Act
        task.Complete();

        // Assert
        task.Status.Should().Be(TaskItemStatus.Completed);
        task.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void TaskItem_DeveLancarExcecao_AoCompletarTarefaCancelada()
    {
        // Arrange
        var task = new TaskItem("Título", "Descrição", DateTime.UtcNow.AddDays(1));
        task.Cancel();

        // Act
        var act = () => task.Complete();

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Tarefa cancelada não pode ser concluída.");
    }

    [Fact]
    public void TaskItem_DeveCancelar_QuandoPendente()
    {
        // Arrange
        var task = new TaskItem("Título", "Descrição", DateTime.UtcNow.AddDays(1));

        // Act
        task.Cancel();

        // Assert
        task.Status.Should().Be(TaskItemStatus.Cancelled);
        task.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void TaskItem_DeveLancarExcecao_AoCancelarTarefaConcluida()
    {
        // Arrange
        var task = new TaskItem("Título", "Descrição", DateTime.UtcNow.AddDays(1));
        task.Complete();

        // Act
        var act = () => task.Cancel();

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Tarefa concluída não pode ser cancelada.");
    }

    [Fact]
    public void TaskItem_DeveIniciar_QuandoPendente()
    {
        // Arrange
        var task = new TaskItem("Título", "Descrição", DateTime.UtcNow.AddDays(1));

        // Act
        task.Start();

        // Assert
        task.Status.Should().Be(TaskItemStatus.InProgress);
        task.UpdatedAt.Should().NotBeNull();
    }
}