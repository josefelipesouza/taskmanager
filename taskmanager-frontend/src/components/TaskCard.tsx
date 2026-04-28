import type { Task } from '../types/Task';
import { taskService } from '../services/taskService';

interface TaskCardProps {
  task: Task;
  onUpdate: () => void;
}

const statusColors: Record<string, string> = {
  Pending: 'bg-yellow-100 text-yellow-800',
  InProgress: 'bg-blue-100 text-blue-800',
  Completed: 'bg-green-100 text-green-800',
  Cancelled: 'bg-red-100 text-red-800',
};

const statusLabels: Record<string, string> = {
  Pending: 'Pendente',
  InProgress: 'Em Progresso',
  Completed: 'Concluída',
  Cancelled: 'Cancelada',
};

export function TaskCard({ task, onUpdate }: TaskCardProps) {
  const handleComplete = async () => {
    await taskService.complete(task.id);
    onUpdate();
  };

  const handleCancel = async () => {
    await taskService.cancel(task.id);
    onUpdate();
  };

  return (
    <div className="bg-white rounded-lg shadow p-4 border border-gray-200">
      <div className="flex justify-between items-start mb-2">
        <h3 className="text-lg font-semibold text-gray-800">{task.title}</h3>
        <span className={`text-xs px-2 py-1 rounded-full font-medium ${statusColors[task.status]}`}>
          {statusLabels[task.status]}
        </span>
      </div>
      <p className="text-gray-600 text-sm mb-3">{task.description}</p>
      <p className="text-gray-400 text-xs mb-4">
        Vencimento: {new Date(task.dueDate).toLocaleDateString('pt-BR')}
      </p>
      <div className="flex gap-2">
        {task.status === 'Pending' || task.status === 'InProgress' ? (
          <>
            <button
              onClick={handleComplete}
              className="bg-green-500 hover:bg-green-600 text-white text-sm px-3 py-1 rounded"
            >
              ✅ Concluir
            </button>
            <button
              onClick={handleCancel}
              className="bg-red-500 hover:bg-red-600 text-white text-sm px-3 py-1 rounded"
            >
              ❌ Cancelar
            </button>
          </>
        ) : null}
      </div>
    </div>
  );
}
