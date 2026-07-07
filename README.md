# Controle de Gastos Residenciais

Sistema para controle de gastos residenciais com **cadastro de pessoas**,
**cadastro de transações** (receitas/despesas) e **consulta de totais** por pessoa.

Back-end em **.NET 8 / C#** (Web API), front-end em **React + TypeScript** (Vite)
e persistência em **SQLite** — os dados sobrevivem ao fechamento da aplicação.

---

## Funcionalidades

- **Pessoas**: criação, listagem e exclusão. Ao excluir uma pessoa, todas as suas
  transações são apagadas (exclusão em cascata).
- **Transações**: criação e listagem de receitas/despesas. Regras de negócio:
  - a pessoa informada precisa existir no cadastro;
  - pessoas **menores de 18 anos** só podem cadastrar **despesas**.
- **Totais**: para cada pessoa, total de receitas, despesas e saldo
  (receitas − despesas); ao final, o total geral consolidado de todas as pessoas.

---

## Tecnologias

| Camada    | Stack                                             |
| --------- | ------------------------------------------------- |
| Back-end  | .NET 8, ASP.NET Core Web API, Entity Framework Core |
| Banco     | SQLite (arquivo local, criado automaticamente)    |
| Front-end | React, TypeScript, Vite                           |
| Testes    | xUnit (SQLite em memória)                          |

---

## Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 18+](https://nodejs.org/) (inclui o npm)

---

## Como executar

O projeto tem duas partes que rodam em paralelo: a API (back-end) e o app web
(front-end). Abra dois terminais.

### 1. Back-end (API)

```bash
cd backend/ControleGastos.Api
dotnet run
```

A API sobe em `http://localhost:5285`. O banco SQLite (`controlegastos.db`) e suas
tabelas são criados automaticamente na primeira execução (via migrations do EF Core).
A documentação interativa (Swagger) fica em `http://localhost:5285/swagger`.

### 2. Front-end (app web)

```bash
cd frontend
npm install   # apenas na primeira vez
npm run dev
```

O app abre em `http://localhost:5173`. Em desenvolvimento, as chamadas a `/api`
são encaminhadas para a API pelo proxy do Vite (sem necessidade de configurar CORS).

---

## Testes (back-end)

```bash
cd backend
dotnet test
```

Os testes cobrem as regras de negócio (exclusão em cascata, restrição de menores
de idade, pessoa inexistente) e o cálculo dos totais. Rodam contra um banco SQLite
**em memória** — cada teste usa uma base isolada, sem dependências externas.

---

## Estrutura do projeto

```
.
├── backend/
│   ├── ControleGastos.sln
│   ├── ControleGastos.Api/          # Web API
│   │   ├── Controllers/             # Pessoas, Transações, Totais
│   │   ├── Data/                    # DbContext + migrations
│   │   ├── DTOs/                    # Contratos de entrada/saída
│   │   ├── Models/                  # Entidades de domínio
│   │   └── Program.cs               # Configuração (DI, EF Core, CORS)
│   └── ControleGastos.Tests/        # Testes xUnit
└── frontend/
    └── src/
        ├── api.ts                   # Cliente HTTP
        ├── types.ts                 # Tipos (espelham os DTOs)
        ├── format.ts                # Formatação de moeda (BRL)
        └── pages/                   # Páginas: Pessoas, Transações, Totais
```

---

## Endpoints da API

| Método   | Rota                    | Descrição                                  |
| -------- | ----------------------- | ------------------------------------------ |
| `GET`    | `/api/pessoas`          | Lista as pessoas                           |
| `GET`    | `/api/pessoas/{id}`     | Obtém uma pessoa pelo id                   |
| `POST`   | `/api/pessoas`          | Cria uma pessoa                            |
| `DELETE` | `/api/pessoas/{id}`     | Exclui uma pessoa (e suas transações)      |
| `GET`    | `/api/transacoes`       | Lista as transações                        |
| `GET`    | `/api/transacoes/{id}`  | Obtém uma transação pelo id                |
| `POST`   | `/api/transacoes`       | Cria uma transação (aplica as regras)      |
| `GET`    | `/api/totais`           | Totais por pessoa e total geral            |

Erros seguem o formato **ProblemDetails** (RFC 7807): validação de entrada vem
em `errors` (por campo) e regras de negócio/recurso inexistente em `detail`.

---

## Decisões de escopo

Alguns pontos foram mantidos simples de propósito, por não fazerem parte dos
requisitos do teste:

- **Idade armazenada como número inteiro** (e não data de nascimento). É o que o
  enunciado pede; a consequência é que a idade não é recalculada com o passar do
  tempo. Guardar a data de nascimento e derivar a idade seria o próximo passo
  natural caso a regra precisasse acompanhar o envelhecimento da pessoa.
- **Sem paginação** nas listagens. O volume esperado é pequeno; para bases
  grandes, as rotas de listagem ganhariam paginação/filtros.
- **Proxy do Vite apenas em desenvolvimento.** Em produção, o front-end
  consumiria a API por uma URL configurável (variável de ambiente) ou seria
  servido atrás do mesmo host; o CORS já é configurável via `appsettings`.
