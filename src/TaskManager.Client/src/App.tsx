import { FormEvent, useEffect, useMemo, useState } from 'react';
import { Check, Edit2, Filter, Plus, RefreshCw, Trash2, X } from 'lucide-react';
import './styles.css';

type TaskStatus = 'ToDo' | 'InProgress' | 'Done';

type TaskItem = {
  id: number;
  title: string;
  description?: string | null;
  status: TaskStatus;
  dueDate?: string | null;
  projectId: number;
};

type Project = {
  id: number;
  name: string;
  description?: string | null;
  createdAt: string;
  tasks: TaskItem[];
};

type ProjectForm = {
  name: string;
  description: string;
};

type TaskForm = {
  title: string;
  description: string;
  status: TaskStatus;
  dueDate: string;
};

const apiBaseUrl = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5135';
const statuses: TaskStatus[] = ['ToDo', 'InProgress', 'Done'];
const emptyProjectForm: ProjectForm = { name: '', description: '' };
const emptyTaskForm: TaskForm = { title: '', description: '', status: 'ToDo', dueDate: '' };

async function request<T>(path: string, options?: RequestInit): Promise<T> {
  const response = await fetch(`${apiBaseUrl}${path}`, {
    headers: { 'Content-Type': 'application/json', ...options?.headers },
    ...options,
  });

  if (!response.ok) {
    const message = await response.text();
    throw new Error(message || `Request failed with ${response.status}`);
  }

  if (response.status === 204) {
    return undefined as T;
  }

  return response.json() as Promise<T>;
}

function toDateInput(value?: string | null) {
  if (!value) return '';
  return value.slice(0, 10);
}

export default function App() {
  const [projects, setProjects] = useState<Project[]>([]);
  const [selectedProjectId, setSelectedProjectId] = useState<number | null>(null);
  const [selectedProject, setSelectedProject] = useState<Project | null>(null);
  const [projectForm, setProjectForm] = useState<ProjectForm>(emptyProjectForm);
  const [taskForm, setTaskForm] = useState<TaskForm>(emptyTaskForm);
  const [editingProjectId, setEditingProjectId] = useState<number | null>(null);
  const [editingTaskId, setEditingTaskId] = useState<number | null>(null);
  const [statusFilter, setStatusFilter] = useState<TaskStatus | 'All'>('All');
  const [filteredTasks, setFilteredTasks] = useState<TaskItem[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const selectedTasks = selectedProject?.tasks ?? [];
  const visibleProjectTasks = useMemo(() => {
    if (statusFilter === 'All') return selectedTasks;
    return selectedTasks.filter((task) => task.status === statusFilter);
  }, [selectedTasks, statusFilter]);

  async function loadProjects(preferredId?: number | null) {
    setIsLoading(true);
    setError(null);
    try {
      const data = await request<Project[]>('/api/projects');
      setProjects(data);
      const nextSelectedId = preferredId ?? selectedProjectId ?? data[0]?.id ?? null;
      setSelectedProjectId(nextSelectedId);
      if (nextSelectedId) {
        await loadProjectDetails(nextSelectedId);
      } else {
        setSelectedProject(null);
      }
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Could not load projects.');
    } finally {
      setIsLoading(false);
    }
  }

  async function loadProjectDetails(projectId: number) {
    const data = await request<Project>(`/api/projects/${projectId}/details`);
    setSelectedProject(data);
  }

  useEffect(() => {
    void loadProjects();
  }, []);

  async function handleProjectSubmit(event: FormEvent) {
    event.preventDefault();
    setError(null);
    try {
      const body = JSON.stringify({
        name: projectForm.name,
        description: projectForm.description || null,
      });

      if (editingProjectId) {
        await request(`/api/projects/${editingProjectId}`, { method: 'PUT', body });
        setEditingProjectId(null);
        await loadProjects(editingProjectId);
      } else {
        const created = await request<Project>('/api/projects', { method: 'POST', body });
        await loadProjects(created.id);
      }

      setProjectForm(emptyProjectForm);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Project save failed.');
    }
  }

  function startProjectEdit(project: Project) {
    setEditingProjectId(project.id);
    setProjectForm({ name: project.name, description: project.description ?? '' });
  }

  async function deleteProject(projectId: number) {
    setError(null);
    try {
      await request(`/api/projects/${projectId}`, { method: 'DELETE' });
      await loadProjects(projectId === selectedProjectId ? null : selectedProjectId);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Project delete failed.');
    }
  }

  async function selectProject(projectId: number) {
    setSelectedProjectId(projectId);
    setError(null);
    try {
      await loadProjectDetails(projectId);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Could not load project details.');
    }
  }

  async function handleTaskSubmit(event: FormEvent) {
    event.preventDefault();
    if (!selectedProjectId) return;

    setError(null);
    try {
      const body = JSON.stringify({
        title: taskForm.title,
        description: taskForm.description || null,
        status: taskForm.status,
        dueDate: taskForm.dueDate ? new Date(taskForm.dueDate).toISOString() : null,

      });

      if (editingTaskId) {
        await request(`/api/tasks/${editingTaskId}`, { method: 'PUT', body });
        setEditingTaskId(null);
      } else {
        await request('/api/projects/' + selectedProjectId + '/tasks', { method: 'POST', body });
      }

      setTaskForm(emptyTaskForm);
      await loadProjectDetails(selectedProjectId);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Task save failed.');
    }
  }

  function startTaskEdit(task: TaskItem) {
    setEditingTaskId(task.id);
    setTaskForm({
      title: task.title,
      description: task.description ?? '',
      status: task.status,
      dueDate: toDateInput(task.dueDate),
    });
  }

  async function deleteTask(taskId: number) {
    if (!selectedProjectId) return;
    setError(null);
    try {
      await request(`/api/tasks/${taskId}`, { method: 'DELETE' });
      await loadProjectDetails(selectedProjectId);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Task delete failed.');
    }
  }

  async function updateTaskStatus(taskId: number, status: TaskStatus) {
    if (!selectedProjectId) return;
    setError(null);
    try {
      await request(`/api/tasks/${taskId}/status`, {
        method: 'PATCH',
        body: JSON.stringify({ status }),
      });
      await loadProjectDetails(selectedProjectId);
      if (statusFilter !== 'All') await loadFilteredTasks(statusFilter);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Status update failed.');
    }
  }

  async function loadFilteredTasks(status: TaskStatus | 'All') {
    setStatusFilter(status);
    if (status === 'All') {
      setFilteredTasks([]);
      return;
    }

    setError(null);
    try {
      const data = await request<TaskItem[]>(`/api/tasks/status/${status}`);
      setFilteredTasks(data);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Could not filter tasks.');
    }
  }

  return (
    <main className="app-shell">
      <section className="topbar">
        <div>
          <p className="eyebrow">ElectroPi Technical Task</p>
          <h1>Task Manager</h1>
        </div>
        <button className="icon-button" type="button" onClick={() => void loadProjects(selectedProjectId)} title="Refresh data">
          <RefreshCw size={18} />
          Refresh
        </button>
      </section>

      {error && (
        <div className="alert" role="alert">
          {error}
        </div>
      )}

      <div className="workspace">
        <aside className="panel projects-panel">
          <div className="panel-header">
            <h2>Projects</h2>
            <span>{projects.length}</span>
          </div>

          <form className="stack" onSubmit={handleProjectSubmit}>
            <input
              value={projectForm.name}
              onChange={(event) => setProjectForm((form) => ({ ...form, name: event.target.value }))}
              placeholder="Project name"
              required
              maxLength={100}
            />
            <textarea
              value={projectForm.description}
              onChange={(event) => setProjectForm((form) => ({ ...form, description: event.target.value }))}
              placeholder="Description"
              maxLength={500}
            />
            <div className="button-row">
              <button type="submit">
                {editingProjectId ? <Check size={16} /> : <Plus size={16} />}
                {editingProjectId ? 'Save project' : 'Add project'}
              </button>
              {editingProjectId && (
                <button className="secondary" type="button" onClick={() => { setEditingProjectId(null); setProjectForm(emptyProjectForm); }}>
                  <X size={16} />
                  Cancel
                </button>
              )}
            </div>
          </form>

          <div className="project-list">
            {isLoading ? <p className="muted">Loading projects...</p> : null}
            {projects.map((project) => (
              <article key={project.id} className={project.id === selectedProjectId ? 'project-item active' : 'project-item'}>
                <button type="button" onClick={() => void selectProject(project.id)}>
                  <strong>{project.name}</strong>
                  <small>{new Date(project.createdAt).toLocaleDateString()}</small>
                </button>
                <div className="item-actions">
                  <button className="ghost" type="button" onClick={() => startProjectEdit(project)} title="Edit project">
                    <Edit2 size={15} />
                  </button>
                  <button className="ghost danger" type="button" onClick={() => void deleteProject(project.id)} title="Delete project">
                    <Trash2 size={15} />
                  </button>
                </div>
              </article>
            ))}
          </div>
        </aside>

        <section className="panel details-panel">
          {selectedProject ? (
            <>
              <div className="panel-header split">
                <div>
                  <h2>{selectedProject.name}</h2>
                  <p>{selectedProject.description || 'No description'}</p>
                </div>
                <span>{selectedProject.tasks.length} tasks</span>
              </div>

              <div className="filter-row">
                <Filter size={16} />
                {(['All', ...statuses] as const).map((status) => (
                  <button
                    key={status}
                    type="button"
                    className={statusFilter === status ? 'chip selected' : 'chip'}
                    onClick={() => void loadFilteredTasks(status)}
                  >
                    {status}
                  </button>
                ))}
              </div>

              {statusFilter !== 'All' && (
                <p className="muted compact">Global filter result: {filteredTasks.length} task(s) with {statusFilter} status.</p>
              )}

              <form className="task-form" onSubmit={handleTaskSubmit}>
                <input
                  value={taskForm.title}
                  onChange={(event) => setTaskForm((form) => ({ ...form, title: event.target.value }))}
                  placeholder="Task title"
                  required
                  maxLength={150}
                />
                <select value={taskForm.status} onChange={(event) => setTaskForm((form) => ({ ...form, status: event.target.value as TaskStatus }))}>
                  {statuses.map((status) => <option key={status} value={status}>{status}</option>)}
                </select>
                <input
                  type="date"
                  value={taskForm.dueDate}
                  onChange={(event) => setTaskForm((form) => ({ ...form, dueDate: event.target.value }))}
                />
                <textarea
                  value={taskForm.description}
                  onChange={(event) => setTaskForm((form) => ({ ...form, description: event.target.value }))}
                  placeholder="Task description"
                  maxLength={1000}
                />
                <div className="button-row">
                  <button type="submit">
                    {editingTaskId ? <Check size={16} /> : <Plus size={16} />}
                    {editingTaskId ? 'Save task' : 'Add task to ' + selectedProject.name}
                  </button>
                  {editingTaskId && (
                    <button className="secondary" type="button" onClick={() => { setEditingTaskId(null); setTaskForm(emptyTaskForm); }}>
                      <X size={16} />
                      Cancel
                    </button>
                  )}
                </div>
              </form>

              <div className="task-list">
                {visibleProjectTasks.map((task) => (
                  <article key={task.id} className="task-item">
                    <div>
                      <strong>{task.title}</strong>
                      <p>{task.description || 'No description'}</p>
                      <small>{task.dueDate ? `Due ${new Date(task.dueDate).toLocaleDateString()}` : 'No due date'}</small>
                    </div>
                    <div className="task-actions">
                      <select value={task.status} onChange={(event) => void updateTaskStatus(task.id, event.target.value as TaskStatus)}>
                        {statuses.map((status) => <option key={status} value={status}>{status}</option>)}
                      </select>
                      <button className="ghost" type="button" onClick={() => startTaskEdit(task)} title="Edit task">
                        <Edit2 size={15} />
                      </button>
                      <button className="ghost danger" type="button" onClick={() => void deleteTask(task.id)} title="Delete task">
                        <Trash2 size={15} />
                      </button>
                    </div>
                  </article>
                ))}
                {!visibleProjectTasks.length && <p className="muted">No tasks to show.</p>}
              </div>
            </>
          ) : (
            <div className="empty-state">Create a project to start adding tasks.</div>
          )}
        </section>
      </div>
    </main>
  );
}



