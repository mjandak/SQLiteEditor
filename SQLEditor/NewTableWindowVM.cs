using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace SQLEditor
{
    public class NewTableWindowVM
    {
        public ObservableCollection<ColumnVM> Cols { get; set; }
        public string[] DataTypes { get; set; }
        public string TableName { get; set; }
        public NewTableWindowVM()
        {
            DataTypes = new string[] { "text", "int", "real", "blob" };
            Cols = new ObservableCollection<ColumnVM>
            {
                new ColumnVM("", SQLEditor.DataTypes.text)
            };
        }

        public string GenerateSQL()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("CREATE TABLE ");
            sb.Append(TableName);
            sb.AppendLine(" (");
            
            foreach (ColumnVM col in Cols)
            {
                sb.Append(col.Name);
                sb.Append(' ');
                sb.AppendLine(col.DataType.ToString());
            }
            sb.Append(");");
            return sb.ToString();
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
    }

    public enum DataTypes
    {
        text, @int, real, blob
    }
}
