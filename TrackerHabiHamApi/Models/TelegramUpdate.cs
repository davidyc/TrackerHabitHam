using Newtonsoft.Json;

namespace TrackerHabiHamApi.Models
{
    public class TelegramUpdate
    {
        [JsonProperty("update_id")]
        public long UpdateId { get; set; }

        [JsonProperty("message")]
        public TelegramMessage? Message { get; set; }
    }

    public class TelegramMessage
    {
        [JsonProperty("message_id")]
        public long MessageId { get; set; }

        [JsonProperty("chat")]
        public TelegramChat Chat { get; set; } = new();

        [JsonProperty("text")]
        public string Text { get; set; } = string.Empty;
    }

    public class TelegramChat
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; } = string.Empty;
    }
}


