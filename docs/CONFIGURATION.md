# Конфигурация Tracker HabiHam

## appsettings.json

### Секции

#### Logging

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

#### ConnectionStrings

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=HabiHamTracker.db"
  }
}
```

- **SQLite**: `Data Source=путь\к\файлу.db`
- Файл создаётся при первом запуске. Для Docker — используйте volume.

#### Telegram

```json
{
  "Telegram": {
    "BotToken": "123456789:ABCdefGHIjklMNOpqrsTUVwxyz"
  }
}
```

- Токен получается у [@BotFather](https://t.me/BotFather)
- Webhook: `https://YOUR_DOMAIN/api/Telegram`

#### GoogleSheets

```json
{
  "GoogleSheets": {
    "SpreadsheetId": "1a33Z_7tfPh-Kpzyz-C95TkCuKjXa1ZhvNDHX_7hFkk8",
    "CredentialsFilePath": "C:\\path\\to\\service-account.json"
  }
}
```

- **SpreadsheetId** — из URL таблицы: `https://docs.google.com/spreadsheets/d/{SpreadsheetId}/edit`
- **CredentialsFilePath** — путь к JSON ключу [сервисного аккаунта](https://console.cloud.google.com/iam-admin/serviceaccounts)

#### Mongo

```json
{
  "Mongo": {
    "ConnectionString": "mongodb://localhost:27017",
    "Database": "Json_Collection_DB"
  }
}
```

- Пустой ConnectionString → `mongodb://localhost:27017`
- Используется только для JsonStore API

---

## Переменные окружения

Приоритет: переменные окружения > appsettings.json.

Для вложенных ключей используйте `__` (двойное подчёркивание):

```
ConnectionStrings__DefaultConnection=Data Source=HabiHamTracker.db
Telegram__BotToken=your_token
GoogleSheets__SpreadsheetId=your_spreadsheet_id
GoogleSheets__CredentialsFilePath=/app/creds.json
Mongo__ConnectionString=mongodb://mongo:27017
Mongo__Database=Json_Collection_DB
```

---

## User Secrets (разработка)

```bash
cd TrackerHabiHamApi
dotnet user-secrets init   # если ещё не инициализированы
dotnet user-secrets set "Telegram:BotToken" "YOUR_TOKEN"
dotnet user-secrets set "GoogleSheets:CredentialsFilePath" "C:\\path\\to\\creds.json"
```

User Secrets переопределяют appsettings.json и не попадают в репозиторий.

---

## MAUI: URL API

В `MainPage.xaml.cs` используются захардкоженные URL. Для смены окружения можно:

1. Заменить строки вручную
2. Использовать `#if DEBUG` для переключения local/production
3. Вынести base URL в конфиг или константы

Пример:

```csharp
#if DEBUG
    private const string ApiBase = "http://localhost:5000";
#else
    private const string ApiBase = "https://trackerhabitham.onrender.com";
#endif
```

---

## Google Sheets: структура таблицы

- Листы: названия по году (`2024`, `2025`, …)
- Строка 1: заголовки (дни месяца)
- Колонки B–M: месяцы 1–12
- Строки 2–32: значения веса за день
- Формат ячеек: число или пусто
