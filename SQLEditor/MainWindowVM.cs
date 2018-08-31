using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Configuration;
using System.Diagnostics;
using Common;

namespace SQLEditor
{
    public class MainWindowVM : INotifyPropertyChanged
    {
        private string _sql;
        public string Sql
        {
            get { return _sql; }
            set
            {
                _sql = value;
                if (this.PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Sql"));
                }
            }
        }

        private DataTable _queryResult = null;
        public DataTable QueryResult
        {
            get { return _queryResult; }
            set
            {
                _queryResult = value;
                if (this.PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("QueryResult"));
                }
            }
        }

        private double _lastSqlDuration;
        public double LastSqlDuration
        {
            get { return _lastSqlDuration; }
            set
            {
                _lastSqlDuration = value;
                if (this.PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("LastSqlDuration"));
                }
            }
        }

        private List<TableVM> _tables = new List<TableVM>();
        public List<TableVM> Tables
        {
            get { return _tables; }
            set
            {
                _tables = value;
                if (this.PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Tables"));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindowVM()
        {
            var cmd = new SQLiteCommand(@"
                SELECT name FROM sqlite_master
                WHERE type='table'
                ORDER BY name;",
            Globals.DbConnection);
            cmd.Connection.OpenEx();
            SQLiteDataReader r = cmd.ExecuteReader();
            while (r.Read())
            {
                Tables.Add(new TableVM(r[0].ToString(), this));
            }

            foreach (var t in Tables)
            {
                var cmd2 = new SQLiteCommand($"pragma table_info({t.Name});", Globals.DbConnection);
                cmd2.ExecuteReader();
            }

            cmd.Connection.Close();


            
            
        }

        public void Run()
        {
            SQLiteCommand cmd = new SQLiteCommand(_sql, Globals.DbConnection);

            SQLiteDataAdapter da = new SQLiteDataAdapter(cmd);
            //DataTable dt = new DataTable();
            DataSet ds = new DataSet();
            Stopwatch sw = Stopwatch.StartNew();

            //da.FillEx(dt);
            da.FillEx(ds);

            sw.Stop();
            LastSqlDuration = sw.Elapsed.TotalMilliseconds;
            if (ds.Tables.Count > 0) //check if command was select
            {
                QueryResult = ds.Tables[0];
            }


        }
    }

    public class TableVM
    {
        public MainWindowVM MainWindowVM { get; set; }
        public string Name { get; set; }

        public IEnumerable<ColumnVM> Columns { get; set; }

        public TableVM(string tblName, MainWindowVM mainWinVM)
        {
            MainWindowVM = mainWinVM;
            Name = tblName;
        }

        public void ShowContents()
        {
            SQLiteCommand cmd = new SQLiteCommand(String.Format(@"SELECT * FROM {0}", Name), Globals.DbConnection);
            //cmd.Parameters.Add("tblName", DbType.Object).Value = Name;

            SQLiteDataAdapter da = new SQLiteDataAdapter(cmd);
            DataTable dt = new DataTable();
            Stopwatch sw = Stopwatch.StartNew();
            da.FillEx(dt);
            sw.Stop();
            MainWindowVM.LastSqlDuration = sw.Elapsed.TotalMilliseconds;
            MainWindowVM.QueryResult = dt;
        }


    }

    //public class Column
    //{
    //    public string Name { get; set; }
    //    public Column()
    //    {

    //    }
    //}
}
