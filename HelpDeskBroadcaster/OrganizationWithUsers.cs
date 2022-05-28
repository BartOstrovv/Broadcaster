using System;
using System.Collections.Generic;
using System.ComponentModel;
using static HelpDeskBroadcaster.BroadcasterBot;

namespace HelpDeskBroadcaster
{
   public class OrganizationWithUsers : INotifyPropertyChanged
    {
        public string m_organizationID { get; set; }
        public string m_organizationName { get; set; }

        public List<string> _viberUsersToken { get; set; } = new List<string>();
        public List<string> _telegramUsersToken { get; set; } = new List<string>();

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

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public static class OrganizationsHelper
    {
        public static List<string> GetUniqueTokens(this List<OrganizationWithUsers> list, eMessengerIDFromFields eFlag)
        {
            var ret = new List<string>();
            foreach (var item in list)
            {
                if (!item.Checked)
                    continue;

                var source = ((eFlag & eMessengerIDFromFields.e_ID_Telegram) == eMessengerIDFromFields.e_ID_Telegram) ? item._telegramUsersToken :
                             ((eFlag & eMessengerIDFromFields.e_ID_Viber) == eMessengerIDFromFields.e_ID_Viber) ? item._viberUsersToken : null;

                if (source == null)
                    continue;

                foreach (var token in source)
                {
                    if (ret.Find(x => x == token) == null)
                        ret.Add(token);
                }
            }
            return ret;
        }
    }
}
