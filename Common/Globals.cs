using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.Configuration;
using System.Security;

namespace Common
{
    public class Globals
    {
        public static SecureString Pswd;
        static SQLiteConnection _dbConnection;

        //static Globals()
        //{
        //    string connectionString = ConfigurationManager.ConnectionStrings["sqlite"].ConnectionString;
        //    _dbConnection = new SQLiteConnection(connectionString);
        //}

        public static SQLiteConnection DbConnection
        {
            get
            {
                return _dbConnection;
            }

            set
            {
                _dbConnection = value;
            }
        }
    }
}
