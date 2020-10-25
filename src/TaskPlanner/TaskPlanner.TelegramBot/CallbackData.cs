using Newtonsoft.Json;

namespace TaskPlanner.TelegramBot
{
    class CallbackData
    {
        [JsonProperty("ctype")]
        public CallbackType CallbackType { get; set; }
        [JsonProperty("atype")]
        public ActionType ActionType { get; set; }
        [JsonProperty("id")]
        public int? Id { get; set; }
    }
}
