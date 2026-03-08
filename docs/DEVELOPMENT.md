# Руководство разработчика

## Сборка и запуск

### API

```bash
cd TrackerHabiHamApi
dotnet restore
dotnet run
```

### API с hot reload

```bash
cd TrackerHabiHamApi
dotnet watch run
```

### MAUI (Windows)

```bash
cd HabihamTrackerApp
dotnet build -f net9.0-windows10.0.19041.0
dotnet run -f net9.0-windows10.0.19041.0
```

### MAUI (Android)

```bash
cd HabihamTrackerApp
dotnet build -f net9.0-android
```

---

## Решение проектов

- `TrackerHabiHamApi/TrackerHabiHamApi.sln` — только API
- `HabihamTrackerApp/HabihamTrackerApp.sln` — только MAUI

Для работы с обоими проектами откройте корневую папку или оба .sln.

---

## Миграции БД

```bash
cd TrackerHabiHamApi
dotnet ef migrations add MigrationName
dotnet ef database update
```

Миграции применяются автоматически при `dotnet run` через `context.Database.Migrate()` в Program.cs.

---

## Тестирование API

- Swagger UI: `https://localhost:5001/swagger` или `http://localhost:5000/swagger`
- Scalar: `https://localhost:5001/scalar/v1`

---

## Связка с Telegram

1. Создайте бота через @BotFather, получите токен
2. Укажите токен в appsettings или User Secrets
3. Установите webhook:
   ```
   https://api.telegram.org/bot<TOKEN>/setWebhook?url=https://YOUR_HOST/api/Telegram
   ```
4. Используйте ngrok для локальной разработки:
   ```bash
   ngrok http 5000
   # Указать https URL ngrok в setWebhook
   ```

---

## Структура кода

### Добавление контроллера

1. Создать контроллер в `Controllers/`
2. Зарегистрировать необходимые сервисы в `Program.cs`
3. Swagger подхватит контроллер автоматически

### Добавление сервиса

1. Создать интерфейс в `Services/Interfaces/`
2. Реализация в `Services/`
3. Регистрация в Program.cs: `builder.Services.AddScoped<IYourService, YourService>()`
