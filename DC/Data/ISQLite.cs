using System;
using SQLite;

namespace DC.Data
{
    public interface ISQLite
    {
        SQLiteConnection getConnection();
    }

}
