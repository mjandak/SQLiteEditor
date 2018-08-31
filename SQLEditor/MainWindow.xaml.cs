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

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			((MainWindowVM)((FrameworkElement)sender).DataContext).Run();
		}

        private void lblTable_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ((TableVM)((FrameworkElement)sender).DataContext).ShowContents();

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void btnNewTable_Click(object sender, RoutedEventArgs e)
        {
            NewTableWindow x = new NewTableWindow();
            x.ShowDialog();
        }
	}
}
