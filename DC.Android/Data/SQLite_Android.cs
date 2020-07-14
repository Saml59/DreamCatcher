using System;
using System.IO;
using DC.Data;
using DC.Droid.Data;
using Xamarin.Forms;

[assembly: Dependency(typeof(SQLite_Android))]
namespace DC.Droid.Data
{
    public class SQLite_Android :ISQLite
    {
        public SQLite_Android()
        {
        }

        public SQLite.SQLiteConnection getConnection()
        {
            var sqliteFileName = "TestDB.db3";
            string documentPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            var path = Path.Combine(documentPath, sqliteFileName);
            var conn = new SQLite.SQLiteConnection(path);

            return conn;
        }

    }
}
