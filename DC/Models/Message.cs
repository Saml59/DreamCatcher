using System;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace DC.Models
{
    public class Message
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault), JsonPropertyName("id")]
        public int ID;
        [JsonInclude, JsonPropertyName("sender_id")]
        public int SenderID;
        [JsonInclude, JsonPropertyName("recipient_id")]
        public int RecipientID;
        [JsonInclude, JsonPropertyName("body")]
        public string Text;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault), JsonPropertyName("time_sent")]
        public string TimeSent;



        public Message()
        {
        }
    }
}
