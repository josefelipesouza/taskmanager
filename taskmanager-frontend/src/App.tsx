import { useState, useEffect } from 'react';
import type { Task } from './types/Task';
import { taskService } from './services/taskService';
import { TaskCard } from './components/TaskCard';
import { TaskForm } from './components/TaskForm';

function App() {
  const [tasks, setTasks] = useState<Task[]>([]);
  const [loading, setLoading] = useState(true);
  const [filter, setFilter] = useState('All');

  const loadTasks = async () => {
    setLoading(true);
    const data = await taskService.getAll();
    setTasks(data);
    setLoading(false);
  };

  useEffect(() => {
    loadTasks();
  }, []);

  const filteredTasks = filter === 'All'
    ? tasks
    : tasks.filter(t => t.status === filter);

  return (
    <div className="min-h-screen bg-gray-100">
      {/* Header */}
      <header className="bg-blue-600 text-white py-4 px-6 shadow">
        <h1 className="text-2xl font-bold">📋 Task Manager</h1>
        <p className="text-blue-200 text-sm">Gerencie suas tarefas</p>
      </header>

      <main className="max-w-6xl mx-auto px-4 py-8">
        <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">

          {/* Formulário */}
          <div className="lg:col-span-1">
            <TaskForm onTaskCreated={loadTasks} />
          </div>

          {/* Lista de tarefas */}
          <div className="lg:col-span-2">
            {/* Filtros */}
            <div className="flex gap-2 mb-4 flex-wrap">
              {['All', 'Pending', 'InProgress', 'Completed', 'Cancelled'].map(status => (
                <button
                  key={status}
                  onClick={() => setFilter(status)}
                  className={`px-3 py-1 rounded-full text-sm font-medium ${
                    filter === status
                      ? 'bg-blue-600 text-white'
                      : 'bg-white text-gray-600 border border-gray-300'
                  }`}
                >
                  {status === 'All' ? 'Todas' : status}
                </button>
              ))}
            </div>

            {/* Tasks */}
            {loading ? (
              <div className="text-center text-gray-500 py-12">Carregando...</div>
            ) : filteredTasks.length === 0 ? (
              <div className="text-center text-gray-400 py-12">
                Nenhuma tarefa encontrada.
              </div>
            ) : (
              <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                {filteredTasks.map(task => (
                  <TaskCard key={task.id} task={task} onUpdate={loadTasks} />
                ))}
              </div>
            )}
          </div>
        </div>
      </main>
    </div>
  );
}

export default App;