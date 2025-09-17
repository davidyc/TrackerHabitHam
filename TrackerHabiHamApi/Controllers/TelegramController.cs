using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TrackerHabiHamApi.Services;
using TrackerHabiHamApi.Models;

namespace TrackerHabiHamApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TelegramController : ControllerBase
    {
        private readonly ILogger<TelegramController> _logger;
        private readonly ITelegramService _telegramService;
        private readonly IGoogleSheetsService _googleSheetsService; 
        private readonly IWeightService _weightService;
        private readonly IWeightAnalysisService _weightAnalysisService;
        private readonly ISyncService _syncService;

        public TelegramController(ILogger<TelegramController> logger, ITelegramService telegramService, IGoogleSheetsService googleSheetsService, IWeightService weightService, IWeightAnalysisService weightAnalysisService, ISyncService syncService)
        {
            _logger = logger;
            _telegramService = telegramService;
            _googleSheetsService = googleSheetsService;
            _weightService = weightService;
            _weightAnalysisService = weightAnalysisService;
            _syncService = syncService;
        }

        [HttpPost]
        public async Task<IActionResult> HandleWebhook()
        {
            try
            {
                using (var reader = new StreamReader(Request.Body))
                {
                    var body = await reader.ReadToEndAsync();
                    var update = JsonConvert.DeserializeObject<TelegramUpdate>(body);

                    if (update?.Message == null)
                    {
                        _logger.LogWarning("Received update without message");
                        return BadRequest("Invalid update format");
                    }

                    var chatId = update.Message.Chat.Id;
                    var messageText = update.Message.Text;

                    _logger.LogInformation("Received message: {MessageText}", messageText);

                    if (string.Equals(messageText, "/start", StringComparison.OrdinalIgnoreCase))
                    {
                        await _telegramService.SetMyCommandsAsync(new (string command, string description)[]
                        {
                            ("start", "Показать меню"),
                            ("help", "Помощь"),
                        });

                        await _telegramService.SendMenuAsync(chatId, "Выберите действие:", new[] { "Синхранизация", "Аналитика" });
                        return Ok();
                    }
                    
                    if (string.Equals(messageText, "Синхранизация", StringComparison.OrdinalIgnoreCase))
                    {
                        var year = DateTime.UtcNow.Year;
                        var count = await _syncService.SyncByYearAsync(year);  
                        await _telegramService.SendMessageAsync(chatId, $"Was synced {count}");
                        return Ok();
                    }

                    if (string.Equals(messageText, "Аналитика", StringComparison.OrdinalIgnoreCase))
                    {
                        var summary = await _weightAnalysisService.GetSummaryAsync(null, null);
                        var text = $"Аналитика за {summary.Start:dd.MM.yyyy}–{summary.End:dd.MM.yyyy}:\n" +
                                   $"Мин: {Format(summary.Min)}\n" +
                                   $"Макс: {Format(summary.Max)}\n" +
                                   $"Среднее: {Format(summary.Average)}\n" +
                                   $"Старт: {Format(summary.StartValue)}\n" +
                                   $"Финиш: {Format(summary.EndValue)}\n" +
                                   $"Изменение: {FormatChange(summary.Change)}";

                        await _telegramService.SendMessageAsync(chatId, text);
                        return Ok();
                    }

                    if (!_telegramService.IsValidNumber(messageText))
                    {
                        await _telegramService.SendValidationErrorAsync(chatId, messageText);                         
                        return Ok();
                    }

                    var response = _googleSheetsService.WriteNumberToTodayRow(messageText);
                    await _weightService.UpdateWeightAsync(DateOnly.FromDateTime(DateTime.UtcNow), messageText);
                    var responseText = GetResponse(response);

                    await _telegramService.SendMessageAsync(chatId, responseText);

                    return Ok();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Telegram webhook");
                return StatusCode(500, "Internal server error");
            }
        }

        private string GetResponse(string message)
        {
            if (message.Contains("start", StringComparison.OrdinalIgnoreCase))
            {
                return "How can I help you?";
            }
            else if (message.Contains("help", StringComparison.OrdinalIgnoreCase))
            {
                return "Help!";
            }

            return message;
        }

        private static string Format(double? value)
        {
            return value.HasValue ? value.Value.ToString("0.###", System.Globalization.CultureInfo.InvariantCulture) : "—";
        }

        private static string FormatChange(double? value)
        {
            if (!value.HasValue) return "—";
            var sign = value.Value > 0 ? "+" : string.Empty;
            return sign + value.Value.ToString("0.###", System.Globalization.CultureInfo.InvariantCulture);
        }
    }
}


