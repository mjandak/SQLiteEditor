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
		public MainWindow()
		{
			InitializeComponent();
            sqlEditor.CurrentHighlighter = HighlighterManager.Instance.Highlighters["SQL"];
        }

        private void lblTable_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ((TableVM)((FrameworkElement)sender).DataContext).ShowContents();

        }

        private void btnNewTable_Click(object sender, RoutedEventArgs e)
        {
            var x = new NewTableWindow();
            x.ShowDialog();
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
                ((MainWindowVM)DataContext).AddDatabase(System.IO.Path.GetFileName(openFileDialog.FileName), openFileDialog.FileName);
                ((MainWindowVM)DataContext).LoadDatabase(System.IO.Path.GetFileName(openFileDialog.FileName));
            }
        }

        private void menuCreateDb_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog();
            if (dialog.ShowDialog() == true)
            {
                File.Create(dialog.FileName).Dispose();
                ((MainWindowVM)DataContext).AddDatabase(System.IO.Path.GetFileName(dialog.FileName), dialog.FileName);
                ((MainWindowVM)DataContext).LoadDatabase(System.IO.Path.GetFileName(dialog.FileName));
            }
        }
    }
}
