using System;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace DC.Models
{
    public class Section
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault), JsonPropertyName("id")]
        public int ID;
        [JsonInclude]
        public string Name;
        [JsonInclude]
        public User[] Users;

        public Section(int id, string name, User[] users = null)
        {
            this.ID = id;
            this.Name = name;
            this.Users = users;
        }
    }
}
