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
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Data.Common;

namespace SQLEditor
{
    public class MainWindowVM : ViewModelBase
    {
        public class ExecuteCommand : ICommand
        {
            private MainWindowVM _parent;

            public event EventHandler CanExecuteChanged;

            public ExecuteCommand(MainWindowVM parent)
            {
                _parent = parent;
                _parent.PropertyChanged += _parent_PropertyChanged;
            }

            private void _parent_PropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }

            public bool CanExecute(object parameter)
            {
                return _parent.DbVM != null && !string.IsNullOrWhiteSpace(_parent.Sql);
            }

            public void Execute(object parameter)
            {
                _parent.Run();
            }
        }

        public ExecuteCommand ExecuteCmd { get; set; }

        string _sql;
        public string Sql
        {
            get { return _sql; }
            set
            {
                _sql = value;
                Notify(nameof(Sql));
            }
        }

        private DataTable _queryResult;
        public DataTable QueryResult
        {
            get { return _queryResult; }
            set
            {
                _queryResult = value;
                Notify(nameof(QueryResult));
            }
        }

        double _lastSqlDuration;
        public double LastSqlDuration
        {
            get { return _lastSqlDuration; }
            set
            {
                _lastSqlDuration = value;
                Notify(nameof(LastSqlDuration));
            }
        }

        public DbVM DbVM
        {
            get => _dbVM;
            set
            {
                _dbVM = value;
                Notify(nameof(DbVM));
            }
        }

        public ObservableCollection<DbConnVM> ConnStrings { get; set; } = new ObservableCollection<DbConnVM>();

        private TableVM _selectedTable;
        private DbVM _dbVM;

        public TableVM SelectedTable
        {
            get => _selectedTable;
            set => _selectedTable = value;
        }

        public ICommand RefreshCmd { get; set; }

        public MainWindowVM()
        {
            ExecuteCmd = new ExecuteCommand(this);
            RefreshCmd = new RelayCommand<string>(LoadDatabase);

            foreach (ConnectionStringSettings item in ConfigurationManager.ConnectionStrings)
            {
                ConnStrings.Add(new DbConnVM(this) { Name = item.Name, ConnString = item.ConnectionString });
            }
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

        public void LoadDatabase(string dbName)
        {
            dbName = dbName ?? throw new ArgumentException(nameof(dbName));

            Globals.DbConnection?.Dispose();
            Globals.DbConnection = new SQLiteConnection(ConfigurationManager.ConnectionStrings[dbName].ConnectionString);

            var cmd = new SQLiteCommand(@"
                SELECT name FROM sqlite_master
                WHERE type='table'
                ORDER BY name;",
            Globals.DbConnection);

            //using (Globals.DbConnection)
            //{
            cmd.Connection.OpenEx();
            SQLiteDataReader r = cmd.ExecuteReader();

            var dbVM = new DbVM(this) { Name = dbName };

            QueryResult = null;

            while (r.Read())
            {
                var table = new TableVM(r[0].ToString(), this);
                dbVM.Tables.Add(table);

                var cmd2 = new SQLiteCommand($"pragma table_info({r[0].ToString()});", Globals.DbConnection);
                var r2 = cmd2.ExecuteReader();
                while (r2.Read())
                {
                    DataTypes type = DataTypes.text;
                    Enum.TryParse(r2["type"].ToString(), out type);
                    table.Columns.Add(new ColumnVM(r2["name"].ToString(), type));
                }
            }
            DbVM = dbVM;
            //}
        }

        public void AddDatabase(string name, string filePath)
        {
            var csb = new DbConnectionStringBuilder
                {
                    { "data source", filePath }
                };
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var css = new ConnectionStringSettings(name, csb.ConnectionString);
            if (config.ConnectionStrings.ConnectionStrings.IndexOf(css) > 0)
                config.ConnectionStrings.ConnectionStrings.Remove(css);
            config.ConnectionStrings.ConnectionStrings.Add(css);
            config.Save(ConfigurationSaveMode.Modified, true);
            ConfigurationManager.RefreshSection("connectionStrings");

            ConnStrings.Add(new DbConnVM(this) { Name = css.Name, ConnString = css.ConnectionString });
        }
    }

    public class DbVM
    {
        MainWindowVM _parent;
        public string Name { get; set; }
        public ObservableCollection<TableVM> Tables { get; set; } = new ObservableCollection<TableVM>();
        public DbVM(MainWindowVM parent)
        {
            _parent = parent;
        }
    }

    public class TableVM

    {
        public class GenerateSqlCmd : ICommand
        {
            TableVM _parent;
            public event EventHandler CanExecuteChanged;

            public GenerateSqlCmd(TableVM parent)
            {
                _parent = parent;
            }

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public void Execute(object parameter)
            {
                var sql = "";
                switch (parameter.ToString())
                {
                    case "select":
                        sql = $"select {string.Join(",", _parent.Columns.Select(c => c.Name))} from {_parent.Name}";
                        break;
                    case "update":
                        sql = $"update {_parent.Name} set";
                        break;
                    case "insert":
                        sql = $"insert into {_parent.Name} ({string.Join(",", _parent.Columns.Select(c => c.Name))}) values ()";
                        break;
                    case "delete":
                        sql = $"delete from {_parent.Name}";
                        break;
                    case "drop":
                        sql = $"drop table {_parent.Name}";
                        break;
                    default:
                        break;
                }
                _parent.MainWindowVM.Sql += $"{Environment.NewLine}{sql}";
            }

        }

        public GenerateSqlCmd GenerateSql { get; set; }

        public MainWindowVM MainWindowVM { get; set; }

        public string Name { get; set; }

        public List<ColumnVM> Columns { get; set; }

        public TableVM(string tblName, MainWindowVM mainWinVM)
        {
            MainWindowVM = mainWinVM;
            Name = tblName;
            GenerateSql = new GenerateSqlCmd(this);
            Columns = new List<ColumnVM>();
            //var columns = new List<ColumnVM>();
            //using (Globals.DbConnection)
            //{
            //    var cmd2 = new SQLiteCommand($"pragma table_info({Name});", Globals.DbConnection);
            //    var r = cmd2.ExecuteReader();
            //    while (r.Read())
            //    {
            //        DataTypes type = DataTypes.text;
            //        Enum.TryParse<DataTypes>(r["type"].ToString(), out type);
            //        columns.Add(new ColumnVM(r["name"].ToString(), type));
            //    }
            //}
            //Columns = columns;
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

    public class DbConnVM
    {
        public MainWindowVM MainWindowVM { get; set; }
        public ICommand LoadDbCmd { get; set; }
        public string Name { get; set; }
        public string ConnString { get; set; }
        public DbConnVM(MainWindowVM mainWinVM)
        {
            MainWindowVM = mainWinVM;
            LoadDbCmd = new LoadDbCmd(this);
        }
        public void LoadDatabase()
        {
            MainWindowVM.LoadDatabase(Name);
        }
    }

    public class LoadDbCmd : ICommand
    {
        DbConnVM _owner;

        public event EventHandler CanExecuteChanged;

        public LoadDbCmd(DbConnVM owner)
        {
            _owner = owner;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _owner.LoadDatabase();
        }
    }
}
