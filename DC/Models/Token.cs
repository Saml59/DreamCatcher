using System;
using SQLite;

namespace DC.Models
{
    public class Token
    {
        [PrimaryKey]
        public int ID { get; set; }
        public string access_token { get; set; }
        public string error_description { get; set; }
        public DateTime expire_date { get; set; }
        public int expire_in { get; set; }


        public Token() { }
    }
}
