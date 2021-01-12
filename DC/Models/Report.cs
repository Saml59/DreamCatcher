using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DC.Models
{
    public class Report
    {
        [JsonIgnore(Condition =JsonIgnoreCondition.WhenWritingDefault), JsonPropertyName("id")]
        public int ID;
        [JsonInclude, JsonPropertyName("tutor_id")]
        public int TutorID;
        [JsonInclude, JsonPropertyName("student_id")]
        public int StudentID;
        [JsonInclude, JsonPropertyName("report")]
        public string Body;

    }
}
