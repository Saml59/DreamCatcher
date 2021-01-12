using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace DC.Models
{
    public class User
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault), JsonPropertyName("id")]
        public int ID { get; set; }
        [JsonInclude, JsonPropertyName("email")]
        public string Email { get; set; }
        [JsonInclude, JsonPropertyName("username")]
        public string Username { get; set; }
        [JsonInclude, JsonPropertyName("salt")]
        public string Salt { get; set; }
        [JsonInclude, JsonPropertyName("passhash")]
        public string Passhash { get; set; }
        [JsonInclude, JsonPropertyName("first_name")]
        public string FirstName { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault), JsonPropertyName("middle_initial")]
        public string MiddleInitial { get; set; }
        [JsonInclude, JsonPropertyName("last_name")]
        public string LastName { get; set; }
        [JsonInclude, JsonPropertyName("dob")]
        public string DOB_ISO { get; set; }
        [JsonInclude, JsonPropertyName("role")]
        public string Role { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault), JsonPropertyName("section")]
        public Section User_Section { get; set; }
        [JsonIgnore]
        public Tokens User_tokens { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault), JsonPropertyName("notes")]
        public string Notes { get; set; }


        public User(string email, string Username, string Password, string FirstName, string MiddleInitial, string LastName, int DOBMonth, int DOBDay, int DOBYear, string role)
        {
            this.Email = email;
            this.Username = Username;
            this.Salt = getSalt();
            this.Passhash = getHashedPassword(this.Salt, Password);
            this.FirstName = FirstName;
            this.MiddleInitial = MiddleInitial;
            this.LastName = LastName;
            this.DOB_ISO = DOBYear.ToString() + "-" + DOBMonth.ToString() + "-" + DOBDay.ToString();
            this.Role = role;
        }


        private string getSalt()
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!?@#$%^&*()";
            StringBuilder res = new StringBuilder();
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                byte[] uintBuffer = new byte[sizeof(uint)];

                for (int i = 16; i > 0; i--)
                {
                    rng.GetBytes(uintBuffer);
                    uint num = BitConverter.ToUInt32(uintBuffer, 0);
                    res.Append(valid[(int)(num % (uint)valid.Length)]);
                }
            }

            return res.ToString();
        }

        private string getHashedPassword(string salt, string password)
        {
            //prehashing string of the salt + the password
            string preHash = salt + password;
            SHA256 hasher = SHA256.Create();
            //create the hash in a bytearray form
            byte[] rawHash = hasher.ComputeHash(Encoding.UTF8.GetBytes(preHash));
            return Encoding.UTF8.GetString(rawHash, 0, rawHash.Length);
        }


    }
    public class Student : User
    {
        public Report[] StudentReports;
        public Tutor AssignedTutor;


        public Student(string email, string Username, string Password, string FirstName, string MiddleInitial, string LastName, int DOBMonth, int DOBDay, int DOBYear, string role)
            : base(email, Username, Password, FirstName, MiddleInitial, LastName, DOBMonth, DOBDay, DOBYear, role)
        {
            
        }
    }
    public class Tutor : User
    {
        public Report[] SubmittedReports;
        public Student[] Students;

        public Tutor(string email, string Username, string Password, string FirstName, string MiddleInitial, string LastName, int DOBMonth, int DOBDay, int DOBYear, string role)
            : base(email, Username, Password, FirstName, MiddleInitial, LastName, DOBMonth, DOBDay, DOBYear, role)
        {

        }
    }
    public class Instructor : User
    {
        public Instructor(string email, string Username, string Password, string FirstName, string MiddleInitial, string LastName, int DOBMonth, int DOBDay, int DOBYear, string role)
            : base(email, Username, Password, FirstName, MiddleInitial, LastName, DOBMonth, DOBDay, DOBYear, role)
        {

        }
    }
    public class Admin : User
    {
        public Admin(string email, string Username, string Password, string FirstName, string MiddleInitial, string LastName, int DOBMonth, int DOBDay, int DOBYear, string role)
            : base(email, Username, Password, FirstName, MiddleInitial, LastName, DOBMonth, DOBDay, DOBYear, role)
        {

        }
    }

    public class Tokens
    {
        [JsonInclude, JsonPropertyName("session_token")]
        public string SessionToken { get; set; }
        [JsonInclude, JsonPropertyName("session_expiration")]
        public DateTime SessionExpiration { get; set; }
        [JsonInclude, JsonPropertyName("refresh_token")]
        public string RefreshToken  { get; set; }

        public Tokens(string session_token, string str_session_expiration, string refresh_token)
        {
            this.SessionExpiration = DateTime.Parse(str_session_expiration);
            this.SessionToken = session_token;
            this.RefreshToken = refresh_token;

        }

    }

    public class LoginInfo
    {
        [JsonInclude, JsonPropertyName("username")]
        public string Username;
        [JsonIgnore(Condition =JsonIgnoreCondition.WhenWritingNull), JsonPropertyName("passhash")]
        public string Passhash;

        public LoginInfo(string username, string passhash)
        {
            this.Username = username;
            this.Passhash = passhash;
        }
    }
}
