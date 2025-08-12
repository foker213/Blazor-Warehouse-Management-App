# Warehouse Management System (Blazor WASM + WebAPI)
Решение тестового задания "Приложение для управления складом", реализующей принципы DDD и Луковой архитектуры.

### Ключевые архитектурные принципы
- **Луковая архитектура** с четким разделением слоев:
  - `Client` - Клиентский интерфейс (Blazor WASM)
  - `Server` - Серверная часть (WebApi)
  - `Application` - Бизнес-логика и сценарии использования
  - `DataBase` - Реализация репозиториев, работа и конфигурирование БД
  - `Domain` - Хранилище доменных сущностей
  - `Contracts` - Контрактные модели используемые клиентской и серверной частью для общения

### Коммуникация между слоями
```mermaid
graph LR
    %% Client Side
    A[Blazor Client] -- HTTP REST --> B[WebAPI]
    A --> C[Contracts]
    
    %% Server Side
    B --> D[DataBase]
    D --> E[Application]
    E --> F[Domain]
    E --> C
    
    %% Style
    classDef client fill:#1e88e5,stroke:#0d47a1,color:white;
    classDef server fill:#e53935,stroke:#b71c1c,color:white;
    classDef shared fill:#43a047,stroke:#2e7d32,color:white;
    class A client;
    class B,D,E,F server;
    class C shared;
    
    %% Legend
    subgraph Legend
        H[Client]:::client
        I[Server]:::server
        J[Shared Contracts]:::shared
    end
```
# Инструкция по запуску проекта Server

## Подключение к PostgreSQL

1. Откройте `appsettings.json`
2. Найдите блок `ConnectionStrings`
3. Замените параметры подключения:
"DbContext": {
    "ConnectionString": "Server=*адрес сервера (localhost)*;Port=*порт*;Database=Warehouse;User Id="Имя пользователя";Password=*Пароль*;Pooling=true;"
  }

## Запуск проекта

Запустите проект Server локально и перейдите по выданному https адрес: https://localhost:7085 

Сервер настраивается для обслуживания Blazor WebAssembly-клиента, но само выполнение кода происходит в браузере пользователя. База данных создается автоматически с помощью миграций и заполняется тестовыми данными с помощью сидов.

## Технологический стек
- `Клиент` - Blazor WASM;
- `Сервер` - ASP.NET Core WebAPI;
- `OРМ` - EF Core;
- `База данных` - PostgreSQL;
- `Маппинг` - Mapster;
- `Обработка ошибок` - ErrorOr;
- `Клиент` - Blazor WASM;
- `Паттерны` - UnitOfWork, Repository;
- `Язык` - C#
