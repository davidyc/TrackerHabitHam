# Tracker HabiHam

Система учёта веса с синхронизацией Google Sheets, аналитикой и Telegram-ботом.

## Содержание

- [Обзор](#обзор)
- [Структура проекта](#структура-проекта)
- [Технологии](#технологии)
- [Требования](#требования)
- [Быстрый старт](#быстрый-старт)
- [Конфигурация](#конфигурация)
- [API](#api)
- [Приложение MAUI](#приложение-maui)
- [Интеграции](#интеграции)
- [Развёртывание](#развёртывание)
- [Дополнительная документация](#дополнительная-документация)

---

## Обзор

**Tracker HabiHam** — это приложение для отслеживания веса, состоящее из:

1. **TrackerHabiHamApi** — REST API на ASP.NET Core
2. **HabihamTrackerApp** — кроссплатформенное мобильное приложение (.NET MAUI)

### Функциональность

- Регистрация веса за день
- Аналитика (min, max, среднее, изменение за период)
- Синхронизация данных между БД и Google Sheets
- Telegram-бот для ввода веса и получения аналитики
- Хранение произвольных JSON-документов в MongoDB

---

## Структура проекта

```
TrackerHabitHam/
├── TrackerHabiHamApi/           # Web API
│   ├── Controllers/             # Контроллеры
│   │   ├── WeightController.cs
│   │   ├── WeightAnalysisController.cs
│   │   ├── SyncController.cs
│   │   ├── TelegramController.cs
│   │   ├── GoogleAuthController.cs
│   │   └── JsonStoreController.cs
│   ├── Data/
│   │   └── ApplicationDbContext.cs
│   ├── Migrations/
│   ├── Models/
│   │   ├── Dto/
│   │   ├── JsonItem.cs
│   │   └── TelegramUpdate.cs
│   ├── Services/
│   │   ├── Interfaces/
│   │   ├── WeightService.cs
│   │   ├── WeightAnalysisService.cs
│   │   ├── SyncService.cs
│   │   ├── GoogleSheetsService.cs
│   │   ├── TelegramService.cs
│   │   └── JsonStoreService.cs
│   ├── Program.cs
│   ├── appsettings.json
│   └── Dockerfile
│
└── HabihamTrackerApp/           # MAUI приложение
    ├── MainPage.xaml / .xaml.cs
    ├── AppShell.xaml
    ├── MauiProgram.cs
    └── Platforms/
```

---

## Технологии

| Компонент | Стек |
|-----------|------|
| API | ASP.NET Core 9, .NET 9 |
| БД (вес) | SQLite + Entity Framework Core |
| БД (JSON) | MongoDB |
| MAUI-клиент | .NET MAUI 9 (Android, iOS, Windows, Mac Catalyst) |
| Интеграции | Google Sheets API, Telegram Bot API |

---

## Требования

- .NET 9 SDK
- Для Google Sheets: JSON-ключ сервисного аккаунта
- Для Telegram: токен бота
- (Опционально) MongoDB для JsonStore

---

## Быстрый старт

### API

```bash
cd TrackerHabiHamApi
dotnet run
```

API будет доступен по адресу `https://localhost:5001` (или `http://localhost:5000`).

- Swagger UI: `/swagger`
- Scalar API Reference: `/scalar/v1`

### MAUI-приложение

```bash
cd HabihamTrackerApp
dotnet build -f net9.0-windows10.0.19041.0   # Windows
# или
dotnet build -f net9.0-android
dotnet build -f net9.0-ios
```

---

## Конфигурация

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=HabiHamTracker.db"
  },
  "Telegram": {
    "BotToken": "YOUR_BOT_TOKEN"
  },
  "GoogleSheets": {
    "SpreadsheetId": "YOUR_SPREADSHEET_ID",
    "CredentialsFilePath": "C:\\path\\to\\credentials.json"
  },
  "Mongo": {
    "ConnectionString": "mongodb://localhost:27017",
    "Database": "Json_Collection_DB"
  }
}
```

| Параметр | Описание |
|----------|----------|
| `DefaultConnection` | Строка подключения SQLite. Файл `HabiHamTracker.db` создаётся в рабочей директории. |
| `Telegram:BotToken` | Токен бота от [@BotFather](https://t.me/BotFather). Обязателен для работы TelegramController. |
| `GoogleSheets:SpreadsheetId` | ID таблицы Google (из URL). |
| `GoogleSheets:CredentialsFilePath` | Путь к JSON-ключу сервисного аккаунта. |
| `Mongo:ConnectionString` | Строка подключения MongoDB. По умолчанию `mongodb://localhost:27017`. |
| `Mongo:Database` | Имя БД MongoDB. |

### User Secrets (разработка)

Для локальной разработки лучше использовать User Secrets:

```bash
cd TrackerHabiHamApi
dotnet user-secrets set "Telegram:BotToken" "YOUR_TOKEN"
dotnet user-secrets set "GoogleSheets:CredentialsFilePath" "C:\\path\\to\\creds.json"
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Data Source=HabiHamTracker.db"
```

### MAUI: базовый URL API

В `MainPage.xaml.cs` указаны жёстко закодированные URL:

- Аналитика: `https://trackerhabitham.onrender.com/api/WeightAnalysis/summary`
- Синхронизация: `http://trackerhabitham.onrender.com/api/Sync`
- Вес: `https://trackerhabitham.onrender.com/api/Weight`

Для локальной разработки замените на `http://localhost:5000` или свой хост.

---

## API

### Weight (Вес)

| Метод | Путь | Описание |
|-------|------|----------|
| GET | `/api/Weight?start=&end=` | Получить вес за период (start, end — DateOnly в формате yyyy-MM-dd) |
| PUT | `/api/Weight` | Добавить/обновить вес. Body: `{ "date": "2025-03-08", "weight": "82.5" }` |

### WeightAnalysis (Аналитика)

| Метод | Путь | Описание |
|-------|------|----------|
| GET | `/api/WeightAnalysis/summary?start=&end=` | Сводка за период: min, max, average, startValue, endValue, change |
| GET | `/api/WeightAnalysis/series?start=&end=` | Серия точек для графика: `[{ date, value }, ...]` |

### Sync (Синхронизация)

| Метод | Путь | Описание |
|-------|------|----------|
| GET | `/api/Sync?year=2025` | Синхронизировать Google Sheets → БД за указанный год |

### Telegram (Webhook)

| Метод | Путь | Описание |
|-------|------|----------|
| POST | `/api/Telegram` | Webhook для Telegram Bot API. Настраивается в @BotFather. |

### GoogleAuth

| Метод | Путь | Описание |
|-------|------|----------|
| GET | `/api/GoogleAuth/check-credentials` | Проверить наличие файла credentials |
| GET | `/api/GoogleAuth/Period?year=&mounth=` | Получить данные за месяц из Google Sheets |

### Json Store (MongoDB)

| Метод | Путь | Описание |
|-------|------|----------|
| POST | `/api/json?id=` | Сохранить JSON-объект. Body — сырой JSON. Опционально `id` в query. |
| GET | `/api/json/{id}` | Получить JSON по id |
| GET | `/api/json/check` | Проверить соединение с MongoDB |

---

## Приложение MAUI

### Табы

1. **Аналитика** — выбор периода, запрос summary, отображение min/max/avg и изменения
2. **Синхронизация** — кнопка запуска синхронизации за текущий год
3. **Вес** — ввод веса за сегодня, отправка PUT на `/api/Weight`

### Платформы

- Windows 10.0.17763+
- Android 21+
- iOS 15+
- Mac Catalyst 15+

---

## Интеграции

### Google Sheets

- **Структура таблицы**: листы названы годом (2024, 2025…). Колонки B–M — месяцы 1–12. Строка 1 — заголовки, строки 2–32 — дни.
- **Запись**: `WriteNumberToTodayRow` пишет в ячейку текущей даты.
- **Чтение**: `GetMounth(year, month)` читает колонку месяца.

### Telegram

- **Webhook URL**: `https://YOUR_HOST/api/Telegram`
- **Команды**: `/start` — меню с кнопками «Синхронизация», «Аналитика»
- **Число** — сохраняется как вес за сегодня в БД и Google Sheets

---

## Развёртывание

### Docker

```bash
docker build -t trackerhabiham-api -f TrackerHabiHamApi/Dockerfile .
docker run -p 8080:8080 \
  -e ConnectionStrings__DefaultConnection="Data Source=/app/data/HabiHamTracker.db" \
  -e Telegram__BotToken="YOUR_TOKEN" \
  -v tracker-data:/app/data \
  trackerhabiham-api
```

Важно: SQLite хранит данные в файле. Для Docker используйте volume для `/app/data`.

### Render

Проект рассчитан на развёртывание на [Render](https://render.com). Настройте переменные окружения:

- `ConnectionStrings__DefaultConnection`
- `Telegram__BotToken`
- `GoogleSheets__CredentialsFilePath` или base64 содержимого credentials

### Миграции

При старте API выполняется `context.Database.Migrate()`. Миграции применяются автоматически.

---

## База данных

### SQLite (MounthWeights)

| Таблица | Поля |
|---------|------|
| MounthWeights | Date (PK, date), Weight (varchar 50) |

### MongoDB (JsonStore)

- Коллекция: `json_items`
- Документ: `{ _id, createdAt, payload }`

---

## Дополнительная документация

| Документ | Описание |
|----------|----------|
| [docs/API.md](docs/API.md) | Подробная справка по API |
| [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md) | Архитектура и потоки данных |
| [docs/CONFIGURATION.md](docs/CONFIGURATION.md) | Конфигурация и переменные окружения |
| [docs/DEVELOPMENT.md](docs/DEVELOPMENT.md) | Руководство для разработчиков |

---

## Лицензия

Проект для личного использования.
