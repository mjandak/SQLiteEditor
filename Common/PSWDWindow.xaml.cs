using System;
using System.Collections.Generic;
using System.ComponentModel;
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
            Close();
        }

        private void OKBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (passwordBox.SecurePassword.Length < 1)
            {
                Globals.Pswd = null;
                DialogResult = false;
            }
            else
            {
                Globals.Pswd = passwordBox.SecurePassword;
                DialogResult = true;
            }
            base.OnClosing(e);
        }
    }
}
