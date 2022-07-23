using System;
using System.Collections.Generic;
using System.ComponentModel;
using static HelpDeskBroadcaster.BroadcasterBot;

namespace HelpDeskBroadcaster
{
    public class OrganizationWithUsers : INotifyPropertyChanged
    {
        public string OrganizationName { get; set; }

        public List<string> ViberUsersToken { get; set; } = new();
        public List<string> TelegramUsersToken { get; set; } = new();

        public event PropertyChangedEventHandler PropertyChanged;
        private bool m_bIsChecked;
        public bool Checked
        {
            get { return m_bIsChecked; }
            set
            {
                m_bIsChecked = value;
                OnPropertyChanged();
            }
        }

        protected virtual void OnPropertyChanged(string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public static class OrganizationsHelper
    {
        public static List<string> GetUniqueTokens(this List<OrganizationWithUsers> list, EMessengerIDFromFields eFlag)
        {
            var ret = new List<string>();
            foreach (var item in list)
            {
                if (!item.Checked)
                    continue;

                var source = ((eFlag & EMessengerIDFromFields.e_ID_Telegram) == EMessengerIDFromFields.e_ID_Telegram) ? item.TelegramUsersToken :
                             ((eFlag & EMessengerIDFromFields.e_ID_Viber) == EMessengerIDFromFields.e_ID_Viber) ? item.ViberUsersToken : null;

                if (source == null)
                    continue;

                foreach (string token in source)
                {
                    if ((ret.Find(x => x == token) == null) && !string.IsNullOrEmpty(token))
                        ret.Add(token);
                }
            }
            return ret;
        }
    }
}
