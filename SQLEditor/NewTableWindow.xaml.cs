using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SQLEditor
{
    /// <summary>
    /// Interaction logic for NewTableWindow.xaml
    /// </summary>
    public partial class NewTableWindow : Window
    {
        readonly NewTableWindowVM ViewModel;

        public NewTableWindow(string tableName = null)
        {
            InitializeComponent();
            DataContext = ViewModel = new NewTableWindowVM(tableName);
            ViewModel.Execute.Finished += Execute_Finished;
        }

        void Execute_Finished()
        {
            Close();
        }

        void btnAddColumn_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Cols.Add(new ColumnVM("", DataTypes.text));
        }

        void btnRemoveColumn_Click(object sender, RoutedEventArgs e)
        {
            var x = (ColumnVM)((FrameworkElement)e.Source).DataContext;
            ViewModel.Cols.Remove(x);
        }
    }
}
