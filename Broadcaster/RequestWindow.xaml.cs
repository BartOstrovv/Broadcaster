using System;
using System.Collections.Generic;
using System.Windows;

namespace HelpDeskBroadcaster
{
    public partial class RequestWindow : Window
    {
        public RequestWindow()
        {
            InitializeComponent();
        }

        public Tuple<string, string> Values => new(keyTxt.Text, requestTxt.Text);

        private void OkBtn_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }
    }
}
