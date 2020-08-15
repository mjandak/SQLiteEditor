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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;
using AurelienRibon.Ui.SyntaxHighlightBox;
using Microsoft.Win32;
using Common;
using System.Data.Common;
using System.Configuration;
using System.IO;
using System.Data.SQLite;

namespace SQLEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public readonly MainWindowVM ViewModel;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = ViewModel = new MainWindowVM();
            ViewModel.AddAlterTable += ViewModel_AddAlterTable;
            sqlEditor.CurrentHighlighter = HighlighterManager.Instance.Highlighters["SQL"];
        }

        void ViewModel_AddAlterTable(string tableName)
        {
            new NewTableWindow(tableName).ShowDialog();
        }

        void btnOpebDb_Click(object sender, RoutedEventArgs e)
        {
            (sender as Button).ContextMenu.IsEnabled = true;
            (sender as Button).ContextMenu.PlacementTarget = sender as Button;
            (sender as Button).ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            (sender as Button).ContextMenu.IsOpen = true;
        }

        void menuOpenDb_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = false;
            if (openFileDialog.ShowDialog() == true)
            {
                //var pswdDialog = new PSWDWindow();
                //pswdDialog.ShowDialog();
                //if (!pswdDialog.ShowDialog().Value)
                //{
                //    //Close or cancel button clicked
                //    return;
                //}
                ((MainWindowVM)DataContext).AddDatabase(System.IO.Path.GetFileName(openFileDialog.FileName), openFileDialog.FileName);
                ((MainWindowVM)DataContext).LoadDatabase(System.IO.Path.GetFileName(openFileDialog.FileName), true);
            }
        }

        void menuCreateDb_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog();
            if (dialog.ShowDialog() == true)
            {
                File.Create(dialog.FileName).Dispose();
                ((MainWindowVM)DataContext).AddDatabase(System.IO.Path.GetFileName(dialog.FileName), dialog.FileName);
                ((MainWindowVM)DataContext).LoadDatabase(System.IO.Path.GetFileName(dialog.FileName), true);
            }
        }

        static TreeViewItem VisualUpwardSearch(DependencyObject source)
        {
            while (source != null && !(source is TreeViewItem))
                source = VisualTreeHelper.GetParent(source);

            return source as TreeViewItem;
        }

        void trvDatabase_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            //TreeViewItem treeViewItem = e.Source as TreeViewItem;
            //if (treeViewItem == null)
            //{
            //    treeViewItem = VisualUpwardSearch(e.OriginalSource as DependencyObject);
            //}
            //treeViewItem.Focus();
            //e.Handled = true;

            TreeViewItem treeViewItem = VisualUpwardSearch(e.OriginalSource as DependencyObject);
            if (treeViewItem != null)
            {
                treeViewItem.Focus();
                e.Handled = true;
            }
        }

        void sqlEditor_SelectionChanged(object sender, RoutedEventArgs e)
        {
            ViewModel.SelectionStart = sqlEditor.SelectionStart;
            ViewModel.SelectionLength = sqlEditor.SelectionLength;
        }
    }
}
