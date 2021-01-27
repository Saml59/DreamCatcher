using System;
using DC.Models;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace DC.Services
{
    public class UserConverter : JsonConverter<User>
    {
        public override User Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string user_string = reader.GetString();
            var dict = JsonSerializer.Deserialize<Dictionary<string, Object>>(user_string);
            Object value;
            string[] keys = { "id", "email", "username", "first_name", "middle_initial", "last_name", "role", "dob", "section", "notes" };
            Object[] items = new Object[10];
            //Attempts to retrieve each serialized value
            for (int i = 0; i < keys.Length; i++)
            {
                if (dict.TryGetValue(keys[i], out value))
                {
                    items[i] = Convert.ChangeType(value, items[i].GetType());
                }
                else
                {
                    throw new JsonException("Invalid serialization, key " + keys[i] + " not found");
                }

            }

            User user = new User((int)items[0], (string)items[1], (string)items[2],
                (string)items[3], (string)items[4], (string)items[5], (string)items[6], (string)items[7], (Section)items[8], (string)items[9]);
            return user;

        }

        public override void Write(Utf8JsonWriter writer, User userValue, JsonSerializerOptions options)
        {
            string email, username, passhash, salt, first_name, middle_initial, last_name, role;
            int dob_year, dob_month, dob_day;
            email = userValue.Email;
            username = userValue.Username;
            passhash = userValue.Passhash;
            salt = userValue.Salt;
            first_name = userValue.FirstName;
            middle_initial = userValue.MiddleInitial;
            last_name = userValue.LastName;
            role = userValue.Role;
            dob_year = userValue.DOB_Year;
            dob_month = userValue.DOB_Month;
            dob_day = userValue.DOB_day;
            writer.WriteString("email", email);
            writer.WriteString("username", username);
            writer.WriteString("passhash", passhash);
            writer.WriteString("salt", salt);
            writer.WriteString("first_name", first_name);
            if (middle_initial != null && middle_initial != "") {
                writer.WriteString("middle_intitial", middle_initial);
            }
            writer.WriteString("last_name", last_name);
            writer.WriteString("role", role);
            writer.WriteNumber("dob_year", dob_year);
            writer.WriteNumber("dob_month", dob_month);
            writer.WriteNumber("dob_day", dob_day);
        }
    }
}
