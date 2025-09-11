using System.Net.Http;

namespace TrackerHabiHamApi.Services
{
    public class TelegramService : ITelegramService
    {
        private readonly HttpClient _httpClient;
        private readonly string _botToken;
        private readonly ILogger<TelegramService> _logger;

        public TelegramService(HttpClient httpClient, IConfiguration configuration, ILogger<TelegramService> logger)
        {
            _httpClient = httpClient;
            _botToken = configuration["Telegram:BotToken"] ?? throw new InvalidOperationException("Telegram bot token not configured");
            _logger = logger;
        }

        public async Task SendMessageAsync(long chatId, string message)
        {
            try
            {
                var url = $"https://api.telegram.org/bot{_botToken}/sendMessage?chat_id={chatId}&text={Uri.EscapeDataString(message)}";
                var response = await _httpClient.GetAsync(url);
                
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Failed to send Telegram message. Status: {StatusCode}", response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending Telegram message");
                throw;
            }
        }

        public async Task SendMenuAsync(long chatId, string message, IEnumerable<string> buttonsInSingleRow)
        {
            try
            {
                var replyMarkup = new
                {
                    keyboard = new[]
                    {
                        buttonsInSingleRow.Select(text => new { text })
                    },
                    resize_keyboard = true,
                    one_time_keyboard = false
                };

                var payload = new
                {
                    chat_id = chatId,
                    text = message,
                    reply_markup = replyMarkup
                };

                var request = new HttpRequestMessage(HttpMethod.Post, $"https://api.telegram.org/bot{_botToken}/sendMessage")
                {
                    Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(payload), System.Text.Encoding.UTF8, "application/json")
                };

                var response = await _httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Failed to send Telegram menu. Status: {StatusCode}", response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending Telegram menu");
                throw;
            }
        }

        public async Task SetMyCommandsAsync(IEnumerable<(string command, string description)> commands)
        {
            try
            {
                var payload = new
                {
                    commands = commands.Select(c => new { command = c.command, description = c.description })
                };

                var request = new HttpRequestMessage(HttpMethod.Post, $"https://api.telegram.org/bot{_botToken}/setMyCommands")
                {
                    Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(payload), System.Text.Encoding.UTF8, "application/json")
                };

                var response = await _httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Failed to set Telegram commands. Status: {StatusCode}", response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting Telegram commands");
                throw;
            }
        }

        public bool IsValidNumber(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return false;

            if (int.TryParse(message, out _))
                return true;

            if (double.TryParse(message, out _))
                return true;

            if (message.StartsWith("-") && double.TryParse(message.Substring(1), out _))
                return true;

            return false;
        }

        public async Task SendValidationErrorAsync(long chatId, string invalidMessage)
        {
            var errorMessage = $"Сообщение '{invalidMessage}' не является числом. Пожалуйста, отправьте числовое значение.";
            await SendMessageAsync(chatId, errorMessage);
        }
    }
}


