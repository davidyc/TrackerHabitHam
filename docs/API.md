# Tracker HabiHam API — полная справка

## Базовый URL

- Локально: `http://localhost:5000` или `https://localhost:5001`
- Продакшен: `https://trackerhabitham.onrender.com`

---

## Weight

### GET /api/Weight

Получить записи о весе за период.

**Query parameters**

| Параметр | Тип | Обязательный | Описание |
|----------|-----|--------------|----------|
| start | DateOnly | Нет | Начало периода (yyyy-MM-dd). По умолчанию — 1 января текущего года |
| end | DateOnly | Нет | Конец периода. По умолчанию — сегодня |

**Пример ответа**

```json
[
  { "date": "2025-03-01", "weight": "82.5" },
  { "date": "2025-03-08", "weight": "82.3" }
]
```

---

### PUT /api/Weight

Добавить или обновить вес на дату.

**Body (JSON)**

```json
{
  "date": "2025-03-08",
  "weight": "82.5"
}
```

- `date` — необязателен. Если не указан, используется сегодняшняя дата в UTC.
- `weight` — строка (например, "82.5" или "82,5").

**Ответ**: объект `MounthWeight` с `date` и `weight`.

---

## WeightAnalysis

### GET /api/WeightAnalysis/summary

Сводная статистика за период.

**Query parameters**

| Параметр | Тип | Описание |
|----------|-----|----------|
| start | DateOnly | Начало периода |
| end | DateOnly | Конец периода |

Без параметров — с 1 января текущего года по сегодня.

**Пример ответа**

```json
{
  "start": "2025-01-01",
  "end": "2025-03-08",
  "min": 81.2,
  "max": 84.0,
  "average": 82.5,
  "startValue": 83.0,
  "endValue": 82.3,
  "change": -0.7
}
```

Поля могут быть `null`, если нет валидных данных в диапазоне.

---

### GET /api/WeightAnalysis/series

Серия точек для графика.

**Query parameters**

| Параметр | Тип | Описание |
|----------|-----|----------|
| start | DateOnly | Начало периода |
| end | DateOnly | Конец периода |

**Пример ответа**

```json
[
  { "date": "2025-03-01", "value": 82.5 },
  { "date": "2025-03-02", "value": null },
  { "date": "2025-03-08", "value": 82.3 }
]
```

`value` может быть `null`, если вес не удалось распарсить.

---

## Sync

### GET /api/Sync

Синхронизация данных из Google Sheets в БД.

**Query parameters**

| Параметр | Тип | Обязательный | Описание |
|----------|-----|--------------|----------|
| year | int | Да | Год (1900 — текущий+1) |

**Пример ответа**

```json
{
  "affected": "Was synced 15"
}
```

---

## Telegram

### POST /api/Telegram

Webhook для Telegram Bot API. Принимает тело запроса в формате [Update](https://core.telegram.org/bots/api#update).

**Маршрут**: должен быть указан в BotFather как `setWebhook`.

---

## GoogleAuth

### GET /api/GoogleAuth/check-credentials

Проверка наличия файла credentials Google.

**Ответ**: `"File exist: true"` или `"File exist: false"`.

---

### GET /api/GoogleAuth/Period

Получить данные за месяц из Google Sheets.

**Query parameters**

| Параметр | Тип | Описание |
|----------|-----|----------|
| year | int | Год |
| mounth | int | Месяц (1–12) |

**Пример ответа**

```json
{
  "result": [
    { "date": "2025-03-01", "weight": "82.5" },
    { "date": "2025-03-02", "weight": "" }
  ]
}
```

---

## Json Store

### POST /api/json

Сохранить JSON-объект.

**Query parameters**

| Параметр | Тип | Описание |
|----------|-----|----------|
| id | string | Необязательный ID. Если не указан, генерируется ObjectId |

**Body**: сырой JSON (только объект, не массив и не примитив).

**Пример ответа**

```json
{
  "id": "6742a1b2c3d4e5f6a7b8c9d0"
}
```

---

### GET /api/json/{id}

Получить сохранённый документ по id.

**Ответ**: JSON-объект (payload документа).

**Ошибки**: 404 если документ не найден.

---

### GET /api/json/check

Проверка подключения к MongoDB.

**Ответ**

```json
{
  "connected": true,
  "message": "MongoDB connection is healthy."
}
```

Или 503 при сбое подключения.
