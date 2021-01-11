using System;
using System.Security.Cryptography;
using System.Text;
using Amazon.DynamoDBv2.DataModel;
using SQLite;

namespace DC.Models
{
    [DynamoDBTable("Accounts")]
    public class User
    {
        [DynamoDBHashKey]
        public string Username { get; set; }
        public string Salt { get; set; }
        public string Passhash { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int DOBMonth { get; set; }
        public int DOBDay { get; set; }
        public int DOBYear { get; set; }

        public User() {}
        public User(string Username, string Password, string FirstName, string LastName, int DOBMonth, int DOBDay, int DOBYear)
        {
            this.Username = Username;
            this.Salt = getSalt();
            this.Passhash = getHashedPassword(this.Salt, Password);
            this.FirstName = FirstName;
            this.LastName = LastName;
            this.DOBMonth = DOBMonth;
            this.DOBDay = DOBDay;
            this.DOBYear = DOBYear;
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
}
