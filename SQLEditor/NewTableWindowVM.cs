using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Common;
using System.ComponentModel;
using System.Data.SQLite;

namespace SQLEditor
{
    public class NewTableWindowVM : ViewModelBase
    {
        public class ExecuteCommand : ICommand
        {
            NewTableWindowVM Parent;

            public event EventHandler CanExecuteChanged;

            public event Action Finished;

            public ExecuteCommand(NewTableWindowVM parent)
            {
                Parent = parent;
                Parent.PropertyChanged += Parent_PropertyChanged;
            }

            void Parent_PropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }

            public bool CanExecute(object parameter)
            {
                //CanExecuteChanged?.Invoke(this, EventArgs.Empty);
                return Parent.Cols.Count > 0 && !string.IsNullOrWhiteSpace(Parent.TableName);
            }

            public void Execute(object parameter)
            {
                var conn = Globals.DbConnection;
                conn.OpenEx();
                new SQLiteCommand(Parent.GenerateSQL(), conn).ExecuteNonQuery();
                Parent.TableId = Parent.TableName;
                Finished?.Invoke();
            }
        }

        public GenerateMode Mode { get; set; }

        public ObservableCollection<ColumnVM> Cols { get; set; } = new ObservableCollection<ColumnVM>();

        public string[] DataTypes { get; set; }

        string tableName;
        public string TableName
        {
            get
            {
                return tableName;
            }

            set
            {
                tableName = value;
                Notify("");
            }
        }

        string TableId { get; set; }

        public ICommand Command { get; set; }
        public ExecuteCommand Execute { get; set; }

        public NewTableWindowVM(string tableName)
        {
            TableId = TableName = tableName;
            Mode = string.IsNullOrEmpty(tableName) ? GenerateMode.Create : GenerateMode.Alter;
            Command = new RelayCommand<object>(CommandDispatch);
            Execute = new ExecuteCommand(this);
            DataTypes = new string[] { "text", "int", "real", "blob" };

            if (Mode == GenerateMode.Alter)
            {
                var cmd = new SQLiteCommand($"pragma table_info({TableName});", Globals.DbConnection);
                var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    DataTypes type = SQLEditor.DataTypes.text;
                    Enum.TryParse(rdr["type"].ToString(), out type);
                    Cols.Add(new ColumnVM(rdr["name"].ToString(), type, Convert.ToBoolean(rdr["pk"]), rdr["cid"].ToString()));
                }
            }
            else
            {
                Cols.Add(new ColumnVM("", SQLEditor.DataTypes.text, false));
            }
        }

        void CommandDispatch(object commandParam)
        {

        }

        public string GenerateSQL()
        {
            if (string.IsNullOrWhiteSpace(TableName)) throw new Exception("Table name cannot be empty.");
            if (Mode == GenerateMode.Alter)
            {
                List<ColumnVM> addedCols = Cols.Where(c => c.CID == null).ToList();

                var cmd = new SQLiteCommand($"pragma table_info({TableName});", Globals.DbConnection);
                var rdr = cmd.ExecuteReader();

                var sql = new StringBuilder();
                if (TableId != TableName)
                {
                    sql.AppendLine($"ALTER TABLE {TableId} RENAME TO {TableName};");
                }

                //column renaming supported since SLQite 3.25.0
                //while (rdr.Read())
                //{
                //    var col = Cols.FirstOrDefault(c => c.CID == rdr["cid"].ToString() && c.Name != rdr["name"].ToString());
                //    if (col != null)
                //    {
                //        sql.AppendLine($"ALTER TABLE {TableName} RENAME COLUMN {rdr["name"].ToString()} TO {col.Name};");
                //    }
                //}

                foreach (var addedCol in addedCols)
                {
                    sql.AppendLine($"ALTER TABLE {TableName} ADD COLUMN {addedCol.Name} {addedCol.DataType};");
                }

                return sql.ToString();
            }
            if (Mode == GenerateMode.Create)
            {
                var cols = new string[Cols.Count];
                for (int i = 0; i < Cols.Count; i++)
                {
                    cols[i] = Cols[i].GetSQLPartial();
                }
                string sql = $"CREATE TABLE {TableName} ({string.Join(", ", cols)}";
                var pKColumns = Cols.Where(c => c.PrimaryKey);
                if (pKColumns.Count() > 0)
                {
                    sql = $"{sql}, PRIMARY KEY({string.Join(",", pKColumns.Select(c => c.Name))})";
                }
                return $"{sql});";
            }
            throw new InvalidEnumArgumentException();
        }
    }

    public class ColumnVM
    {
        private readonly string _cID;
        public string CID => _cID;

        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private DataTypes _dataType;
        public DataTypes DataType
        {
            get { return _dataType; }
            set { _dataType = value; }
        }

        public bool PrimaryKey { get; set; }

        public ColumnVM(string name, DataTypes dataType, bool pk, string cid = null)
        {
            _name = name;
            _dataType = dataType;
            _cID = cid;
            PrimaryKey = pk;
        }

        public string GetSQLPartial()
        {
            if (string.IsNullOrWhiteSpace(Name)) throw new Exception("Column name cannot be empty");
            return $"{Name} {DataType.ToString()}";
        }
    }

    public enum DataTypes
    {
        text, @int, real, blob
    }

    public enum GenerateMode
    {
        Create, Alter
    }
}
