using Newtonsoft.Json;

namespace HouseholdTaskPlanner.TelegramBot
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
