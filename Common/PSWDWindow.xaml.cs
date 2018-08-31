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

namespace Common
{
    /// <summary>
    /// Interaction logic for PSWDWindow.xaml
    /// </summary>
    public partial class PSWDWindow : Window
    {
        public PSWDWindow()
        {
            InitializeComponent();
            passwordBox.Focus();
        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void OKBtn_Click(object sender, RoutedEventArgs e)
        {
            Globals.Pswd = passwordBox.SecurePassword;
            DialogResult = true;
            Close();
        }
    }
}
