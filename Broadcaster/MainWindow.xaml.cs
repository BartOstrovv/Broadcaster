using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace Broadcaster
{
    public partial class MainWindow : Window
    {
        //private readonly string REGISTRY_BRANCH = @"SOFTWARE\Broadcaster";
        //private readonly string REGISTRY_KEY_VALUE = @"API_KEY";
        //private readonly string REGISTRY_REQUEST_VALUE = @"API_REQUEST";

        private readonly RootService _service;

        public BroadcasterBot Bot { get; }
        public static RoutedCommand SendCommand = new();

        private List<OrganizationWithUsers> _organizationsList;

        public MainWindow(List<OrganizationWithUsers> list, RootService serv, BroadcasterBot bot)
        {
            InitializeComponent();
            _service = serv;
            _organizationsList = list;
            Bot = bot;

            UpdateListControl();
            sendBtn.DataContext = Bot;
        }

        private void UpdateListControl()
        {
            organizationsListBox.ItemsSource = null;
            if (FillDataList())
                organizationsListBox.ItemsSource = _organizationsList;
        }

        private bool FillDataList()
        {
            _organizationsList.Clear();
            if (_service == null)
                return false;

            _organizationsList = _service.GetUsersFromJSON();
            return _organizationsList.Count != 0;
        }

        private void UpdateBtn_Click(object sender, RoutedEventArgs e) => UpdateListControl();

        /*private void KeyToRegistry(Tuple<string, string> sVal)
        {
            using RegistryKey key = Registry.CurrentUser.CreateSubKey(REGISTRY_BRANCH);
            if (key != null)
            {
                key.SetValue(REGISTRY_KEY_VALUE, sVal.Item1);
                key.SetValue(REGISTRY_REQUEST_VALUE, sVal.Item2);
            }
        }*/

        /*private Tuple<string, string> KeyFromRegistry()
        {
            using (var key = Registry.CurrentUser.OpenSubKey(REGISTRY_BRANCH))
            {
                if (key != null)
                    return new Tuple<string, string>(key.GetValue(REGISTRY_KEY_VALUE)?.ToString(), key.GetValue(REGISTRY_REQUEST_VALUE)?.ToString());
            }
            return null;
        }*/

        private async void ExecutedSendCommand(object sender, ExecutedRoutedEventArgs e)
        {
            //messageTxtBox.Text = System.DateTime.Now.ToString(); // 

            BroadcasterBot.EMessengerIDFromFields eFlags = 0;
            if (telegramCheckBox.IsChecked == true)
                eFlags |= BroadcasterBot.EMessengerIDFromFields.e_ID_Telegram;
            if (viberCheckBox.IsChecked == true)
                eFlags |= BroadcasterBot.EMessengerIDFromFields.e_ID_Viber;

            await Bot.SendAllAsync(_organizationsList, messageTxtBox.Text, eFlags);
            MessageBox.Show($"{Bot.CountMessages} messages sent");
            Bot.Progress = 0;
        }

        private void CanExecuteSendCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            bool bHasCheck = false;
            foreach (OrganizationWithUsers organization in organizationsListBox.Items)
            {
                if (organization.Checked)
                {
                    bHasCheck = true;
                    break;
                }
            }
            e.CanExecute = Bot != null && (Bot.Progress == 0) && !string.IsNullOrEmpty(messageTxtBox.Text) && (telegramCheckBox.IsChecked == true || viberCheckBox.IsChecked == true) && bHasCheck;
        }
    }
}
