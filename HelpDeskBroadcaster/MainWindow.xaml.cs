using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace HelpDeskBroadcaster
{
    public partial class MainWindow : Window
    {
        private readonly string REGISTRY_BRANCH = @"SOFTWARE\HelpDeskBroadcaster";
        private readonly string REGISTRY_KEY_VALUE = @"API_KEY";
        private readonly string REGISTRY_REQUEST_VALUE = @"API_REQUEST";

        private readonly HelpDeskService _service;
        private readonly BroadcasterBot _bot;

        public BroadcasterBot Bot => _bot;
        public static RoutedCommand SendCommand = new RoutedCommand();

        private List<OrganizationWithUsers> _organizationsList;

        public MainWindow(List<OrganizationWithUsers> list, HelpDeskService serv, BroadcasterBot bot)
        {
            InitializeComponent();
            _service = serv;
            _organizationsList = list;
            _bot = bot;

            var HelpDeskData = KeyFromRegistry();
            if ((HelpDeskData != null) && (!string.IsNullOrEmpty(HelpDeskData.Item1) || !string.IsNullOrEmpty(HelpDeskData.Item2)))
                _service.SetCurrentUser(HelpDeskData);


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

            _organizationsList = _service.GetClientsData();
            return _organizationsList.Count != 0;
        }

        private void updateBtn_Click(object sender, RoutedEventArgs e)
        {
            UpdateListControl();
        }

        private void KeyToRegistry(Tuple<string, string> sVal)
        {
            using (var key = Registry.CurrentUser.CreateSubKey(REGISTRY_BRANCH))
            {
                if (key != null)
                {
                    key.SetValue(REGISTRY_KEY_VALUE, sVal.Item1);
                    key.SetValue(REGISTRY_REQUEST_VALUE, sVal.Item2);
                }
            }
        }

        private Tuple<string, string> KeyFromRegistry()
        {
            using (var key = Registry.CurrentUser.OpenSubKey(REGISTRY_BRANCH))
            {
                if (key != null)
                    return new Tuple<string, string>(key.GetValue(REGISTRY_KEY_VALUE)?.ToString(), key.GetValue(REGISTRY_REQUEST_VALUE)?.ToString());
            }
            return null;
        }

        private async void ExecutedSendCommand(object sender, ExecutedRoutedEventArgs e)
        {
            //messageTxtBox.Text = System.DateTime.Now.ToString(); // 

            BroadcasterBot.eMessengerIDFromFields eFlags = 0;
            if (telegramCheckBox.IsChecked == true)
                eFlags |= BroadcasterBot.eMessengerIDFromFields.e_ID_Telegram;
            if (viberCheckBox.IsChecked == true)
                eFlags |= BroadcasterBot.eMessengerIDFromFields.e_ID_Viber;

            await _bot.SendAllAsync(_organizationsList, messageTxtBox.Text, eFlags);
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
            e.CanExecute = (Bot == null) ? false :
                (Bot.Progress == 0) && !string.IsNullOrEmpty(messageTxtBox.Text) && (telegramCheckBox.IsChecked == true || viberCheckBox.IsChecked == true) && bHasCheck;
        }

        private void keyBtn_Click(object sender, RoutedEventArgs e)
        {
            var wind = new RequestWindow();
            if (wind.ShowDialog() == true)
            {
                var pair = wind.GetValues();
                if (_service.SetCurrentUser(pair))
                    KeyToRegistry(pair);
                else
                    MessageBox.Show("API key not found");
                UpdateListControl();
            }
        }
    }
}
