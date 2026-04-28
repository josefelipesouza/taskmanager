# 📋 TaskManager

API REST para gerenciamento de tarefas, desenvolvida em ASP.NET Core, utilizando Clean Architecture, MediatR, Entity Framework Core, RabbitMQ para mensageria e React + TypeScript + Tailwind CSS no frontend.

---

## 🚀 Tecnologias Utilizadas

### Backend
- **.NET 8** — Plataforma principal
- **ASP.NET Core Web API** — Camada HTTP
- **Entity Framework Core 8** — ORM para persistência
- **PostgreSQL** — Banco de dados relacional
- **MediatR 14** — Implementação do padrão CQRS
- **FluentValidation** — Validação de dados de entrada
- **RabbitMQ** — Mensageria para eventos assíncronos
- **Swagger (OpenAPI)** — Documentação da API

### Frontend
- **React 18** — Biblioteca de interface
- **TypeScript** — Tipagem estática
- **Tailwind CSS 4** — Estilização utilitária
- **Vite** — Ambiente de build
- **Axios** — Cliente HTTP

### Infraestrutura
- **Docker** — Containerização
- **Docker Compose** — Orquestração dos serviços
- **GitHub Actions** — CI/CD

---

## 📂 Estrutura do Projeto

```
taskmanager/
│
├── TaskManager.API/                  # Camada de apresentação
│   ├── Controllers/                  # Endpoints HTTP
│   │   └── TasksController.cs
│   ├── Program.cs                    # Configuração da aplicação
│   ├── appsettings.json              # Configurações
│   └── Dockerfile                    # Container da API
│
├── TaskManager.Application/          # Casos de uso / Regras de aplicação
│   ├── Commands/                     # Comandos (escrita)
│   │   ├── CreateTaskCommand.cs
│   │   ├── CompleteTaskCommand.cs
│   │   └── CancelTaskCommand.cs
│   ├── Queries/                      # Consultas (leitura)
│   │   ├── GetAllTasksQuery.cs
│   │   └── GetTaskByIdQuery.cs
│   ├── Handlers/                     # Handlers MediatR (CQRS)
│   │   ├── CreateTaskHandler.cs
│   │   ├── CompleteTaskHandler.cs
│   │   ├── CancelTaskHandler.cs
│   │   ├── GetAllTasksHandler.cs
│   │   └── GetTaskByIdHandler.cs
│   ├── Validators/                   # Validações FluentValidation
│   │   └── CreateTaskValidator.cs
│   ├── Interfaces/                   # Contratos
│   │   └── IMessageService.cs
│   └── DTOs/                         # Objetos de transferência
│       └── TaskItemDto.cs
│
├── TaskManager.Domain/               # Domínio puro
│   ├── Entities/
│   │   └── TaskItem.cs               # Entidade principal
│   ├── Enums/
│   │   └── TaskItemStatus.cs         # Status da tarefa
│   └── Interfaces/
│       └── ITaskRepository.cs        # Contrato do repositório
│
├── TaskManager.Infrastructure/       # Infraestrutura
│   ├── Persistence/
│   │   └── AppDbContext.cs           # Contexto do EF Core
│   ├── Repositories/
│   │   └── TaskRepository.cs         # Implementação do repositório
│   ├── Messaging/
│   │   └── RabbitMqService.cs        # Serviço RabbitMQ
│   └── Migrations/                   # Migrations do EF Core
│
├── TaskManager.Authentication/       # Estrutura para autenticação futura (JWT)
│
├── TaskManager.Tests/                # Testes automatizados
│   ├── Domain/
│   │   └── TaskItemTests.cs          # Testes da entidade
│   ├── Application/
│   │   └── CreateTaskHandlerTests.cs # Testes dos handlers
│   └── Validators/
│       └── CreateTaskValidatorTests.cs
│
├── taskmanager-frontend/             # Frontend React
│   ├── src/
│   │   ├── components/
│   │   │   ├── TaskCard.tsx          # Card de tarefa
│   │   │   └── TaskForm.tsx          # Formulário de criação
│   │   ├── services/
│   │   │   └── taskService.ts        # Comunicação com a API
│   │   ├── types/
│   │   │   └── Task.ts               # Tipos TypeScript
│   │   └── App.tsx                   # Componente principal
│   └── Dockerfile                    # Container do frontend
│
├── .github/
│   └── workflows/
│       └── ci.yml                    # Pipeline GitHub Actions
│
└── docker-compose.yml                # Orquestração dos serviços
```

---

## 🔑 Responsabilidades das Camadas

| Camada | Responsabilidade |
|---|---|
| **API** | Receber requisições HTTP e retornar respostas |
| **Application** | Orquestrar regras de negócio (CQRS com MediatR) |
| **Domain** | Entidades e regras de negócio puras |
| **Infrastructure** | Banco de dados, RabbitMQ, repositórios |
| **Authentication** | Estrutura para JWT (implementação futura) |
| **Tests** | Testes unitários com xUnit, Moq e FluentAssertions |

---

## ⚙️ Pré-requisitos

- [.NET SDK 8](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 20+](https://nodejs.org/)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [Git](https://git-scm.com/)

Verifique as instalações:

```bash
dotnet --version   # 8.0.x
node --version     # v20.x
docker --version   # 20+
```

---

## ▶️ Como Rodar o Projeto

### 🐳 Opção 1 — Docker Compose (recomendado)

Sobe todos os serviços automaticamente:

```bash
git clone https://github.com/josefelipesouza/taskmanager.git
cd taskmanager
docker-compose up --build -d
```

Aguarde todos os containers iniciarem e acesse:

| Serviço | URL |
|---|---|
| 🌐 Frontend | http://localhost:3000 |
| ⚙️ API Swagger | http://localhost:5266/swagger |
| 🐰 RabbitMQ Painel | http://localhost:15672 |

---

### 🛠️ Opção 2 — Rodar manualmente (desenvolvimento)

#### 1️⃣ Iniciar os serviços de infraestrutura

```bash
docker-compose up postgres rabbitmq -d
```

#### 2️⃣ Iniciar o Backend

Abra um terminal e execute:

```bash
cd taskmanager
dotnet restore
dotnet run --project TaskManager.API/TaskManager.API.csproj
```

A API estará disponível em:
```
http://localhost:5266
```

Swagger (documentação interativa):
```
http://localhost:5266/swagger
```

#### 3️⃣ Iniciar o Frontend

Abra **outro terminal** e execute:

```bash
cd taskmanager/taskmanager-frontend
npm install
npm run dev
```

O frontend estará disponível em:
```
http://localhost:5173
```

---

## 🗄️ Banco de Dados e Migrations

O projeto utiliza Entity Framework Core com PostgreSQL.

### Criar uma nova migration

```bash
dotnet ef migrations add NomeDaMigration \
  --project TaskManager.Infrastructure/TaskManager.Infrastructure.csproj \
  --startup-project TaskManager.API/TaskManager.API.csproj
```

### Aplicar migrations

```bash
dotnet ef database update \
  --project TaskManager.Infrastructure/TaskManager.Infrastructure.csproj \
  --startup-project TaskManager.API/TaskManager.API.csproj
```

> As migrations ficam em `TaskManager.Infrastructure/Migrations/`

---

## 📦 Endpoints da API

### Tarefas

| Método | Endpoint | Descrição |
|---|---|---|
| `GET` | `/api/tasks` | Listar todas as tarefas |
| `GET` | `/api/tasks/{id}` | Buscar tarefa por ID |
| `POST` | `/api/tasks` | Criar nova tarefa |
| `PATCH` | `/api/tasks/{id}/complete` | Concluir tarefa |
| `PATCH` | `/api/tasks/{id}/cancel` | Cancelar tarefa |

### Exemplo de criação de tarefa

```json
POST /api/tasks
{
  "title": "Minha tarefa",
  "description": "Descrição da tarefa",
  "dueDate": "2026-12-31T00:00:00Z"
}
```

Resposta `201 Created`:
```json
{
  "id": "guid",
  "title": "Minha tarefa",
  "description": "Descrição da tarefa",
  "dueDate": "2026-12-31T00:00:00Z",
  "status": "Pending",
  "createdAt": "2026-04-27T00:00:00Z",
  "updatedAt": null
}
```

---

## 🐰 Mensageria — RabbitMQ

Eventos publicados automaticamente na fila `task-events`:

| Evento | Quando é disparado |
|---|---|
| `task.created` | Ao criar uma tarefa |
| `task.completed` | Ao concluir uma tarefa |
| `task.cancelled` | Ao cancelar uma tarefa |

Acesse o painel de gerenciamento:
```
http://localhost:15672
Usuário: guest
Senha: guest
```

---

## 🧪 Testes

### Rodar todos os testes

```bash
dotnet test TaskManager.Tests/TaskManager.Tests.csproj
```

### Testes disponíveis (14 no total)

| Categoria | Quantidade | O que testa |
|---|---|---|
| **Domain** | 7 | Regras de negócio da entidade TaskItem |
| **Application** | 1 | Handler de criação com Mock do repositório |
| **Validators** | 4 | Validações do FluentValidation |

---

## 🔄 CI/CD — GitHub Actions

O pipeline roda automaticamente a cada push na branch `main`:

```
Push → Build → Testes → Docker Build
```

Acesse os resultados em:
```
https://github.com/josefelipesouza/taskmanager/actions
```

---

## 🏗️ Padrões e Princípios Utilizados

- **Clean Architecture** — Separação clara de responsabilidades
- **CQRS** — Commands e Queries separados via MediatR
- **SOLID** — Princípios aplicados em todas as camadas
- **Repository Pattern** — Abstração do acesso a dados
- **DTO Pattern** — Objetos de transferência entre camadas
- **AAA Pattern** — Testes organizados em Arrange, Act, Assert

---

## 👨‍💻 Autor

Projeto desenvolvido para fins de estudo e evolução em arquitetura backend com .NET, mensageria e frontend moderno.

---

## ✅ Status

| Funcionalidade | Status |
|---|---|
| Clean Architecture | ✔️ |
| CQRS com MediatR | ✔️ |
| FluentValidation | ✔️ |
| PostgreSQL + EF Core | ✔️ |
| RabbitMQ | ✔️ |
| Testes xUnit | ✔️ |
| CI/CD GitHub Actions | ✔️ |
| Frontend React + TypeScript + Tailwind | ✔️ |
| Docker Compose | ✔️ |
| Autenticação JWT | 🔲 Planejado |