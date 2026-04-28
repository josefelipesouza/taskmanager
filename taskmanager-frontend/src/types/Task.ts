export interface Task {
  id: string;
  title: string;
  description: string;
  dueDate: string;
  status: 'Pending' | 'InProgress' | 'Completed' | 'Cancelled';
  createdAt: string;
  updatedAt?: string;
}

export interface CreateTaskRequest {
  title: string;
  description: string;
  dueDate: string;
}