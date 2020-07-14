using System;
using System.IO;
using DC.iOS.Data;
using Xamarin.Forms;

[assembly: Dependency(typeof(SQLite_IOS))]
namespace DC.iOS.Data
{
    public class SQLite_IOS
    {
        public SQLite_IOS()
        {
        }

        public SQLite.SQLiteConnection getConnection()
        {
            var sqliteFileName = "TestDB.db3";
            string documentPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var libraryPath = Path.Combine(documentPath, "..", "Library");
            var path = Path.Combine(libraryPath, sqliteFileName);
            var conn = new SQLite.SQLiteConnection(path);

            return conn;
        }
    }
}
