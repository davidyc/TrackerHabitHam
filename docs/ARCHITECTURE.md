# Архитектура Tracker HabiHam

## Обзор

```
┌─────────────────────┐     HTTPS      ┌──────────────────────┐
│  HabihamTrackerApp  │◄──────────────►│  TrackerHabiHamApi   │
│  (MAUI)             │                │  (ASP.NET Core)      │
└─────────────────────┘                └──────────┬───────────┘
                                                  │
                    ┌─────────────────────────────┼─────────────────────────────┐
                    │                             │                             │
                    ▼                             ▼                             ▼
           ┌────────────────┐           ┌─────────────────┐           ┌─────────────────┐
           │  SQLite        │           │  MongoDB        │           │  Google Sheets   │
           │  (MounthWeight)│           │  (JsonStore)    │           │  (Исходные данные)│
           └────────────────┘           └─────────────────┘           └─────────────────┘
                    ▲                                                           ▲
                    │                                                           │
                    └───────────────────────────────────────────────────────────┘
                                        SyncService
                    ┌───────────────────────────────────────────────────────────┐
                    │                    Telegram Bot                            │
                    └───────────────────────────────────────────────────────────┘
```

---

## Слой API (Controllers)

| Контроллер | Назначение |
|------------|------------|
| **WeightController** | CRUD веса, обращается к WeightService |
| **WeightAnalysisController** | Аналитика, WeightAnalysisService |
| **SyncController** | Синхронизация Sheets → БД, SyncService |
| **TelegramController** | Webhook Telegram, оркестрация сервисов |
| **GoogleAuthController** | Проверка credentials и чтение периода из Sheets |
| **JsonStoreController** | Хранение JSON в MongoDB, JsonStoreService |

---

## Сервисы

### WeightService

- **Зависимости**: ApplicationDbContext, IGoogleSheetsService
- **Методы**:
  - `GetFromPeriod(start?, end?)` — выборка за период
  - `UpdateWeightAsync(date, weight)` — upsert + запись в Google Sheets
  - `UpsertManyAsync(items)` — массовый upsert для SyncService

### WeightAnalysisService

- **Зависимости**: ApplicationDbContext
- **Методы**:
  - `GetSummaryAsync(start?, end?)` — min, max, average, startValue, endValue, change
  - `GetSeriesAsync(start?, end?)` — серия точек для графика
- **Логика**: парсинг веса (поддержка `,` и `.`), фильтрация невалидных значений

### SyncService

- **Зависимости**: IGoogleSheetsService, IWeightService
- **Метод**: `SyncByYearAsync(year)` — читает год из Google Sheets, сравнивает с БД, записывает отличия через `UpsertManyAsync`

### GoogleSheetsService

- **Конфигурация**: SpreadsheetId, CredentialsFilePath
- **Методы**:
  - `WriteNumberToTodayRow(number)` — запись в ячейку текущей даты
  - `GetMounth(year, month)` — чтение колонки месяца (B–M → 1–12)
  - `CredentialExists()` — проверка файла credentials

### TelegramService

- **Конфигурация**: Telegram:BotToken
- **Методы**: SendMessageAsync, SendMenuAsync, SetMyCommandsAsync, IsValidNumber, SendValidationErrorAsync

### JsonStoreService

- **Зависимости**: IMongoDatabase
- **Коллекция**: `json_items`
- **Методы**: CreateAsync (upsert), GetAsync, CheckConnectionAsync

---

## Потоки данных

### Ввод веса (API/Telegram/MAUI)

1. Клиент → `PUT /api/Weight` или Telegram (число)
2. WeightService.UpdateWeightAsync → SQLite (upsert)
3. WeightService → GoogleSheetsService.WriteNumberToTodayRow

### Синхронизация

1. `GET /api/Sync?year=2025`
2. SyncService читает год из Google Sheets
3. Сравнивает с данными из БД
4. Отличающиеся записи → UpsertManyAsync

### Аналитика

1. `GET /api/WeightAnalysis/summary?start=&end=`
2. WeightAnalysisService читает MounthWeights за период
3. Парсит веса, считает агрегаты
4. Возвращает WeightSummaryDto

---

## Модели данных

### MounthWeight (SQLite)

```csharp
Date: DateOnly (PK)
Weight: string (max 50)
```

### WeightSummaryDto

```csharp
Start, End: DateOnly
Min, Max, Average, StartValue, EndValue, Change: double?
```

### WeightPointDto

```csharp
Date: DateOnly
Value: double?
```

### JsonItem (MongoDB)

```csharp
Id: string
CreatedAt: DateTime
Payload: BsonDocument
```
