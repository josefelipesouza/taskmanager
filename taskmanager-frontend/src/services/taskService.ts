import axios from 'axios';
import type { Task, CreateTaskRequest } from '../types/Task';

const api = axios.create({
  baseURL: 'http://localhost:5266/api',
});

export const taskService = {
  getAll: async (): Promise<Task[]> => {
    const response = await api.get('/tasks');
    return response.data;
  },

  getById: async (id: string): Promise<Task> => {
    const response = await api.get(`/tasks/${id}`);
    return response.data;
  },

  create: async (task: CreateTaskRequest): Promise<Task> => {
    const response = await api.post('/tasks', task);
    return response.data;
  },

  complete: async (id: string): Promise<void> => {
    await api.patch(`/tasks/${id}/complete`);
  },

  cancel: async (id: string): Promise<void> => {
    await api.patch(`/tasks/${id}/cancel`);
  },

  start: async (id: string): Promise<void> => { 
    await api.patch(`/tasks/${id}/start`);
  },
};