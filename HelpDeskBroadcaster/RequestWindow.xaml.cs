using System;
using System.Collections.Generic;
using System.Windows;

namespace HelpDeskBroadcaster
{
    public partial class RequestWindow : Window
    {
        public RequestWindow(Tuple<string,string> key)
        {
            InitializeComponent();
            keyTxt.Text = key.Item1;
            requestTxt.Text = key.Item2;
        }

        public Tuple<string, string> GetValues() => new Tuple<string, string>(keyTxt.Text, requestTxt.Text);

        private void okBtn_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }
    }
}
