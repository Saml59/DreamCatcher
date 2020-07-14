using System;
using DC.Models;
using SQLite;
using Xamarin.Forms;

namespace DC.Data
{
    public class TokenDatabaseController
    {
        static object locker = new object();
        SQLiteConnection database;

        public TokenDatabaseController()
        {
            database = DependencyService.Get<ISQLite>().getConnection();
            database.CreateTable<Token>();
        }

        public Token getToken()
        {
            lock (locker)
            {
                if (database.Table<Token>().Count() == 0)
                {
                    return null;
                }
                else
                {
                    return database.Table<Token>().First();
                }
            }
        }

        public int saveToken(Token token)
        {
            lock (locker)
            {
                if (token.ID != 0)
                {
                    database.Update(token);
                    return token.ID;
                }
                else return database.Insert(token);
            }
        }

        public int deleteToken(int id)
        {
            lock (locker)
            {
                return database.Delete<Token>(id);
            }
        }
    }
}
