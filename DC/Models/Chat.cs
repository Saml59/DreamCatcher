using System;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace DC.Models
{
    public class Chat
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault), JsonPropertyName("id")]
        public int ID;
        [JsonInclude, JsonPropertyName("user_1")]
        public User User1;
        [JsonInclude, JsonPropertyName("user_2")]
        public User User2;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault), JsonPropertyName("messages")]
        public Message[] Messages;
    }
}
