using System;
using SQLite;

namespace DC.Models
{
    public class User
    {
        [PrimaryKey]
        public int ID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }



        public User() {}
        public User(string Username, string Password)
        {
            this.Username = Username;
            this.Password = Password;


        }

        public bool checkInformation()
        {
            if (this.Username.Equals("") || this.Password.Equals(""))
                return false;
            else return true;
        }


    }
}
