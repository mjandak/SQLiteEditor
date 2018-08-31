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
        public NewTableWindow()
        {
            InitializeComponent();
            DataContext = new NewTableWindowVM();
        }

        private void btnAddColumn_Click(object sender, RoutedEventArgs e)
        {
            var x = (NewTableWindowVM)((FrameworkElement)e.Source).DataContext;
            x.Cols.Add(new ColumnVM("", DataTypes.text));
        }

        private void btnRemoveColumn_Click(object sender, RoutedEventArgs e)
        {
            var x = (ColumnVM)((FrameworkElement)e.Source).DataContext;
            ((NewTableWindowVM)this.DataContext).Cols.Remove(x);
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
