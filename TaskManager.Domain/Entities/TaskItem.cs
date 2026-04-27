using TaskManager.Domain.Enums;

namespace TaskManager.Domain.Entities;

public class TaskItem
{
    public Guid Id { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public DateTime DueDate { get; private set; }
    public TaskItemStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private TaskItem()
    {
        Title = null!;
        Description = null!;
    }

    public TaskItem(string title, string description, DateTime dueDate)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Título não pode ser vazio.");

        if (dueDate < DateTime.UtcNow)
            throw new ArgumentException("Data de vencimento não pode ser no passado.");

        Id = Guid.NewGuid();
        Title = title;
        Description = description;
        DueDate = dueDate;
        Status = TaskItemStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    public void Complete()
    {
        if (Status == TaskItemStatus.Cancelled)
            throw new InvalidOperationException("Tarefa cancelada não pode ser concluída.");

        Status = TaskItemStatus.Completed;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        if (Status == TaskItemStatus.Completed)
            throw new InvalidOperationException("Tarefa concluída não pode ser cancelada.");

        Status = TaskItemStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Start()
    {
        if (Status != TaskItemStatus.Pending)
            throw new InvalidOperationException("Apenas tarefas pendentes podem ser iniciadas.");

        Status = TaskItemStatus.InProgress;
        UpdatedAt = DateTime.UtcNow;
    }
}

