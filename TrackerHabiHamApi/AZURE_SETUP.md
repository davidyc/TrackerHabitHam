# Настройка Azure SQL Database

## Изменения, внесенные в проект

✅ **Заменены пакеты PostgreSQL на SQL Server:**

- `Npgsql.EntityFrameworkCore.PostgreSQL` → `Microsoft.EntityFrameworkCore.SqlServer`
- `Npgsql.EntityFrameworkCore.PostgreSQL.Design` → `Microsoft.EntityFrameworkCore.SqlServer.Design`

✅ **Обновлен Program.cs:**

- `options.UseNpgsql()` → `options.UseSqlServer()`

✅ **Обновлен appsettings.json:**

- Добавлен пример connection string для Azure SQL Database
- Сохранен комментарий с PostgreSQL connection string для справки

✅ **Создана новая миграция:**

- Удалены старые миграции PostgreSQL
- Создана новая миграция `20251019073012_Init.cs` для SQL Server

## Настройка Azure SQL Database

### 1. Создание Azure SQL Database

1. Войдите в [Azure Portal](https://portal.azure.com)
2. Создайте новый ресурс "Azure SQL Database"
3. Настройте:
   - **Имя сервера**: `your-server-name` (будет доступен как `your-server-name.database.windows.net`)
   - **Имя базы данных**: `habihamtrackerdb`
   - **Учетные данные администратора**: создайте логин и пароль

### 2. Настройка Connection String

Замените пустое значение `"DefaultConnection"` в `appsettings.json` на:

```json
"DefaultConnection": "Server=tcp:your-server-name.database.windows.net,1433;Initial Catalog=habihamtrackerdb;Persist Security Info=False;User ID=your-username;Password=your-password;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
```

### 3. Настройка Firewall

В Azure Portal для вашего SQL Server:

1. Перейдите в "Firewalls and virtual networks"
2. Добавьте ваш IP-адрес в список разрешенных
3. Или включите "Allow Azure services and resources to access this server"

### 4. Применение миграций

После настройки connection string выполните:

```bash
dotnet ef database update
```

### 5. Проверка работы

Запустите приложение:

```bash
dotnet run
```

## Важные замечания

- **Безопасность**: Не храните connection string с паролем в коде. Используйте Azure Key Vault или User Secrets для разработки
- **Firewall**: Убедитесь, что ваш IP-адрес добавлен в firewall правила Azure SQL Server
- **SSL**: Azure SQL Database требует SSL соединения (уже настроено в connection string)

## User Secrets для разработки

Для локальной разработки рекомендуется использовать User Secrets:

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "your-azure-sql-connection-string"
```

Это позволит хранить connection string безопасно без коммита в репозиторий.
