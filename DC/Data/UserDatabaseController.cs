using System;
using DC.Models;
using SQLite;
using Xamarin.Forms;

namespace DC.Data
{
    public class UserDatabaseController
    {
        static object locker = new object();
        SQLiteConnection database;

        public UserDatabaseController()
        {
            database = DependencyService.Get<ISQLite>().getConnection();
            database.CreateTable<User>();
        }

        public User getUser()
        {
            lock (locker)
            {
                if (database.Table<User>().Count() == 0)
                {
                    return null;
                }
                else
                {
                    return database.Table<User>().First();
                }
            }
        }

        public int saveUser(User user)
        {
            lock (locker)
            {
                if (user.ID != 0)
                {
                    database.Update(user);
                    return user.ID;
                }
                else return database.Insert(user);
            }
        }

        public int deleteUser(int id)
        {
            lock (locker)
            {
                return database.Delete<User>(id);
            }
        }
    }
}
