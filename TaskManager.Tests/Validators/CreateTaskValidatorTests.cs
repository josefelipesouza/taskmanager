using FluentAssertions;
using TaskManager.Application.Commands;
using TaskManager.Application.Validators;

namespace TaskManager.Tests.Validators;

public class CreateTaskValidatorTests
{
    private readonly CreateTaskValidator _validator = new();

    [Fact]
    public async Task Validator_DevePassar_QuandoCommandValido()
    {
        // Arrange
        var command = new CreateTaskCommand(
            "Título válido",
            "Descrição válida",
            DateTime.UtcNow.AddDays(1)
        );

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validator_DeveFalhar_QuandoTituloVazio()
    {
        // Arrange
        var command = new CreateTaskCommand("", "Descrição", DateTime.UtcNow.AddDays(1));

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Title");
    }

    [Fact]
    public async Task Validator_DeveFalhar_QuandoDataNoPassado()
    {
        // Arrange
        var command = new CreateTaskCommand("Título", "Descrição", DateTime.UtcNow.AddDays(-1));

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "DueDate");
    }

    [Fact]
    public async Task Validator_DeveFalhar_QuandoTituloMaiorQue100Caracteres()
    {
        // Arrange
        var command = new CreateTaskCommand(
            new string('A', 101),
            "Descrição",
            DateTime.UtcNow.AddDays(1)
        );

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Title");
    }
}