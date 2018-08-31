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
    public class NewTableWindowVM : INotifyPropertyChanged
    {
        class CreateCmd : ICommand
        {
            NewTableWindowVM Parent;

            public event EventHandler CanExecuteChanged;

            public CreateCmd(NewTableWindowVM parent)
            {
                Parent = parent;
                Parent.PropertyChanged += Parent_PropertyChanged;
            }

            private void Parent_PropertyChanged(object sender, PropertyChangedEventArgs e)
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
                using (var conn = Globals.DbConnection)
                {
                    conn.OpenEx();
                    new SQLiteCommand(Parent.GenerateSQL(), conn).ExecuteNonQuery();
                }
            }
        }

        public ObservableCollection<ColumnVM> Cols { get; set; }
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

        public ICommand Command { get; set; }
        public ICommand Create { get; set; }
        public NewTableWindowVM()
        {
            Command = new RelayCommand(CommandDispatch);
            Create = new CreateCmd(this);
            DataTypes = new string[] { "text", "int", "real", "blob" };
            Cols = new ObservableCollection<ColumnVM>
            {
                new ColumnVM("", SQLEditor.DataTypes.text)
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;

        void Notify(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        void CommandDispatch(object commandParam)
        {

        }

        public string GenerateSQL()
        {
            if (string.IsNullOrWhiteSpace(TableName)) throw new Exception("Table name cannot be empty.");
            var cols = new string[Cols.Count];
            for (int i = 0; i < Cols.Count; i++)
            {
                cols[i] = Cols[i].GetSQLPartial();
            }
            return $"CREATE TABLE {TableName} ({string.Join(", ", cols)});";
        }
    }

    public class ColumnVM
    {
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

        public ColumnVM(string name, DataTypes dataType)
        {
            _name = name;
            _dataType = dataType;
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
}
